using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class ConversationLine
{
    public int id;
    public int characterImage;
    public string speaker;
    public List<string> texts;
}

[System.Serializable]
public class ConversationData
{
    public List<ConversationLine> conversation;
}

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    public Text speakerText;
    public Text dialogueText;
    public GameObject Character1;
    public GameObject Character2;
    public List<Sprite> ImageList;

    [Header("JSON Settings")]
    public string jsonFileName = "Tutorial.json";

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    private ConversationData conversationData;
    private int currentIndex = 0;

    private Image Character1Img;
    private Image Character2Img;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentLineFullText;
    private int currentTextIndex = 0; // texts �迭 �� ���� ���� �ε���

    void OnEnable()
    {
        // InputManager�� ��ġ �̺�Ʈ ����
        InputManager.Instance.OnStageTapPerformed += HandleTap;
    }

    void OnDisable()
    {
        // �̺�Ʈ ���� ����
        InputManager.Instance.OnStageTapPerformed -= HandleTap;
    }

    private void HandleTap()
    {
        if(conversationData == null || currentIndex >= conversationData.conversation.Count)
        {
            //���⼭ ���� ������ �ѱ��? �ɵ�?
            return;
        }

        if (isTyping)
        {
            // Ÿ���� ���̸� ���? ��ü ���� ǥ��
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueText.text = currentLineFullText;
            isTyping = false;
        }
        else
        {
            // ���� ȭ���� texts �迭���� ���� �������� �̵�
            ConversationLine line = conversationData.conversation[currentIndex];
            currentTextIndex++;

            if (line.texts != null && currentTextIndex < line.texts.Count)
            {
                typingCoroutine = StartCoroutine(TypeLine(line)); // ���� ���� ���?
            }
            else
            {
                // ���� ȭ�� ���? ���� �Ϸ� �� ���� ���?
                currentIndex++;
                currentTextIndex = 0;
                ShowLine();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Character1Img = Character1.GetComponent<Image>();
        Character2Img = Character2.GetComponent<Image>();
        Color c = Character1Img.color;
        c.a = 0;
        Character1Img.color = c;
        Character2Img.color = c;
        LoadConversation();
        ShowLine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadConversation()
    {
        string filePath = Path.Combine(Application.dataPath, "Texts", jsonFileName);

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            conversationData = JsonUtility.FromJson<ConversationData>(jsonData);
        }
        else
        {
            Debug.LogError("JSON ������ ã�� �� �����ϴ�: " + filePath);
        }
    }

    /*
     characterImage
    -1 : �ƹ� �̹����� �����? �ʴ´�.
    0 : ���ΰ� ȥ�� ��ȭâ�� ����, ��ġ �ٲٰ� �迭 3��
    1 : ù ���� - ü���� ��ȭ ����, left element 2, right element 1
    2 : ���ΰ��� ü���� �� ���?. ���ΰ� ���� ȿ��. left element 0, right element 3
    3 : ü�� ���� ȿ��.
     */
    void ShowLine()
    {
        if (conversationData != null && currentIndex < conversationData.conversation.Count)
        {
            ConversationLine line = conversationData.conversation[currentIndex];
            speakerText.text = line.speaker;

            if(line.characterImage == -1)
            {
                Color c = Character2Img.color;
                c.a = 0;
                Character1Img.color = c;
                Character2Img.color = c;
            }
            else if(line.characterImage == 0)
            {
                Character2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 59);
                StartCoroutine(FadeIn(Character2Img, 1f));
                Character2Img.sprite = ImageList[3];
            }
            else if(line.characterImage == 1)
            {
                Character1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-444, 42);
                Character2.GetComponent<RectTransform>().anchoredPosition = new Vector2(499, 59);
                StartCoroutine(FadeIn(Character1Img, 1f));
                StartCoroutine(FadeIn(Character2Img, 1f));
                Character1Img.sprite = ImageList[2];
                Character2Img.sprite = ImageList[1];
            }
            else if(line.characterImage == 2)
            {
                Character1Img.sprite = ImageList[0];
                Character2Img.sprite = ImageList[3];
            }
            else if(line.characterImage == 3)
            {
                Character1Img.sprite = ImageList[2];
                Character2Img.sprite = ImageList[1];
            }

            // coroutine���� Ÿ���� ȿ�� ����
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLine(line));
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator FadeIn(Image img, float duration)
    {
        float time = 0f;
        Color c = img.color;
        c.a = 0f;
        img.color = c;

        while (time < duration)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, time / duration); // 0��1�� ���� ����
            img.color = c;
            yield return null;
        }

        c.a = 1f;
        img.color = c;
    }

    IEnumerator TypeLine(ConversationLine line)
    {
        isTyping = true;

        // texts�� ������ ���� ���常 ���?
        if (line.texts != null && currentTextIndex < line.texts.Count)
        {
            currentLineFullText = line.texts[currentTextIndex];
            dialogueText.text = "";

            foreach (char c in currentLineFullText)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        isTyping = false;
    }

    public void NextLine()
    {
        currentIndex++;
        if (currentIndex < conversationData.conversation.Count)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    //����׿�? �Լ�
    private void EndDialogue()
    {
        Debug.Log("��ȭ ����");
        speakerText.text = "";
        dialogueText.text = "";

        // TEST CODE
        StageManager.Instance.Initialize(
            new List<string> {
                SceneNames.WindowClosing,
                SceneNames.FindSeat,
                SceneNames.FindSeat,
                SceneNames.SnackThrowing,
            },
            "tutorial"
            ,
            StageManager.GameMode.Tutorial
        );
        StageManager.Instance.LoadNextStage();
    }

    //Skip ��ư�� �޼���
    public void SkipAll()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        currentIndex = conversationData.conversation.Count; // ��ȭ �ε����� ������ �̵�
        EndDialogue();
    }
}
