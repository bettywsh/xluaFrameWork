using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

public class PackFile 
{
    public static string Trans2AssetPath(string path)
    {
        if (path.StartsWith(Application.dataPath))
            return "Assets" + path.Replace(Application.dataPath, string.Empty);
        return path;
    }

    public static List<string> EachModuleList(string root)
    {
        string[] dirs = Directory.GetDirectories(root);
        List<string> modules = new List<string>();
        for (int i = 0; i < dirs.Length; i++)
        {
            var item = dirs[i];
            item = item.Replace('\\', '/');
            //if (item.EndsWith("Module"))
            modules.Add(item);
        }
        return modules;
    }

    public static void CopySourceDirTotargetDir(string sourceDir, string targetDir)
    {
        ClearDir(targetDir);

        string[] files = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
        try
        {
            for (int i = 0; i < files.Length; i++)
            {
                var sourceFile = files[i].Replace('\\', '/');
                if (!File.Exists(sourceFile) || sourceFile.EndsWith(".meta")) continue;
                var targetFile = targetDir + sourceFile.Replace(sourceDir, string.Empty);
                var destDir = Path.GetDirectoryName(targetFile);
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                File.Copy(sourceFile, targetFile, true);
                ResPack.UpdateProgress("File Copy", i + 1, files.Length, string.Format("File:{0}", targetFile));
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
        ResPack.ClearProgress();
    }

    public static void CopySourceDirTotargetDir(string sourceDir, string targetDir, string targetExt)
    {
   
        AssetDatabase.Refresh();

        string[] files = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
        try
        {
            for (int i = 0; i < files.Length; i++)
            {
                var sourceFile = files[i].Replace('\\', '/');
                if (!File.Exists(sourceFile) || sourceFile.EndsWith(".meta") || sourceFile.EndsWith(".manifest") || sourceFile.EndsWith(".bat")) continue;
                var targetFile = targetDir + sourceFile.Replace(sourceDir, string.Empty);
                var destDir = Path.GetDirectoryName(targetFile);
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                targetFile = targetFile + targetExt;
                File.Copy(sourceFile, targetFile, true);
                ResPack.UpdateProgress("File Copy", i + 1, files.Length, string.Format("File:{0}", targetFile));
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
        ResPack.ClearProgress();
    }


    public static void ClearDir(string targetDir)
    {
        if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);
        AssetDatabase.Refresh();
        Directory.CreateDirectory(targetDir);
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string MD5File(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    /// <summary>
    /// 创建文件夹
    /// </summary>
    public static bool CreateDirectory(string directory)
    {
        if (Directory.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
            return true;
        }
        else
        {
            return false;
        }
    }
}
