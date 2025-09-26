using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Stage;

public class OnStageInputManager : MonoBehaviour
{
    private SnackThrowingStage _snackThrowingStage;
    private InputManager _ipManager;
    private SnackDetector _snackDetector;

    public void OnTouch(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:

                if (InputManager.PressedCollider)
                {
                    _snackDetector = InputManager.PressedCollider.GetComponent<SnackDetector>();

                    if (_snackDetector)
                    {
                        _snackDetector.Ready2Catch();
                    }
                }

                break;

            case InputActionPhase.Canceled:

                if (_snackDetector)
                {
                    _snackDetector.Try2Catch();
                }

                _snackDetector = null;

                break;
        }
    }
    // ============ Lifecycle methods ============ //
    void Awake()
    {
        _ipManager = FindObjectOfType<InputManager>();
        _snackThrowingStage = FindObjectOfType<SnackThrowingStage>();
    }

    void Start()
    {
        if (_ipManager != null)
        {
            InputManager._pressAction.performed += OnTouch;
            InputManager._pressAction.canceled  += OnTouch;
        }
    }

    public void OnDisable()
    {

        if (_ipManager != null)
        {
            InputManager._pressAction.performed -= OnTouch;
            InputManager._pressAction.canceled  -= OnTouch;
        }
    }

    void Update()
    {
        if (_snackThrowingStage && (_snackThrowingStage.CurrentState == StageState.Clear
                                 || _snackThrowingStage.CurrentState == StageState.Over))
        {
            gameObject.SetActive(false);
        }
    }
}
