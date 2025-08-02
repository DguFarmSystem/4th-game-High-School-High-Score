using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.UI;

public class PowerGauge : MonoBehaviour
{
    [SerializeField] private GameObject gauge;

    private enum State { Idle, Pressed, Released }
    private State _currentState = State.Idle;

    private SnackDetector _student;

    public float yOffset;

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
        _student = InputManager.PressedCollider?.GetComponent<SnackDetector>();

        if (_student)
        {
            TransitionToState(State.Pressed);
        }
    }

    private void HandlePressedState()
    {
        if (InputManager.IsPressing)
        {
            gauge.transform.Find("FullGauge").GetComponent<Image>().fillAmount = _student.PressedTime / 3f;
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
        gauge.transform.Find("EmptyGauge").GetComponent<Image>().color = color;
        gauge.transform.Find("FullGauge").GetComponent<Image>().color = color;
    }

    private IEnumerator WaitSnackArrival(float delay)
    {
        yield return new WaitForSeconds(delay); // 약간의 딜레이 후에 스낵이 도착했다고 가정
        gauge.SetActive(false);

        setGaugeColor(Color.white);

        gauge.transform.Find("FullGauge").GetComponent<Image>().fillAmount = 0f;
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
