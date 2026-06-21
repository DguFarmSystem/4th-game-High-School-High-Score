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
    void ChangeCommand(int index)
    {
        CommandImage.sprite = CommandSprites[index];
        StartCoroutine(ShowAndOut());
    }
}
