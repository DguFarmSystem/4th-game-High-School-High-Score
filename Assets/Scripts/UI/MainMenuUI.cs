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
    public TextMeshProUGUI PlayerName;

    public void NewGame()
    {
        //SceneManager.LoadScene(SceneNames.WindowClosing); // 추후 변경 필요
        LoadingSceneController.Instance.LoadScene(SceneNames.WindowClosing);
    }

    public void PopUpSettings()
    {
        Toggle[] BGMToggles = SettingsPopUp.transform.Find("BGMSet").GetComponentsInChildren<Toggle>();
        Toggle[] VibFXToggles = SettingsPopUp.transform.Find("VibFXSet").GetComponentsInChildren<Toggle>();
        Toggle[] ScriptSpeedToggles = SettingsPopUp.transform.Find("ScriptSpeedSet").GetComponentsInChildren<Toggle>();

        TextMeshProUGUI playerNameText = SettingsPopUp.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();

        if (DataManager.Instance.Player.GetBGMSetting()) BGMToggles[0].isOn = true;
        else BGMToggles[1].isOn = true;

        if (DataManager.Instance.Player.GetVibFXSetting()) VibFXToggles[0].isOn = true;
        else VibFXToggles[1].isOn = true;

        switch (DataManager.Instance.Player.GetScriptSpeed())
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

        playerNameText.text = DataManager.Instance.Player.GetName();

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
        DataManager.Instance.ResetData();
        Debug.Log("Player data reset.");

        SceneManager.LoadScene(0);
    }

    // ============ Lifecycle methods ============ //
    void Start()
    {

    }
}
