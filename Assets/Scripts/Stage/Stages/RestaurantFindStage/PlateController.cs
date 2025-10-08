using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// 개별 접시 UI 프리팹의 컨트롤러
/// 프리팹에는 음식이 보이는 이미지, 닫힌 이미지, 선택된 이미지가 모두 포함되어 있음
public class PlateController : MonoBehaviour, IPointerClickHandler
{
    [Header("Plate Images")]
    [SerializeField] private Image openPlateImage; // 음식이 보이는 접시 (처음에 활성)
    [SerializeField] private Image closedPlateImage; // 닫힌 접시 (Default)
    [SerializeField] private Image selectedPlateImage; // 선택된 접시 (Selected)
    
    private RestaurantFindStage _stage;
    private bool isCorrectPlate = false;
    
    public bool IsCorrectPlate => isCorrectPlate;
    
    /// 정답 접시 여부 설정
    public void SetCorrectPlate(bool isCorrect)
    {
        isCorrectPlate = isCorrect;
    }
    
    /// 뚜껑 닫기 (Default 이미지로 전환)
    public void CloseLid()
    {
        if (openPlateImage != null) openPlateImage.gameObject.SetActive(false);
        if (closedPlateImage != null) closedPlateImage.gameObject.SetActive(true);
        if (selectedPlateImage != null) selectedPlateImage.gameObject.SetActive(false);
    }
    
    /// 뚜껑 열기 (음식 보이는 이미지로)
    public void OpenLid()
    {
        if (openPlateImage != null) openPlateImage.gameObject.SetActive(true);
        if (closedPlateImage != null) closedPlateImage.gameObject.SetActive(false);
        if (selectedPlateImage != null) selectedPlateImage.gameObject.SetActive(false);
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
        
        // 처음에는 음식이 보이는 상태
        if (openPlateImage != null) openPlateImage.gameObject.SetActive(true);
        if (closedPlateImage != null) closedPlateImage.gameObject.SetActive(false);
        if (selectedPlateImage != null) selectedPlateImage.gameObject.SetActive(false);
    }
}
