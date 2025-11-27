using System.Collections;
using UnityEngine;
using InputSystem = UnityEngine.InputSystem;
using System.Linq;

// 접시 이동 관련 (World Space)
public class PlateController : MonoBehaviour
{
    [Header("접시 스프라이트")]
    [SerializeField] private SpriteRenderer openPlateSprite; // 음식이 보이는 접시 (처음에 활성)
    [SerializeField] private SpriteRenderer lidSprite; // 접시 뚜껑 (DomeLid)
    [SerializeField] private SpriteRenderer closedPlateSprite; // 닫힌 접시 (Default)
    [SerializeField] private SpriteRenderer selectedPlateSprite; // 선택된 반짝이는 접시 (Selected)

    [Header("뚜껑 설정")]
    [SerializeField] private Vector3 lidStartOffset = new Vector3(0, 2f, 0); // 뚜껑 시작 오프셋 (위쪽)

    private RestaurantFindStage _stage;
    private Collider2D _collider2D;
    private bool isCorrectPlate = false;
    private Transform lidTransform;
    private Vector3 lidOriginalPos; // 뚜껑의 원래 위치 (프리팹에서 설정한 위치)

    public bool IsCorrectPlate => isCorrectPlate;

    // 정답 접시 여부 설정
    public void SetCorrectPlate(bool isCorrect)
    {
        isCorrectPlate = isCorrect;
    }

    // 뚜껑 닫기 (Default 이미지로 전환)
    public void CloseLid()
    {
        StartCoroutine(CloseLidWithDelay());
    }

    private IEnumerator CloseLidWithDelay()
    {
        // RestaurantFindStage의 lidCloseDuration 사용
        float duration = _stage != null ? _stage.LidCloseDuration : 1f;
        
        // 뚜껑이 이미 위쪽에 있으므로, 바로 아래로 이동
        yield return StartCoroutine(MoveLidDown(duration));

        // 스프라이트 상태 전환
        if (openPlateSprite != null) openPlateSprite.gameObject.SetActive(false);
        if (closedPlateSprite != null) closedPlateSprite.gameObject.SetActive(true);
        if (selectedPlateSprite != null) selectedPlateSprite.gameObject.SetActive(false);
    }

    private IEnumerator MoveLidDown(float duration)
    {
        Vector3 startPos = lidOriginalPos + lidStartOffset;
        Vector3 endPos = lidOriginalPos;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            lidTransform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        lidTransform.localPosition = endPos; // 정확히 원래 위치로
        
        // 뚜껑 다 내려갔으면 사라지게
        if (lidSprite != null)
        {
            lidSprite.gameObject.SetActive(false);
        }
    }

    // 뚜껑 다시 올리기 (위로 이동)
    public void RaiseLid()
    {
        StartCoroutine(RaiseLidCoroutine());
    }

    private IEnumerator RaiseLidCoroutine()
    {
        float duration = _stage != null ? _stage.LidCloseDuration : 1f;
        
        // 뚜껑이 올라가기 시작할 때 접시를 음식이 보이는 스프라이트로 변경
        if (openPlateSprite != null) openPlateSprite.gameObject.SetActive(true);
        if (closedPlateSprite != null) closedPlateSprite.gameObject.SetActive(false);
        if (selectedPlateSprite != null) selectedPlateSprite.gameObject.SetActive(false);
        
        // 뚜껑 활성화
        if (lidSprite != null)
        {
            lidSprite.gameObject.SetActive(true);
        }
        
        // 아래에서 위로
        Vector3 startPos = lidOriginalPos;
        Vector3 endPos = lidOriginalPos + lidStartOffset;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            lidTransform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        lidTransform.localPosition = endPos;
    }

    // 선택 표시 (Selected 스프라이트로)
    public void ShowSelected()
    {
        if (openPlateSprite != null) openPlateSprite.gameObject.SetActive(false);
        if (closedPlateSprite != null) closedPlateSprite.gameObject.SetActive(false);
        if (selectedPlateSprite != null) selectedPlateSprite.gameObject.SetActive(true);
    }

    // InputManager 기반 Plate 선택 처리
    private void OnEnable()
    {
        InputManager.Instance.OnStageTapPerformed += OnTapPerformed;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnStageTapPerformed -= OnTapPerformed;
    }

    private void OnTapPerformed()
    {
        if (_stage == null || _collider2D == null || InputManager.Instance == null)
            return;

        // InputManager의 TappedCollider가 비어있을 수 있으니, TouchWorldPos로 직접 판정
        Vector3 touchWorld = InputManager.Instance.TouchWorldPos;

        Collider2D top = GetTopColliderAtPoint(touchWorld);

        if (top == _collider2D)
        {
            _stage.OnPlateSelected(this);
        }
    }

    // 터치 지점에서 top Collider2D를 가져오는 보조 함수 (자식 SpriteRenderer 고려, 반경 보정)
    private Collider2D GetTopColliderAtPoint(Vector3 worldPos)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);
        if (hits != null && hits.Length > 0)
        {
            var rendered = hits
                .Select(h => new { col = h, rend = h.GetComponent<SpriteRenderer>() ?? h.GetComponentInChildren<SpriteRenderer>() })
                .Where(x => x.rend != null)
                .OrderBy(x => x.rend.sortingOrder)
                .Select(x => x.col)
                .ToArray();

            if (rendered.Length > 0)
            {
                return rendered.Last();
            }

            // 여기에 도달하면 hits는 존재하지만 SpriteRenderer 기반 필터에 걸리지 않음
            // 가장 가까운 콜라이더를 폴백으로 반환
            var nearest = hits.OrderBy(h => Vector2.Distance(h.bounds.ClosestPoint(worldPos), worldPos)).FirstOrDefault();
            if (nearest != null) return nearest;
        }

        // point로 못 잡으면 작은 반경으로 보정
        float radius = 0.1f;
        Collider2D[] circleHits = Physics2D.OverlapCircleAll(worldPos, radius);
        if (circleHits != null && circleHits.Length > 0)
        {
            var rendered = circleHits
                .Select(h => new { col = h, rend = h.GetComponent<SpriteRenderer>() ?? h.GetComponentInChildren<SpriteRenderer>() })
                .Where(x => x.rend != null)
                .OrderBy(x => x.rend.sortingOrder)
                .Select(x => x.col)
                .ToArray();

            if (rendered.Length > 0)
            {
                return rendered.Last();
            }

            var nearest = circleHits.OrderBy(h => Vector2.Distance(h.bounds.ClosestPoint(worldPos), worldPos)).FirstOrDefault();
            return nearest;
        }

        return null;
    }

    void Awake()
    {
        _stage = FindObjectOfType<RestaurantFindStage>();
        _collider2D = GetComponent<Collider2D>();

        // Lid 스프라이트의 Transform 가져오기
        if (lidSprite != null)
        {
            lidTransform = lidSprite.transform;
            lidOriginalPos = lidTransform.localPosition; // 프리팹에서 설정한 원래 위치 저장
            lidTransform.localPosition = lidOriginalPos + lidStartOffset;
            lidSprite.gameObject.SetActive(true);
        }

        // 처음에는 음식이 보이는 상태
        if (openPlateSprite != null) openPlateSprite.gameObject.SetActive(true);
        if (closedPlateSprite != null) closedPlateSprite.gameObject.SetActive(false);
        if (selectedPlateSprite != null) selectedPlateSprite.gameObject.SetActive(false);
    }
}
