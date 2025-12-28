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
        //임시 세팅
        //StageManager.Instance.SetTutorialCleared(true);
        //StageManager.Instance.SetRestaurantCleared(true);

        //Lock 세팅
        MusicRoomLocked.SetActive(!StageManager.Instance.isRestaurantCleared);
        GymLocked.SetActive(true);
        HealthRoomLocked.SetActive(true);

        //내 토글 읽어오기
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
                return StageManager.Instance.isTutorialCleared;
            case StageType.MusicRoom:
                return StageManager.Instance.isRestaurantCleared;
            case StageType.Gym:
                return false;
            case StageType.HealthRoom:
                return false;
        }
        return false;
    }

    public void TutorialButtonAction(bool isOn)
    {
        //버튼 눌럿을때 할 액션
    }

    public void RestaurantButtonAction(bool isOn)
    {
        //버튼 눌럿을때 할 액션
    }

    public void MusicRoomButtonAction(bool isOn)
    {
        //버튼 눌럿을때 할 액션
    }
}
