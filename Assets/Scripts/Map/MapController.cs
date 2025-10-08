using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public string[] SceneNames;
    private GameObject[] NextUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //button ´­·µÀ»¶§ ¶ç¿ö¾ßÇÒ UI Ãâ·Â
    public void LoadMapUI(GameObject canvasUI)
    {
        bool newState = !canvasUI.activeSelf;
        canvasUI.SetActive(newState);
    }

    public void LoadGameScene()
    {
        Debug.Log("Let's play 1-A class");
    }
}
