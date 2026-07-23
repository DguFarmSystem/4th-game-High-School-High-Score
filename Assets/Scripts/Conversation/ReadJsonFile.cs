using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class ConversationLine
{
    public int characterImage;
    public string speaker;
    public List<string> texts;

    //selection part
    public int BubbleColor = 0;
    public string EffectType = null;
    public int specificSoundIndex = -1;
    public int ParsePlayerName = 0;
}

[System.Serializable]
public class ConversationData
{
    public List<ConversationLine> conversation;
}

public class ReadJsonFile : MonoBehaviour
{
    [Header("JSON Settings")]
    public string jsonFileName;

    private ConversationData FileData;

    public ConversationData GetFileData()
    {
        return FileData;
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadConversation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

//    public IEnumerator LoadConversationRoutine()
//    {
//        string jsonData = "";
//        string filePath = Path.Combine(Application.streamingAssetsPath, "Texts", jsonFileName);

//#if UNITY_EDITOR || UNITY_STANDALONE
//        // --- PC 에디터 또는 PC 빌드 환경 ---

//        Debug.Log("PC/Editor 로드 경로: " + filePath);

//        if (File.Exists(filePath))
//        {
//            jsonData = File.ReadAllText(filePath);
//        }
//        else
//        {
//            Debug.LogError("파일을 찾을 수 없습니다: " + filePath);
//        }

//#elif UNITY_ANDROID
//        // --- Android 빌드 환경 ---

//        Debug.Log("Android 로드 시도 경로: " + filePath);

//        using (UnityWebRequest www = UnityWebRequest.Get(filePath))
//        {
//            yield return www.SendWebRequest();

//            if (www.result == UnityWebRequest.Result.Success)
//            {
//                jsonData = www.downloadHandler.text;
//            }
//            else
//            {
//                // 상세 에러와 경로를 함께 출력해 대소문자 오타 확인
//                Debug.LogError($"[Load Error] 경로: {filePath} | 에러: {www.error}");
//            }
//        }

//#endif

//        // 데이터 할당 (공통)
//        if (!string.IsNullOrEmpty(jsonData))
//        {
//            FileData = JsonUtility.FromJson<ConversationData>(jsonData);
//            Debug.Log("JSON 파싱 완료");

//            yield return null;
//            ShowLine();
//        }
//    }

    void LoadConversation()
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
            FileData = JsonUtility.FromJson<ConversationData>(jsonData);
            Debug.Log("JSON 파싱 완료");
        }
    }
}
