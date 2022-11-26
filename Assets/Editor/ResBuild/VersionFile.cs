using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEditor;

public static class VersionFile
{
    //[MenuItem("Builds/New Version", false, 4)]
    public static void CreateVersion()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        sb.Append("\"version\":\"1.0.5\",");
        sb.Append("\"url\":\"https://www.sojson.com/simple_json.html\",");
        sb.Append("\"channel\":[");
        sb.Append("{");
        sb.Append("\"channelID\":10001,");
        sb.Append("\"channelName\":\"豌豆荚\",");
        sb.Append("\"URL\":\"http://down.fasthorse.world/\"");
        sb.Append("},");
        sb.Append("{");
        sb.Append("\"channelID\":10002,");
        sb.Append("\"channelName\":\"豌豆荚\",");
        sb.Append("\"URL\":\"www.baiducom\"");
        sb.Append("}");
        sb.Append("]");
        sb.Append("}");

        File.WriteAllText(ResPack.AppNewAssetBuildPath + "/" + ResConst.VerFile, sb.ToString(), new System.Text.UTF8Encoding(false));

        PackFile.CopySourceDirTotargetDir(ResPack.AppNewAssetBuildPath, ResPack.AppOldAssetBuildPath);

        PackFile.CopySourceDirTotargetDir(ResPack.AppNewAssetBuildPath, Application.streamingAssetsPath);
        AssetDatabase.Refresh();
    }


    //[MenuItem("Builds/Update Version", false, 5)]
    //public static void UpdateVersion()
    //{
    //    if (!File.Exists(ResPack.AppOldAssetBuildPath + "/" + ResConst.VerFile))
    //    {
    //        Debug.LogError("找不到旧版本资源做比对。。。");
    //        return;
    //    }
    //    StringBuilder sb = new StringBuilder();
    //    string[] oldVersion = File.ReadAllText(ResPack.AppOldAssetBuildPath + "/" + ResConst.VerFile).Split('\n');
    //    Dictionary<string, int> oldVersionDic = new Dictionary<string, int>();
    //    for (int d = 0; d < oldVersion.Length; d++)
    //    {
    //        string[] oldver = oldVersion[d].Split('|');
    //        oldVersionDic.Add(oldver[1], int.Parse(oldVersion[0]));
    //    }
    //    List<string> modules = PackFile.EachModuleList(ResPack.AppNewAssetBuildPath);
    //    for (int i = 0; i < modules.Count; i++)
    //    {
    //        int ver = 1;
    //        var item = modules[i];
    //        string moduleName = item.Substring(item.LastIndexOf("/") + 1);
    //        if (sb.Length != 0) sb.Append("\n");
    //        //读取旧资源比对
    //        bool isNewRes = false;
    //        string[] newFiles = File.ReadAllText(item + "/" + ResConst.CheckFile).Split('\n');
    //        string[] oldFiles = File.ReadAllText(ResPack.AppOldAssetBuildPath + "/" + moduleName + "/" + ResConst.CheckFile).Split('\n');
    //        Dictionary<string, string> oldFilesDic = new Dictionary<string, string>();
    //        for (int d = 0; d < oldFiles.Length; d++)
    //        {
    //            string[] oldfile = oldFiles[d].Split('|');
    //            oldFilesDic.Add(oldfile[0], oldFiles[d]);
    //        }
    //        for (int x = 0; x < newFiles.Length; x++)
    //        {
    //            string[] newfile = newFiles[x].Split('|');
    //            string oldfileArry = null;
    //            oldFilesDic.TryGetValue(newfile[0], out oldfileArry);
    //            if (oldfileArry == null)
    //            {
    //                //新资源
    //                isNewRes = true;
    //            }
    //            else {
    //                string[] oldfile = oldfileArry.Split('|');
    //                if (newfile[3] != oldfile[3] || newfile[4] != oldfile[4])
    //                {
    //                    isNewRes = true;
    //                }
    //            }
    //        }
    //        if (isNewRes)
    //        {
    //            sb.Append(string.Format("{0}|{1}", 1, moduleName));
    //        }
    //        else
    //        {
    //            oldVersionDic.TryGetValue(moduleName, out ver);
    //            sb.Append(string.Format("{0}|{1}", ver + 1, moduleName));
    //        }
    //    }
    //    File.WriteAllText(ResPack.AppNewAssetBuildPath + "/" + ResConst.VerFile, sb.ToString(), new UTF8Encoding(false));
    //}


}
