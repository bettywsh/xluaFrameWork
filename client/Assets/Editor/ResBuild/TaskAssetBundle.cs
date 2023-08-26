using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TaskAssetBundle : ITask
{
    public void Run(PackSetting packSetting)
    {
        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        string buildPath = Path.Combine(ResPath.AppRelativePath, ResConst.BuildJson) + ResConst.JsonExtName;
        string buildJson = AssetDatabase.LoadAssetAtPath<TextAsset>(buildPath).text;
        Dictionary<string, BuildJson> json = LitJson.JsonMapper.ToObject<Dictionary<string, BuildJson>>(buildJson);
        foreach (var item in json)
        {
            if (item.Value.BuildType == BuildType.OneAB)
            {
                CreateSingleBuild(item.Value.FolderName, builds);
            }
            else if (item.Value.BuildType == BuildType.EveryFileAB)
            {
                CreateMultiFileBuilds(item.Value.FolderName, builds);
            }
            else if (item.Value.BuildType == BuildType.EveryFolderAB)
            {
                CreateMultiFolderBuilds(item.Value.FolderName, builds);
            }
        }


        AssetDatabase.Refresh();
        if (Directory.Exists(ResPack.BuildPath)) Directory.Delete(ResPack.BuildPath, true);
        AssetDatabase.Refresh();
        Directory.CreateDirectory(ResPack.BuildPath);
        AssetDatabase.Refresh();
        BuildPipeline.BuildAssetBundles(ResPack.BuildPath, builds.ToArray(), BuildAssetBundleOptions.None, packSetting.Target);
        AssetDatabase.Refresh();

    }

    private static void CreateSingleBuild(string folderName, List<AssetBundleBuild> builds)
    {
        string dir = Path.Combine(ResPath.AppFullPath, folderName);
        string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
        List<string> fileList = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            if (Path.GetExtension(files[i]).ToLower() == ".meta")
                continue;
            var target = files[i].Replace('\\', '/');
            fileList.Add(PackFile.Trans2AssetPath(target));
        }
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = ResPath.GetSingleAssetBunldeName(folderName);
        build.assetNames = fileList.ToArray();
        builds.Add(build);
    }

    private static void CreateMultiFileBuilds(string folderName, List<AssetBundleBuild> builds)
    {
        string dir = Path.Combine(ResPath.AppFullPath, folderName);
        if (!Directory.Exists(dir))
            return;
        string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
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
            build.assetBundleName = ResPath.GetMultiFileAssetBunldeName(path);
            build.assetNames = paths;
            builds.Add(build);
        }
    }
    private static void CreateMultiFolderBuilds(string folderName, List<AssetBundleBuild> builds)
    {
        string dir = Path.Combine(ResPath.AppFullPath, folderName);
        if (!Directory.Exists(dir))
            return;
        string[] folders = Directory.GetDirectories(dir);
        foreach (string folder in folders)
        {
            string childfolder = folder.Substring(folder.LastIndexOf("\\") + 1, folder.Length - folder.LastIndexOf("\\") - 1);
            string[] files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
            List<string> fileList = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetExtension(files[i]).ToLower() == ".meta")
                    continue;
                var target = files[i].Replace('\\', '/');
                fileList.Add(PackFile.Trans2AssetPath(target));
            }
            AssetBundleBuild build = new AssetBundleBuild();

            build.assetBundleName = ResPath.GetMultiFolderAssetBunldeName(folderName, childfolder);
            build.assetNames = fileList.ToArray();
            builds.Add(build);
        }
    }
}
