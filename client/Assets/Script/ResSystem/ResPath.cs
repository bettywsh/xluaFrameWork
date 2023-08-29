using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

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
                        _SourceFolder = Application.streamingAssetsPath + "/";
                        //_SourceFolder = Application.dataPath + @"!assets";
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
    /// 获取App包内初始资源绝对路径
    /// </summary>
    /// <param name="fileNameWithExtension"></param>
    /// <returns></returns>
    public static string GetStreamingAssetsFilePath(string fileNameWithExtension)
    {
        return Path.Combine(SourceFolder, fileNameWithExtension);
    }

    /// <summary>
    /// 获取App当前资源绝对路径
    /// </summary>
    /// <param name="fileNameWithExtension"></param>
    /// <returns></returns>
    public static string GetPersistentFilePath(string fileNameWithExtension)
    {
        return Path.Combine(DataFolder, fileNameWithExtension);
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

    public static string GetAssetBunldePath(string path, Type type, Dictionary<string, BuildJson> BuildJson)
    {
        if (type == typeof(AssetBundleManifest))
        {
            return ResConst.RootFolderName.ToLower() + "/" + path;
        }
        path = path.Replace(Path.GetExtension(path), "");
        //不能用Path.Combine 因为这样出来的路径会变成\\ 而依赖文件是/导致文件路径不统一会被认为是不同资源
        string folderName = path.Substring(0, path.IndexOf("/"));
        if (BuildJson.Count == 0)
        {
            return (ResConst.RootFolderName.ToLower() + "/" + folderName + "/" + ResConst.BuildFolderName + ResConst.AssetBunldExtName).ToLower();
        }
        BuildJson buildJson;
        BuildJson.TryGetValue(folderName, out buildJson);
        if (buildJson.BuildType == BuildType.OneAB)
        {
            return (ResConst.RootFolderName.ToLower() + "/" + folderName.ToLower() + "/" + folderName.ToLower() + ResConst.AssetBunldExtName).ToLower();
        }
        else if (buildJson.BuildType == BuildType.EveryFileAB)
        {
            return (ResConst.RootFolderName.ToLower() + "/" + path + ResConst.AssetBunldExtName).ToLower();
        }
        else
        {
            string[] folders =  path.Split('/');
            path = folders[0] + "/" + folders[1];
            return (ResConst.RootFolderName.ToLower() + "/" + path + ResConst.AssetBunldExtName).ToLower();
        }
        
    }
    public static string GetAssetPath(string path, Type type)
    {
        if (type == typeof(AssetBundleManifest))
        {
            return "AssetBundleManifest";
        }
        return Path.Combine(ResPath.AppRelativePath, path);
    }

}
