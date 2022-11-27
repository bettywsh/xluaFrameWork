using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.U2D;
using XLua;
using System.IO;
using System.Text;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;
using UObject = UnityEngine.Object;

[LuaCallCSharp]
public class ResManager : Singleton<ResManager>
{

    Dictionary<string, List<string>> ResLoaders = new Dictionary<string, List<string>>();


    //resName 资源卸载标识 Common为不卸载 其他通过标识卸载
    public UObject OnLoadAsset(string resName, string relativePath, ResType resType)
    {
        if (relativePath == "Protobuf/msgdef.proto")
        {
            int a = 0;
        }
        if (AppConst.DebugMode)
        {
            return ResLocalManager.Instance.LoadLocalUObject(relativePath, resType);
        }
        else
        {
            string assetName = ResPath.GetAssetName(relativePath, resType);
            string abName = ResPath.GetAssetBunldeName(relativePath, resType);
            AddReloader(resName, abName);
            return AssetBundleManager.Instance.LoadAssetBundleUObject(resName, abName, assetName);
        }
    }

    //resName 资源卸载标识 Common为不卸载 其他通过标识卸载
    public void LoadAssetAsync(string resName, string relativePath, ResType resType, Action<UObject> sharpFunc = null, LuaFunction luaFunc = null)
    {

        if (AppConst.DebugMode)
        {
            string assetName = ResPath.GetEditorAssetName(relativePath, resType);
            ResLocalManager.Instance.LoadLocalUObjectAsync(relativePath, resType, sharpFunc, luaFunc);
        }
        else
        {
            string assetName = ResPath.GetAssetName(relativePath, resType);
            string abName = ResPath.GetAssetBunldeName(relativePath, resType);
            AddReloader(resName, abName);
            AssetBundleManager.Instance.LoadAssetBundleUObjectAsync(resName, abName, assetName, sharpFunc, luaFunc);
        }
    }

    #region 资源加载标识
    public void AddReloader(string resName, string abName)
    {
        if (resName == "Common") return;
        List<string> abNames = null;
        if (!ResLoaders.TryGetValue(resName, out abNames))
        {
            abNames = new List<string>();
            abNames.Add(abName);
            ResLoaders.Add(resName, abNames);
        }
        else
        {
            abNames.Add(abName);
        }
    }

    public void UnLoadAssetBundle(string resLoaderName)
    {
        List<string> abNames = null;
        if (!ResLoaders.TryGetValue(resLoaderName, out abNames))
        {
            return;
        }
        for (int i = 0; i < abNames.Count; i++)
        {
            AssetBundleManager.Instance.UnloadAssetBundle(abNames[i], true);
        }
    }
    #endregion

    public void UnLoadAssetBundle(string relativePath, ResType resType)
    {
        if (!AppConst.DebugMode)
        {
            string abName = ResPath.GetAssetBunldeName(relativePath, resType);
            AssetBundleManager.Instance.UnloadAssetBundle(abName, true);
        }
    }

    public void DestroyCache()
    {
        //uobjectAsyncList.Clear();
    }
}

