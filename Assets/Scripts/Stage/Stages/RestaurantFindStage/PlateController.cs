using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 접시 이동 관련
public class PlateController : MonoBehaviour, IPointerClickHandler
{
    [Header("접시 이미지")]
    [SerializeField] private Image openPlateImage; // 음식이 보이는 접시 (처음에 활성)
    [SerializeField] private Image LidImage; // 접시 뚜껑 (DomeLid)
    [SerializeField] private Image closedPlateImage; // 닫힌 접시 (Default)
    [SerializeField] private Image selectedPlateImage; // 선택된 반짝이는 접시 (Selected)

    [Header("뚜껑 속도")]
    [SerializeField] private Vector2 lidStartOffset = new Vector2(0, 200); // 뚜껑 시작 오프셋 (위쪽)

    private RestaurantFindStage _stage;
    private bool isCorrectPlate = false;
    private RectTransform lidRectTransform;
    private Vector2 lidOriginalPos; // 뚜껑의 원래 위치 (프리팹에서 설정한 위치)

    public bool IsCorrectPlate => isCorrectPlate;

    /// 정답 접시 여부 설정
    public void SetCorrectPlate(bool isCorrect)
    {
        isCorrectPlate = isCorrect;
    }

    /// 뚜껑 닫기 (Default 이미지로 전환)
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

        // 이미지 상태 전환
        if (openPlateImage != null) openPlateImage.gameObject.SetActive(false);
        if (closedPlateImage != null) closedPlateImage.gameObject.SetActive(true);
        if (selectedPlateImage != null) selectedPlateImage.gameObject.SetActive(false);
    }

    private IEnumerator MoveLidDown(float duration)
    {
        Vector2 startPos = lidOriginalPos + lidStartOffset;
        Vector2 endPos = lidOriginalPos;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            lidRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        lidRectTransform.anchoredPosition = endPos; // 정확히 원래 위치로
        
        // 뚜껑 다 내려갔으면 사라지게
        if (LidImage != null)
        {
            LidImage.gameObject.SetActive(false);
        }
    }

    /// 뚜껑 열기 (음식 보이는 이미지로)
    public void OpenLid()
    {
        if (openPlateImage != null) openPlateImage.gameObject.SetActive(true);
        if (closedPlateImage != null) closedPlateImage.gameObject.SetActive(false);
        if (selectedPlateImage != null) selectedPlateImage.gameObject.SetActive(false);
    }

    // 뚜껑 다시 올리기 (위로 이동)
    public void RaiseLid()
    {
        StartCoroutine(RaiseLidCoroutine());
    }

    private IEnumerator RaiseLidCoroutine()
    {
        float duration = _stage != null ? _stage.LidCloseDuration : 1f;
        
        // 뚜껑이 올라가기 시작할 때 접시를 음식이 보이는 이미지로 변경
        if (openPlateImage != null) openPlateImage.gameObject.SetActive(true);
        if (closedPlateImage != null) closedPlateImage.gameObject.SetActive(false);
        if (selectedPlateImage != null) selectedPlateImage.gameObject.SetActive(false);
        
        // 뚜껑 활성화
        if (LidImage != null)
        {
            LidImage.gameObject.SetActive(true);
        }
        
        // MoveLidDown의 반대: 아래(원래 위치)에서 위(시작 오프셋)로
        Vector2 startPos = lidOriginalPos;
        Vector2 endPos = lidOriginalPos + lidStartOffset;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            lidRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        lidRectTransform.anchoredPosition = endPos;
    }


    /// 선택 표시 (Selected 이미지로)
    public void ShowSelected()
    {
        if (openPlateImage != null) openPlateImage.gameObject.SetActive(false);
        if (closedPlateImage != null) closedPlateImage.gameObject.SetActive(false);
        if (selectedPlateImage != null) selectedPlateImage.gameObject.SetActive(true);
    }


    /// UI 클릭 이벤트 (IPointerClickHandler)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_stage != null)
        {
            _stage.OnPlateSelected(this);
        }
    }

    void Awake()
    {
        _stage = FindObjectOfType<RestaurantFindStage>();

        // LidImage의 RectTransform 가져오기
        if (LidImage != null)
        {
            lidRectTransform = LidImage.GetComponent<RectTransform>();
            lidOriginalPos = lidRectTransform.anchoredPosition; // 프리팹에서 설정한 원래 위치 저장
            lidRectTransform.anchoredPosition = lidOriginalPos + lidStartOffset;
            LidImage.gameObject.SetActive(true);
        }

        // 처음에는 음식이 보이는 상태
        if (openPlateImage != null) openPlateImage.gameObject.SetActive(true);
        if (closedPlateImage != null) closedPlateImage.gameObject.SetActive(false);
        if (selectedPlateImage != null) selectedPlateImage.gameObject.SetActive(false);
    }
}
