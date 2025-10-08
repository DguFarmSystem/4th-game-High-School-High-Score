using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 개별 접시 UI 프리팹의 컨트롤러
/// 프리팹에는 음식이 보이는 이미지, 닫힌 이미지, 선택된 이미지가 모두 포함되어 있음
/// </summary>
public class PlateController : MonoBehaviour, IPointerClickHandler
{
    [Header("Plate Images")]
    [SerializeField] private Image openPlateImage; // 음식이 보이는 접시 (처음에 활성)
    [SerializeField] private Image closedPlateImage; // 닫힌 접시 (Default)
    [SerializeField] private Image selectedPlateImage; // 선택된 접시 (Selected)
    
    private RestaurantFindStage _stage;
    private bool isCorrectPlate = false;
    
    public bool IsCorrectPlate => isCorrectPlate;
    
    /// <summary>
    /// 정답 접시 여부 설정
    /// </summary>
    public void SetCorrectPlate(bool isCorrect)
    {
        isCorrectPlate = isCorrect;
    }
    
    /// <summary>
    /// 뚜껑 닫기 (Default 이미지로 전환)
    /// </summary>
    public void CloseLid()
    {
        if (openPlateImage != null) openPlateImage.gameObject.SetActive(false);
        if (closedPlateImage != null) closedPlateImage.gameObject.SetActive(true);
        if (selectedPlateImage != null) selectedPlateImage.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 뚜껑 열기 (음식 보이는 이미지로)
    /// </summary>
    public void OpenLid()
    {
        if (openPlateImage != null) openPlateImage.gameObject.SetActive(true);
        if (closedPlateImage != null) closedPlateImage.gameObject.SetActive(false);
        if (selectedPlateImage != null) selectedPlateImage.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 선택 표시 (Selected 이미지로)
    /// </summary>
    public void ShowSelected()
    {
        if (openPlateImage != null) openPlateImage.gameObject.SetActive(false);
        if (closedPlateImage != null) closedPlateImage.gameObject.SetActive(false);
        if (selectedPlateImage != null) selectedPlateImage.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// UI 클릭 이벤트 (IPointerClickHandler)
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_stage != null)
        {
            _stage.OnPlateSelected(this);
        }
    }
    
    // ============ Lifecycle methods ============ //
    void Awake()
    {
        _stage = FindObjectOfType<RestaurantFindStage>();
        
        if (_stage == null)
        {
            Debug.LogError("RestaurantFindStage not found!");
        }
        
        // 처음에는 음식이 보이는 상태
        if (openPlateImage != null) openPlateImage.gameObject.SetActive(true);
        if (closedPlateImage != null) closedPlateImage.gameObject.SetActive(false);
        if (selectedPlateImage != null) selectedPlateImage.gameObject.SetActive(false);
    }
}
