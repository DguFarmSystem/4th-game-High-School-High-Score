using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnackDetector : MonoBehaviour
{
    private SnackThrowingStage _stage;

    private enum State { Waiting, Caught }
    private State _currentState = State.Waiting;

    private float _pressedTime = 0f;
    private bool _isPressing = false;

    public float PressedTime => _pressedTime;
    public int Distance = 0;

    public bool SnackArrived { get; private set; } = false;

    public void Ready2Catch()
    {
        _isPressing = true;
    }

    public void Try2Catch()
    {
        _isPressing = false;
    }

    private void TransitionToState(State newState)
    {
        _currentState = newState;

        switch (newState)
        {
            case State.Waiting:

                break;

            case State.Caught:
                if (_stage)
                {
                    _stage.StudentGotSnack();
                    gameObject.GetComponent<Collider2D>().enabled = false;
                }

                break;
        }
    }

    private void HandleWaitingState()
    {
        if (_isPressing)
        {
            if (_pressedTime + Time.deltaTime > 4f) _pressedTime -= 4f;
            else _pressedTime += Time.deltaTime;
        }
        else
        {
            PowerGauge gauge = FindObjectOfType<PowerGauge>();
            Teacher teacher = FindObjectOfType<Teacher>();

            if (_pressedTime > 0f && teacher.IsTurning)
            {
                FindObjectOfType<SnackThrowingStage>().SetStageFailed();
                gauge.setGaugeColor(new Color(255f / 255f, 127f / 255f, 127f / 255f, 1f)); // 빨간색
                _pressedTime = 0f;
                return;
            }
            
            if (_pressedTime >= Distance && _pressedTime < Distance + 1f)
            {
                if (gauge.transform.GetChild(0).gameObject.activeSelf)
                {
                    gauge.setGaugeColor(new Color(127f / 255f, 255f / 255f, 127f / 255f, 1f)); // 초록색

                    Candies candies = FindObjectOfType<Candies>();
                    if (candies)
                    {
                        StartCoroutine(candies.ThrowCandy(this.GetComponent<Collider2D>()));
                    }
                }

                TransitionToState(State.Caught);
            }
            else if (!Mathf.Approximately(_pressedTime, 0f) && gauge.transform.GetChild(0).gameObject.activeSelf)
            {
                gauge.setGaugeColor(new Color(255f / 255f, 127f / 255f, 127f / 255f, 1f)); // 빨간색
            }
            

            if (!Mathf.Approximately(_pressedTime, 0f)) _pressedTime = 0f;
        }
    }

    private void HandleCaughtState()
    {
        
    }

    public void GetCandy() => SnackArrived = true;

    // ============ Lifecycle methods ============ //

    void Awake()
    {
        _stage = FindObjectOfType<SnackThrowingStage>();
        if (!_stage)
        {
            Debug.LogError("SnackThrowingStage not found in the scene.");
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        switch (_currentState)
        {
            case State.Waiting:
                HandleWaitingState();

                break;

            case State.Caught:
                HandleCaughtState();

                break;
        }
    }
}
