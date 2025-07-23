using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Seat : MonoBehaviour
{
    [SerializeField] private GameObject _studentObject; // 학생 오브젝트

    public bool IsOccupied { get; private set; } = false; // 좌석이 점유되었는지 여부

    // private static bool _isAllSeatOccupied = false; // 모든 좌석이 점유되었는지 여부
    // 모든 좌석이 점유된 경우 목적지가 없어 스테이지 자체가 오류 발생
    // 그러나 그 확률이 극히 낮아 따로 장치는 하지 않았음

    // ============ Lifecycle methods ============ //
    void Awake()
    {
        ClassRoomStage stage = FindObjectOfType<ClassRoomStage>();

        if (stage)
        {
            int stageLevel = stage.stageLevel;
            bool flag = (Random.Range(0, 2) == 0); // 무작위로 true 또는 false 설정

            switch (stageLevel)
            {
                case 1:

                    break;

                case 2:
                    if (flag)
                    {
                        _studentObject.SetActive(true);
                        IsOccupied = true;
                        _studentObject.GetComponent<Student>().tacklingTime = 1.0f;
                    }

                    break;

                case 3:
                    if (flag)
                    {
                        _studentObject.SetActive(true);
                        IsOccupied = true;
                        _studentObject.GetComponent<Student>().tacklingTime = 2.0f;
                    }

                    break;

                default:
                    Debug.LogWarning("Unknown stage level: " + stageLevel);

                    break;
            }
        }
    }
    void Start()
    {

    }
    
    void Update()
    {
    
    }
}
