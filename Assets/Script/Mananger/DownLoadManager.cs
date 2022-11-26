using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Net;
using System;
using UnityEngine;

public class ThreadEvent
{
    public string Key;
    public List<object> evParams = new List<object>();
}

public class NotiConst
{
    public const string UPDATE_DOWNLOAD = "UpdateDownload";
    public const string UPDATE_PROGRESS = "UpdateProgress";
    public const string UPDATE_ERROR = "UPDATE_ERROR";
}

public class NotiData
{
    public string evName;
    public object evParam1;
    public object evParam2;

    public NotiData(string name, object param1 = null, object param2 = null)
    {
        this.evName = name;
        this.evParam1 = param1;
        this.evParam2 = param2;
    }
}

public class DownLoadManager : MonoSingleton<DownLoadManager>
{
    private Thread thread;
    private Action<NotiData> func;
    private Stopwatch sw = new Stopwatch();
    private string currDownFile = string.Empty;

    static readonly object m_lockObject = new object();
    static Queue<ThreadEvent> events = new Queue<ThreadEvent>();

    delegate void ThreadSyncEvent(NotiData data);
    private ThreadSyncEvent m_SyncEvent;
    private double totalSeconds = 0;

    void Awake()
    {
        m_SyncEvent = OnSyncEvent;
        thread = new Thread(OnUpdate);
        thread.Start();
    }
    /// <summary>
    /// 添加到事件队列
    /// </summary>
    public void AddEvent(ThreadEvent ev, Action<NotiData> func)
    {
        lock (m_lockObject)
        {
            this.func = func;
            events.Enqueue(ev);
        }
    }

    /// <summary>
    /// 通知事件
    /// </summary>
    /// <param name="state"></param>
    private void OnSyncEvent(NotiData data)
    {
        if (this.func != null) func(data);  //回调逻辑层
    }

    // Update is called once per frame
    void OnUpdate()
    {
        while (true)
        {
            lock (m_lockObject)
            {
                if (events.Count > 0)
                {
                    ThreadEvent e = events.Dequeue();
                    try
                    {
                        OnDownloadFile(e.evParams);
                    }
                    catch (System.Exception ex)
                    {
                        UnityEngine.Debug.LogError(ex.Message);
                    }
                }
            }
            Thread.Sleep(1);
        }
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    void OnDownloadFile(List<object> evParams)
    {
        string url = evParams[0].ToString();
        currDownFile = evParams[1].ToString();
        using (WebClient client = new WebClient())
        {
            sw.Start();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DownLoadComplete);
            client.DownloadFileAsync(new System.Uri(url), currDownFile);
        }
    }
    private void DownLoadComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        //UnityEngine.Debug.LogError(e.Error);
        sw.Reset();
        if (e.Error != null)
        {
            NotiData data = new NotiData(NotiConst.UPDATE_DOWNLOAD, currDownFile);
            if (m_SyncEvent != null) m_SyncEvent(data);
        }
        else
        {           
            NotiData data = new NotiData(NotiConst.UPDATE_ERROR, currDownFile);
            if (m_SyncEvent != null) m_SyncEvent(data);
        }        
    }


    private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        //if (sw.Elapsed.TotalSeconds / 1 > totalSeconds)
        //{
        //    totalSeconds = sw.Elapsed.TotalSeconds / 1;
            string value = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));
            NotiData data = new NotiData(NotiConst.UPDATE_PROGRESS, value, e.BytesReceived);
            if (m_SyncEvent != null) m_SyncEvent(data);
        //}        
    }


    /// <summary>
    /// 应用程序退出
    /// </summary>
    public void OnDestroy()
    {
        thread.Abort();
    }
}
