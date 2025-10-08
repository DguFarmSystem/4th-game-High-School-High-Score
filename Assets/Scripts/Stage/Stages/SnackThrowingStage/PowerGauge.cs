using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.UI;

public class PowerGauge : MonoBehaviour
{
    [SerializeField] private GameObject gauge;
    [SerializeField] private Sprite gauge_blank;
    [SerializeField] private Sprite gauge_lv1;
    [SerializeField] private Sprite gauge_lv2;
    [SerializeField] private Sprite gauge_lv3;

    private enum State { Idle, Pressed, Released }
    private State _currentState = State.Idle;

    private SnackDetector _student;

    private float yOffset;

    private Coroutine _throwingCoroutine;

    private void TransitionToState(State newState)
    {
        _currentState = newState;
        
        if (FindObjectOfType<SnackThrowingStage>().CurrentState == StageState.Playing)
        {
            switch (newState)
            {
                case State.Idle:

                    break;

                case State.Pressed:
                    switch (_student.Distance)
                    {
                        case 1:
                            yOffset = 5f;
                            break;
                        case 2:
                            yOffset = 3.5f;
                            break;
                        case 3:
                            yOffset = 2.5f;
                            break;
                    }
                    Vector3 worldPos = _student.transform.position + Vector3.up * yOffset;
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
                    gauge.GetComponent<RectTransform>().position = screenPos;
                    gauge.SetActive(true);
                    break;

                case State.Released:
                    _throwingCoroutine = StartCoroutine(WaitSnackArrival(0.5f));
                    _student = null;
                    break;
            }
        }
    }

    private void HandleIdleState()
    {
        _student = InputManager.Instance.PressedCollider?.GetComponent<SnackDetector>();

        if (_student)
        {
            TransitionToState(State.Pressed);
        }
    }

    private void HandlePressedState()
    {
        if (InputManager.Instance.IsPressing)
        {
            Sprite currentSprite = gauge.transform.GetComponent<Image>().sprite;
            int scaledPressedTime = Mathf.FloorToInt(_student.PressedTime);

            switch (scaledPressedTime)
            {
                case 1:
                    if (currentSprite != gauge_lv1)
                        gauge.transform.GetComponent<Image>().sprite = gauge_lv1;
                    break;
                case 2:
                    if (currentSprite != gauge_lv2)
                        gauge.transform.GetComponent<Image>().sprite = gauge_lv2;
                    break;
                case 3:
                    if (currentSprite != gauge_lv3)
                        gauge.transform.GetComponent<Image>().sprite = gauge_lv3;
                    break;
                default:
                    if (currentSprite != gauge_blank)
                        gauge.transform.GetComponent<Image>().sprite = gauge_blank;
                    break;
            }
        }
        else
        {
            TransitionToState(State.Released);
        }
    }

    private void HandleReleasedState()
    {
        
    }
    
    public void setGaugeColor(Color color)
    {
        gauge.transform.GetComponent<Image>().color = color;
    }

    private IEnumerator WaitSnackArrival(float delay)
    {
        yield return new WaitForSeconds(delay); // 약간의 딜레이 후에 스낵이 도착했다고 가정
        gauge.SetActive(false);

        setGaugeColor(Color.white);

        gauge.transform.GetComponent<Image>().sprite = gauge_blank;
        TransitionToState(State.Idle);
    }

    // ============ Lifecycle methods ============ //
    void Start()
    {

    }

    void Update()
    {
        switch (_currentState)
        {
            case State.Idle:
                HandleIdleState();

                break;

            case State.Pressed:
                HandlePressedState();

                break;

            case State.Released:
                HandleReleasedState();

                break;
        }
    }
}
