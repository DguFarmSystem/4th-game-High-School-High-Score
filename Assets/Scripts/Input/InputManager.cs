using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private GameObject testObject; // 테스트용 오브젝트
    private PlayerInput _playerInput;

    private InputAction _touchPressAction;
    private InputAction _touchPositionAction;
    private InputAction _dragAction;

    private Vector3 _touchWorldPos;
    public override void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        if (_playerInput != null)
        {
            // InputActions 설정
            //touchPressAction = _playerInput.actions["TouchPress"]; // 짧은 탭 입력은 현재로선 필요 없을 것으로 판단
            _touchPositionAction = _playerInput.actions["TouchPosition"];
            //DragAction = _playerInput.actions["Drag"];

            //touchPressAction.performed += (context) => { Debug.Log("Touch Pressed"); };
            _touchPositionAction.started += (context) => { /* 손이 닿았을 때의 이벤트 */ };
            _touchPositionAction.performed += touchPerformed;
            _touchPositionAction.canceled += (context) => { /* 손을 떼었을 때의 입력 */ };
            //DragAction.performed += (context) => { Debug.Log($"Delta Position: {context.ReadValue<Vector2>()}"); }; 
            // touchPositionAction 입력만으로 드래그시 복수 콜백
            // DragAction, 즉, delta를 이용한 입력은 짧은 시간 이내에 얼마나 빠르게 드래그 했는지에 대한 정보가 필요할 때 사용하면 될 듯
        }
    }

    private void touchPerformed(InputAction.CallbackContext context)
    {
        Vector2 screenPos = context.ReadValue<Vector2>();
        Vector3 screenPos3d = new Vector3(screenPos.x, screenPos.y, Mathf.Abs(Camera.main.transform.position.z));
        _touchWorldPos = Camera.main.ScreenToWorldPoint(screenPos3d);
    }
    
    public void LateUpdate()
    {
        if (_touchPositionAction.WasPerformedThisFrame())
        {

            if (EventSystem.current.IsPointerOverGameObject())
            {
                // ui 터치 시 입력 이벤트
                return;
            }

            testObject.transform.position = _touchWorldPos;
            Debug.Log($"Touch Destination: {_touchWorldPos}");

            Collider2D hit = Physics2D.OverlapPoint((Vector2)_touchWorldPos);
            if (hit != null)
            {
                Debug.Log($"Touched Object: {hit.name}");
            }
        }
    }

}
