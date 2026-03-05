using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public enum ScriptSpeedState { Slow, Normal, Fast }

public class GameData
{
    [SerializeField] private string Name;
    [SerializeField] private bool isTutorialCleared = false;
    [SerializeField] private bool isRestaurantCleared = false;
    [SerializeField] private bool isMusicCleared = false;

    // 생성자
    public GameData(string name)
    {
        Name = string.IsNullOrEmpty(name) ? "나학생" : name;
    }

    // ======= Methods ======= //
    public string GetName() => Name;
    public bool GetTutorialCleared() => isTutorialCleared;
    public bool GetRestaurantCleared() => isRestaurantCleared;
    public bool GetMusicCleared() => isMusicCleared;

    public void SetTutorialCleared(bool cleared)
    {
        isTutorialCleared = cleared;
        DataManager.Instance.SaveGameData();
    }

    public void SetRestaurantCleared(bool cleared)
    {
        isRestaurantCleared = cleared;
        DataManager.Instance.SaveGameData();
    }
    
    public void SetMusicCleared(bool cleared)
    {
        isMusicCleared = cleared;
        DataManager.Instance.SaveGameData();
    }

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

    public string path { get; private set; }
    
//#if UNITY_ANDROID
    // Android: persistentDataPath 사용 (쓰기 가능)
    // Application.streamingAssetsPath는 읽기 전용이므로, Android에서는 persistentDataPath를 사용하여 저장합니다.
    public string savePath => Path.Combine(Application.persistentDataPath, "Data", SaveFile);
    public string settingsPath => Path.Combine(Application.persistentDataPath, "Data", SettingsFile);
/*
#else
    // PC/에디터: 개발 편의성을 위해 streamingAssets 또는 persistentDataPath 사용
    public string savePath => Path.Combine(Application.persistentDataPath, "Data", SaveFile);
    public string settingsPath => Path.Combine(Application.persistentDataPath, "Data", SettingsFile);
#endif
*/

    // ========= Game Data Management ========= //
    public void CreateNewGame(string playerName)
    {
        Player = new GameData(playerName);
        SaveGameData();
    }

    public void SaveGameData()
    {
        // 현재 데이터를 JSON으로 직렬화
        string currentData = JsonUtility.ToJson(Player);

        // 저장 폴더 확인
        string directory = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

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
            Player = null;
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

        // 저장 폴더 확인
        string directory = Path.GetDirectoryName(settingsPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

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
            Settings = new SettingsData();
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

//#if UNITY_ANDROID
        // Android: persistentDataPath 사용 (쓰기 가능)
        path = Path.Combine(Application.persistentDataPath, "Data");
/*
#else
        // PC/에디터: streamingAssets 또는 persistentDataPath
        path = Path.Combine(Application.persistentDataPath, "Data");
#endif
*/

        // Save 폴더가 없으면 생성
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log("Save folder created at: " + path);
        }

        if (File.Exists(settingsPath)) LoadSettingsData();
        else                           SaveSettingsData();
        
        if (File.Exists(savePath)) LoadGameData();
    }

    void Start()
    {

    }
}
