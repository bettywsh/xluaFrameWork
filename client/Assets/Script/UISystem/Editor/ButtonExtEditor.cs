using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(ButtonExt), true)]
[CanEditMultipleObjects]
public class ButtonExtEditor : SelectableEditor
{
    SerializedProperty m_OnClickProperty;
    SerializedProperty m_OnDoubleClickProperty;
    SerializedProperty m_OnLongPressProperty;
    SerializedProperty m_OnLongClickProperty;
    SerializedProperty m_ScaleTweenInfo;
    SerializedProperty m_DoubleTime;
    SerializedProperty m_LongClickTime;
    SerializedProperty m_LongPressTime;
    SerializedProperty m_LongIntervalTime;


    private bool m_ShowCallbacks;
    ButtonExt btn;
    protected override void OnEnable()
    {
        base.OnEnable();
        m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
        m_OnDoubleClickProperty = serializedObject.FindProperty("m_OnDoubleClick");
        m_OnLongClickProperty = serializedObject.FindProperty("m_OnLongClick");
        m_OnLongPressProperty = serializedObject.FindProperty("m_OnLongPress");
        m_ScaleTweenInfo = serializedObject.FindProperty("m_ScaleTweenInfo");
        m_DoubleTime = serializedObject.FindProperty("m_DoubleTime");
        m_LongClickTime = serializedObject.FindProperty("m_LongClickTime");
        m_LongPressTime = serializedObject.FindProperty("m_LongPressTime");
        m_LongIntervalTime = serializedObject.FindProperty("m_LongIntervalTime");

        
        btn = target as ButtonExt;

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginVertical("box");
        m_ScaleTweenInfo.FindPropertyRelative("shouldTween").boolValue = EditorGUILayout.Toggle("ScaleTween", m_ScaleTweenInfo.FindPropertyRelative("shouldTween").boolValue);
        if (m_ScaleTweenInfo.FindPropertyRelative("shouldTween").boolValue)
        {
            m_ScaleTweenInfo.FindPropertyRelative("localScale").vector3Value = EditorGUILayout.Vector3Field("localScale", m_ScaleTweenInfo.FindPropertyRelative("localScale").vector3Value);
            m_ScaleTweenInfo.FindPropertyRelative("duration").floatValue = EditorGUILayout.FloatField("duration", m_ScaleTweenInfo.FindPropertyRelative("duration").floatValue);
        }
        else
        {
            m_ScaleTweenInfo.FindPropertyRelative("localScale").vector3Value = new Vector3(0.9f, 0.9f, 1);
            m_ScaleTweenInfo.FindPropertyRelative("duration").floatValue = 0.2f;
        }


        EditorGUILayout.EndVertical();
        
         var foldoutStyle = new GUIStyle("Foldout");
            foldoutStyle.fontStyle = FontStyle.Bold;
            m_ShowCallbacks = EditorGUILayout.Foldout(m_ShowCallbacks, "Callbacks", foldoutStyle);
            ++EditorGUI.indentLevel;
            if(m_ShowCallbacks)
            {
                EditorGUILayout.Space();
                //EditorGUILayout.LabelField("CallBack", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(m_OnClickProperty);
                EditorGUILayout.Space();
                m_DoubleTime.floatValue = EditorGUILayout.FloatField("Double Click Time(s)", Mathf.Clamp(m_DoubleTime.floatValue, 0.1f, 10f));
                bool shouldDouble = (btn.onDoubleClick.GetPersistentEventCount() > 0);
                EditorGUILayout.PropertyField(m_OnDoubleClickProperty);
                EditorGUILayout.Space();
                m_LongClickTime.floatValue = EditorGUILayout.Slider("Long Click Time(s)", m_LongClickTime.floatValue, 0.2f, 10f);
                bool shouldLongClick = (btn.onLongClick.GetPersistentEventCount() > 0);
                if (shouldDouble && shouldLongClick)
                {
                    if (m_LongClickTime.floatValue < m_DoubleTime.floatValue)
                    {
                        EditorGUILayout.HelpBox("长点击的触发时间应大于双击间隔", MessageType.Warning);
                    }
                }
                EditorGUILayout.PropertyField(m_OnLongClickProperty);
                EditorGUILayout.Space();
                m_LongPressTime.floatValue = EditorGUILayout.Slider("Long Press Time(s)", m_LongPressTime.floatValue, 0.2f, 10f);
                m_LongIntervalTime.floatValue = EditorGUILayout.Slider("Interval Time(s)", m_LongIntervalTime.floatValue, 0.1f, 10);
                bool shouldPress = (btn.onLongPress.GetPersistentEventCount() > 0);
                if (shouldDouble && shouldPress)
                {
                    if (m_LongPressTime.floatValue < m_DoubleTime.floatValue)
                    {
                        EditorGUILayout.HelpBox("长按的触发时间应大于双击间隔", MessageType.Warning);
                    }
                }
                EditorGUILayout.PropertyField(m_OnLongPressProperty);
                EditorGUILayout.EndVertical();
            }

        serializedObject.ApplyModifiedProperties();
    }
}