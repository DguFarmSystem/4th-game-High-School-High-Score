using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using System.Linq;
using System;

public class InputManager : Singleton<InputManager>
{
    //[SerializeField] private GameObject testObject; // 테스트용 오브젝트
    private PlayerInput _playerInput;

    public static InputAction _tapAction;   // 탭 입력을 위한 액션
    public static InputAction _pressAction;   // 지속 입력을 위한 액션
    public static InputAction _positionAction; // 터치 위치 값을 받기 위한 액션
    public static InputAction _dragAction;  // 드래그 델타 값을 받기 위한 액션

    public static event Action OnStageTapPerformed;

    private static bool _isPressing = false; // 터치 상태를 나타내는 변수
    private static Vector2 _screenPos;         // 터치 스크린 좌표
    private static Vector3 _touchWorldPos;      // 터치 월드 좌표
    private static Vector2 _worldDelta;         // 드래그 시의 변화량
    private static Collider2D _tappedCollider; // 탭한 콜라이더
    private static Collider2D _touchedCollider; // 터치한 콜라이더
    private static Collider2D _pressedCollider; // 선택된 콜라이더

    public static bool IsPressing => Instance ? _isPressing : false; // 외부에서 터치 상태를 확인할 수 있는 프로퍼티
    public static Vector3 TouchWorldPos => Instance ? _touchWorldPos : default; // 외부에서 접근 시 해당 프로퍼티 사용
    public static Vector2 Delta => Instance ? _worldDelta : default; // 외부에서 접근 시 해당 프로퍼티 사용
    public static Collider2D TappedCollider => Instance ? _tappedCollider : default; // 외부에서 접근 시 해당 프로퍼티 사용
    public static Collider2D TouchedCollider => Instance ? _touchedCollider : default; // 외부에서 접근 시 해당 프로퍼티 사용
    public static Collider2D PressedCollider => Instance ? _pressedCollider : default; // 외부에서 접근 시 해당 프로퍼티 사용

    private void OnEnable()
    {
        // InputActions 활성화
        if (_playerInput != null)
        {
            // InputActions 설정
            _tapAction = _playerInput.actions["Tap"];
            _pressAction = _playerInput.actions["Press"];
            _positionAction = _playerInput.actions["TouchPosition"];
            _dragAction = _playerInput.actions["Drag"];

            _tapAction.performed += tapPerformed;

            _pressAction.performed += pressPerformed;
            _pressAction.canceled += pressCanceled;

            _positionAction.performed += getTouchPosition;
            _positionAction.canceled += initTouchPosition;

            _dragAction.performed += getDragDelta;
            _dragAction.canceled += initDragDelta;
        }
    }
    private void OnDisable()
    {
        // InputActions 비활성화
        if (_playerInput != null)
        {
            _tapAction.performed -= tapPerformed;

            _pressAction.performed -= pressPerformed;
            _pressAction.canceled -= pressCanceled;

            _positionAction.performed -= getTouchPosition;
            _positionAction.canceled -= initTouchPosition;

            _dragAction.performed -= getDragDelta;
            _dragAction.canceled -= initDragDelta;
        }
    }

    private void tapPerformed(InputAction.CallbackContext context)
    {
        //Vector2 pos = Touchscreen.current.primaryTouch.position.ReadValue();
        OnStageTapPerformed?.Invoke(); // 탭이 발생했을 때 이벤트 호출
    }

    private void pressPerformed(InputAction.CallbackContext context)
    {
        _isPressing = true; // 터치 상태를 true로 설정
        _pressedCollider = GetTouchedCollider2D(_touchWorldPos); // 현재 터치한 콜라이더를 선택된 콜라이더로 설정
    }

    private void pressCanceled(InputAction.CallbackContext context)
    {
        _isPressing = false; // 터치 상태를 false로 설정
        _pressedCollider = null; // 터치가 해제되면 선택된 콜라이더를 null로 설정
    }

    private void getTouchPosition(InputAction.CallbackContext context)
    {
        // 터치 월드 좌표 계산
        _screenPos = context.ReadValue<Vector2>();

        Vector3 screenPos3d = new Vector3(_screenPos.x, _screenPos.y, Mathf.Abs(Camera.main.transform.position.z));
        _touchWorldPos = Camera.main.ScreenToWorldPoint(screenPos3d); // z = 0에서의 터치 월드 좌표

        _touchedCollider = GetTouchedCollider2D(_touchWorldPos);

        // TEST CODE
        //testObject.transform.position = _touchWorldPos;

        // UI 터치 여부 확인
        //if (IsPointerOverUI(screenPos)) return; // UI를 터치한 경우, 월드 상호작용을 중단
    }
    private void initTouchPosition(InputAction.CallbackContext context)
    {
        _screenPos = Vector2.zero; // 터치가 끝나면 스크린 좌표 초기화
        _touchWorldPos = Vector3.zero; // 터치 월드 좌표 초기화
        _touchedCollider = null; // 터치한 콜라이더 초기화
    }

    private void getDragDelta(InputAction.CallbackContext context)
    {
        Vector2 screenDelta = context.ReadValue<Vector2>();
        _worldDelta = Camera.main.ScreenToWorldPoint(screenDelta + _screenPos)
                    - Camera.main.ScreenToWorldPoint(_screenPos);

    }
    private void initDragDelta(InputAction.CallbackContext context)
    {
        _worldDelta = Vector2.zero; // 드래그가 끝나면 변화량 초기화
    }

    // 터치 위치의 Collider2D를 반환하는 메서드
    private Collider2D GetTouchedCollider2D(Vector3 position)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position); // 터치한 위치에 있는 모든 Collider2D를 가져옴
        Collider2D collider = null;

        if (hits.Length > 0)
        {
            collider = hits
                .Where(hit => hit.GetComponent<SpriteRenderer>() != null) // SpriteRenderer가 null이 아닌 경우만 선택
                .OrderBy(hit => hit.GetComponent<SpriteRenderer>().sortingOrder) // sortingOrder 기준으로 정렬
                .LastOrDefault(); // 가장 위에 있는 콜라이더를 반환 (없으면 null 반환)
            //Debug.Log($"Top object: {collider?.name}");
        }
        else
        {
            collider = null;
            //Debug.Log("No collider found at touch position.");
        }

        return collider;
    }

    // UI 터치 여부를 확인하는 메서드
    private bool IsPointerOverUI(Vector2 screenPos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPos // raycast 위치는 현재 터치 위치로 설정
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0; // UI 요소가 감지되면 true 반환
    }

    // Lifecycle methods
    public override void Awake()
    {
        base.Awake();

        /*
        #if UNITY_EDITOR

        // Touchscreen 디바이스를 설정
        var touchscreen = InputSystem.GetDevice<Touchscreen>();
            if (touchscreen != null)
            {
                Debug.Log("Touchscreen 디바이스가 설정되었습니다.");
            }
            else
            {
                Debug.LogWarning("Touchscreen 디바이스를 찾을 수 없습니다.");
            }
        #endif
        */

        _playerInput = GetComponent<PlayerInput>();
    }
    
}
