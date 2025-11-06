using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public abstract class StageIntervalCSController : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] protected List<GameObject> _HPGameObjects;

    [Header("Stage")]
    [SerializeField] protected Image _currentStageImage;
    [SerializeField] protected List<Sprite> _stageCountSprites;
    [SerializeField] protected Image _notificationImage;
    [SerializeField] protected Sprite _bossStageSprite;
    [SerializeField] protected Sprite _gameClearSprite;
    [SerializeField] protected Sprite _gameOverSprite;
    [SerializeField] protected Sprite _speedUpSprite;

    protected abstract IEnumerator StageIntervalCutscene();

    protected int _previousHP;
    protected int _stageIndex => StageManager.Instance.GetCurrentStage();
    protected int _maxStageIndex
    {
        get
        {
            switch (StageManager.Instance.GetGameMode())
            {
                case StageManager.GameMode.Tutorial:
                    return 9;
                case StageManager.GameMode.Normal:
                    return 12;
                case StageManager.GameMode.Infinite:
                    return int.MaxValue;
                default:
                    return 12;
            }
        }
    }

    protected virtual void ShowHP()
    {
        int currentHP = StageManager.Instance.GetHP();

        if (currentHP < _previousHP)
        {
            foreach (var hp in _HPGameObjects)
            {
                hp.GetComponent<Animator>().speed = 0f;
            }

            for (int i = _previousHP - 1; i >= currentHP; i--)
            {
                StartCoroutine(HPDecreaseCoroutine(i));
            }
        }

        for (int i = 0; i < currentHP; i++)
        {
            if (_stageIndex > _maxStageIndex)
            {
                switch (i)
                {
                    case 0:
                        _HPGameObjects[i].GetComponent<Animator>().Play("HP_1"); // animation state 호출
                        break;
                    case 1:
                        _HPGameObjects[i].GetComponent<Animator>().Play("HP_2");
                        break;
                    case 2:
                        _HPGameObjects[i].GetComponent<Animator>().Play("HP_3");
                        break;
                    case 3:
                        _HPGameObjects[i].GetComponent<Animator>().Play("HP_4");
                        break;
                }

                RectTransform rt = _HPGameObjects[i].GetComponent<RectTransform>();
                Vector2 pos = rt.anchoredPosition;
                pos.y += -45f; // 위치 조정
                rt.anchoredPosition = pos;
            }

            _HPGameObjects[i].GetComponent<Image>().enabled = true; 
        }

        _previousHP = currentHP;
    }

    protected IEnumerator HPDecreaseCoroutine(int index) // HP 감소 애니메이션 코루틴
    {
        RectTransform rt = _HPGameObjects[index].GetComponent<RectTransform>();
        Vector2 originalPos = rt.anchoredPosition;
        Vector2 targetPos = originalPos + new Vector2(0, -2000f);
        _HPGameObjects[index].GetComponent<Image>().enabled = true;
        yield return null;

        float elapsed = 0f;
        float gravity = 2f * 2000f / (1f * 1f); // 중력 가속도 계산

        while (true)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 1f;
            // 중력 공식: y = 0.5 * g * t^2
            float offsetY = 0.5f * gravity * t * t;
            rt.anchoredPosition = originalPos + new Vector2(0, -offsetY);
            yield return null;

            if (rt.anchoredPosition.y <= targetPos.y)
            {
                _HPGameObjects[index].GetComponent<Image>().enabled = false;
                rt.anchoredPosition = originalPos; // 위치 초기화
                break;
            }
        }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    /* =========== Lifecycle Methods =========== */

    protected virtual void Awake()
    {
        _previousHP = StageManager.Instance.GetHP();
    }
    protected virtual void OnEnable()
    {
        StartCoroutine(StageIntervalCutscene());
    }

    protected abstract void OnDisable();
}
