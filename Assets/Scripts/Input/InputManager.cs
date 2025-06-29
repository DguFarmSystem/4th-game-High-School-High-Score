using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using System.Linq;
using System;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private GameObject testObject; // 테스트용 오브젝트
    private PlayerInput _playerInput;

    private static InputAction _tapAction;   // 단일 터치 입력을 위한 액션
    private static InputAction _touchAction; // 연속적인 터치(드래그) 입력을 위한 액션
    private static InputAction _dragAction;  // 드래그 델타 값을 받기 위한 액션

    public static event Action OnStageTapPerformed;   // 스테이지 탭이 수행되었을 때 발생하는 이벤트
    public static event Action OnStageTouchPerformed; // 스테이지 연속적인 터치(드래그)가 수행되었을 때 발생하는 이벤트

    private static Vector3 _touchWorldPos;      // 터치 월드 좌표
    private static Vector2 _delta;              // 드래그 시의 변화량
    private static Collider2D _touchedCollider; // 터치한 콜라이더
    
    public static Vector3 TouchWorldPos      => Instance? _touchWorldPos   : default; // 외부에서 접근 시 해당 프로퍼티 사용
    public static Vector2 Delta              => Instance? _delta           : default; // 외부에서 접근 시 해당 프로퍼티 사용
    public static Collider2D TouchedCollider => Instance? _touchedCollider : default; // 외부에서 접근 시 해당 프로퍼티 사용

    private void OnEnable()
    {
        // InputActions 활성화
        if (_playerInput != null)
        {
            // InputActions 설정
            _tapAction   = _playerInput.actions["Tap"];
            _touchAction = _playerInput.actions["Touch"];
            _dragAction  = _playerInput.actions["Drag"];

            _tapAction.performed   += tapPerformed;
            _touchAction.performed += touchPerformed;
            _dragAction.performed  += dragPerformed;
        }
    }
    private void OnDisable()
    {
        // InputActions 비활성화
        if (_playerInput != null)
        {
            _tapAction.performed   -= tapPerformed;
            _touchAction.performed -= touchPerformed;
            _dragAction.performed  -= dragPerformed;
        }
    }

    private void tapPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Tap performed!"); // 탭이 수행되었을 때 로그 출력

        // 탭 입력에 대한 스테이지 기믹 수행
        OnStageTapPerformed?.Invoke();
    }
    private void touchPerformed(InputAction.CallbackContext context)
    {
        // 터치 월드 좌표 계산
        Vector2 screenPos = context.ReadValue<Vector2>();
        Vector3 screenPos3d = new Vector3(screenPos.x, screenPos.y, Mathf.Abs(Camera.main.transform.position.z));
        _touchWorldPos = Camera.main.ScreenToWorldPoint(screenPos3d); // z = 0에서의 터치 월드 좌표

        // TEST CODE
        Debug.Log($"Touched WorldPosition: {_touchWorldPos}"); // 터치한 위치의 월드 좌표
        testObject.transform.position = _touchWorldPos;        // 테스트 오브젝트 이동

        // 터치한 위치에 있는 Collider2D를 가져옴
        Collider2D[] hits = Physics2D.OverlapPointAll(_touchWorldPos); // 터치한 위치에 있는 모든 Collider2D를 가져옴
        if (hits.Length > 0)
        {
            _touchedCollider = hits.OrderBy(hit => hit.GetComponent<SpriteRenderer>().sortingOrder).Last(); // 가장 위에 있는 콜라이더를 반환
            Debug.Log($"Top object: {_touchedCollider.name}");
        }
        else Debug.Log("No collider found at touch position.");

        // 연속적인 터치(드래그)에 대한 스테이지 기믹 수행
        OnStageTouchPerformed?.Invoke();
    }

    private void dragPerformed(InputAction.CallbackContext context)
    {
        _delta = context.ReadValue<Vector2>();
    }


    // Lifecycle methods
    public override void Awake()
    {
        base.Awake();

        _playerInput = GetComponent<PlayerInput>();
    }

    public void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) // UI 위에 포인터가 있을 경우
            _playerInput.actions.Disable(); // 모든 입력 액션 비활성화
        else 
            _playerInput.actions.Enable(); // 입력 액션 활성화
    }
}
