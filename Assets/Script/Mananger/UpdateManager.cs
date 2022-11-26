using LitJson;
using XLua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[LuaCallCSharp]
public class UpdateManager : MonoSingleton<UpdateManager>
{
    private string ResIpAddress;
    private List<string> downloadFiles = new List<string>();
    private bool isProgress = false;
    private string param1 = "";
    private float param2 = 0;
    private int allDownLoadSize = 0;
    private int curDownLoadSize = 0;
    private string sVersion = "";
    private string sJsonStr = "";
    private bool isError = false;
    private double downloadSizeCount;
    private string FirstRun = "FirstRun3";
    public void CheckVersion(string ip)
    {
        if (!AppConst.UpdateModel)
        {
            MessageManager.Instance.EventNotify(MessageConst.MsgUpdateNo);
            return;
        }
        else
        {
            ResIpAddress = ip;
            //if (PlayerPrefs.GetInt(FirstRun) == 0)
            //{
            //    MessageManager.Instance.EventNotify(MessageConst.MsgUpdateFristCopy);
            //    StartCoroutine(ExtractStreamingAssetsPath());
            //}
            //else
            //{
            //    StartCoroutine(OnCheckVersion());
            //}

            StartCoroutine(OnCheckVersion());
        }
    }

    public string GetAllNeedDownloadSize()
    {
        return HumanReadableFilesize(allDownLoadSize);
    }


    IEnumerator ExtractStreamingAssetsPath()
    {
        string infile = string.Format("{0}/{1}", Application.streamingAssetsPath, ResConst.VerFile);
        string outfile = string.Format("{0}/{1}", Application.persistentDataPath, ResConst.VerFile);
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(infile);
            yield return www;
            if (www.isDone)
            {
                File.WriteAllBytes(outfile, www.bytes);
            }
        }
        else File.Copy(infile, outfile, true);
        yield return new WaitForEndOfFrame();

