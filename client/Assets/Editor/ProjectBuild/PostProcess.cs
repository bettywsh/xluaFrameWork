using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;

public static class PostProcess
{
    [PostProcessBuild(90)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuildProject)
    {
        if (buildTarget == BuildTarget.Android)
        {
            //判断具体平台 ProjectBuild.platform
        }
        else if (buildTarget == BuildTarget.iOS)
        {

        }

    }
}
