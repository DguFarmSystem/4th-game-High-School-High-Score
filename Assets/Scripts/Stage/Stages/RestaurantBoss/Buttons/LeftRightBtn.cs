using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LeftRightBtn : MonoBehaviour
{
    [SerializeField] protected Button selfButton;
    [SerializeField] protected Button otherButton;
    [SerializeField] protected TextMeshProUGUI goalLeftText;
    [SerializeField] protected ParticleSystem criticalEffect;
    [SerializeField] protected Image criticalImage;
    [SerializeField] protected Image failImage;
    [SerializeField] protected GameObject comboNotifier;

    [SerializeField] protected AudioClip failedInputSFX;

    protected Conveyor Conveyor;
    protected RestaurantBossStage RestaurantBossStage;
    protected static float criticalTimer = 1f;
    protected static float failTimer = 1f;
    protected static float criticalDuration = 1f;
    protected static float failDuration = 1f;

    protected static bool isFailedInput = false;

    public static int CorrectInputCount { get; protected set; } = 0;
    public static int Combo { get; protected set; } = 0;

    private int btnCount = 2;

    protected int RemainingItemCount 
    {
        get
        {
            int diff = RestaurantBossStage.ClearItemCount - CorrectInputCount;
            return diff > 0 ? diff : 0;
        }
    }

    protected IEnumerator DisableButtonsTemporarily()
    {
        selfButton.interactable = false;
        otherButton.interactable = false;
        isFailedInput = true;
        yield return new WaitForSeconds(0.4f); // 0.4초 동안 비활성화
        isFailedInput = false;
        selfButton.interactable = true;
        otherButton.interactable = true;
    }

    protected void OnDisable()
    {
        criticalTimer = 1f;
        CorrectInputCount = 0;
        Combo = 0;
        isFailedInput = false;
    }

    protected void Start()
    {
        Conveyor = FindObjectOfType<Conveyor>();
        RestaurantBossStage = FindObjectOfType<RestaurantBossStage>();
    }

    protected virtual void Update()
    {
        if (criticalTimer >= 0f)
        {
            criticalTimer -= Time.deltaTime / btnCount;
        }
        else
        {
            criticalImage.enabled = false;
        }

        if (failTimer >= 0f)
        {
            failTimer -= Time.deltaTime / btnCount;
        }
        else
        {
            failImage.enabled = false;
        }
    }
}
