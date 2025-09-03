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
    public Image Character1;
    public Image Character2;
    public List<Sprite> ImageList;

    [Header("JSON Settings")]
    public string jsonFileName = "Tutorial.json";

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    private ConversationData conversationData;
    private int currentIndex = 0;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentLineFullText;
    private int currentTextIndex = 0; // texts �迭 �� ���� ���� �ε���

    void OnEnable()
    {
        // InputManager�� ��ġ �̺�Ʈ ����
        InputManager.OnStageTapPerformed += HandleTap;
    }

    void OnDisable()
    {
        // �̺�Ʈ ���� ����
        InputManager.OnStageTapPerformed -= HandleTap;
    }

    private void HandleTap()
    {
        if(conversationData == null || currentIndex >= conversationData.conversation.Count)
        {
            //���⼭ ���� ������ �ѱ�� �ɵ�?
            return;
        }

        if (isTyping)
        {
            // Ÿ���� ���̸� ��� ��ü ���� ǥ��
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
                typingCoroutine = StartCoroutine(TypeLine(line)); // ���� ���� ���
            }
            else
            {
                // ���� ȭ�� ��� ���� �Ϸ� �� ���� ���
                currentIndex++;
                currentTextIndex = 0;
                ShowLine();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Color c = Character1.color;
        c.a = 0;
        Character1.color = c;
        Character2.color = c;
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

    void ShowLine()
    {
        if (conversationData != null && currentIndex < conversationData.conversation.Count)
        {
            ConversationLine line = conversationData.conversation[currentIndex];
            speakerText.text = line.speaker;

            if(line.characterImage == -1)
            {
            }
            else if(line.characterImage == 0)
            {
                StartCoroutine(FadeIn(Character1, 1f));
                Character1.sprite = ImageList[line.characterImage];
            }
            else if(line.characterImage == 1)
            {
                StartCoroutine(FadeIn(Character2, 1f));
                Character2.sprite = ImageList[line.characterImage];
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

        // texts�� ������ ���� ���常 ���
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

    //����׿� �Լ�
    private void EndDialogue()
    {
        Debug.Log("��ȭ ����");
        speakerText.text = "";
        dialogueText.text = "";
    }

    //Skip ��ư�� �޼���
    public void SkipAll()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        currentIndex = conversationData.conversation.Count; // ��ȭ �ε����� ������ �̵�
        EndDialogue();
    }
}
