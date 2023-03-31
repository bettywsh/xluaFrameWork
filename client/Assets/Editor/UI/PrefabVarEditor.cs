using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Text;

[CustomEditor(typeof(VarPrefab))]
public class PrefabVarEditor : BaseEditor
{
    VarPrefab mPrefabVar;
    ReorderableList mReordList;

    void OnEnable()
    {
        mPrefabVar = target as VarPrefab;
        mReordList = this.CreateItemList();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        mReordList.DoLayoutList();
        // if (GUILayout.Button("Auto bind"))
        // {
        //     Undo.RecordObject(mPrefabVar, "Auto Bind");
        //     mPrefabVar.AutoBind();
        // }
        // if (GUILayout.Button("Clear bind"))
        // {
        //     Undo.RecordObject(mPrefabVar, "Clear Bind");
        //     mPrefabVar.varData.Clear();
        // }
        
        if (GUILayout.Button("Auto bind"))
        {
            Undo.RecordObject(mPrefabVar, "Clear Bind");
            mPrefabVar.varData.Clear();
            Undo.RecordObject(mPrefabVar, "Auto Bind");
            mPrefabVar.AutoBind();
        }
        
        if (GUILayout.Button("Create Lua Panel"))
        {
             var fullFilePath =
                EditorUtility.SaveFilePanel($"Please select a folder to create", Application.dataPath + "/App/Lua/UI/Panel", mPrefabVar.gameObject.name, "lua");
            fullFilePath = fullFilePath + ".bytes";
            if (fullFilePath.StartsWith(".."))
            {
                EditorUtility.DisplayDialog("错误", "当前指定路径并不在任何一个lua根目录下!!!", "知道了");
                return;
            }
       
            mPrefabVar.SetLuaPath("UI/Panel/" + mPrefabVar.gameObject.name);
            string templua = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Editor/UI/TempLuaPanel.lua.bytes").text;
            templua = templua.Replace("#SCRIPTNAME#", mPrefabVar.gameObject.name);
            byte[] buffer1 = Encoding.Default.GetBytes(templua.ToString() );
            byte[] buffer2 = Encoding.Convert(Encoding.UTF8, Encoding.Default, buffer1, 0, buffer1.Length);
            File.WriteAllBytes(fullFilePath, buffer2);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("成功", "创建lua成功!!!", "知道了");
        }
        
        if (GUILayout.Button("Create Lua Cell"))
        {
            var fullFilePath =
                EditorUtility.SaveFilePanel($"Please select a folder to create", Application.dataPath + "/App/Lua/UI/Cell", mPrefabVar.gameObject.name.Replace("Panel","Cell"), "lua");
            fullFilePath = fullFilePath + ".bytes";
            if (fullFilePath.StartsWith(".."))
            {
                EditorUtility.DisplayDialog("错误", "当前指定路径并不在任何一个lua根目录下!!!", "知道了");
                return;
            }
            mPrefabVar.SetLuaPath("UI/Cell/" + mPrefabVar.gameObject.name);
            string templua = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Editor/UI/TempLuaCell.lua.bytes").text;
            templua = templua.Replace("#SCRIPTNAME#", mPrefabVar.gameObject.name);
            byte[] buffer1 = Encoding.Default.GetBytes(templua.ToString() );
            byte[] buffer2 = Encoding.Convert(Encoding.UTF8, Encoding.Default, buffer1, 0, buffer1.Length);
            File.WriteAllBytes(fullFilePath, buffer2);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("成功", "创建lua成功!!!", "知道了");    
        }
        serializedObject.ApplyModifiedProperties();
    }

    private ReorderableList CreateItemList()
    {
        void OnAddItem(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("objName").stringValue = string.Empty;
            // element.FindPropertyRelative("type").enumValueIndex = 0;
            // element.FindPropertyRelative("lastType").enumValueIndex = 0;
            element.FindPropertyRelative("objValue").objectReferenceValue = null;
        }

        void OnRemoveItem(ReorderableList list)
        {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete the var?", "Yes", "No"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);

                if (mReordList.index == mPrefabVar.varData.Count - 1)
                {
                    serializedObject.FindProperty("m_selectedIndex").intValue = mReordList.index = mReordList.index - 1;
                }
            }
        }

        void OnSelectItem(ReorderableList list)
        {
            serializedObject.FindProperty("m_selectedIndex").intValue = list.index;
            serializedObject.ApplyModifiedProperties();
            GUI.changed = true;
        }

        void OnReorderItem(ReorderableList list)
        {
            Repaint();
        }

        var reordList = CreateRecordList(serializedObject, "varData", "Prefab Var List", OnReorderItem, OnSelectItem, OnAddItem, OnRemoveItem);
        reordList.index = serializedObject.FindProperty("m_selectedIndex").intValue;
        reordList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            if (reordList.elementHeight == 0) return;
            var e = reordList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 160, EditorGUIUtility.singleLineHeight),
                e.FindPropertyRelative("objName"), GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.x + 160, rect.y, 160, EditorGUIUtility.singleLineHeight),
                e.FindPropertyRelative("objValue"), GUIContent.none);
        };
        return reordList;
    }
    
}
