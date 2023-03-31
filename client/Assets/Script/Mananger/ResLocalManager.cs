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
        string assetName = ResPath.GetAssetPath(relativePath, resType);
#if UNITY_EDITOR
        var obj = AssetDatabase.LoadAssetAtPath(assetName, ResPath.GetResTypeToType(resType));
        if (sharpFunc != null)
        {
            sharpFunc(obj);
            sharpFunc = null;
        }
        if (luaFunc != null)
        {
            luaFunc.Call(obj);
            luaFunc.Dispose();
            luaFunc = null;
        }
#endif
    }

    public UObject LoadLocalUObject(string relativePath, ResType resType)
    {
        string assetName = ResPath.GetAssetPath(relativePath, resType);
#if UNITY_EDITOR
        var obj = AssetDatabase.LoadAssetAtPath(assetName, ResPath.GetResTypeToType(resType));
        return obj;
#else
        return null;
#endif
    }

}
