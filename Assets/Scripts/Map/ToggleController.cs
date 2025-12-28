using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleController : MonoBehaviour
{
    [SerializeField]
    private GameObject NextUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnToggleChanged(bool isOn)
    {
        //스테이지 다양화 되면 필요, 지금은 일단 필요없다.
        //NextUI.SetActive(isOn);

        //바로 대화 씬으로 넘어갈 수 있도록
    }
}
