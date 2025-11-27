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

        LoadingSceneController.Instance.LoadScene(SceneNames.ConvScene); // 추후 세이브 데이터와 연계하도록 변경
        
        /*
        StageManager.Instance.Initialize(
            new List<string> {
                SceneNames.FindSeat,
                SceneNames.FindSeat,
                SceneNames.FindSeat,
                SceneNames.SnackThrowing,
            },
            "tutorial"
            ,
            StageManager.GameMode.Tutorial
        );
        StageManager.Instance.LoadNextStage();
        */
    }

    public void PopUpSettings()
    {
        Toggle[] BGMToggles = SettingsPopUp.transform.Find("BGMSet").GetComponentsInChildren<Toggle>();
        Toggle[] VibFXToggles = SettingsPopUp.transform.Find("VibFXSet").GetComponentsInChildren<Toggle>();
        Toggle[] ScriptSpeedToggles = SettingsPopUp.transform.Find("ScriptSpeedSet").GetComponentsInChildren<Toggle>();

        if (DataManager.Instance.Settings.GetBGMSetting()) BGMToggles[0].isOn = true;
        else BGMToggles[1].isOn = true;

        if (DataManager.Instance.Settings.GetVibFXSetting()) VibFXToggles[0].isOn = true;
        else VibFXToggles[1].isOn = true;

        switch (DataManager.Instance.Settings.GetScriptSpeed())
        {
            case ScriptSpeedState.Slow:
                ScriptSpeedToggles[0].isOn = true;
                break;
            case ScriptSpeedState.Normal:
                ScriptSpeedToggles[1].isOn = true;
                break;
            case ScriptSpeedState.Fast:
                ScriptSpeedToggles[2].isOn = true;
                break;
        }
        if (DataManager.Instance.Player != null)
        {
            PlayerName.text = DataManager.Instance.Player.GetName();
        }

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
            LoadingSceneController.Instance.LoadScene(SceneNames.WindowClosing); // 추후 튜토리얼 화면으로 넘어가도록 변경
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
