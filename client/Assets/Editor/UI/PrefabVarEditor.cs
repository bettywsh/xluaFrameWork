using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Text;

[CustomEditor(typeof(VarPrefab))]
public class PrefabVarEditor : Editor
{
    VarPrefab mPrefabVar;
    ReorderableList mReordList;

    void OnEnable()
    {
        mPrefabVar = target as VarPrefab;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

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
    
}
