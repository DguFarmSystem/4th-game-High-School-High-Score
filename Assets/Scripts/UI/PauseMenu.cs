using Stage;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private StageNormal currentStage;
    private CutScene currentCutScene;

    public void Pause()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
        
        // 일시정지 시 입력 기능 매니저 차단
        InputManager.Instance.SetInputActive(false); 
    }

    public void Resume()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        
        // 재개 시 입력 기능 다시 활성화
        InputManager.Instance.SetInputActive(true); 
    }

    // Restart 버튼에 연결할 함수
    public void Restart()
    {
        currentStage = FindObjectOfType<StageNormal>();
        currentCutScene = FindObjectOfType<CutScene>();
        currentStage?.SetGameInterrupted(true);
        currentCutScene?.SetGameInterrupted(true);

        Time.timeScale = 1f; // 게임 속도 초기화

        StageManager.Instance.RequestSceneChange(SceneNames.StageInitScene, 
        () =>
        {
            StageManager.Instance.Restart(); // StageManager의 Restart() 호출
            InputManager.Instance.SetInputActive(true); // 입력 기능 다시 활성화
            panel.SetActive(false); // 패널 비활성화
        });
    }

    // Exit 버튼에 연결할 함수
    public void Exit()
    {
        currentStage = FindObjectOfType<StageNormal>();
        currentCutScene = FindObjectOfType<CutScene>();
        currentStage?.SetGameInterrupted(true);
        currentCutScene?.SetGameInterrupted(true);

        Time.timeScale = 1f; // 게임 속도 초기화

        StageManager.Instance.RequestSceneChange(SceneNames.Map, 
        () =>
        {
            InputManager.Instance.SetInputActive(true); // 입력 기능 다시 활성화
            StageManager.Instance.gameObject.SetActive(false); // StageManager 비활성화
            panel.SetActive(false); // 패널 비활성화
        });
    }
}