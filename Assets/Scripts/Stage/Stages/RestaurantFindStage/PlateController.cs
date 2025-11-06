using System.Collections;
using UnityEngine;

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

    // 마우스 클릭 이벤트
    private void OnMouseDown()
    {
        if (_stage != null)
        {
            _stage.OnPlateSelected(this);
        }
    }

    void Awake()
    {
        _stage = FindObjectOfType<RestaurantFindStage>();

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
