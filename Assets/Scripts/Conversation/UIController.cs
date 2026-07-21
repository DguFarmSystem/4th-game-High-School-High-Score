using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("BackGroundImage Resource")]
    public Image Background;
    public List<Sprite> BackGroundImageList;

    [Header("UI Components")]
    public GameObject DialogueBox;
    public Image DialogueBubble;
    public Image Character;
    public List<Sprite> ImageList;
    public List<Sprite> DialogueList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
     characterImage
    -1 : Narration
    0 : 나 Talk
    1 : 캐릭터 Talk
     */
    public void SetNarration()
    {
        Color c = Character.color;
        c.a = 0;
        Character.color = c;
        DialogueBubble.gameObject.SetActive(false);
    }

    public void SetMyCharacterTalk()
    {
        Character.sprite = ImageList[0];
        DialogueBubble.gameObject.SetActive(false);
    }

    public void SetCharacterTalk()
    {
        Character.sprite = ImageList[1];
        DialogueBubble.gameObject.SetActive(true);
        DialogueBubble.sprite = DialogueList[0];
    }
}
