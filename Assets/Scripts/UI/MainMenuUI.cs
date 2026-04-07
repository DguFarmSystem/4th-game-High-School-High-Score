using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public GameObject SettingsPopUp;
    public GameObject ResetPopUp;
    public GameObject EnterNamePopUp;
    public TextMeshProUGUI PlayerName;

    [SerializeField] private TMP_InputField _nameInputField;

    public void GameStart()
    {

        if (DataManager.Instance.Player == null)
        {
            PopUpEnterNameWindow(true);
            return;
        }

        if (!DataManager.Instance.Player.GetTutorialCleared())
        {
            StageManager.Instance.Initialize(
                new List<string>()
                {
                    SceneNames.BoardErasing,
                    SceneNames.FindSeat,
                    SceneNames.WindowClosing,
                    SceneNames.SnackThrowing
                },
                "TutorialCS",
                StageManager.GameMode.Tutorial,
                SceneNames.TutorialConvStart
            );

            // 플레이어 데이터가 있지만 튜토리얼을 클리어하지 않은 경우, 튜토리얼로 이동
            LoadingSceneController.Instance.LoadScene(SceneNames.TutorialConvStart);
        }
        else
        {
            // 플레이어 데이터가 존재하는 경우, 맵으로 이동
            LoadingSceneController.Instance.LoadScene(SceneNames.Map);
        }
    }

    public void PopUpSettings()
    {
        SettingsPopUp.SetActive(true);
        
    }

    public void CloseSettings()
    {
        SettingsPopUp.SetActive(false);
    }

    public void PopUpResetWindow()
    {
        ResetPopUp.SetActive(true);
    }

    public void CloseResetWindow()
    {
        ResetPopUp.SetActive(false);
    }

    public void ResetData()
    {
        DataManager.Instance.ResetGameData();
        DataManager.Instance.ResetSettingsData(); // 설정 데이터도 초기화?
        Debug.Log("Player data reset.");

        LoadingSceneController.Instance.LoadScene(SceneNames.Main);
    }

    public void PopUpEnterNameWindow(bool isGameStart = false)
    {
        if (!isGameStart)
        {
            if (DataManager.Instance.Player != null)
                EnterNamePopUp.SetActive(true);
        }
        else EnterNamePopUp.SetActive(true);
    }

    public void EnterName()
    {
        string name = _nameInputField.text;

        if (name.Length == 0) return;

        if (DataManager.Instance.Player == null)
        {
            DataManager.Instance.CreateNewGame(name);
            StageManager.Instance.Initialize(
                new List<string>()
                {
                    SceneNames.BoardErasing,
                    SceneNames.FindSeat,
                    SceneNames.WindowClosing,
                    SceneNames.SnackThrowing
                },
                "TutorialCS",
                StageManager.GameMode.Tutorial,
                SceneNames.TutorialConvStart
            );
            LoadingSceneController.Instance.LoadScene(SceneNames.TutorialConvStart); // 추후 튜토리얼 화면으로 넘어가도록 변경
        }
        else
        {
            DataManager.Instance.Player.UpdatePlayerName(name);
            PlayerName.text = DataManager.Instance.Player.GetName();
        }

        EnterNamePopUp.SetActive(false);
    }

    public void CloseEnterNameWindow()
    {
        EnterNamePopUp.SetActive(false);
    }

    // ============ Lifecycle methods ============ //
    void Start()
    {

    }
}
