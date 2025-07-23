/*
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using StageNormal = Stage.StageNormal;

[CustomEditor(typeof(StageNormal), true)]
public class StageTimerModifier : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SerializedProperty timerProp = serializedObject.FindProperty("timerTime");

        serializedObject.Update();
        EditorGUILayout.PropertyField(timerProp);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
*/