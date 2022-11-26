using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UObject = UnityEngine.Object;
using XLua;

class UnloadAssetBundleRequest
{
    public string abName;
    public bool unloadNow;
    public AssetBundleInfo abInfo;
}


public class AssetBundleManager : MonoSingleton<AssetBundleManager>
{
    AssetBundleManifest assetBundleManifest = null;
    Dictionary<string, AssetBundleInfo> loadedAssetBundles = new Dictionary<string, AssetBundleInfo>();    
    Dictionary<string, int> assetBundleLoading = new Dictionary<string, int>();
    Dictionary<string, string[]> dependencies = new Dictionary<string, string[]>();
    Dictionary<string, UnloadAssetBundleRequest> assetBundleUnloading = new Dictionary<string, UnloadAssetBundleRequest>();
    Dictionary<string, List<LoadUObjectAsyncRequest>> uobjectAsyncList = new Dictionary<string, List<LoadUObjectAsyncRequest>>();

    public void Init()
    {
        if (!AppConst.DebugMode)
        {
            assetBundleManifest = LoadAssetBundleUObject("Common", ResConst.AssetBundleManifest, ResType.AssetBundleManifest.ToString(), typeof(AssetBundleManifest)) as AssetBundleManifest;
        }
    }

    #region 同步加载
    public UObject LoadAssetBundleUObject(string resName, string abName, string assetName, Type assetType)
    {
        AssetBundleInfo bundle = GetLoadedAssetBundle(abName);
        if (bundle == null)
        {
            OnLoadAssetBundle(abName);
            bundle = GetLoadedAssetBundle(abName);
            if (bundle == null)
            {
                uobjectAsyncList.Remove(abName);
                Debug.LogError("OnLoadAsset--->>>" + abName);
            }
        }

        AssetBundle ab = bundle.assetBundle;
        var request = ab.LoadAsset(assetName, assetType);
        return request;
    }

    public AssetBundleInfo GetLoadedAssetBundle(string abName)
    {
        AssetBundleInfo bundle = null;
        loadedAssetBundles.TryGetValue(abName, out bundle);
        if (bundle == null)
        {
            return null;
        }
        string[] m_dependencies = null;
        if (!dependencies.TryGetValue(abName, out m_dependencies))
            return bundle;

        // Make sure all dependencies are loaded
        foreach (var dependency in m_dependencies)
        {
            AssetBundleInfo dependentBundle;
            loadedAssetBundles.TryGetValue(dependency, out dependentBundle);
            if (dependentBundle == null) return null;
        }
        return bundle;
    }

    public void OnLoadAssetBundle(string abName)
    {
        string path = ResPath.GetAssetBundleFilePath(abName);
        if (assetBundleLoading.ContainsKey(path))
        {
            assetBundleLoading[path]++;
            return;
        }
        assetBundleLoading.Add(path, 1);
        var assetObj = AssetBundle.LoadFromFile(path);
        if (assetObj != null)
        {
            var RefCount = assetBundleLoading[path];
            var bundleInfo = new AssetBundleInfo(assetObj, RefCount);
            loadedAssetBundles.Add(abName, bundleInfo);
        }
    }

    #endregion

    #region 异步加载

    IEnumerator OnLoadAssetBundleAsync(string abName)
    {
        string path = ResPath.GetAssetBundleFilePath(abName);
        if (assetBundleLoading.ContainsKey(path))
        {
            assetBundleLoading[path]++;
            yield break;
        }
        assetBundleLoading.Add(path, 1);
        var request = AssetBundle.LoadFromFileAsync(path);

        string[] dep = assetBundleManifest.GetAllDependencies(abName);
        if (dep.Length > 0)
        {
            dependencies.Add(abName, dep);
            for (int i = 0; i < dep.Length; i++)
            {
                string depName = dep[i];
                AssetBundleInfo bundleInfo = null;
                if (loadedAssetBundles.TryGetValue(depName, out bundleInfo))
                {
                    bundleInfo.referencedCount++;
                }
                else
                {
                    yield return StartCoroutine(OnLoadAssetBundleAsync(depName));
                }
            }
        }

        yield return request;

        AssetBundle assetObj = request.assetBundle;
        if (assetObj != null)
        {
            //var RefCount = assetBundleLoading[path];
            var bundleInfo = new AssetBundleInfo(assetObj, 0);
            loadedAssetBundles.Add(abName, bundleInfo);
        }
        assetBundleLoading.Remove(path);
    }

