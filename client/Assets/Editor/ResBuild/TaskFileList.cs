using System.Collections.Generic;
using System.IO;
using System.Text;

public class TaskFileList :ITask
{

    public void Run(PackSetting packSetting)
    {
        if (packSetting.IsHotfix)
        {
            CreateFiles(ResPack.BuildHotfixPath);
        }
        else
        {
            CreateFiles(ResPack.BuildCreatePath);
        }
    }

    public void CreateFiles(string path)
    {
        var filesPath = path + "/" + ResConst.CheckFile;
        List<string> lines = new List<string>();
        UTF8Encoding utf8 = new UTF8Encoding(false);
        if (File.Exists(filesPath)) File.Delete(filesPath);
        var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        for (int j = 0; j < files.Length; j++)
        {
            var file = files[j].Replace('\\', '/');
            if (file.EndsWith("/" + ResConst.VerFile) || file.EndsWith("/NewUpdata")) continue;
            var ext = Path.GetExtension(file).ToLower();
            if (string.IsNullOrEmpty(ext) || (ext != ".meta" && ext != ".manifest"))
            {
                FileInfo fileContent = new FileInfo(file);
                var md5 = PackFile.MD5File(file);
                string relativePath = file.Replace(path, string.Empty).Substring(1);
                //relativePath = Path.GetFileNameWithoutExtension(relativePath);
                //»ñÈ¡manifest
                string manifestContent = File.ReadAllText(file + ".manifest");
                //string crc = Regex.Match(manifestContent, @"CRC:.(\d*)").Groups[1].ToString();
                //string hash = Regex.Match(manifestContent, @"Hash:.(.*)\s{3}Type").Groups[1].ToString();

                lines.Add(string.Format("{0}|{1}|{2}", relativePath, fileContent.Length, md5));
            }
        }
        File.WriteAllText(filesPath, string.Join("\n", lines.ToArray()), utf8);
    }
}
