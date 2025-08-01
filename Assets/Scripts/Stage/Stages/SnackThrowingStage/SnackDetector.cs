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
                    gameObject.SetActive(false);
                }

                break;
        }
    }

    private void HandleWaitingState()
    {
        if (_isPressing)
        {
            if (_pressedTime + Time.deltaTime > 4f) _pressedTime = 0f;
            else _pressedTime += Time.deltaTime;
            Debug.Log($"Pressed Time: {_pressedTime}");
        }
        else
        {
            if (_pressedTime >= Distance)
            {
                /* if (선생님이 뒤돌아 보고 있는 상태라면)
                {
                    ex) 게임 오버
                }
                else */TransitionToState(State.Caught);
            }

            if (!Mathf.Approximately(_pressedTime, 0f)) _pressedTime = 0f;
        }
    }

    private void HandleCaughtState()
    {
        Debug.Log("Snack is caught, processing...");
    }

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
