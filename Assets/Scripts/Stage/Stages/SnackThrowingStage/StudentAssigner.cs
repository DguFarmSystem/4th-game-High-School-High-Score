using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class StudentAssigner : MonoBehaviour
{
    /*
    private List<List<SnackDetector>> _students;

    void ActivateStudents(List<List<SnackDetector>> students, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            int row = Random.Range(0, students.Count);
            int col = Random.Range(0, students[row].Count);
            if ((row == 2 && col == 1) || students[row][col].gameObject.activeSelf)
            {
                i--;
                continue;
            }
            students[row][col].gameObject.SetActive(true);
            students[row][col].Distance = new Vector2(col - 1, 2 - row);
        }
    }

    public List<List<T>> ConvertTo2DList<T>(List<T> list, int rows, int cols)
    {
        if (list.Count != rows * cols)
        {
            throw new ArgumentException("The size of the 1D array does not match the specified dimensions.");
        }

        List<List<T>> result = new List<List<T>>();

        for (int i = 0; i < rows; i++)
        {
            List<T> currentRow = new List<T>();
            for (int j = 0; j < cols; j++)
            {
                int index = i * cols + j; // 1차원 리스트의 인덱스 계산
                currentRow.Add(list[index]);
            }
            result.Add(currentRow); // 행 추가
        }

        return result;
    }

    // ============ Lifecycle methods ============ //
    void Awake()
    {
        SnackThrowingStage stage = FindObjectOfType<SnackThrowingStage>();

        if (stage)
        {
            SnackDetector[] temp = GetComponentsInChildren<SnackDetector>(true);
            List<SnackDetector> temp12 = new List<SnackDetector>(temp);
            temp12.Insert(9, null);
            _students = ConvertTo2DList(temp12, 3, 4);

            ActivateStudents(_students, 3);
        }
    }
    */

    [SerializeField] private GameObject _columnOne;
    [SerializeField] private GameObject _columnTwo;
    [SerializeField] private GameObject _columnThree;

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
            students[num].Distance = 3 - num; // 학생의 위치 설정
        }
    }

    // ============ Lifecycle methods ============ //
    void Awake()
    {
        SnackThrowingStage stage = FindObjectOfType<SnackThrowingStage>();

        if (stage)
        {
            SnackDetector[] studentsOne = _columnOne.GetComponentsInChildren<SnackDetector>(true); // true로 설정하여 비활성화된 오브젝트도 포함
            SnackDetector[] studentsTwo = _columnTwo.GetComponentsInChildren<SnackDetector>(true);
            SnackDetector[] studentsThree = _columnThree.GetComponentsInChildren<SnackDetector>(true);

            ActivateStudents(studentsOne, 1);
            ActivateStudents(studentsTwo, 1);
            ActivateStudents(studentsThree, 1);
        }
        else
        {
            Debug.LogWarning("SnackThrowingStage not found in the scene.");
        }
    }
}
