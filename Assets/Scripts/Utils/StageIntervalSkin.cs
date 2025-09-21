using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loading/StageIntervalSkin")]
public class StageIntervalSkin : ScriptableObject {
    public string skinName;

    [Header("Background Sprites")]

    public Sprite backgroundDefault;
    public Sprite backgroundSuccess;
    public Sprite backgroundFailure;

    [Header("Character Sprites")]

    public Sprite characterDefault;
    public Sprite characterSuccess;
    public Sprite characterFailure;

    [Header("HP")]

    public Sprite hpDefault;
    public RuntimeAnimatorController animatorControllerHP;

    [Header("Stage Count Sprites")]

    public List<Sprite> stageCountImage;

    [Header("Optional Layout")]

    public GameObject optionalLayoutPrefab; // null이면 기본 레이아웃 사용
}
