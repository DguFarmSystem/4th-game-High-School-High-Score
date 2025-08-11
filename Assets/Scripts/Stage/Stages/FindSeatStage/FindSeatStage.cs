using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;
using System.Linq;
using UnityEditor.Rendering;

public class FindSeatStage : StageNormal
{
    [SerializeField] public int stageLevel;
    
    private Student[] _allSeats;
    private Student[] _noneOccupiedSeats;
    public Transform TargetDesk { get; private set; }

    public bool stageClearFlag = false;
    public StageState CurrentState => CurrentStageState;

    // TEST CODE
    [SerializeField] private GameObject _greenSphere;
    [SerializeField] private GameObject _redSphere;

    public override void OnStageStart()
    {
        base.OnStageStart();
    }
    protected override void OnStageEnd()
    {
        base.OnStageEnd();
    }

    protected override void OnStageClear()
    {
        base.OnStageClear();
    }

    private void OnStageEndedGimmik(bool isStageCleared)
    {

        if (isStageCleared)
        {
            //TEST CODE
            Debug.Log("Stage cleared!");
            _greenSphere.SetActive(true);
        }
        else
        {
            //TEST CODE
            Debug.Log("Stage failed!");
            _redSphere.SetActive(true);
        }
    }

    // ============ Lifecycle methods ============ //
    public void OnEnable()
    {
        OnStageEnded += OnStageEndedGimmik;
    }

    public void OnDisable()
    {
        OnStageEnded -= OnStageEndedGimmik;
    }

    void Start()
    {
        _allSeats = FindObjectsOfType<Student>(true);
        _noneOccupiedSeats = _allSeats.Where(seat => !seat.gameObject.activeSelf).ToArray();
        Student TargetSeat = _noneOccupiedSeats[Random.Range(0, _noneOccupiedSeats.Length)];

        foreach (Transform child in TargetSeat.transform.parent)
        {
            if (child.CompareTag("Desk"))
            {
                TargetDesk = child;
                break;
            }
        }
        TargetDesk.gameObject.GetComponent<SpriteRenderer>().color = Color.green;

        // 스테이지 시작
        OnStageStart();
    }

    
    void Update()
    {
        if (stageClearFlag && CurrentStageState == StageState.Playing)
        {
            OnStageClear(); // 모든 조건이 완료되면 스테이지 클리어 처리
        }
    }
}
