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
        }

        if (_instance == this)
        {
            // 이미 Instance 프로퍼티를 통해 나 자신으로 설정된 경우라도 
            // 여기서 파괴 방지를 한 번 더 보장해줍니다.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 정말로 중복된 다른 객체라면 파괴
            Destroy(gameObject);
        }
    }
}