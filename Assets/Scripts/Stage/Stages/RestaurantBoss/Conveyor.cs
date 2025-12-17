using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Conveyor : MonoBehaviour
{
    [Header("Conveyor Settings")]
    [SerializeField, Min(2)] private int _HowManyConveyorPositions = 2;
    [SerializeField] private Vector3 _conveyorStartPosition;
    [SerializeField] private Vector3 _conveyorEndPosition;

    [Space(10)]

    [Header("Items")]
    [SerializeField] private GameObject _puddingPrefab;
    [SerializeField] private GameObject _spoonSetPrefab;
    [SerializeField] private GameObject _cakePrefab;
    [SerializeField] private GameObject _cupPrefab;
    [SerializeField] private GameObject _goldenSpongePrefab;
    [SerializeField] private GameObject _timerPrefab;

    [Space(10)]

    [Header("Item Icons")]
    [SerializeField] private GameObject _cakeIcon;
    [SerializeField] private GameObject _cupIcon;

    [Space(10)]

    [Header("SFX")]
    [SerializeField] private AudioClip _normalItemSFX;
    [SerializeField] private AudioClip _timerSFX;

    [Space(10)]

    [Header("Test Settings")]

    private List<Vector3> _conveyorPositions = new List<Vector3>();
    private List<GameObject> _itemSpawnList;
    public Queue<GameObject> ConveyorQueue { get; private set; } = new Queue<GameObject>();

    private int _itemSpawnIndex = 0;

    public ConveyorItem NextItem => ConveyorQueue.Peek().GetComponent<ConveyorItem>();

    public void RemoveNextItem(bool direction)
    {
        StartCoroutine(MoveConveyor(direction)); // 컨베이어 이동
        StartCoroutine(GetNextItem());
    }

    private IEnumerator MoveConveyor(bool direction)
    {
        GameObject item = ConveyorQueue.Dequeue();

        // 아이템의 특수 메소드 호출 //
        item.GetComponent<ConveyorItem>().OnRemovedFromConveyor();

        if (item.GetComponent<ConveyorItem>() is TimerItem) // 타이머는 이미 파괴됨
        {
            SoundManager.Instance.PlaySFX(_timerSFX);
            yield break;
        }

        SoundManager.Instance.PlaySFX(_normalItemSFX);
        // 아이템 옆으로 밀어냄
        float elapsedTime = 0f;
        float duration = 3f;
        float speed = 6.2f;
        Vector3 offset = direction ? Vector3.right : Vector3.left;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            item.transform.Translate(offset * Time.deltaTime * speed);

            yield return null;
        }

        Destroy(item);
    }

    private IEnumerator GetNextItem()
    {
        // 아이템 내려 보냄
        float elapsedTime = 0f;
        float duration = 0.05f;

        List<GameObject> items = new List<GameObject>(ConveyorQueue);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            for (int i = 0; i < items.Count; i++)
            {
                items[i].transform.position = Vector3.Lerp(
                    _conveyorPositions[i + 1],
                    _conveyorPositions[i],
                    t
                );
            }

            yield return null;
        }

        // 위치 보정
        for (int i = 0; i < items.Count; i++)
        {
            items[i].transform.position = _conveyorPositions[i];
        }

        if (_itemSpawnIndex != 0 && _itemSpawnIndex % 50 == 0) // 타이머 추가
        {
            _itemSpawnList.Add(_timerPrefab);
        }

        // 아이템 스폰 프로세스 

        // 콤보 시 황금수세미 스폰
        if (LeftRightBtn.Combo == 30 || LeftRightBtn.Combo == 70 || LeftRightBtn.Combo == 120)
        {
            GameObject initialSponge = SpawnItem(_goldenSpongePrefab, _conveyorEndPosition);

            Action handler = null;
            handler = () => // 최초 사용 시 모든 아이템을 황금수세미로 변환
            {
                GoldenSpongeItem.SpongeCount = 10;
                // 모든 기존 아이템을 황금수세미로 변환
                foreach (var item in ConveyorQueue)
                {
                    item.GetComponent<ConveyorItem>().SwitchToGoldenSponge();
                }
                initialSponge.GetComponent<GoldenSpongeItem>().OnInitialSpongeUsed -= handler;
            };

            initialSponge.GetComponent<GoldenSpongeItem>().OnInitialSpongeUsed += handler;

            ConveyorQueue.Enqueue(initialSponge);

            yield break;
        }

        // 맨 앞에 새로운 아이템 추가
        if (_itemSpawnIndex == 30) // 케이크 추가
        {
            GameObject cakeItem = SpawnItem(_cakePrefab, _conveyorEndPosition);
            if (GoldenSpongeItem.SpongeCount > 0)
                cakeItem.GetComponent<ConveyorItem>().SwitchToGoldenSponge();
            ConveyorQueue.Enqueue(cakeItem);
            _itemSpawnList.Add(_cakePrefab);
            _cakeIcon.GetComponent<Image>().enabled = true;

            yield break;
        }

        if (_itemSpawnIndex == 60) // 컵 추가
        {
            GameObject cupItem = SpawnItem(_cupPrefab, _conveyorEndPosition);
            if (GoldenSpongeItem.SpongeCount > 0)
                cupItem.GetComponent<ConveyorItem>().SwitchToGoldenSponge();
            ConveyorQueue.Enqueue(cupItem);
            _itemSpawnList.Add(_cupPrefab);
            _cupIcon.GetComponent<Image>().enabled = true;

            yield break;
        }

        GameObject randomItem = _itemSpawnList[Random.Range(0, _itemSpawnList.Count)];

        if (randomItem == _timerPrefab)
        {
            _itemSpawnList.Remove(_timerPrefab); // 타이머는 한 번만 추가
        }

        GameObject item = SpawnItem(randomItem, _conveyorEndPosition);

        if (GoldenSpongeItem.SpongeCount > 0)
            item.GetComponent<ConveyorItem>().SwitchToGoldenSponge();
        ConveyorQueue.Enqueue(item);
}

    private void InitializeConveyor()
    {
        _conveyorPositions.Clear();

        Vector3 step = (_conveyorEndPosition - _conveyorStartPosition) / (_HowManyConveyorPositions - 1);

        for (int i = 0; i < _HowManyConveyorPositions; i++)
        {
            Vector3 position = _conveyorStartPosition + step * i;

            GameObject randomItem = _itemSpawnList[Random.Range(0, _itemSpawnList.Count)];
            ConveyorQueue.Enqueue(SpawnItem(randomItem, position));
            _conveyorPositions.Add(position);
        }
    }

    private GameObject SpawnItem(GameObject itemPrefab, Vector3 position)
    {
        GameObject item = Instantiate(itemPrefab, position, Quaternion.identity, transform);
        _itemSpawnIndex++;

        return item;
    }

    // ============= Life Cycle ============= //
    void Awake()
    {
        _itemSpawnList = new List<GameObject>
        {
            _puddingPrefab,
            _spoonSetPrefab,
        };
    }

    void Start()
    {
        InitializeConveyor();
    }
}
