using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageIntervalCSController : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] private Image _backgroundSuccess;
    [SerializeField] private Image _backgroundFailure;

    [Header("Character")]
    [SerializeField] private Image _character;
    [SerializeField] private Sprite _characterDefaultSprite;
    [SerializeField] private Sprite _characterSuccessSprite;
    [SerializeField] private Sprite _characterFailureSprite;

    [Header("HP")]
    [SerializeField] private List<GameObject> _HPGameObjects;

    [Header("Stage")]
    [SerializeField] private Image _currentStageImage;
    [SerializeField] private List<Sprite> _stageCountSprites;
    [SerializeField] private Image _bossStageImage;

    private int _previousHP;
    private int _stageIndex => StageManager.Instance.GetCurrentStage();
    private bool _startFlag = true;

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    private IEnumerator StageIntervalCutscene()
    {
        // HP 출력
        ShowHP();

        /* ============ 스테이지 클리어 여부에 따른 출력 ============ => 첫 스테이지면 무시 */
        if (!_startFlag)
        {
            // 배경화면 출력
            if (StageManager.Instance.GetStageCleared())
            {
                if (_backgroundSuccess.sprite) _backgroundSuccess.enabled = true;
            }
            else _backgroundFailure.enabled = true;

            // 캐릭터 출력 
            if (StageManager.Instance.GetStageCleared()) _character.sprite = _characterSuccessSprite;
            else _character.sprite = _characterFailureSprite;

            yield return new WaitForSeconds(2.5f);
        }

        /* ============ 기본 출력 ============ */

        foreach (var hp in _HPGameObjects)
        {
            hp.GetComponent<Animator>().speed = 1f;
        }

        _character.sprite = _characterDefaultSprite;
        _backgroundSuccess.enabled = false;
        _backgroundFailure.enabled = false;

        yield return new WaitForSeconds(1.5f);

        yield return ShowStageInfo();
        
        _startFlag = false;
        StageManager.Instance.ShowComplete(); // 다 출력했다고 알림
    }

    private void ShowHP()
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

                IEnumerator HPDecreaseCoroutine(int index)
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
            }
        }

        for (int i = 0; i < currentHP; i++)
        {
            if (_stageIndex > 9)
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
            }

            _HPGameObjects[i].GetComponent<Image>().enabled = true;
        }

        _previousHP = currentHP;
    }

    private IEnumerator ShowStageInfo()
    {
        if (_stageIndex < _stageCountSprites.Count)
        {
            _currentStageImage.sprite = _stageCountSprites[_stageIndex];
            _currentStageImage.enabled = true;
        }

        yield return new WaitForSeconds(1.5f);

        if (_stageIndex == _stageCountSprites.Count - 1) // 보스 스테이지 알림
        {
            _bossStageImage.enabled = true;
            yield return new WaitForSeconds(2.5f);
        }
    }

    /* =========== Lifecycle Methods =========== */
    private void Awake()
    {
        _previousHP = StageManager.Instance.GetHP();
    }
    private void OnEnable()
    {
        StartCoroutine(StageIntervalCutscene());
    }

    private void OnDisable()
    {
        _backgroundSuccess.enabled = false;
        _backgroundFailure.enabled = false;
        _currentStageImage.enabled = false;
        _bossStageImage.enabled = false;

        _character.sprite = _characterDefaultSprite;

        foreach (var hp in _HPGameObjects)
        {
            hp.GetComponent<Image>().enabled = false;
        }
    }
}
