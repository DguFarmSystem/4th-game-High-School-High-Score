using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthObjectiveSpawner : MonoBehaviour
{
    [Header("GameObject Settings")]
    [SerializeField] private Collider2D _objectiveUpperSpawnArea;
    [SerializeField] private Collider2D _objectiveLowerSpawnArea;

    [Space(10)]
    [Header("Objective Prefabs")]
    [SerializeField] private GameObject _objectiveBubble1;
    [SerializeField] private GameObject _objectiveBubble2;
    [SerializeField] private GameObject _objectiveBubble3;
    [SerializeField] private GameObject _objectiveVirus1;
    [SerializeField] private GameObject _objectiveVirus2;

    private Bounds upperSpawnBounds;
    private Bounds lowerSpawnBounds;

    private GameObject[][] spawnPool = new GameObject[4][];

    public void Initialize(int stageLevel)
    {
        upperSpawnBounds = _objectiveUpperSpawnArea.bounds;
        lowerSpawnBounds = _objectiveLowerSpawnArea.bounds;

        spawnPool = new GameObject[][] {
            new GameObject[] { _objectiveBubble1, _objectiveBubble3 },
            new GameObject[] { _objectiveVirus1 },
            new GameObject[] { _objectiveBubble1, _objectiveBubble2, _objectiveBubble3 },
            new GameObject[] { _objectiveVirus1, _objectiveVirus2 }
        };

        SpawnObjectives(stageLevel);
    }

    private void SpawnObjectives(int stageLevel)
    {
        GameObject[] objectivesToSpawn = spawnPool[stageLevel - 1];
        int spawnCount = 0;
        switch (stageLevel)
        {
            case 1: spawnCount = 2; break;
            case 2: spawnCount = 4; break;
            case 3: spawnCount = 4; break;
            case 4: spawnCount = 6; break;
            default: spawnCount = 0; break;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos;

            int randomIdx = Random.Range(0, objectivesToSpawn.Length);
            GameObject prefab = objectivesToSpawn[randomIdx];

            do
            {
                // 범위 내에서 랜덤 위치 선정
                randomPos = new Vector3(
                    Random.Range(lowerSpawnBounds.min.x, lowerSpawnBounds.max.x),
                    Random.Range(lowerSpawnBounds.min.y, upperSpawnBounds.max.y),
                    0f
                );

                if (lowerSpawnBounds.Contains(randomPos) || upperSpawnBounds.Contains(randomPos))
                {
                    // 위치가 유효함
                    break;
                }

            } while (true); // 위치가 유효하지 않으면 다시 반복

            Instantiate(prefab, randomPos, Quaternion.identity);
        }
    }
}
