using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] private StageIntervalSkin _skinData;

    private GameObject _ui;
    private StageIntervalCSController _uiController;
    private bool _showCompleted = false;

    private int _sceneIndex = 0;
    private int _difficulty = 1;
    private int _hp = 4;
    private bool _isStageCleared = false;
    private List<string> _sceneNames = new List<string>();

    private enum GameMode { Normal, Infinite }
    private GameMode _gameMode = GameMode.Normal;

    public void Initialize(List<string> sceneNames, string skin, bool gameMode = false)
    {
        _sceneNames = sceneNames;
        _ui = Instantiate(_skinData.GetDictionary()[skin]);
        _ui.transform.SetParent(transform);
        gameObject.SetActive(true);

        _gameMode = gameMode ? GameMode.Infinite : GameMode.Normal;
    }

    public void StageClear(bool clear)
    {
        _isStageCleared = clear;

        if (clear)
        {
            switch (_gameMode)
            {
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

    public void ExitGame()
    {
        StartCoroutine(ExitGameCoroutine());

        IEnumerator ExitGameCoroutine()
        {
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }
    }

    public int GetCurrentStage() => _sceneIndex;
    public int GetDifficulty() => _difficulty;

    public int GetHP() => _hp;
    public void IncreaseHP() => _hp++;
    public void DecreaseHP() => _hp--;

    public bool GetStageCleared() => _isStageCleared;

    public void ShowComplete() => _showCompleted = true;

    /* =========== Scene Loading Process =========== */

    void EnsureUI() {
        if (_uiController != null) return;
        _uiController = gameObject.GetComponentInChildren<StageIntervalCSController>();
    }

    void Show()
    {
        EnsureUI();
        _uiController.Show();
        _showCompleted = false;
    }

    public void LoadNextStage() {
        StartCoroutine(LoadSceneCoroutine());
    }

    IEnumerator LoadSceneCoroutine() {
        Show();
        AsyncOperation op = null;
        switch (_gameMode)
        {
            case GameMode.Normal:
                if (_sceneIndex > 9)
                {
                    yield return new WaitForSeconds(3f);
                    ExitGame();
                    LoadingSceneController.Instance.LoadScene(SceneNames.Main); // 일단 메인으로!!
                    yield break;
                }

                if (_sceneIndex < 9) op = SceneManager.LoadSceneAsync(_sceneNames[_sceneIndex % 3]); // 일반
                else op = SceneManager.LoadSceneAsync(_sceneNames[3]); // 보스

                break;

            case GameMode.Infinite:
                
                break;
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