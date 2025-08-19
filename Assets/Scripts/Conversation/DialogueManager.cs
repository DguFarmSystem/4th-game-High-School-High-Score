using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class ConversationLine
{
    public int id;
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

    [Header("JSON Settings")]
    public string jsonFileName = "Tutorial.json";

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    private ConversationData conversationData;
    private int currentIndex = 0;

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

    void ShowLine()
    {
        if (conversationData != null && currentIndex < conversationData.conversation.Count)
        {
            ConversationLine line = conversationData.conversation[currentIndex];
            speakerText.text = line.speaker;

            // coroutine으로 타이핑 효과 시작
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLine(line));
        }
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
