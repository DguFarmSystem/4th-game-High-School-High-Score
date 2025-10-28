using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using EasyTransition;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] private StageIntervalSkin _skinData;
    //[SerializeField] private GameObject _transitionTemplate;
    //[SerializeField] private TransitionSettings _transition;


    private GameObject _ui;
    private StageIntervalCSController _uiController;
    private bool _showCompleted = false;

    private int _sceneIndex = 0;
    private int _difficulty = 1;
    private int _hp = 4;
    private bool _isStageCleared = false;
    private List<string> _sceneNames = new List<string>();

    public enum GameMode { Tutorial, Normal, Infinite }
    private GameMode _gameMode = GameMode.Normal;

    public void Initialize(List<string> sceneNames, string skin, GameMode gameMode = GameMode.Normal)
    {
        _sceneNames = sceneNames;
        _ui = Instantiate(_skinData.GetDictionary()[skin]);
        _ui.transform.SetParent(transform);
        gameObject.SetActive(true);

        _gameMode = gameMode;
    }

    public void StageClear(bool clear)
    {
        _isStageCleared = clear;

        if (clear)
        {
            switch (_gameMode)
            {
                case GameMode.Tutorial:
                case GameMode.Normal:
                    if ((_sceneIndex + 1) % 3 == 0) _difficulty++;

                    _sceneIndex++;

                    break;

                case GameMode.Infinite:

                    break;

            }
        }
        else DecreaseHP();

        LoadNextStage();
    }

    private void SpeedUp(float speed)
    {
        Time.timeScale = speed;
        //AudioSource audioSource = GetComponent<AudioSource>(); // 예시
        //audioSource.pitch = speed; // 배속
    }

    private void SpeedInitialize()
    {
        Time.timeScale = 1f;
        //AudioSource audioSource = GetComponent<AudioSource>(); // 예시
        //audioSource.pitch = 1f; // 원래 속도로 재생
    }

    private IEnumerator GameExitFade()
    {
        SpeedInitialize();
        /*
        GameObject template = Instantiate(_transitionTemplate) as GameObject;
        template.GetComponent<Transition>().transitionSettings = _transition;

        float transitionTime = _transition.transitionTime;
        if (_transition.autoAdjustTransitionTime)
            transitionTime = transitionTime / _transition.transitionSpeed;

        yield return new WaitForSecondsRealtime(transitionTime);
        */
        _uiController.Hide();
        yield return null;
    }

    private IEnumerator WaitForTap()
    {
        bool tapFlag = false;

        Action<InputAction.CallbackContext> handler = null;
        handler = (context) => {
            tapFlag = true;
            InputManager.Instance._tapAction.performed -= handler;
        };
        InputManager.Instance._tapAction.performed += handler;

        yield return new WaitUntil(() => tapFlag);
    }

    private IEnumerator ExitToScene(string sceneName)
    {
        yield return new WaitUntil(() => _showCompleted);
        yield return WaitForTap();
        yield return GameExitFade();
        LoadingSceneController.Instance.LoadScene(sceneName);
        yield return new WaitUntil(() => LoadingSceneController.Instance.IsSceneLoaded);
        //yield return new WaitForSecondsRealtime(_transition.destroyTime);

        gameObject.SetActive(false);
    }

    public GameMode GetGameMode() => _gameMode;
    public int GetCurrentStage() => _sceneIndex;
    public int GetDifficulty() => _difficulty;

    public int GetHP() => _hp;
    public void IncreaseHP() => _hp++;
    public void DecreaseHP() => _hp--;

    public bool GetStageCleared() => _isStageCleared;

    public void ShowComplete() => _showCompleted = true;

    /* =========== Scene Loading Process =========== */

    void EnsureUI()
    {
        if (_uiController != null) return;
        _uiController = gameObject.GetComponentInChildren<StageIntervalCSController>();
    }

    void Show()
    {
        EnsureUI();
        _uiController.Show();
        _showCompleted = false;
    }

    public void LoadNextStage()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    IEnumerator LoadSceneCoroutine()
    {
        Show();
        AsyncOperation op = null;

        if (_hp > 0)
        {
            switch (_gameMode)
            {
                case GameMode.Tutorial:
                    if (_sceneIndex > 9)
                    {
                        yield return ExitToScene(SceneNames.Main); // 튜토리얼 끝나면 메인으로
                        yield break;
                    }

                    if (_sceneIndex < 9) op = SceneManager.LoadSceneAsync(_sceneNames[_sceneIndex % 3]); // 일반
                    else op = SceneManager.LoadSceneAsync(_sceneNames[3]); // 보스

                    break;

                case GameMode.Normal:
                    if (_sceneIndex > 12)
                    {
                        yield return ExitToScene(SceneNames.Main); // 일반 모드 끝나면 메인으로
                        yield break;
                    }

                    if (_sceneIndex < 12) op = SceneManager.LoadSceneAsync(_sceneNames[_sceneIndex % 3]); // 일반
                    else op = SceneManager.LoadSceneAsync(_sceneNames[3]); // 보스

                    break;

                case GameMode.Infinite:

                    break;
            }
        }
        else
        {
            yield return ExitToScene(SceneNames.Main); // 일단 메인으로!!
            yield break;
        }

        op.allowSceneActivation = false;
        
        while (op.progress < 0.9f)
        {
            yield return null;
        }

        // 연출이 끝날 때까지 대기
        yield return new WaitUntil(() => _showCompleted);

        op.allowSceneActivation = true;
        UnityEngine.Debug.Log($"Stage {_sceneIndex + 1}, Difficulty {_difficulty}");

        yield return null;

        _uiController.Hide();
        _isStageCleared = false;
    }

    /* ========== Life Cycle Methods ========== */

    void OnDisable()
    {
        _sceneIndex = 0;
        _difficulty = 1;
        _hp = 4;
        _sceneNames.Clear();
        _gameMode = GameMode.Normal;
        if (_ui != null) Destroy(_ui);
    }

    public override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }
}