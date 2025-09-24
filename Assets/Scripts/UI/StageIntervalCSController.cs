using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private List<GameObject> _HPs;

    [Header("Stage")]
    [SerializeField] private Image _currentStageImage;
    [SerializeField] private List<Sprite> _stageCountSprites;
    [SerializeField] private Image _bossStageImage;

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

            yield return new WaitForSeconds(1.5f);
        }

        /* ============ 기본 출력 ============ */

        _character.sprite = _characterDefaultSprite;
        _backgroundSuccess.enabled = false;
        _backgroundFailure.enabled = false;

        yield return new WaitForSeconds(1.5f);

        yield return ShowStageInfo();
        
        _startFlag = false;
        StageManager.Instance.ShowComplete();
    }

    private void ShowHP()
    {
        int hpCount = StageManager.Instance.GetHP();

        for (int i = 0; i < hpCount; i++)
        {
            if (_stageIndex > 9)
            {
                switch (i)
                {
                    case 0:
                        _HPs[i].GetComponent<Animator>().Play("HP_1");
                        break;
                    case 1:
                        _HPs[i].GetComponent<Animator>().Play("HP_2");
                        break;
                    case 2:
                        _HPs[i].GetComponent<Animator>().Play("HP_3");
                        break;
                    case 3:
                        _HPs[i].GetComponent<Animator>().Play("HP_4");
                        break;
                }
            }

            _HPs[i].GetComponent<Image>().enabled = true;
        }
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
            yield return new WaitForSeconds(1.5f);
        }
    }

    /* =========== Lifecycle Methods =========== */
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

        foreach (var hp in _HPs)
        {
            hp.GetComponent<Image>().enabled = false;
        }
    }
}
