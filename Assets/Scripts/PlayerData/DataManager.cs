using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public enum ScriptSpeedState { Slow, Normal, Fast }

public class GameData
{
    [SerializeField] private string Name;
    [SerializeField] private bool isNewGame = true;
    [SerializeField] private bool TutorialCompleted = false;

    // 기본 생성자
    public GameData()
    {
        Name = "나고점";
    }

    // Name을 입력받는 생성자
    public GameData(string name)
    {
        Name = string.IsNullOrEmpty(name) ? "나고점" : name;
    }

    // ======= Methods ======= //
    public string GetName() => Name;
    public bool IsNewGame() => isNewGame;
    public void SetNewGameFalse() { isNewGame = false; }
    public bool GetTutorialCompleted() => TutorialCompleted;

    public void UpdatePlayerName(string newName)
    {
        Name = newName;
        DataManager.Instance.SaveGameData();
    }
}

public class SettingsData
{
    [SerializeField] private bool IsBGMOn = true;
    [SerializeField] private bool IsVibFXOn = true;
    [SerializeField] private ScriptSpeedState ScriptSpeed = ScriptSpeedState.Normal;


    // ======= Methods ======= //
    public bool GetBGMSetting() => IsBGMOn;
    public bool GetVibFXSetting() => IsVibFXOn;
    public ScriptSpeedState GetScriptSpeed() => ScriptSpeed;

    public void UpdateBGMSetting(bool isOn)
    {
        IsBGMOn = isOn;
        DataManager.Instance.SaveSettingsData();
    }

    public void UpdateVibFXSetting(bool isOn)
    {
        IsVibFXOn = isOn;
        DataManager.Instance.SaveSettingsData();
    }

    public void UpdateScriptSpeed(ScriptSpeedState newSpeed)
    {
        ScriptSpeed = newSpeed;
        DataManager.Instance.SaveSettingsData();
    }
}

public class DataManager : Singleton<DataManager>
{
    public GameData Player { get; private set; } = null;
    public SettingsData Settings { get; private set; } = new SettingsData();

    public string SaveFile { get; private set; } = "Save.json";
    public string SettingsFile { get; private set; } = "Settings.json";

    public string Path { get; private set; }
    public string savePath => Path + SaveFile;
    public string settingsPath => Path + SettingsFile;

    // ========= Game Data Management ========= //
    public void SaveGameData()
    {
        // 현재 데이터를 JSON으로 직렬화
        string currentData = JsonUtility.ToJson(Player);

        // 데이터 저장
        File.WriteAllText(savePath, currentData);
        Debug.Log("Saving Player Data: " + currentData);
    }

    public void LoadGameData()
    {
        if (File.Exists(savePath))
        {
            string data = File.ReadAllText(savePath);
            Player = JsonUtility.FromJson<GameData>(data);
            print("Loading Player Data: " + data);
        }
    }

    public void ResetGameData()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Player data file deleted: " + savePath);
        }
        else
        {
            Debug.Log("No file found at path: " + savePath);
        }
    }

    // ========= Settings Data Management ========= //
    public void SaveSettingsData()
    {
        // 현재 데이터를 JSON으로 직렬화
        string currentData = JsonUtility.ToJson(Settings);

        /*
        // 기존 저장된 데이터 읽기
        string existingData = null;
        if (File.Exists(settingsPath))
        {
            existingData = File.ReadAllText(settingsPath);
        }

        // 기존 데이터와 현재 데이터 비교
        if (existingData == currentData) return; // 변경 사항이 없으면 저장하지 않음
        */

        // 데이터 저장
        File.WriteAllText(settingsPath, currentData);
        Debug.Log("Saving Settings Data: " + currentData);
    }

    public void LoadSettingsData()
    {
        if (File.Exists(settingsPath))
        {
            string data = File.ReadAllText(settingsPath);
            Settings = JsonUtility.FromJson<SettingsData>(data);
            print("Loading Settings Data: " + data);
        }
    }

    public void ResetSettingsData()
    {
        if (File.Exists(settingsPath))
        {
            File.Delete(settingsPath);
            Debug.Log("Settings data file deleted: " + settingsPath);
        }
        else
        {
            Debug.Log("No file found at path: " + settingsPath);
        }
    }

    public override void Awake()
    {
        base.Awake();

        Path = Application.persistentDataPath + "/Data/"; // Save 폴더 경로 초기화

        // Save 폴더가 없으면 생성
        if (!Directory.Exists(Path))
        {
            Directory.CreateDirectory(Path);
            Debug.Log("Save folder created at: " + Path);
        }

        if (File.Exists(settingsPath)) LoadSettingsData();
        else                           SaveSettingsData();
        
        if (File.Exists(savePath)) LoadGameData();
    }

    void Start()
    {

    }
}
