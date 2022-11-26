using System.Text;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class ResPath
{
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
                        if (AppConst.DebugMode)
                        {
                            _SourceFolder = ResConst.AppRootPath;
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
                        _DataFolder = ResConst.AppRootPath;
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
    public static string CombinePath(string root, params string[] args)
    {
        string path = root.Replace('\\', '/');
        if (path.EndsWith("/")) path = path.Remove(path.Length - 1);

        StringBuilder sb = new StringBuilder();
        sb.Append(path);
        for (int i = 0; i < args.Length; i++)
        {
            if (string.IsNullOrEmpty(args[i])) continue;
            args[i] = args[i].Replace('\\', '/');
            if (args[i].StartsWith("/")) sb.Append(args[i]);
            else sb.Append("/").Append(args[i]);
        }
        path = sb.ToString();
        if (path.EndsWith("/")) path = path.Remove(path.Length - 1);
        return path;
    }

    /// <summary>
    /// 获取App包内初始资源绝对路径
    /// </summary>
    /// <param name="fileNameWithExtension"></param>
    /// <returns></returns>
    public static string GetStreamingAssetsFilePath(string fileNameWithExtension)
    {
        return CombinePath(SourceFolder, fileNameWithExtension);
    }

    /// <summary>
    /// 获取App当前资源绝对路径
    /// </summary>
    /// <param name="fileNameWithExtension"></param>
    /// <returns></returns>
    public static string GetPersistentFilePath(string fileNameWithExtension)
    {
        return CombinePath(DataFolder, fileNameWithExtension);
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

    public static string GetAssetBunldeName(string path, ResType resType)
    {
        if (resType == ResType.Lua || resType == ResType.Bytes || resType == ResType.Bytes)
        {
            return ResConst.LuaFolderName.ToLower() + "/" + ResConst.LuaFolderName.ToLower() + ResConst.AssetBunldExtName;
        }
        else
        {
            if (path.IndexOf(".") > 0)
                path = path.Substring(0, path.IndexOf("."));
            string extName;
            string folderName;
            GetFolderAndExtName(resType, out folderName, out extName);
            return folderName.ToLower() + "/" + path.ToLower() + ResConst.AssetBunldExtName;
        }
    }

    public static string GetAssetName(string path, ResType resType)
    {
        string pathRoot = "";
        if (resType == ResType.Lua || resType == ResType.Bytes || resType == ResType.Bytes)
        {
            pathRoot = ResConst.AppRootRelativePath + "/" + ResConst.LuaFolderName;
            if (path.IndexOf(".") > 0)
                path = path.Substring(0, path.IndexOf("."));
            string extName;
            string folderName;
            GetFolderAndExtName(resType, out folderName, out extName);
            return pathRoot + "/" + path.ToLower() + extName;
        }
        else
        {
            pathRoot = ResConst.AppRootRelativePath;
            //if (path.IndexOf(".") > 0)
            //    path = path.Substring(0, path.IndexOf("."));
            string extName;
            string folderName;
            GetFolderAndExtName(resType, out folderName, out extName);
            return pathRoot + "/" + folderName + "/" + path.ToLower() + extName;
        }
    }

    public static string GetEditorAssetName(string path, ResType resType)
    {
        string extName;
        string folderName;
        GetFolderAndExtName(resType, out folderName, out extName);
        return ResConst.AppRootRelativePath + "/" + folderName + "/" + path + extName;
    }


    public static void GetFolderAndExtName(ResType resType, out string folderName, out string extName)
    {
        extName = "";
        folderName = "";
        switch (resType)
        {
            case ResType.Prefab:
                extName = ResConst.PrefabExtName;
                folderName = ResConst.PrefabFolderName;
                break;
            case ResType.Sprite:
                extName = ResConst.TextureExtName;
                folderName = ResConst.TextureFolderName;
                break;
            case ResType.AudioClip:
                folderName = ResConst.SoundFolderName;
                break;
            case ResType.Lua:
                folderName = ResConst.LuaFolderName;
                extName = ResConst.LuaExtName;
                break;
            case ResType.Bytes:
                folderName = ResConst.LuaFolderName;
                extName = ResConst.BytesExtName;
                break;
            case ResType.Txt:
                folderName = ResConst.LuaFolderName;
                extName = ResConst.TxtExtName;
                break;
            case ResType.Scene:
                folderName = ResConst.SceneFolderName;
                extName = ResConst.SceneExtName;
                break;
            case ResType.Font:
                folderName = ResConst.FontFolderName;
                extName = ResConst.FontExtName;
                break;
            case ResType.Asset:
                folderName = ResConst.AssetFolderName;
                extName = ResConst.AssetExtName;                
                break;
            case ResType.Material:
                folderName = ResConst.MaterialFolderName;
                extName = ResConst.MaterialExtName;
                break;
            case ResType.Atlas:
                folderName = ResConst.AtlasFolderName;
                extName = ResConst.AtlasExtName;
                break;
        }
    }
}
