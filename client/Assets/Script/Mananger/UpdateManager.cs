using LitJson;
using XLua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

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
    private string Version = "Version";
    List<DownLoadFile> needUpdate = new List<DownLoadFile>();
    public void CheckVersion(string ip)
    {
        if (!AppConst.UpdateModel)
        {
            if (AppConst.IsABMode)
            {
                DeletePersistentDataPath();
            }
            MessageManager.Instance.EventNotify(MessageConst.MsgUpdateNo);
            return;
        }
        else
        {
            ResIpAddress = ip;
            //if (PlayerPrefs.GetInt(FirstRun) == 0)
            //{
            //    StartCoroutine(ExtractStreamingAssetsPath());
            //}
            //else
            //{
            //    StartCoroutine(OnCheckVersion());
            //}

            StartCoroutine(OnCheckVersion());
        }
    }

    void DeletePersistentDataPath()
    {
        string[] files = null;
        string[] dirs = null;
        files = Directory.GetFiles(Application.persistentDataPath, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Contains("\\Player.log"))
                continue;
            File.Delete(files[i]);
        }
        dirs = Directory.GetDirectories(Application.persistentDataPath);
        for (int i = 0; i < dirs.Length; i++)
        {
            Directory.Delete(dirs[i], true);
        }
        //PlayerPrefs.SetInt("Version", PlayerSettings.bundleVersion);
    }


    IEnumerator ExtractStreamingAssetsPath()
    {
        MessageManager.Instance.EventNotify(MessageConst.MsgUpdateFristCopy);
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
        //PlayerPrefs.SetInt(FirstRun, 1);

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
        string[] serverVersion = new string[4];
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
            string sversion = (string)jsonDataInfoServer["GameVersion"];
            string[] sversions = sversion.Split('.');
            serverVersion[0] = sversions[0];
            serverVersion[1] = sversions[1];
            serverVersion[2] = sversions[2];
            serverVersion[3] = (string)jsonDataInfoServer["ResVersion"];
            //sVersion = version;

        }
       
        string[] clientVersion = new string[4];
        LocalText localText = new LocalText();
        LocalFile(ResConst.VerFile, localText);
        jsonDataInfoClient = JsonMapper.ToObject(localText.text);
        string cversion = (string)jsonDataInfoClient["GameVersion"];
        string[] cversions = cversion.Split('.');
        clientVersion[0] = cversions[0];
        clientVersion[1] = cversions[1];
        clientVersion[2] = cversions[2];
        clientVersion[3] = (string)jsonDataInfoClient["ResVersion"];

        //可写文件夹版本太低， 可能覆盖安装了
        if (int.Parse(serverVersion[2]) > int.Parse(clientVersion[2]))
        {            
            DeletePersistentDataPath();
            localText = new LocalText();
            LocalFile(ResConst.VerFile, localText);
            jsonDataInfoClient = JsonMapper.ToObject(localText.text);
            cversion = (string)jsonDataInfoClient["GameVersion"];
            cversions = cversion.Split('.');
            clientVersion[0] = cversions[0];
            clientVersion[1] = cversions[1];
            clientVersion[2] = cversions[2];
            clientVersion[3] = (string)jsonDataInfoClient["ResVersion"];
        }
        ResManager.Instance.UnLoadAssetBundle("Version");
        //AppConst.GameVersion = sVersion;

        if (int.Parse(serverVersion[2]) > int.Parse(clientVersion[2]))
        {
            //大版本更新
            MessageManager.Instance.EventNotify(MessageConst.MsgUpdateBigVersion, GetDownloadURLFromJSON(jsonDataInfoServer));
            yield break;
        }
        else if (int.Parse(serverVersion[3]) > int.Parse(clientVersion[3]))
        {
            //小版本更新
            StartCoroutine(TotalDownloadSize());
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
#if UNITY_ANDROID
        return (string)jsonDataInfo["AndroidUrl"];
#elif UNITY_IOS
        return (string)jsonDataInfo["IosUrl"];
#endif

    }

    //计算所需要下载的时间
    IEnumerator TotalDownloadSize()
    {
        string serverFile = "";
        string serverHttpFile = GetFilePath(ResConst.CheckFile);

        UnityWebRequest webRequest = UnityWebRequest.Get(serverHttpFile);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            MessageManager.Instance.EventNotify(MessageConst.MsgUpdateLostConnect);
            yield break;
        }
        else
        {
            serverFile = webRequest.downloadHandler.text;
        }
        TextAsset ta = ResManager.Instance.LoadAsset("VersionFiles", ResConst.CheckFile, typeof(TextAsset)) as TextAsset;
        string clientFile = ta.text;

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

        double downloadSize = 0;
        needUpdate.Clear();
        foreach (KeyValuePair<string, string> keyValuePair in sFiles)
        {
            string[] sFile = keyValuePair.Value.ToString().Split('|');
            string file = null;
            cFiles.TryGetValue(sFile[0], out file);
            if (file == null)
            {
                //新增
                downloadSize += int.Parse(sFile[1]);
                DownLoadFile dlf = new DownLoadFile();
                dlf.file = sFile[0];
                dlf.fileInfo = keyValuePair.Value;
                needUpdate.Add(dlf);
            }
            else
            {
                if (sFile[3] != cFiles[sFile[0]].Split('|')[3])
                {
                    //修改
                    downloadSize += int.Parse(sFile[1]);
                    DownLoadFile dlf = new DownLoadFile();
                    dlf.file = sFile[0];
                    dlf.fileInfo = keyValuePair.Value;
                    needUpdate.Add(dlf);
                }
                
            }
        }

        MessageManager.Instance.EventNotify(MessageConst.MsgUpdateSmallVersion, HumanReadableFilesize(downloadSize));
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
        yield break;
        //if(needUpdate.Count <= 0)
        //    writeClientFiles = writeClientFiles.Substring(0, writeClientFiles.Length - 2);
        //StartCoroutine(OnDownLoad(needUpdate, writeClientFiles));
    }
    //IEnumerator OnDownLoad(List<DownLoadFile> needUpdate, string writecFiles)
    //{
        //string writeClientFiles = writecFiles;
        //for (int i = 0; i < needUpdate.Count; i++)
        //{
        //    curDownLoadSize += int.Parse(needUpdate[i].fileInfo.Split('|')[1]);
        //    string savePath = string.Format("{0}/{1}", Application.persistentDataPath, needUpdate[i].file);
        //    string dir = savePath.Substring(0, savePath.LastIndexOf("/") + 1);
        //    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        //    BeginDownload(GetFilePath(needUpdate[i].file), savePath);
        //    while (!(IsDownOK(savePath)))
        //    {
        //        if (isError)
        //        {
        //            BeginDownload(GetFilePath(needUpdate[i].file), savePath);
        //            isError = false;
        //        }
        //        yield return new WaitForEndOfFrame();
        //    }
        //    if (i == needUpdate.Count)
        //    {
        //        writeClientFiles += needUpdate[i].fileInfo;
        //    }
        //    else
        //    { 
        //        writeClientFiles += needUpdate[i].fileInfo + "\n";
        //    }
        //    File.WriteAllText(string.Format("{0}/{1}", Application.persistentDataPath, ResConst.CheckFile), writeClientFiles);
        //}
        //isProgress = false;
        //MessageManager.Instance.EventNotify(MessageConst.MsgUpdateDownLoadComplete);
        //File.WriteAllText(string.Format("{0}/{1}", Application.persistentDataPath, ResConst.VerFile), sJsonStr, new System.Text.UTF8Encoding(false));
        //更新完毕后重新
        //AppConst.GameVersion = sVersion;
    //}

    IEnumerator LocalFile(string file, LocalText retText)
    {
        string path = string.Format("{0}/{1}", Application.persistentDataPath, file);
        if (File.Exists(path))
        {
            retText.text = File.ReadAllText(path);
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
            retText.text = uwr.downloadHandler.text;
        }
    }

    //private void Update()
    //{
    //    if (isProgress)
    //    {
    //        MessageManager.Instance.EventNotify(MessageConst.MsgUpdateDownLoadUpdate, param1, (curDownLoadSize + param2) / allDownLoadSize);
    //    }
    //}


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

public class LocalText
{
    public string text;
}
