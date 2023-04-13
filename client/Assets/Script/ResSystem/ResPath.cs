using System.Text;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using System.Collections.Generic;

public class ResPath
{

    public static string AppFullPath
    {
        get { return Path.Combine(Application.dataPath, ResConst.RootFolderName); }
    }

    public static string AppRelativePath
    {
        get { return Path.Combine("Assets", ResConst.RootFolderName); }
    }

    private static string _SourceFolder;
    /// <summary>
    /// App包内初始资源文件夹
    /// </summary>
    public static string SourceFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_SourceFolder))
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.IPhonePlayer:
                        _SourceFolder = Application.dataPath + @"/Raw";
                        break;
                    case RuntimePlatform.Android:
                        _SourceFolder = Application.dataPath + @"!assets";
                        break;
                    default:
                        if (!AppConst.IsABMode)
                        {
                            _SourceFolder = AppFullPath;
                        }
                        else
                        {
                            _SourceFolder = Application.streamingAssetsPath;
                        }
                        break;
                }
            }
            return _SourceFolder;
        }
    }

    private static string _DataFolder;
    /// <summary>
    /// App可写资源文件夹
    /// </summary>
    public static string DataFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_DataFolder))
            {
                if (Application.isMobilePlatform)
                {
                    _DataFolder = Application.persistentDataPath;
                }
                else
                {
                    if (!AppConst.UpdateModel)
                    {
                        _DataFolder = AppFullPath;
                    }
                    else
                    {
                        _DataFolder = Application.persistentDataPath;
                    }
                }
            }

            return _DataFolder;
        }
    }

    /// <summary>
    /// 合并路径，任意参数中存在 '\' 字符都会被替换为 '/' 字符。
    /// 如果路径为目录，字符串结尾必定不包含 '/' 字符。
    /// </summary>
    /// <param name="root">起始路径</param>
    /// <param name="args">中间路径</param>
    /// <returns>合并后的合法路径</returns>
    //public static string CombinePath(string root, params string[] args)
    //{
    //    string path = root.Replace('\\', '/');
    //    if (path.EndsWith("/")) path = path.Remove(path.Length - 1);

    //    StringBuilder sb = new StringBuilder();
    //    sb.Append(path);
    //    for (int i = 0; i < args.Length; i++)
    //    {
    //        if (string.IsNullOrEmpty(args[i])) continue;
    //        args[i] = args[i].Replace('\\', '/');
    //        if (args[i].StartsWith("/")) sb.Append(args[i]);
    //        else sb.Append("/").Append(args[i]);
    //    }
    //    path = sb.ToString();
    //    if (path.EndsWith("/")) path = path.Remove(path.Length - 1);
    //    return path;
    //}

    /// <summary>
    /// 获取App包内初始资源绝对路径
    /// </summary>
    /// <param name="fileNameWithExtension"></param>
    /// <returns></returns>
    public static string GetStreamingAssetsFilePath(string fileNameWithExtension)
    {
        return Path.Combine(SourceFolder, fileNameWithExtension);
        //return CombinePath(SourceFolder, fileNameWithExtension);
    }

    /// <summary>
    /// 获取App当前资源绝对路径
    /// </summary>
    /// <param name="fileNameWithExtension"></param>
    /// <returns></returns>
    public static string GetPersistentFilePath(string fileNameWithExtension)
    {
        return Path.Combine(DataFolder, fileNameWithExtension);
        //return CombinePath(DataFolder, fileNameWithExtension);
    }
    public static bool CheckPersistentFileExsits(string filePath)
    {
        var path = GetPersistentFilePath(filePath);
        return File.Exists(path);
    }

    public static string GetAssetBundleFilePath(string filePath)
    {
        if (CheckPersistentFileExsits(filePath))
        {
            return GetPersistentFilePath(filePath);
        }
        else
        {
            return GetStreamingAssetsFilePath(filePath);
        }
    }


    public static string GetSingleAssetBunldeName(string folderName)
    {
        return Path.Combine(folderName.ToLower(), folderName.ToLower()) + ResConst.AssetBunldExtName;
    }

    public static string GetMultiFileAssetBunldeName(string path)
    {
        return path.ToLower() + ResConst.AssetBunldExtName;
    }

    public static string GetMultiFolderAssetBunldeName(string rootFolderName, string folderName)
    {
        return Path.Combine(rootFolderName.ToLower(), folderName.ToLower()) + ResConst.AssetBunldExtName;
    }

    public static string GetAssetBunldePath(string path, Dictionary<string, BuildJson> BuildJson)
    {
        //不能用Path.Combine 因为这样出来的路径会变成\\ 而依赖文件是/导致文件路径不统一会被认为是不同资源
        string folderName = path.Substring(0, path.IndexOf("/"));
        if (BuildJson.Count == 0)
        {
            return (folderName.ToLower() + "/" + folderName.ToLower() + ResConst.AssetBunldExtName).ToLower();
        }
        BuildJson buildJson;
        BuildJson.TryGetValue(folderName, out buildJson);
        if (buildJson.BuildType == BuildType.OneAB)
        {
            return (folderName.ToLower() + "/" + folderName.ToLower() + ResConst.AssetBunldExtName).ToLower();
        }
        else if (buildJson.BuildType == BuildType.EveryFileAB)
        {
            return (path + ResConst.AssetBunldExtName).ToLower();
        }
        else
        {
            string[] folders =  path.Split('/');
            path = folders[0] + "/" + folders[1];
            return (path + ResConst.AssetBunldExtName).ToLower();
        }
        
    }
    public static string GetAssetPath(string path)
    {
        return Path.Combine(ResPath.AppRelativePath, path);
    }

}
