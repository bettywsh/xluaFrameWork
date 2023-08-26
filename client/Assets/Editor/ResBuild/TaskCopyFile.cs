using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TaskCopyFile : ITask
{
    public void Run(PackSetting packSetting)
    {
        if (!packSetting.IsHotfix)
        {
            PackFile.CopySourceDirTotargetDir(ResPack.BuildPath, Application.streamingAssetsPath);
            AssetDatabase.Refresh();
        }
    }
}
