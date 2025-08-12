using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            SceneManager.LoadScene(1); // 마지막 씬이면 첫 번째 씬으로 돌아감
        }

        Debug.Log("다음 레벨 로드");
    }

    public void LoadPreviousLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int prevIndex = currentIndex - 1;

        if (prevIndex >= 1)
        {
            SceneManager.LoadScene(prevIndex);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1); // 첫 번째 씬 이전이면 마지막 씬으로 돌아감
        }

        Debug.Log("이전 레벨 로드");
    }  
}
