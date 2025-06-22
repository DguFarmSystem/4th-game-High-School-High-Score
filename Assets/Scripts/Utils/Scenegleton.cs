using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 특정 씬에서만 쓰이는 싱글톤. DontDestroyOnLoad가 호출되지 않음.
/// 씬이 다시 로드되면 해당 Scenegleton 객체는 사라짐
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Scenegleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                    Debug.LogError("No object of " + typeof(T).Name + " is no found");
            }
            return instance;
        }
    }
    public static bool IsValid => instance != null;
    static T instance = null;

    protected virtual void Awake()
    {
        if (instance != null)
        {
            if (instance.gameObject != gameObject)
                Destroy(Instance.gameObject);
        }

        instance = GetComponent<T>();
    }
}