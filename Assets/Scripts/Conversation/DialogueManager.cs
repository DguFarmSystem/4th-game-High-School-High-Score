using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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
    public string jsonFileName;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    [Header("BGM Settings")]
    public AudioSource BGMSource;

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
        //LoadConversation();
        //ShowLine();
        StartCoroutine(LoadConversationRoutine());
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

    public IEnumerator LoadConversationRoutine()
    {
        string jsonData = "";
        string filePath = Path.Combine(Application.streamingAssetsPath, "Texts", jsonFileName);
        
    #if UNITY_EDITOR || UNITY_STANDALONE
        // --- PC 에디터 또는 PC 빌드 환경 ---

        Debug.Log("PC/Editor 로드 경로: " + filePath);

        if (File.Exists(filePath))
        {
            jsonData = File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("파일을 찾을 수 없습니다: " + filePath);
        }

    #elif UNITY_ANDROID
        // --- Android 빌드 환경 ---

        Debug.Log("Android 로드 시도 경로: " + filePath);

        using (UnityWebRequest www = UnityWebRequest.Get(filePath))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                jsonData = www.downloadHandler.text;
            }
            else
            {
                // 상세 에러와 경로를 함께 출력해 대소문자 오타 확인
                Debug.LogError($"[Load Error] 경로: {filePath} | 에러: {www.error}");
            }
        }

    #endif

        // 데이터 할당 (공통)
        if (!string.IsNullOrEmpty(jsonData))
        {
            conversationData = JsonUtility.FromJson<ConversationData>(jsonData);
            Debug.Log("JSON 파싱 완료");

            yield return null;
            ShowLine();
        }
    }

    /*
     characterImage
    -1 : �ƹ� �̹����� �����?? �ʴ´�.
    0 : ���ΰ� ȥ�� ��ȭâ�� ����, ��ġ �ٲٰ� �迭 3��
    1 : ù ���� - ü���� ��ȭ ����, left element 2, right element 1
    2 : ���ΰ��� ü���� �� ���??. ���ΰ� ���� ȿ��. left element 0, right element 3
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
