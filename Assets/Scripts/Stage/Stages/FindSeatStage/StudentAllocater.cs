using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class StudentAllocater : MonoBehaviour
{
    [SerializeField] private GameObject _columnOne;
    [SerializeField] private GameObject _columnTwo;
    [SerializeField] private GameObject _columnThree;

    void ActivateStudents(Student[] students, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            int num = Random.Range(0, students.Length);

            if (students[num].gameObject.activeSelf)
            {
                i--;
                continue; // 이미 활성화된 학생은 건너뜀
            }
            students[num].gameObject.SetActive(true);
        }
    }
    
    // ============ Lifecycle methods ============ //
    void Start()
    {
        FindSeatStage stage = FindObjectOfType<FindSeatStage>();

        if (stage)
        {
            int stageLevel = StageManager.Instance.GetDifficulty();

            Student[] studentsOne   = _columnOne.GetComponentsInChildren<Student>(true); // true로 설정하여 비활성화된 오브젝트도 포함
            Student[] studentsTwo   = _columnTwo.GetComponentsInChildren<Student>(true);
            Student[] studentsThree = _columnThree.GetComponentsInChildren<Student>(true);

            switch (stageLevel)
            {
                case 1: // 1분단 1명, 3분단 1명
                    ActivateStudents(studentsOne, 1);
                    ActivateStudents(studentsThree, 1);

                    break;

                case 2: // 1분단 1명, 2분단 1명, 3분단 1명
                    ActivateStudents(studentsOne, 1);
                    ActivateStudents(studentsTwo, 1);
                    ActivateStudents(studentsThree, 1);

                    break;

                case 3: // 1분단 2명, 2분단 2명, 3분단 2명
                    ActivateStudents(studentsOne, 2);
                    ActivateStudents(studentsTwo, 2);
                    ActivateStudents(studentsThree, 2);

                    break;

                default:
                    Debug.LogWarning("Unknown stage level: " + stageLevel);

                    break;
            }
        }
    }
}
