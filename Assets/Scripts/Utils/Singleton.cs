using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour를 상속하는 싱글톤.
/// 씬이 바뀌어도 사라지지 않는다.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject container = new GameObject(typeof(T).Name);
                    _instance = container.AddComponent<T>();
                    DontDestroyOnLoad(container);
                }
            }
            return _instance;
        }
    }

    public virtual void Awake()
    {
        RemoveDuplicates();
    }

    private void RemoveDuplicates()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}