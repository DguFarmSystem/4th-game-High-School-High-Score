using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
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
    public string EffectType = null;
    public int specificSoundIndex = -1;
    public int ParsePlayerName = 0;
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
    //private string prevScene;

    [SerializeField]
    private bool isEndConv;

    [Header("Audio Source")]
    public AudioSource TypingSound;
    public AudioSource EventSound = null;
    public List<AudioClip> EffectSounds;

    [Header("UI Components")]
    public Text speakerText;
    public Text dialogueText;
    public GameObject DialogueBox;
    public GameObject Character1;
    public GameObject Character2;
    public List<Sprite> ImageList;
    

    [Header("BackGroundImage Resource")]
    public GameObject Background;
    public List<Sprite> BackGroundImageList;
    private int NextImageIndex = 0;

    [Header("JSON Settings")]
    public string jsonFileName;
    private static int matchCount = 0;

    //[Header("Typing Settings")]
    public float typingSpeed => DataManager.Instance != null && DataManager.Instance.Settings != null
        ? DataManager.Instance.Settings.GetScriptSpeed() switch
        {
            ScriptSpeedState.Slow => 0.1f,
            ScriptSpeedState.Normal => 0.05f,
            ScriptSpeedState.Fast => 0.02f,
            _ => 0.05f,
        }
        : 0.05f; // Default to Normal if settings are unavailable

    [Header("BGM Settings")]
    public AudioSource BGMSource;

    protected ConversationData conversationData;
    protected int currentIndex = 0;
     
    protected Image Character1Img;
    protected Image Character2Img;
    protected Image BackgroundImg;
     
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
            dialogueText.text = currentLineFullText; // ISSUE: 치환된 <PLAYER_NAME> 토큰이 포함된 전체 텍스트를 즉시 표시해야함!
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
        //prevScene = SceneManager.GetActiveScene().name;

        Character1Img = Character1.GetComponent<Image>();
        Character2Img = Character2.GetComponent<Image>();
        BackgroundImg = Background.GetComponent<Image>();
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
        //Settings for clearing image
        Color c = Character2Img.color;
        c.a = 0;

        if (conversationData != null && currentIndex < conversationData.conversation.Count)
        {
            ConversationLine line = conversationData.conversation[currentIndex];
            if(!string.IsNullOrEmpty(line.EffectType))
            {
                ExecuteEffect(line.EffectType);
            }
            speakerText.text = ResolveSpeakerName(line.speaker);

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

    ///////////////////////////////////////IEnumerator Part///////////////////////////////////////

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
            // 원본 텍스트에서 모든 <PLAYER_NAME> 토큰을 미리 치환
            currentLineFullText = line.texts[currentTextIndex];
            //int matchCount = 0;
            if(line.ParsePlayerName == 1)
            {
                string playerName = GetPlayerNameOrDefault();

                currentLineFullText = Regex.Replace(
                    currentLineFullText,
                    "<PLAYER_NAME>",
                    match =>
                    {
                        matchCount++; // 매칭될 때마다 1씩 증가 (첫 번째는 1, 두 번째는 2)

                        int targetLength = 1; // 기본값

                        // 순서에 따라 원하는 길이 지정
                        if (matchCount == 1)
                        {
                            // 첫 번째 <PLAYER_NAME> 자르기 규칙
                            targetLength = Math.Min(1, playerName.Length);
                        }
                        else if (matchCount == 2)
                        {
                            // 두 번째 <PLAYER_NAME> 자르기 규칙 (예: 이름의 절반 크기)
                            targetLength = Math.Min(Mathf.RoundToInt((float)playerName.Length / 2), playerName.Length);
                        }
                        else
                        {
                            // 혹시 세 번째 이상이 나올 경우 처리 (필요에 따라)
                            targetLength = playerName.Length;
                        }

                        // 0보다 작아지는 것 방지
                        if (targetLength <= 0) targetLength = 1;

                        return playerName.Substring(0, targetLength);
                    });
            }
            else
            {
                currentLineFullText = currentLineFullText.Replace("<PLAYER_NAME>", GetPlayerNameOrDefault());
            }
            //currentLineFullText = currentLineFullText.Replace("<PLAYER_NAME>", GetPlayerNameOrDefault());

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

            TypingSound.Stop();
            TypingSound.clip = EffectSounds[0];
            TypingSound.loop = true;
            TypingSound.Play();

            // 치환된 텍스트를 글자별로 출력
            for (int i = 0; i < currentLineFullText.Length; i++)
            {
                dialogueText.text += currentLineFullText[i];
                yield return new WaitForSeconds(typingSpeed);
            }

            // 문장 출력 완료 → 사운드 정지
            TypingSound.Stop();
            TypingSound.loop = false;
        }

        isTyping = false;
    }

    IEnumerator BlackoutCoroutine(float duration)
    {
        float time = 0.0f;
        DialogueBox.SetActive(false);
        while (time < duration)
        {
            time += Time.deltaTime;
            Color c = BackgroundImg.color;
            c.a = Mathf.Lerp(1f, 0f, time / duration);
            BackgroundImg.color = c;
            yield return null;
        }

        //3초 기다리고
        yield return new WaitForSeconds(3.0f);
        //배경 이미지 바꾸고 다시 FadeIn
        BackgroundImg.sprite = BackGroundImageList[NextImageIndex];
        ++NextImageIndex;
        DialogueBox.SetActive(true);

        time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            Color c = BackgroundImg.color;
            c.a = Mathf.Lerp(0f, 1f, time / duration);
            BackgroundImg.color = c;
            yield return null;
        }
    }

    IEnumerator ConfusingCoroutine(float duration)
    {
        float time = 0.0f;
        DialogueBox.SetActive(false);
        BackgroundImg.sprite = BackGroundImageList[NextImageIndex];
        ++NextImageIndex;
        while (time < duration)
        {
            time += Time.deltaTime;
            Color c = BackgroundImg.color;
            c.a = Mathf.Lerp(0f, 1f, time / duration);
            BackgroundImg.color = c;
            yield return null;
        }

        //3초 기다리고
        yield return new WaitForSeconds(3.0f);
        DialogueBox.SetActive(true);
    }

    private string ResolveSpeakerName(string speaker)
    {
        if (string.IsNullOrEmpty(speaker)) return speaker;
        if (speaker == "<PLAYER_NAME>") return GetPlayerNameOrDefault();
        return speaker;
    }

    private string GetPlayerNameOrDefault()
    {
        if (DataManager.Instance == null || DataManager.Instance.Player == null)
        {
            return "나학생";
        }

        string playerName = DataManager.Instance.Player.GetName();
        return string.IsNullOrEmpty(playerName) ? "나학생" : playerName;
    }

    public void ExecuteEffect(string Type)
    {
        switch(Type)
        {
            case "BlackOut":
                {
                    StartCoroutine(BlackoutCoroutine(0.2f));
                    break;
                }

            case "Confusing":
                {
                    StartCoroutine(ConfusingCoroutine(0.2f));
                    break;
                }

            case "ImageChange":
                {
                    BackgroundImg.sprite = BackGroundImageList[NextImageIndex];
                    ++NextImageIndex;
                    StartCoroutine(FadeIn(BackgroundImg, 0.2f));
                    break;
                }
        }
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
            /*
            StageManager.Instance.Initialize(
                nextScenes,
                stageKey,
                gameMode,
                prevScene
            );
            */
            LoadingSceneController.Instance.LoadScene(SceneNames.StageInitScene, StageManager.Instance.LoadNextStage);
            
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
