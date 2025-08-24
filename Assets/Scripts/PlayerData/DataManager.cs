using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public enum ScriptSpeedState { Slow, Normal, Fast }

public class PlayerData
{
    [SerializeField] private string Name;
    [SerializeField] private bool TutorialCompleted = false;
    [SerializeField] private bool IsBGMOn = true;
    [SerializeField] private bool IsVibFXOn = true;
    [SerializeField] private ScriptSpeedState ScriptSpeed = ScriptSpeedState.Normal;

    // 기본 생성자
    public PlayerData()
    {
        Name = "나고점";
    }

    // Name을 입력받는 생성자
    public PlayerData(string name)
    {
        Name = string.IsNullOrEmpty(name) ? "나고점" : name;
    }

    // ======= Methods ======= //
    public string GetName() => Name;
    public bool GetTutorialCompleted() => TutorialCompleted;
    public bool GetBGMSetting() => IsBGMOn;
    public bool GetVibFXSetting() => IsVibFXOn;
    public ScriptSpeedState GetScriptSpeed() => ScriptSpeed;

    public void UpdatePlayerName(string newName)
    {
        Name = newName;
        DataManager.Instance.SaveData();
    }

    public void UpdateBGMSetting(bool isOn)
    {
        IsBGMOn = isOn;
        DataManager.Instance.SaveData();
    }

    public void UpdateVibFXSetting(bool isOn)
    {
        IsVibFXOn = isOn;
        DataManager.Instance.SaveData();
    }

    public void UpdateScriptSpeed(ScriptSpeedState newSpeed)
    {
        ScriptSpeed = newSpeed;
        DataManager.Instance.SaveData();
    }
}

public class DataManager : Singleton<DataManager>
{
    public PlayerData Player { get; private set; } = new PlayerData();

    public string Path { get; private set; }
    public string Filename { get; private set; } = "playerData.json";

    public void SaveData()
    {
        // 현재 데이터를 JSON으로 직렬화
        string currentData = JsonUtility.ToJson(Player);

        // 기존 저장된 데이터 읽기
        string existingData = null;
        if (File.Exists(Path))
        {
            existingData = File.ReadAllText(Path);
        }

        // 기존 데이터와 현재 데이터 비교
        if (existingData == currentData) return; // 변경 사항이 없으면 저장하지 않음

        // 데이터 저장
        File.WriteAllText(Path, currentData);
        Debug.Log("Saving Player Data: " + currentData);
    }

    public void LoadData()
    {
        if (File.Exists(Path))
        {
            string data = File.ReadAllText(Path);
            Player = JsonUtility.FromJson<PlayerData>(data);
            print("Loading Player Data: " + data);
        }
        else
        {
            SaveData(); // 파일이 없으면 새로 저장
        }
    }

    public void ResetData()
    {
        if (File.Exists(Path))
        {
            File.Delete(Path);
            Debug.Log("Player data file deleted: " + Path);
        }
        else
        {
            Debug.Log("No file found at path: " + Path);
        }

        Player = new PlayerData(); // 기본 데이터로 초기화
    }

    public override void Awake()
    {
        base.Awake();

        Path = Application.persistentDataPath + "/Save/"; // Save 폴더 경로 초기화

        // Save 폴더가 없으면 생성
        if (!Directory.Exists(Path))
        {
            Directory.CreateDirectory(Path);
            Debug.Log("Save folder created at: " + Path);
        }

        Path += Filename;
    }

    void Start()
    {
        
    }
}
