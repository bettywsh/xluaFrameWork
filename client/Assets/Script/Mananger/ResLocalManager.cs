using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;
using XLua;
using UnityEditor;

public class ResLocalManager : MonoSingleton<ResLocalManager>
{
    // Start is called before the first frame update
    public void LoadLocalUObjectAsync(string relativePath, ResType resType, Action<UObject> sharpFunc = null, LuaFunction luaFunc = null)
    {
        string assetName = ResPath.GetEditorAssetName(relativePath, resType);
#if UNITY_EDITOR
        var obj = AssetDatabase.LoadAssetAtPath<UObject>(assetName);
        if (sharpFunc != null)
        {
            sharpFunc(obj);
        }
        if (luaFunc != null)
        {
            luaFunc.Call(obj);
            luaFunc.Dispose();
        }
#endif
    }

    public UObject LoadLocalUObject(string relativePath, ResType resType)
    {
        string assetName = ResPath.GetEditorAssetName(relativePath, resType);
#if UNITY_EDITOR
        var obj = AssetDatabase.LoadAssetAtPath<UObject>(assetName);
        return obj;
#else
        return null;
#endif
    }

}
