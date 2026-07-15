using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandProcessor : StageCommand
{
    [SerializeField]
    private Sprite[] CommandSprites;
    [SerializeField]
    private Image CommandImage;
    
    public GameObject TouchBlocker;

    IEnumerator CustomShowAndOut()
    {
        TouchBlocker.SetActive(true);
        StartCoroutine(ShowAndOut());
        yield return null;
        TouchBlocker.SetActive(false);
    }

    public void ChangeCommand(int index)
    {
        CommandImage.sprite = CommandSprites[index];
        StartCoroutine(CustomShowAndOut());
    }
}
