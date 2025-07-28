using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class StudentAssigner : MonoBehaviour
{
    void ActivateStudents(SnackDetector[] students, int amount = 1)
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
    void Awake()
    {
        SnackThrowingStage stage = FindObjectOfType<SnackThrowingStage>();

        if (stage)
        {
            SnackDetector[] students = GetComponentsInChildren<SnackDetector>(true);

            ActivateStudents(students, 3);
        }
    }
}
