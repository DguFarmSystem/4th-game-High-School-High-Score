using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestaurantCS : CutScene
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
            if (StageManager.Instance.GetStageCleared()) 
            {
                SoundManager.Instance.PlayBGM(_bgaudioClip);
                _character.sprite = _characterSuccessSprite;
            }
            else 
            {
                SoundManager.Instance.StopBGM();
                _character.sprite = _characterFailureSprite;
            }

            yield return new WaitForSeconds(2.5f);

            if (_stageIndex > _maxStageIndex)
            {
                _notificationImage.sprite = _gameClearSprite;
                SoundManager.Instance.PlaySFX(_allClearAudioClip);
                _notificationImage.enabled = true;
                yield return new WaitForSeconds(2.5f);
                StageManager.Instance.ShowComplete();
                yield break;
            }

            if (StageManager.Instance.GetHP() <= 0)
            {
                _notificationImage.sprite = _gameOverSprite;
                SoundManager.Instance.PlaySFX(_gameOverAudioClip);
                _notificationImage.enabled = true;
                yield return new WaitForSeconds(2.5f);
                StageManager.Instance.ShowComplete();
                yield break;
            }
        }

        /* ============ 기본 출력 ============ */
        if (!SoundManager.Instance.BGMSource.isPlaying) SoundManager.Instance.PlayBGM(_bgaudioClip);

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

        SoundManager.Instance.StopBGM();

        StageManager.Instance.ShowComplete(); // 다 출력했다고 알림
    }

    protected override void ShowHP()
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
                SoundManager.Instance.PlaySFX(_hpDecreaseAudioClip);
            }
        }

        for (int i = 0; i < currentHP; i++)
        {
            if (_stageIndex > _maxStageIndex)
            {
                _HPGameObjects[i].GetComponent<Animator>().Play("Clear"); // animation state 호출
            }

            _HPGameObjects[i].GetComponent<Image>().enabled = true; 
        }

        _previousHP = currentHP;
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
