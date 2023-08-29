using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public interface ITask
{
    void Run(PackSetting packSetting);
}
public static class ResPack
{

    /// <summary>
    /// 打包输出目录
    /// </summary>
    /// 
    public static string BuildHotfixPath
    {
        get { return Application.dataPath.Replace("Assets", "ResHotfix/Android/") + ResConst.RootFolderName.ToLower(); }
    }


    /// <summary>
    /// 旧资源目录
    /// </summary>
    public static string BuildCreatePath
    {
        get { return Application.dataPath.Replace("Assets", "ResCreate/Android/") + ResConst.RootFolderName.ToLower(); }
    }

    [MenuItem("Builds/Create", false, 1)]
    public static void BuildAndroidCreate()
    {
        PackSetting packSetting = new PackSetting();
        packSetting.Target = EditorUserBuildSettings.activeBuildTarget;
        packSetting.IsHotfix = false;
        Build(packSetting);
    }

    [MenuItem("Builds/Hotfix", false, 2)]
    public static void BuildAndroidHotfix()
    {
        PackSetting packSetting = new PackSetting();
        packSetting.Target = EditorUserBuildSettings.activeBuildTarget;
        packSetting.IsHotfix = true;
        Build(packSetting);
    }


    public static void Build(PackSetting packSetting)
    {
        List<ITask> pipeline = new List<ITask>
                {
                    new TaskAtals(), //打包ui图集
					new TaskAssetBundle(), //打包资源
					new TaskFileList(), //生成资源列表
                    new TaskVersion(), //生成版本文件
                    new TaskCopyFile(), // 拷贝文件到对应目录

                };
        foreach (ITask t in pipeline)
        {
            t.Run(packSetting);
        }

        Debug.Log("打包资源完成");
    }

    //private static void BuildLuaByBuildHotUpdateTarget(BuildTarget target)
    //{
    //    BuildLuaByBuildTarget(target);


    //    //copy文件到固定的SVN目录下
    //    if (target == BuildTarget.iOS)
    //    {
    //        PackFile.CopySourceDirTotargetDir(ResPack.AppNewAssetBuildPath, ResPack.IOSHotUpdateBuildPath);
    //    }

    //    //copy文件到固定的SVN目录下
    //    if (target == BuildTarget.Android)
    //    {
    //        PackFile.CopySourceDirTotargetDir(ResPack.AppNewAssetBuildPath, ResPack.AndroidHotUpdateBuildPath);
    //    }
    //}


    public static void ClearProgress()
    {
        EditorUtility.ClearProgressBar();
    }

    public static void UpdateProgress(string preTitle, int progress, int progressMax, string desc)
    {
        string title = preTitle + "[" + progress + "/" + progressMax + "]...";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }
}
