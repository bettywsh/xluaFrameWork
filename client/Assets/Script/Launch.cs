using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Resources;
using System.Linq;

public class Launch : MonoBehaviour
{
    //private string Version = "Version";
    void Awake()
    {
        Debug.unityLogger.logEnabled = AppConst.DebugLog;

        DontDestroyOnLoad(gameObject);
        QualitySettings.vSyncCount = 2;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = AppConst.GameFrameRate;        

        //if (PlayerPrefs.GetString(Version, "") == PlayerSettings.bundleVersion)
        //{
        //    DeletePersistentDataPath();
        //}


        StartUpdate();
    }

    public void StartUpdate()
    {
        SoundManager.Instance.Init();
        UIManager.Instance.Init();
        AssetBundleManager.Instance.Init();
        ResManager.Instance.Init();
        LuaManager.Instance.Init();

        AtlasManager.Instance.Init();
        LuaManager.Instance.DoFile("UpdateModule");
    }

    public void StartGame()
    {
        UpdateManager.Instance.OnDestroy();
        ResManager.Instance.Init();
        //AssetBundleManager.Instance.Init();
        NetworkManager.Instance.Init();
        LuaManager.Instance.DoFile("StartModule");
        //#if UNITY_EDITOR        
        //        EditorApplication.playModeStateChanged += OnUnityPlayModeChanged;
        //#endif
    }

    void OnApplicationQuit()
    {
        MessageManager.Instance.EventNotify(MessageConst.MsgOnApplicationQuit);
    }


    private void OnApplicationPause(bool focus)
    {
        //进入程序状态更改为前台
        if (focus)
        {
            MessageManager.Instance.EventNotify(MessageConst.EventApplicationPause, true);
        }
        else
        {
            //离开程序进入到后台状态
            MessageManager.Instance.EventNotify(MessageConst.EventApplicationPause, false);
        }
    }

   

}
