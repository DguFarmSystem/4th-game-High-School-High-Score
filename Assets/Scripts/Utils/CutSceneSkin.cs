using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MyPair
{
    public string key;
    public GameObject value;
}

[CreateAssetMenu(menuName = "Loading/CutSceneSkin")]
public class CutSceneSkin : ScriptableObject
{
    [Header("CutScene Skin Prefabs")]
    public List<MyPair> NameAndGameObjectPairs;

    // 런타임에서만 딕셔너리 캐싱
    private Dictionary<string, GameObject> _dict;

    public Dictionary<string, GameObject> GetDictionary()
    {
        _dict = NameAndGameObjectPairs.ToDictionary(p => p.key, p => p.value);

        return _dict;
    }
}
