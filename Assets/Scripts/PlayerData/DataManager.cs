using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class PlayerData
{
    public string Name;
    public bool TutorialCompleted = false;

    // 기본 생성자
    public PlayerData()
    {
        Name = "DefaultName";
    }

    // Name을 입력받는 생성자
    public PlayerData(string name)
    {
        Name = string.IsNullOrEmpty(name) ? "DefaultName" : name;
    }
}

public class DataManager : Singleton<DataManager>
{
    public PlayerData Player { get; private set; } = new PlayerData();

    public string Path { get; private set; }
    public string Filename { get; private set; } = "playerData.json";

    public void SaveData()
    {
        string data = JsonUtility.ToJson(Player);
        File.WriteAllText(Path, data);
        print("Saving Player Data: " + data);
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
            print("No Player Data found.");
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

    public void UpdatePlayerName(string newName)
    {
        Player.Name = newName;
        SaveData();
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
