using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] private GameObject _stageIntervalUIPrefab;
    [SerializeField] private StageIntervalSkin _defaultSkin;

    private StageIntervalSkin _currentSkin;
    private StageIntervalCSController _ui;
    private bool _showCompleted = false;

    //StageState 설정 기반으로 씬 넘기기 설정
    //현재 진행되고 있는 스테이지 레벨 관리
    private int _sceneIndex = 0;
    private int _difficulty = 0;
    private List<string> _sceneNames = new List<string>();

    public void LoadStages(List<string> sceneNames, StageIntervalSkin skin = null)
    {
        _sceneNames = sceneNames;
        _currentSkin = skin ?? _defaultSkin;
    }

    void EnsureUI() {
        if (_ui != null) return;
        var gameObject = Instantiate(_stageIntervalUIPrefab);
        DontDestroyOnLoad(gameObject);
        _ui = gameObject.GetComponent<StageIntervalCSController>();
    }

    void Show() {
        EnsureUI();
        _ui.ApplySkin(_currentSkin ?? _defaultSkin);
        _ui.Show();
    }

    void Hide() {
        StartCoroutine(HideAfter(_showCompleted));
    }

    IEnumerator HideAfter(bool completed) {
        yield return new WaitUntil(() => completed);
        _ui.Hide();
    }

    // 씬 로드용 유틸: 자동으로 progress 표시하고 씬 활성화까지 처리
    public void LoadNextScene() {
        StartCoroutine(LoadSceneCoroutine());
    }

    IEnumerator LoadSceneCoroutine() {
        Show();
        var op = SceneManager.LoadSceneAsync(_sceneNames[_sceneIndex]);
        op.allowSceneActivation = false;
        
        while (op.progress < 0.9f)
        {
            yield return null;
        }

        op.allowSceneActivation = true;
        // 씬 활성화 후 숨김
        while (!op.isDone) yield return null;
        Hide();
    }

    /* ========== Life Cycle Methods ========== */

    void OnDisable()
    {
        _sceneIndex = 0;
        _difficulty = 0;
        _sceneNames.Clear();
        _currentSkin = null;
    }

    public override void Awake()
    {
        base.Awake();
    }
}