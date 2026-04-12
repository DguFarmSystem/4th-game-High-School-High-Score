using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StageType
{
    Tutorial,
    Restaurant,
    MusicRoom,
    Gym,
    HealthRoom
}

public class ToggleController : MonoBehaviour
{
    [SerializeField]
    private GameObject NextUI;
    [SerializeField]
    private GameObject GymLocked;
    [SerializeField]
    private GameObject MusicRoomLocked;
    [SerializeField]
    private GameObject HealthRoomLocked;
    [SerializeField]
    private StageType RoomType;

    private Toggle MyToggle;

    // Start is called before the first frame update
    void Start()
    {
        //�ӽ� ����
        //StageManager.Instance.SetTutorialCleared(true);
        //StageManager.Instance.SetRestaurantCleared(true);

        //Lock ����
        MusicRoomLocked.SetActive(!DataManager.Instance.Player.GetRestaurantCleared());
        GymLocked.SetActive(true);
        HealthRoomLocked.SetActive(true);

        //�� ��� �о����
        MyToggle = GetComponent<Toggle>();
        MyToggle.interactable = CheckUnlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool CheckUnlock()
    {
        switch (RoomType)
        {
            case StageType.Tutorial:
                return true;
            case StageType.Restaurant:
                return true; //DataManager.Instance.Player.GetTutorialCleared();
            case StageType.MusicRoom:
                return true; //DataManager.Instance.Player.GetRestaurantCleared();
            case StageType.Gym:
                return true; //DataManager.Instance.Player.GetMusicRoomCleared();
            case StageType.HealthRoom:
                return false;
        }
        return false;
    }

    public void TutorialButtonAction(bool isOn)
    {
        //��ư �������� �� �׼�
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
            LoadingSceneController.Instance.LoadScene(SceneNames.TutorialConvStart);
        }
        else
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
                null
            );
            LoadingSceneController.Instance.LoadScene(SceneNames.StageInitScene, StageManager.Instance.LoadNextStage);
        }
    }

    public void RestaurantButtonAction(bool isOn)
    {
        //��ư �������� �� �׼�
        if (!DataManager.Instance.Player.GetRestaurantCleared())
        {
            StageManager.Instance.Initialize(
                new List<string>()
                {
                    SceneNames.RestaurantSpread,
                    SceneNames.RestaurantCakeFire,
                    SceneNames.RestaurantFruit,
                    SceneNames.RestaurantFind,
                    SceneNames.RestaurantBoss
                },
                "RestaurantCS",
                StageManager.GameMode.Normal,
                SceneNames.RestaurantConvStart
            );
            LoadingSceneController.Instance.LoadScene(SceneNames.RestaurantConvStart);
        }
        else
        {
            StageManager.Instance.Initialize(
                new List<string>()
                {
                    SceneNames.RestaurantSpread,
                    SceneNames.RestaurantCakeFire,
                    SceneNames.RestaurantFruit,
                    SceneNames.RestaurantFind,
                    SceneNames.RestaurantBoss
                },
                "RestaurantCS",
                StageManager.GameMode.Normal,
                null
            );
            LoadingSceneController.Instance.LoadScene(SceneNames.StageInitScene, StageManager.Instance.LoadNextStage);
        }
    }

    public void MusicRoomButtonAction(bool isOn)
    {
        //��ư �������� �� �׼�
        if (!DataManager.Instance.Player.GetMusicRoomCleared())
        {
            StageManager.Instance.Initialize(
            new List<string>()
                {
                    SceneNames.MusicGuitar,
                    SceneNames.MusicPlay,
                    SceneNames.MusicPiano,
                    SceneNames.MusicDance,
                    SceneNames.MusicBeat
                },
                "MusicCS",
                StageManager.GameMode.Normal,
                SceneNames.MusicConvStart
            );
            LoadingSceneController.Instance.LoadScene(SceneNames.MusicConvStart);
        }
        else
        {
            StageManager.Instance.Initialize(
                new List<string>()
                {
                    SceneNames.MusicGuitar,
                    SceneNames.MusicPlay,
                    SceneNames.MusicPiano,
                    SceneNames.MusicDance,
                    SceneNames.MusicBeat
                },
                "MusicCS",
                StageManager.GameMode.Normal,
                null
            );
            LoadingSceneController.Instance.LoadScene(SceneNames.StageInitScene, StageManager.Instance.LoadNextStage);
        }
    }

    public void PlayGroundButtonAction(bool isOn)
    {
        //��ư �������� �� �׼�
        if (!DataManager.Instance.Player.GetPlaygroundCleared())
        {
            StageManager.Instance.Initialize(
            new List<string>()
                {
                    SceneNames.PlaygroundAttach,
                    SceneNames.PlaygroundPutAway,
                    SceneNames.PlaygroundScales,
                    SceneNames.PlaygroundCount,
                    SceneNames.PlaygroundBoss,
                },
                "MusicCS", // playground CS is not yet implemented, so temporarily use music CS
                StageManager.GameMode.Normal,
                SceneNames.MusicConvStart
            );
            LoadingSceneController.Instance.LoadScene(SceneNames.MusicConvStart);
        }
        else
        {
            StageManager.Instance.Initialize(
                new List<string>()
                {
                    SceneNames.PlaygroundAttach,
                    SceneNames.PlaygroundPutAway,
                    SceneNames.PlaygroundScales,
                    SceneNames.PlaygroundCount,
                    SceneNames.PlaygroundBoss,
                },
                "MusicCS", // playground CS is not yet implemented, so temporarily use music CS 
                StageManager.GameMode.Normal,
                null
            );
            LoadingSceneController.Instance.LoadScene(SceneNames.StageInitScene, StageManager.Instance.LoadNextStage);
        }
    }
}
