using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject ContinuePopUp;
    public TextMeshProUGUI PlayerName;

    public void NewGame()
    {
        DataManager.Instance.UpdatePlayerName("TESTNAME");
        DataManager.Instance.SaveData();
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        if (File.Exists(DataManager.Instance.Path))
        {
            DataManager.Instance.LoadData();
            PlayerName.text = DataManager.Instance.Player.Name;
            ContinuePopUp.SetActive(true);
        }
        else
        {
            Debug.Log("No saved game found.");
        }
    }

    public void ClosePopUp()
    {
        ContinuePopUp.SetActive(false);
    }

    public void ResetData()
    {
        DataManager.Instance.ResetData();
        Debug.Log("Player data reset.");
    }

    // ============ Lifecycle methods ============ //
    void Start()
    {
        
    }
}
