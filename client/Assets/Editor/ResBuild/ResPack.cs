using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public static class ResPack
{

    /// <summary>
    /// 打包输出目录
    /// </summary>
    public static string AppNewAssetBuildPath
    {
        get { return Application.dataPath.Replace("Assets", "NewUpdata"); }
    }


    public static string IOSHotUpdateBuildPath
    {
        get { return "/Users/unityIOS/Documents/work/hotUpdate/client/IOS"; }
    }

    public static string AndroidHotUpdateBuildPath
    {
        get { return "E:\\work\\hotfix\\client\\Android"; }
    }



    /// <summary>
    /// 旧资源目录
    /// </summary>
    public static string AppOldAssetBuildPath
    {
        get { return Application.dataPath.Replace("Assets", "OldUpdata"); }
    }

    [MenuItem("Builds/Build IOS", false, 1)]
    public static void BuildIOS()
    {
        BuildLuaByBuildTarget(BuildTarget.iOS);
    }

    [MenuItem("Builds/Build Android", false, 2)]
    public static void BuildAndroid()
    {
        BuildLuaByBuildTarget(BuildTarget.Android);
    }

    [MenuItem("Builds/Build Pc", false, 3)]
    public static void BuildPc()
    {
        BuildLuaByBuildTarget(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Builds/Build IOS Update ", false, 4)]
    public static void BuildUpdateIOS()
    {
        BuildLuaByBuildHotUpdateTarget(BuildTarget.iOS);
    }

    [MenuItem("Builds/Build Android Update ", false, 5)]
    public static void BuildUpdateAndroid()
    {
        BuildLuaByBuildHotUpdateTarget(BuildTarget.Android);
    }

    private static void BuildLuaByBuildHotUpdateTarget(BuildTarget target)
    {
        BuildLuaByBuildTarget(target);


        //copy文件到固定的SVN目录下
        if (target == BuildTarget.iOS)
        {
            PackFile.CopySourceDirTotargetDir(ResPack.AppNewAssetBuildPath, ResPack.IOSHotUpdateBuildPath);
        }

        //copy文件到固定的SVN目录下
        if (target == BuildTarget.Android)
        {
            PackFile.CopySourceDirTotargetDir(ResPack.AppNewAssetBuildPath, ResPack.AndroidHotUpdateBuildPath);
        }



    }

    /// <summary>
    /// 该方法只会生成对应的AB文件放到UPDATRE文件夹下
    /// </summary>
    /// <param name="target"></param>

    private static void BuildLuaByBuildTarget(BuildTarget target)
    {
        string root = ResPath.AppFullPath;
        //List<string> modules = PackFile.EachModuleList(root);  

        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        string buildPath = Path.Combine(ResPath.AppRelativePath, ResConst.BuildJson) + ResConst.JsonExtName;
        string buildJson = AssetDatabase.LoadAssetAtPath<TextAsset>(buildPath).text;
        Dictionary<string, BuildJson> json = LitJson.JsonMapper.ToObject<Dictionary<string, BuildJson>>(buildJson);
        foreach (var item in json)
        {
            if (item.Value.BuildType == BuildType.OneAB)
            {
                CreateSingleBuild(item.Value.FolderName, item.Value.ResType, builds);
            }
            else if (item.Value.BuildType == BuildType.EveryAB)
            {
                CreateMultiBuilds(item.Value.FolderName, item.Value.ResType, builds);
            }
        }


        AssetDatabase.Refresh();
        if (Directory.Exists(ResPack.AppNewAssetBuildPath)) Directory.Delete(ResPack.AppNewAssetBuildPath, true);
        AssetDatabase.Refresh();
        Directory.CreateDirectory(ResPack.AppNewAssetBuildPath);
        AssetDatabase.Refresh();
        BuildPipeline.BuildAssetBundles(ResPack.AppNewAssetBuildPath, builds.ToArray(), BuildAssetBundleOptions.None, target);
        AssetDatabase.Refresh();
        Debug.Log("Build all module completed!!! ");

        CreateFiles();
        //VersionFile.CreateVersion();
        AssetDatabase.Refresh();

        //if (File.Exists(ResConst.AppDataPath + "NewUpdata.manifest")) File.Delete(ResConst.AppDataPath + "NewUpdata.manifest");
        AssetDatabase.Refresh();
        VersionFile.CreateVersion();
        AssetDatabase.Refresh();
    }

    private static void CreateSingleBuild(string folderName, ResType restype, List<AssetBundleBuild> builds)
    {
        string extName = ResPath.GetExtName(restype);
        string dir = Path.Combine(ResPath.AppFullPath, folderName);
        string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
        List<string> luaList = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            if (Path.GetExtension(files[i]).ToLower() == ".meta")
                continue;
            var target = files[i].Replace('\\', '/');
            luaList.Add(PackFile.Trans2AssetPath(target));
        }
        AssetDatabase.Refresh();

        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = ResPath.GetSingleAssetBunldeName(folderName, ResType.Lua);
        build.assetNames = luaList.ToArray();
        builds.Add(build);
    }

    private static void CreateMultiBuilds(string folderName, ResType restype, List<AssetBundleBuild> builds)
    {
        string extName = ResPath.GetExtName(restype);
        string dir = Path.Combine(ResPath.AppFullPath, folderName);
        if (!Directory.Exists(dir))
            return;
        string[] files = null;
        if (extName == "")
            extName = ".*";
        files = Directory.GetFiles(dir, "*" + extName, SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (Path.GetExtension(files[i]).ToLower() == ".meta")
                continue;
            var absolutePath = files[i].Replace("\\", "/");
            int start = absolutePath.IndexOf(folderName + "/");
            string path = absolutePath.Substring(start, absolutePath.Length - start);

            path = path.Replace(Path.GetExtension(files[i]), "");
            var assetPath = PackFile.Trans2AssetPath(absolutePath);

            string[] paths = new string[1] { assetPath };
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = ResPath.GetMultiFileAssetBunldeName(path, restype);
            build.assetNames = paths;
            builds.Add(build);
        }
    }

    public static void CreateFiles()
    {
        var filesPath = AppNewAssetBuildPath + "/" + ResConst.CheckFile;
        List<string> lines = new List<string>();
        UTF8Encoding utf8 = new UTF8Encoding(false);
        if (File.Exists(filesPath)) File.Delete(filesPath);
        var files = Directory.GetFiles(AppNewAssetBuildPath, "*", SearchOption.AllDirectories);
        for (int j = 0; j < files.Length; j++)
        {
            var file = files[j].Replace('\\', '/');
            if (file.EndsWith("/" + ResConst.VerFile) || file.EndsWith("/NewUpdata")) continue;
            var ext = Path.GetExtension(file).ToLower();
            if (string.IsNullOrEmpty(ext) || (ext != ".meta" && ext != ".manifest"))
            {
                FileInfo fileContent = new FileInfo(file);
                //var md5 = PackFile.MD5File(file);
                string relativePath = file.Replace(AppNewAssetBuildPath, string.Empty).Substring(1);
                //relativePath = Path.GetFileNameWithoutExtension(relativePath);
                //获取manifest
                string manifestContent = File.ReadAllText(file + ".manifest");
                string crc = Regex.Match(manifestContent, @"CRC:.(\d*)").Groups[1].ToString();
                string hash = Regex.Match(manifestContent, @"Hash:.(.*)\s{3}Type").Groups[1].ToString();
                lines.Add(string.Format("{0}|{1}|{2}|{3}", relativePath, fileContent.Length, crc, hash));
            }
        }
        File.WriteAllText(filesPath, string.Join("\n", lines.ToArray()), utf8);
    }


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
