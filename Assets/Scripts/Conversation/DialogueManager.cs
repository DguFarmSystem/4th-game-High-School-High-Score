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
    private int currentTextIndex = 0; // texts 배열 내 현재 문장 인덱스

    void OnEnable()
    {
        // InputManager의 터치 이벤트 구독
        InputManager.OnStageTapPerformed += HandleTap;
    }

    void OnDisable()
    {
        // 이벤트 구독 해제
        InputManager.OnStageTapPerformed -= HandleTap;
    }

    private void HandleTap()
    {
        if(conversationData == null || currentIndex >= conversationData.conversation.Count)
        {
            //여기서 다음 씬으로 넘기면 될듯?
            return;
        }

        if (isTyping)
        {
            // 타이핑 중이면 즉시 전체 문장 표시
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueText.text = currentLineFullText;
            isTyping = false;
        }
        else
        {
            // 현재 화자의 texts 배열에서 다음 문장으로 이동
            ConversationLine line = conversationData.conversation[currentIndex];
            currentTextIndex++;

            if (line.texts != null && currentTextIndex < line.texts.Count)
            {
                typingCoroutine = StartCoroutine(TypeLine(line)); // 다음 문장 출력
            }
            else
            {
                // 현재 화자 모든 문장 완료 → 다음 대사
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
            Debug.LogError("JSON 파일을 찾을 수 없습니다: " + filePath);
        }
    }

    /*
     characterImage
    -1 : 아무 이미지도 띄우지 않는다.
    0 : 주인공 혼자 대화창에 등장, 위치 바꾸고 배열 3번
    1 : 첫 등장 - 체인이 대화 진행, left element 2, right element 1
    2 : 주인공과 체인이 한 장면에. 주인공 강조 효과. left element 0, right element 3
    3 : 체인 강조 효과.
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

            // coroutine으로 타이핑 효과 시작
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
            c.a = Mathf.Lerp(0f, 1f, time / duration); // 0→1로 점점 증가
            img.color = c;
            yield return null;
        }

        c.a = 1f;
        img.color = c;
    }

    IEnumerator TypeLine(ConversationLine line)
    {
        isTyping = true;

        // texts가 있으면 현재 문장만 출력
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

    //디버그용 함수
    private void EndDialogue()
    {
        Debug.Log("대화 종료");
        speakerText.text = "";
        dialogueText.text = "";
    }

    //Skip 버튼용 메서드
    public void SkipAll()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        currentIndex = conversationData.conversation.Count; // 대화 인덱스를 끝으로 이동
        EndDialogue();
    }
}
