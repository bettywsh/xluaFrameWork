


using System.IO;
using UnityEngine;

public  class FileUtil
{

    /// <summary>
    /// 检查文件夹是否存在
    /// </summary>
    /// <param name="dir"></param>
    public static void CheckDir(string dir)
    {
        System.IO.DirectoryInfo diri = new System.IO.DirectoryInfo(dir);
        if (!diri.Exists)
            diri.Create();
    }

    /// <summary>
    /// 存储文件
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="txt"></param>
    public static void SaveTextFile(string fn, string txt)
    {
        byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(txt);
        SaveFileData(fn, data);
    }

    public static void SaveFileData(string fn, byte[] data)
    {
        string dir = Path.GetDirectoryName(fn);
        System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(dir);
        if (!dirinfo.Exists)
            dirinfo.Create();
        FileStream fs = null;
        try
        {
            fs = new FileStream(fn, FileMode.Create);
            fs.Write(data, 0, data.Length);
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveFileData error!" + e.Message);
        }
        finally
        {
            if (fs != null)
                fs.Close();
        }
    }


    public static byte[] GetFileData(string fn)
    {
        if (!File.Exists(fn))
            return null;
        FileStream fs = new FileStream(fn, FileMode.Open);
        try
        {
            if (fs.Length > 0)
            {
                byte[] data = new byte[(int)fs.Length];
                fs.Read(data, 0, (int)fs.Length);
                return data;
            }
            else
            {
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
        finally
        {
            fs.Close();
        }
    }
}
