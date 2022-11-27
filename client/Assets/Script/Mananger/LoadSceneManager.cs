using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;
using UnityEditor;
using XLua;

[LuaCallCSharp]
public class LoadSceneManager : MonoSingleton<LoadSceneManager>
{
    // 加载进度
    float loadPro = 0;
    bool isfinish = false;
    string cutName = "";
    string lastName = "";
    // 用以接受异步加载的返回值
    AsyncOperation AsyncOp = null;

    public void LoadScene(string name, LuaFunction func)
    {
        ResManager.Instance.LoadAssetAsync(name, name, ResType.Scene, null, func);
    }

    public void ChangeScene(string name)
    {
        isfinish = false;
        cutName = name;

        ResManager.Instance.DestroyCache();

        AsyncOp = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        AsyncOp.allowSceneActivation = false;
        AsyncOp.completed += (AsyncOperation ao) => {
            AsyncOp.allowSceneActivation = true;       
            GC();
            LuaManager.Instance.CallFunction("SceneMgr", "FinishScene");
        };
        //});

    }

    private void Update()
    {
        if (isfinish)
            return;
        if (AsyncOp != null)//如果已经开始加载
        {
            loadPro = AsyncOp.progress; //获取加载进度,此处特别注意:加载场景的progress值最大为0.9!!!
        }
        if (loadPro >= 0.9f)//因为progress值最大为0.9,所以我们需要强制将其等于1
        {
            AsyncOp.allowSceneActivation = true;
        }
        
    }

    public void GC()
    {
        System.GC.Collect();
        LuaManager.Instance.LuaGC();
        Resources.UnloadUnusedAssets();
    }
}
