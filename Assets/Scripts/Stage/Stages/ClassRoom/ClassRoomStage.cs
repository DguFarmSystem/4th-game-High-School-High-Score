using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;
using System.Linq;
using UnityEditor.Rendering;

public class ClassRoomStage : StageNormal
{
    [SerializeField] public int stageLevel;
    
    private Seat[] _allSeats;
    private Seat[] _noneOccupiedSeats;
    public Seat TargetSeat { get; private set; }

    public StageState CurrentStageState => currentStageState;
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

    // ============ Lifecycle methods ============ //
    void Start()
    {
        _allSeats = FindObjectsOfType<Seat>();
        _noneOccupiedSeats = _allSeats.Where(seat => !seat.IsOccupied).ToArray();
        Debug.Log($"None occupied seats count: {_noneOccupiedSeats.Length}");
        TargetSeat = _noneOccupiedSeats[Random.Range(0, _noneOccupiedSeats.Length)];

        Transform desk = null;

        foreach (Transform child in TargetSeat.transform)
        {
            if (child.CompareTag("Desk"))
            {
                desk = child;
                break;
            }
        }
        desk.gameObject.GetComponent<SpriteRenderer>().color = Color.green;

        // 스테이지 시작
        OnStageStart();
    }

    
    void Update()
    {
        if (/*자리 찾음 &&*/ CurrentStageState == StageState.Playing)
        {
            OnStageClear(); // 모든 조건이 완료되면 스테이지 클리어 처리
        }
    }
}
