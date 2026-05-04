using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthObjective : MonoBehaviour
{
    public static int totalObjectives { get; private set; } = 0; // 총 목적물 수

    [SerializeField] private GameObject _nextObjective; // 다음 목적물

    public void Interact()
    {
        if (_nextObjective != null)
        {
            Instantiate(_nextObjective, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        totalObjectives++;
        Debug.Log("Objective spawned. Total objectives: " + totalObjectives);
    }

    private void OnDestroy()
    {
        totalObjectives--;
        Debug.Log("Objective destroyed. Total objectives: " + totalObjectives);
    }
}
