using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
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
        StartCoroutine(LoadConversation());
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
//        // --- PC ������ �Ǵ� PC ���� ȯ�� ---

//        Debug.Log("PC/Editor �ε� ���: " + filePath);

//        if (File.Exists(filePath))
//        {
//            jsonData = File.ReadAllText(filePath);
//        }
//        else
//        {
//            Debug.LogError("������ ã�� �� �����ϴ�: " + filePath);
//        }

//#elif UNITY_ANDROID
//        // --- Android ���� ȯ�� ---

//        Debug.Log("Android �ε� �õ� ���: " + filePath);

//        using (UnityWebRequest www = UnityWebRequest.Get(filePath))
//        {
//            yield return www.SendWebRequest();

//            if (www.result == UnityWebRequest.Result.Success)
//            {
//                jsonData = www.downloadHandler.text;
//            }
//            else
//            {
//                // �� ������ ��θ� �Բ� ����� ��ҹ��� ��Ÿ Ȯ��
//                Debug.LogError($"[Load Error] ���: {filePath} | ����: {www.error}");
//            }
//        }

//#endif

//        // ������ �Ҵ� (����)
//        if (!string.IsNullOrEmpty(jsonData))
//        {
//            FileData = JsonUtility.FromJson<ConversationData>(jsonData);
//            Debug.Log("JSON �Ľ� �Ϸ�");

//            yield return null;
//            ShowLine();
//        }
//    }

    IEnumerator LoadConversation()
    {
        string jsonData = "";
        string filePath = Path.Combine(Application.streamingAssetsPath, "Texts", jsonFileName);

#if UNITY_EDITOR || UNITY_STANDALONE
        // --- PC ������ �Ǵ� PC ���� ȯ�� ---

        Debug.Log("PC/Editor �ε� ���: " + filePath);

        if (File.Exists(filePath))
        {
            jsonData = File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("������ ã�� �� �����ϴ�: " + filePath);
        }

#elif UNITY_ANDROID
                // --- Android ���� ȯ�� ---

                Debug.Log("Android �ε� �õ� ���: " + filePath);

                using (UnityWebRequest www = UnityWebRequest.Get(filePath))
                {
                    yield return www.SendWebRequest();

                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        jsonData = www.downloadHandler.text;
                    }
                    else
                    {
                        // �� ������ ��θ� �Բ� ����� ��ҹ��� ��Ÿ Ȯ��
                        Debug.LogError($"[Load Error] ���: {filePath} | ����: {www.error}");
                    }
                }

#endif

        // ������ �Ҵ� (����)
        if (!string.IsNullOrEmpty(jsonData))
        {
            FileData = JsonUtility.FromJson<ConversationData>(jsonData);
            Debug.Log("JSON �Ľ� �Ϸ�");
            yield return null;
        }
    }
}
