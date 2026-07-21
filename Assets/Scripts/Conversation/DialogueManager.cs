using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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
    public UIController DialogueController;
    public Text speakerText;
    public Text dialogueText;
    private int NextImageIndex = 0;

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

    [Header("Json File Settings")]
    public ReadJsonFile JsonReader;
    protected ConversationData conversationData;
    protected int currentIndex = 0;
    private static int matchCount = 0;
     
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

        StartCoroutine(LoadJsonRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowLine()
    {
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
                DialogueController.SetNarration();
            }
            else if(line.characterImage == 0)
            {
                DialogueController.SetMyCharacterTalk();
            }
            else if(line.characterImage == 1)
            {
                DialogueController.SetCharacterTalk();
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

    IEnumerator LoadJsonRoutine()
    {
        //데이터가 들어올 1프레임의 시간
        yield return null;
        conversationData = JsonReader.GetFileData();
        ShowLine();
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
        DialogueController.DialogueBox.SetActive(false);
        while (time < duration)
        {
            time += Time.deltaTime;
            Color c = DialogueController.Background.color;
            c.a = Mathf.Lerp(1f, 0f, time / duration);
            DialogueController.Background.color = c;
            yield return null;
        }

        //3초 기다리고
        yield return new WaitForSeconds(3.0f);
        //배경 이미지 바꾸고 다시 FadeIn
        DialogueController.Background.sprite = DialogueController.BackGroundImageList[NextImageIndex];
        ++NextImageIndex;
        DialogueController.DialogueBox.SetActive(true);

        time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            Color c = DialogueController.Background.color;
            c.a = Mathf.Lerp(0f, 1f, time / duration);
            DialogueController.Background.color = c;
            yield return null;
        }
    }

    IEnumerator ConfusingCoroutine(float duration)
    {
        float time = 0.0f;
        DialogueController.DialogueBox.SetActive(false);
        DialogueController.Background.sprite = DialogueController.BackGroundImageList[NextImageIndex];
        ++NextImageIndex;
        while (time < duration)
        {
            time += Time.deltaTime;
            Color c = DialogueController.Background.color;
            c.a = Mathf.Lerp(0f, 1f, time / duration);
            DialogueController.Background.color = c;
            yield return null;
        }

        //3초 기다리고
        yield return new WaitForSeconds(3.0f);
        DialogueController.DialogueBox.SetActive(true);
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
            case "Debut":
                {
                    StartCoroutine(FadeIn(DialogueController.Character, 1f));
                    break;
                }

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
                    DialogueController.Background.sprite = DialogueController.BackGroundImageList[NextImageIndex];
                    ++NextImageIndex;
                    StartCoroutine(FadeIn(DialogueController.Background, 0.2f));
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
        Debug.Log("End Dialogue");
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