        infile = string.Format("{0}/{1}", Application.streamingAssetsPath, ResConst.CheckFile);
        outfile = string.Format("{0}/{1}", Application.persistentDataPath, ResConst.CheckFile);
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(infile);
            yield return www;
            if (www.isDone)
            {
                File.WriteAllBytes(outfile, www.bytes);
            }
        }
        else File.Copy(infile, outfile, true);
        yield return new WaitForEndOfFrame();



        // 释放所有文件到数据目录
        string[] files = File.ReadAllLines(outfile);     
        for (int i =0;i< files.Length;i++)        {
            string[] fs = files[i].Split('|');
            infile = string.Format("{0}/{1}", Application.streamingAssetsPath, fs[0]);
            outfile = string.Format("{0}/{1}", Application.persistentDataPath, fs[0]);

            MessageManager.Instance.EventNotify(MessageConst.MsgUpdateFristProgress, i, files.Length);

            string dir = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(infile);
                yield return www;

                if (www.isDone)
                {
                    File.WriteAllBytes(outfile, www.bytes);
                }
                yield return 0;
            }
            else
            {
                if (File.Exists(outfile))
                {
                    File.Delete(outfile);
                }
                File.Copy(infile, outfile, true);
            }
            yield return new WaitForEndOfFrame();
        }
        PlayerPrefs.SetInt(FirstRun, 1);

        //启动版本检测
        StartCoroutine(OnCheckVersion());
    }

    private string GetFilePath(string fileName)
    {
        string serverHttpFile = "";
#if UNITY_ANDROID
        serverHttpFile = string.Format("{0}Android/", ResIpAddress);
#endif
#if UNITY_IOS
        serverHttpFile = string.Format({0}IOS/, ResIpAddress);
#endif
        serverHttpFile += fileName;
        return serverHttpFile;
    }

    IEnumerator OnCheckVersion()
    {
        MessageManager.Instance.EventNotify(MessageConst.MsgUpdateYes);
        string[] serverVersion = new string[3];
        string serverHttpFile = GetFilePath(ResConst.VerFile);
        JsonData jsonDataInfoServer;
        JsonData jsonDataInfoClient;
        UnityWebRequest webRequest = UnityWebRequest.Get(serverHttpFile);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            MessageManager.Instance.EventNotify(MessageConst.MsgUpdateLostConnect);
            yield break;
        }
        else
        {
            sJsonStr = webRequest.downloadHandler.text;
            jsonDataInfoServer = JsonMapper.ToObject(sJsonStr);
            string version = (string)jsonDataInfoServer["version"];

            serverVersion = version.Split('.');
            sVersion = version;

        }
        string[] clientVersion = new string[3];
        string path = string.Format("{0}/{1}", Application.persistentDataPath, ResConst.VerFile);

        if (File.Exists(path) && !string.IsNullOrWhiteSpace(File.ReadAllText(path)))
        {
            string fileContent = File.ReadAllText(path,System.Text.Encoding.UTF8);
            jsonDataInfoClient = JsonMapper.ToObject(fileContent);
            string version = (string)jsonDataInfoClient["version"];
            clientVersion = version.Split('.');
        }
        else
        {

#if UNITY_ANDROID
        path = string.Format("{0}/{1}", Application.streamingAssetsPath, ResConst.VerFile);
#endif

#if UNITY_IOS
        path = string.Format("file://{0}/{1}", Application.streamingAssetsPath, ResConst.VerFile);
#endif
            UnityWebRequest uwr = UnityWebRequest.Get(path);
            yield return uwr.SendWebRequest();

            jsonDataInfoClient = JsonMapper.ToObject(uwr.downloadHandler.text);

            string version = (string)jsonDataInfoClient["version"];
            clientVersion = version.Split('.');
        }

        AppConst.GameVersion = sVersion;

        if (int.Parse(serverVersion[0]) > int.Parse(clientVersion[0]))
        {
            //大版本更新
            MessageManager.Instance.EventNotify(MessageConst.MsgUpdateBigVersion, GetDownloadURLFromJSON(jsonDataInfoServer));
            yield break;
        }
        else if (int.Parse(serverVersion[1]) > int.Parse(clientVersion[1]) || int.Parse(serverVersion[2]) > int.Parse(clientVersion[2]))
        {
            //小版本更新
            StartCoroutine(CancelTotalDownload());
}
        else
        {
            //不需要更新
            MessageManager.Instance.EventNotify(MessageConst.MsgUpdateNo);
        }
    }

    //通过JSON的解析获取到下载的URL内容
    public string GetDownloadURLFromJSON(JsonData jsonDataInfo)
    {
        JsonData jsonData = jsonDataInfo["channel"];

        for (int index = 0; index < jsonData.Count; index++)
        {
            int channelID = (int)jsonData[index]["channelID"];


            if(channelID == AppConst.ChannelID)
            {
                return (string)jsonData[index]["URL"];
            }
        }

        return (string)jsonDataInfo["url"]; ;

    }

    //计算所需要下载的时间
    IEnumerator CancelTotalDownload()
    {
        string serverFile = "";
        string serverHttpFile = GetFilePath(ResConst.CheckFile);

        UnityWebRequest webRequest = UnityWebRequest.Get(serverHttpFile);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            yield break;
        }
        else
        {
            serverFile = webRequest.downloadHandler.text;
        }
        string clientFile = "";
        string path = string.Format("{0}/{1}", Application.persistentDataPath, ResConst.CheckFile);
        if (File.Exists(path))
        {
            clientFile = File.ReadAllText(path);
        }
        else
        {


#if UNITY_ANDROID
        path = string.Format("{0}/{1}", Application.streamingAssetsPath, ResConst.CheckFile);
#endif

#if UNITY_IOS
        path = string.Format("file://{0}/{1}", Application.streamingAssetsPath, ResConst.CheckFile);
#endif

            UnityWebRequest uwr = UnityWebRequest.Get(path);
            yield return uwr.SendWebRequest();
            clientFile = uwr.downloadHandler.text;
        }

        string[] serverFiles = serverFile.Split('\n');
        string[] clientFiles = clientFile.Split('\n');
        Dictionary<string, string> sFiles = new Dictionary<string, string>();
        Dictionary<string, string> cFiles = new Dictionary<string, string>();
        for (int i = 0; i < clientFiles.Length; i++)
        {
            if (clientFiles[i] != "")
            {
                string[] cFile = clientFiles[i].Split('|');
                cFiles.Add(cFile[0], clientFiles[i]);
            }
        }
        for (int i = 0; i < serverFiles.Length; i++)
        {
            if (serverFiles[i] != "")
            {
                string[] sFile = serverFiles[i].Split('|');
                sFiles.Add(sFile[0], serverFiles[i]);
            }
        }

        double download = 0;

        foreach (KeyValuePair<string, string> keyValuePair in sFiles)
        {
            string[] sFile = keyValuePair.Value.ToString().Split('|');
            string file = null;
            cFiles.TryGetValue(sFile[0], out file);
            if (file == null)
            {
                //新增
                download += int.Parse(sFile[1]);
            }
            else
            {
                if (sFile[2] != cFiles[sFile[0]].Split('|')[2] || sFile[3] != cFiles[sFile[0]].Split('|')[3])
                {
                    //修改
                    download += int.Parse(sFile[1]);
                }
                
            }
        }

        MessageManager.Instance.EventNotify(MessageConst.MsgUpdateSmallVersion, HumanReadableFilesize(download));
    }


    /// <summary>
    /// 转换方法
    /// </summary>
    /// <param name="size">字节值</param>
    /// <returns></returns>
    private string HumanReadableFilesize(double size)
    {
        String[] units = new String[] { "B", "KB", "MB", "GB", "TB", "PB" };
        double mod = 1024.0;
        int i = 0;
        while (size >= mod)
        {
            size /= mod;
            i++;
        }
        return Math.Round(size) + units[i];
    }



    public void DownLoadFiles()
    {
        StartCoroutine(OnCheckFiles());
    }

    IEnumerator OnCheckFiles()
    {
        string serverFile = "";
        string serverHttpFile = GetFilePath(ResConst.CheckFile);

        UnityWebRequest webRequest = UnityWebRequest.Get(serverHttpFile);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            yield break;
        }
        else
        {
            serverFile = webRequest.downloadHandler.text;
        }
        string clientFile = "";
        string path = string.Format("{0}/{1}", Application.persistentDataPath, ResConst.CheckFile);
        if (File.Exists(path))
        {
            clientFile = File.ReadAllText(path);
        }
        else
        {

#if UNITY_ANDROID
        path = string.Format("{0}/{1}", Application.streamingAssetsPath, ResConst.CheckFile);
#endif

#if UNITY_IOS
            path = string.Format("file://{0}/{1}", Application.streamingAssetsPath, ResConst.CheckFile);
#endif


            UnityWebRequest uwr = UnityWebRequest.Get(path);
            yield return uwr.SendWebRequest();
            clientFile = uwr.downloadHandler.text;
        }

        string[] serverFiles = serverFile.Split('\n');
        string[] clientFiles = clientFile.Split('\n');
        Dictionary<string, string> sFiles = new Dictionary<string, string>();
        Dictionary<string, string> cFiles = new Dictionary<string, string>();
        for (int i = 0; i < clientFiles.Length; i++)
        {
            if (clientFiles[i] != "")
            {
                string[] cFile = clientFiles[i].Split('|');
                cFiles.Add(cFile[0], clientFiles[i]);
            }
        }
        for (int i = 0; i < serverFiles.Length; i++)
        {
            if (serverFiles[i] != "")
            {
                string[] sFile = serverFiles[i].Split('|');
                sFiles.Add(sFile[0], serverFiles[i]);
            }
        }
        //string writeClientFiles = "";
        foreach(KeyValuePair<string, string> keyValuePair in cFiles)
        {
            string cfile = keyValuePair.Value;
            string[] cFile = cfile.Split('|');
            string sfile = null;
            sFiles.TryGetValue(cFile[0], out sfile);
            if (sfile == null)
            {


#if UNITY_ANDROID
         //删除
                string filePathe = string.Format("{0}/{1}", Application.streamingAssetsPath, cFile[0]);
                if (File.Exists(filePathe))
                    File.Delete(filePathe);
#endif

#if UNITY_IOS
                //删除
                string filePathe = string.Format("file://{0}/{1}", Application.streamingAssetsPath, cFile[0]);
                if (File.Exists(filePathe))
                    File.Delete(filePathe);
#endif
            }
        }

        string writeClientFiles = "";
        List<DownLoadFile> needUpdate = new List<DownLoadFile>();
        foreach (KeyValuePair<string, string> keyValuePair in sFiles)
        {
            string[] sFile = keyValuePair.Value.ToString().Split('|');
            string file = null;
            cFiles.TryGetValue(sFile[0], out file);
            if (file == null)
            {
                //新增
                allDownLoadSize += int.Parse(sFile[1]);
                DownLoadFile dlf = new DownLoadFile();
                dlf.file = sFile[0];
                dlf.fileInfo = keyValuePair.Value.ToString();
                needUpdate.Add(dlf);
            }
            else
            {
                if (sFile[2] != cFiles[sFile[0]].Split('|')[2] || sFile[3] != cFiles[sFile[0]].Split('|')[3])
                {
                    //修改
                    allDownLoadSize += int.Parse(sFile[1]);
                    DownLoadFile dlf = new DownLoadFile();
                    dlf.file = sFile[0];
                    dlf.fileInfo = keyValuePair.Value;
                    needUpdate.Add(dlf);
                }
                else
                {
                    writeClientFiles += keyValuePair.Value + "\n";
                }
            }
        }
        if(needUpdate.Count <= 0)
            writeClientFiles = writeClientFiles.Substring(0, writeClientFiles.Length - 2);
        StartCoroutine(OnDownLoad(needUpdate, writeClientFiles));
    }
    IEnumerator OnDownLoad(List<DownLoadFile> needUpdate, string writecFiles)
    {
        string writeClientFiles = writecFiles;
        for (int i = 0; i < needUpdate.Count; i++)
        {
            curDownLoadSize += int.Parse(needUpdate[i].fileInfo.Split('|')[1]);
            string savePath = string.Format("{0}/{1}", Application.persistentDataPath, needUpdate[i].file);
            string dir = savePath.Substring(0, savePath.LastIndexOf("/") + 1);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            BeginDownload(GetFilePath(needUpdate[i].file), savePath);
            while (!(IsDownOK(savePath)))
            {
                if (isError)
                {
                    BeginDownload(GetFilePath(needUpdate[i].file), savePath);
                    isError = false;
                }
                yield return new WaitForEndOfFrame();
            }
            if (i == needUpdate.Count)
            {
                writeClientFiles += needUpdate[i].fileInfo;
            }
            else
            { 
                writeClientFiles += needUpdate[i].fileInfo + "\n";
            }
            File.WriteAllText(string.Format("{0}/{1}", Application.persistentDataPath, ResConst.CheckFile), writeClientFiles);
        }
        isProgress = false;
        MessageManager.Instance.EventNotify(MessageConst.MsgUpdateDownLoadComplete);
        File.WriteAllText(string.Format("{0}/{1}", Application.persistentDataPath, ResConst.VerFile), sJsonStr, new System.Text.UTF8Encoding(false));
        //更新完毕后重新
        AppConst.GameVersion = sVersion;
    }

    /// <summary>
    /// 是否下载完成
    /// </summary>
    bool IsDownOK(string file)
    {
        return downloadFiles.Contains(file);
    }

    /// <summary>
    /// 线程下载
    /// </summary>
    void BeginDownload(string url, string file)
    {     //线程下载
        object[] param = new object[2] { url, file };

        ThreadEvent ev = new ThreadEvent();
        ev.Key = NotiConst.UPDATE_DOWNLOAD;
        ev.evParams.AddRange(param);
        DownLoadManager.Instance.AddEvent(ev, OnThreadCompleted);   //线程下载
    }

    /// <summary>
    /// 线程完成
    /// </summary>
    /// <param name="data"></param>
    void OnThreadCompleted(NotiData data)
    {
        if (data.evName == NotiConst.UPDATE_PROGRESS)
        {
            param1 = data.evParam1.ToString();
            param2 = float.Parse(data.evParam2.ToString());
            isProgress = true;
        }
        else if (data.evName == NotiConst.UPDATE_DOWNLOAD)
        {
            if (!IsDownOK(data.evParam1.ToString()))
            {
                isProgress = false;
                downloadFiles.Add(data.evParam1.ToString());
            }
        }
        else if (data.evName == NotiConst.UPDATE_ERROR)
        {
            isError = true;
        }    
    }

    private void Update()
    {
        if (isProgress)
        {
            MessageManager.Instance.EventNotify(MessageConst.MsgUpdateDownLoadUpdate, param1, (curDownLoadSize + param2) / allDownLoadSize);
        }
    }


    public void OnDestroy()
    {
        Destroy(this);
    }
}

public class DownLoadFile
{
    public string file;
    public string fileInfo;
}