    public void LoadAssetBundleUObjectAsync(string resName, string abName, string assetName, Type assetType, Action<UObject> sharpFunc = null, LuaFunction luaFunc = null)
    {
        LoadUObjectAsyncRequest request = new LoadUObjectAsyncRequest();
        request.assetNames = assetName;
        request.sharpFunc = sharpFunc;
        request.luaFunc = luaFunc;

        List<LoadUObjectAsyncRequest> requests = null;
        if (!uobjectAsyncList.TryGetValue(assetName, out requests))
        {
            requests = new List<LoadUObjectAsyncRequest>();
            requests.Add(request);
            uobjectAsyncList.Add(abName, requests);       
        }
        else
        {
            requests.Add(request);
        }
        StartCoroutine(OnLoadAssetAsync(abName, assetType));
    }

    IEnumerator OnLoadAssetAsync(string abName, Type assetType)
    {

        AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
        if (bundleInfo == null)
        {
            yield return StartCoroutine(OnLoadAssetBundleAsync(abName));
            bundleInfo = GetLoadedAssetBundle(abName);
            if (bundleInfo == null)
            {
                uobjectAsyncList.Remove(abName);
                Debug.LogError("OnLoadAsset--->>>" + abName);
                yield break;
            }
        }

        List<LoadUObjectAsyncRequest> requests = null;
        if (!uobjectAsyncList.TryGetValue(abName, out requests))
        {
            uobjectAsyncList.Remove(abName);
            yield break;
        }

        for (int i = 0; i < requests.Count; i++)
        {
            string assetNames = requests[i].assetNames;
            UObject result = new UObject();
            
            AssetBundle ab = bundleInfo.assetBundle;
            if (!ab.isStreamedSceneAssetBundle)
            {
                var request = ab.LoadAssetAsync(assetNames, assetType);
                yield return request;
                result = request.asset;
            }
           
            if (requests[i].sharpFunc != null)
            {
                requests[i].sharpFunc(result);
                requests[i].sharpFunc = null;
            }
            if (requests[i].luaFunc != null)
            {
                requests[i].luaFunc.Call((object)result);
                requests[i].luaFunc.Dispose();
                requests[i].luaFunc = null;
            }
            bundleInfo.referencedCount++;
        }
        uobjectAsyncList.Remove(abName);        
    }
    #endregion

    #region 资源卸载
    /// <summary>
    /// 试着去卸载AB
    /// </summary>
    public void UnloadAssetBundle(string abName, bool isThorough = false)
    {
        //string abName = ResPath.GetAssetBunldeName(relativePath, resType);
        UnloadAssetBundleInternal(abName, isThorough);
        UnloadDependencies(abName, isThorough);
        Debug.Log(loadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + abName);
    }


    private void Update()
    {
        TryUnloadAssetBundle();
    }

    private void TryUnloadAssetBundle()
    {
        if (assetBundleUnloading.Count == 0)
        {
            return;
        }
        foreach (var de in assetBundleUnloading)
        {
            if (assetBundleLoading.ContainsKey(de.Key))
            {
                continue;
            }
            var request = de.Value;

            if (request.abInfo != null && request.abInfo.assetBundle != null)
            {
                request.abInfo.assetBundle.Unload(true);
            }
            assetBundleUnloading.Remove(de.Key);
            loadedAssetBundles.Remove(de.Key);
            Debug.Log(de.Key + " has been unloaded successfully");
        }
    }


    private void UnloadAssetBundleInternal(string abName, bool unloadNow)
    {
        AssetBundleInfo bundle = GetLoadedAssetBundle(abName);
        if (bundle == null) return;

        if (--bundle.referencedCount <= 0)
        {
            if (assetBundleLoading.ContainsKey(abName))
            {
                var request = new UnloadAssetBundleRequest();
                request.abName = abName;
                request.abInfo = bundle;
                request.unloadNow = unloadNow;
                assetBundleUnloading.Add(abName, request);
                return;     //如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
            }
            bundle.assetBundle.Unload(unloadNow);
            loadedAssetBundles.Remove(abName);
            Debug.Log(abName + " has been unloaded successfully");
        }
    }

    private void UnloadDependencies(string abName, bool isThorough)
    {
        string[] dep = null;
        if (!dependencies.TryGetValue(abName, out dep))
            return;

        // Loop dependencies.
        foreach (var value in dep)
        {
            UnloadAssetBundleInternal(value, isThorough);
        }
        dependencies.Remove(abName);
    }

    #endregion


 
}


public class AssetBundleInfo
{
    public AssetBundle assetBundle;
    public int referencedCount;

    public AssetBundleInfo(AssetBundle ab, int RefCount = 1)
    {
        assetBundle = ab;
        referencedCount = RefCount;
    }
}


public class LoadUObjectAsyncRequest
{
    public UObject uObject;
    public ResType resType;
    public string assetNames;
    public Action<UObject> sharpFunc;
    public LuaFunction luaFunc;
    public Type assetType;
}
