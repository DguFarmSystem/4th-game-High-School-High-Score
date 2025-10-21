using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LeftRightBtn : MonoBehaviour
{
    [SerializeField] protected Button selfButton;
    [SerializeField] protected Button otherButton;

    protected Conveyor Conveyor;

    protected static bool isFailedInput = false;

    public static int CorrectInputCount { get; protected set; } = 0;
    public static int Combo { get; protected set; } = 0;

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
        CorrectInputCount = 0;
        Combo = 0;
        isFailedInput = false;
    }

    protected void Start()
    {
        Conveyor = FindObjectOfType<Conveyor>();
    }
}
