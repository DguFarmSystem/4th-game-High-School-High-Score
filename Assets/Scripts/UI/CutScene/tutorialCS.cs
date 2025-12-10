using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutorialCS : CutSceneController
{
    [Header("Background")]
    [SerializeField] private Image _backgroundSuccess;
    [SerializeField] private Image _backgroundFailure;
    [SerializeField] private Sprite _bgAllClear;

    [Header("Character")]
    [SerializeField] private Image _character;
    [SerializeField] private Sprite _characterDefaultSprite;
    [SerializeField] private Sprite _characterSuccessSprite;
    [SerializeField] private Sprite _characterFailureSprite;

    private bool _startFlag = true;

    protected override IEnumerator StartCutScene()
    {
        // HP 출력
        ShowHP();

        /* ============ 스테이지 클리어 여부에 따른 출력 ============ => 첫 스테이지면 무시 */
        if (!_startFlag)
        {
            // 배경화면 출력
            if (StageManager.Instance.GetStageCleared())
            {
                if (_stageIndex > _maxStageIndex) // 모든 스테이지 클리어
                {
                    _backgroundSuccess.sprite = _bgAllClear;
                }
                _backgroundSuccess.enabled = true;
            }
            else _backgroundFailure.enabled = true;

            // 캐릭터 출력 
            if (StageManager.Instance.GetStageCleared()) _character.sprite = _characterSuccessSprite;
            else _character.sprite = _characterFailureSprite;

            yield return new WaitForSeconds(2.5f);

            if (_stageIndex > _maxStageIndex)
            {
                _notificationImage.sprite = _gameClearSprite;
                _notificationImage.enabled = true;
                yield return new WaitForSeconds(2.5f);
                StageManager.Instance.ShowComplete();
                yield break;
            }

            if (StageManager.Instance.GetHP() <= 0)
            {
                _notificationImage.sprite = _gameOverSprite;
                _notificationImage.enabled = true;
                yield return new WaitForSeconds(2.5f);
                StageManager.Instance.ShowComplete();
                yield break;
            }
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
            _notificationImage.sprite = _bossStageSprite;
            _notificationImage.enabled = true;
            yield return new WaitForSeconds(2.5f);
        }
    }

    /* =========== Lifecycle Methods =========== */

    protected override void OnDisable()
    {
        _backgroundSuccess.enabled = false;
        _backgroundFailure.enabled = false;
        _currentStageImage.enabled = false;
        _notificationImage.enabled = false;

        _character.sprite = _characterDefaultSprite;

        foreach (var hp in _HPGameObjects)
        {
            hp.GetComponent<Image>().enabled = false;
        }
    }
}
