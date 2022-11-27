using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Launch : MonoBehaviour
{
    private string FirstRunApp = "FirstRunApp3";
    void Awake()
    {
        //Debug.unityLogger.logEnabled = AppConst.DebugLog;

        DontDestroyOnLoad(gameObject);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = AppConst.GameFrameRate;        

        if (PlayerPrefs.GetInt(FirstRunApp) == 0)
        {
            DeletePersistentDataPath();
        }


        StartUpdate();
    }

    public void StartUpdate()
    {
        DownLoadManager.Instance.Init();
        MessageManager.Instance.Init();
        SoundManager.Instance.Init();
        UIManager.Instance.Init();
        AssetBundleManager.Instance.Init();
        LuaManager.Instance.Init();
        UpdateManager.Instance.Init();
        ResManager.Instance.Init();      
        AtlasManager.Instance.Init();
    }

    public void StartGame()
    {
        DownLoadManager.Instance.OnDestroy();
        UpdateManager.Instance.OnDestroy();

        LuaManager.Instance.DoFile("StartModule");
        TimerManager.Instance.Init();
        NetManager.Instance.Init();
        HttpManager.Instance.Init();
   
        AssetBundleManager.Instance.Init();
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

    void DeletePersistentDataPath()
    {
        string[] files = null;
        string[] dirs = null;
        files = Directory.GetFiles(Application.persistentDataPath, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }
        dirs = Directory.GetDirectories(Application.persistentDataPath);
        for (int i = 0; i < dirs.Length; i++)
        {
            Directory.Delete(dirs[i], true);
        }
        PlayerPrefs.SetInt(FirstRunApp, 1);
    }

}
