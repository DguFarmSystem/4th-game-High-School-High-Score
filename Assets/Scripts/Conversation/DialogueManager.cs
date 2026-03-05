using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ConversationLine
{
    public int id;
    public int characterImage;
    public string speaker;
    public List<string> texts;

    //selection part
    public int specificSoundIndex = -1;
}

[System.Serializable]
public class ConversationData
{
    public List<ConversationLine> conversation;
}

public class DialogueManager : MonoBehaviour
{
    [Header("Next Stage Settings")]
    [SerializeField]
    private List<string> nextScenes;
    [SerializeField]
    private string stageKey;
    [SerializeField]
    private StageManager.GameMode gameMode;
    private string prevScene;

    [SerializeField]
    private bool isEndConv;

    [Header("Audio Source")]
    public AudioSource TypingSound;
    public AudioSource EventSound = null;
    public List<AudioClip> EffectSounds;

    [Header("UI Components")]
    public Text speakerText;
    public Text dialogueText;
    public GameObject Character1;
    public GameObject Character2;
    public List<Sprite> ImageList;

    [Header("JSON Settings")]
    public string jsonFileName = null;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    [Header("BGM Settings")]
    public AudioSource BGMSource;

    protected ConversationData conversationData;
    protected int currentIndex = 0;
     
    protected Image Character1Img;
    protected Image Character2Img;
     
    protected Coroutine typingCoroutine;
    protected bool isTyping = false;
    protected string currentLineFullText;
    protected int currentTextIndex = 0; // texts �迭 �� ���� ���� �ε���

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
            //���⼭ ���� ������ �ѱ��?? �ɵ�?
            return;
        }

        if (isTyping)
        {
            // Ÿ���� ���̸� ���?? ��ü ���� ǥ��
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueText.text = currentLineFullText;
            isTyping = false;
            //타이핑 소리 중단
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            TypingSound.Stop();
        }
        else
        {
            // ���� ȭ���� texts �迭���� ���� �������� �̵�
            ConversationLine line = conversationData.conversation[currentIndex];
            currentTextIndex++;

            if (line.texts != null && currentTextIndex < line.texts.Count)
            {
                typingCoroutine = StartCoroutine(TypeLine(line)); // ���� ���� ���??
            }
            else
            {
                // ���� ȭ�� ���?? ���� �Ϸ� �� ���� ���??
                currentIndex++;
                currentTextIndex = 0;
                ShowLine();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        prevScene = SceneManager.GetActiveScene().name;

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
        Debug.Log("Loading conversation from: " + filePath);

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

    public virtual void ShowLine()
    {
        //Settings for clearing image
        Color c = Character2Img.color;
        c.a = 0;

        if (conversationData != null && currentIndex < conversationData.conversation.Count)
        {
            ConversationLine line = conversationData.conversation[currentIndex];
            speakerText.text = line.speaker;

            if(line.characterImage == -1)
            {
                Character1Img.color = c;
                Character2Img.color = c;
            }
            else if(line.characterImage == 0)
            {
                //Character2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 59);
                //StartCoroutine(FadeIn(Character2Img, 1f));
                //Character2Img.sprite = ImageList[3];

                Character2Img.color = c;
                Character1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 59);
                if(Character1Img.color.a == 0)
                {
                    StartCoroutine(FadeIn(Character1Img, 1f));
                }
                Character1Img.sprite = ImageList[2];
            }
            else if(line.characterImage == 1)
            {
                //Character1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-444, 42);
                //Character2.GetComponent<RectTransform>().anchoredPosition = new Vector2(499, 59);
                //StartCoroutine(FadeIn(Character1Img, 1f));
                //StartCoroutine(FadeIn(Character2Img, 1f));
                //Character1Img.sprite = ImageList[2];
                //Character2Img.sprite = ImageList[1];

                Character1Img.color = c;
                Character2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 59);
                if(Character2Img.color.a == 0)
                {
                    StartCoroutine(FadeIn(Character2Img, 1f));
                }
                Character2Img.sprite = ImageList[3];
            }
            else if(line.characterImage == 2)
            {
                //Character1Img.sprite = ImageList[0];
                //Character2Img.sprite = ImageList[3];

                Character1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-550, -10);
                Character2.GetComponent<RectTransform>().anchoredPosition = new Vector2(550, 10);
                if(Character1Img.color.a == 0 || Character2Img.color.a == 0)
                {
                    StartCoroutine(FadeIn(Character1Img, 1f));
                    StartCoroutine(FadeIn(Character2Img, 1f));
                }
                Character1Img.sprite = ImageList[2];
                Character2Img.sprite = ImageList[1];
            }
            else if(line.characterImage == 3)
            {
                //Character1Img.sprite = ImageList[2];
                //Character2Img.sprite = ImageList[1];

                Character1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-550, -10);
                Character2.GetComponent<RectTransform>().anchoredPosition = new Vector2(550, -10);
                if (Character1Img.color.a == 0 || Character2Img.color.a == 0)
                {
                    StartCoroutine(FadeIn(Character1Img, 1f));
                    StartCoroutine(FadeIn(Character2Img, 1f));
                }
                Character1Img.sprite = ImageList[0];
                Character2Img.sprite = ImageList[3];
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

        if (line.texts != null && currentTextIndex < line.texts.Count)
        {
            currentLineFullText = line.texts[currentTextIndex];
            dialogueText.text = "";

            //이벤트 사운드 플레이
            if (line.specificSoundIndex < 0)
            {
                if (EventSound.isPlaying) EventSound.Stop();
                EventSound.clip = null;
            }
            else
            {
                EventSound.clip = EffectSounds[line.specificSoundIndex];
                EventSound.Play();
            }

            ////타이핑 사운드 플레이
            //if (!TypingSound.isPlaying)
            //{
            //    TypingSound.clip = EffectSounds[0];
            //    TypingSound.loop = true;
            //    TypingSound.Play();
            //}

            TypingSound.Stop();
            TypingSound.clip = EffectSounds[0];
            TypingSound.loop = true;
            TypingSound.Play();


            foreach (char c in currentLineFullText)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }

            // 문장 출력 완료 → 사운드 정지
            TypingSound.Stop();
            TypingSound.loop = false;
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

    //����׿�?? �Լ�
    private void EndDialogue()
    {
        Debug.Log("��ȭ ����");
        speakerText.text = "";
        dialogueText.text = "";
        BGMSource.Stop();


        if (!isEndConv) 
        {
            StageManager.Instance.Initialize(
                nextScenes,
                stageKey,
                gameMode,
                prevScene
            );
            StageManager.Instance.LoadNextStage();
        }
        else LoadingSceneController.Instance.LoadScene(SceneNames.Map);
    }

    //Skip ��ư�� �޼���
    public void SkipAll()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        currentIndex = conversationData.conversation.Count; // ��ȭ �ε����� ������ �̵�
        EndDialogue();
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
            TypingSound.Stop();
        }
    }
}
