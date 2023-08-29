using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;

public class ProjectBuild 
{
    public static string platform;
    public static string[] args;
    public static void CommandLineBuildPackage()
    {
        args = System.Environment.GetCommandLineArgs();
        UnityEngine.Debug.LogError("开始打包" + args[args.Length - 2].ToString());
        UnityEngine.Debug.LogError("开始打包" + args[args.Length - 1].ToString());
        if (args[args.Length - 2].ToString() == "android")
        {
            BuildAndroid();
        }
        else if (args[args.Length - 2].ToString() == "ios")
        {
            BuildIos();
        }
        platform = args[args.Length - 1].ToString();
    }


    static void BuildAndroid()
    {

        //UpdateSVN();

        //ResPack.BuildAndroid();
        //VersionFile.CreateVersion();
        Build(BuildTargetGroup.Android, BuildTarget.Android);
    }

    static void UpdateSVN()
    {
        Process proc = null;
        try
        {
            string targetDir = string.Format(@"E:\work\FastHoruse\program\tools\AutoBuild_py");
            proc = new Process();
            proc.StartInfo.WorkingDirectory = targetDir;
            proc.StartInfo.FileName = "startup.bat";
            proc.StartInfo.Arguments = string.Format("10");//this is argument
            proc.StartInfo.CreateNoWindow = false;
            proc.Start();
            proc.WaitForExit();

            UnityEngine.Debug.LogError("--------results-------------" );
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogErrorFormat("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
        }
    }


    static void BuildIos()
    {
        //ResPack.BuildIOS();
        //VersionFile.CreateVersion();
        Build(BuildTargetGroup.iOS, BuildTarget.iOS);
    }

    static void Build(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
    {
        
        string path = Application.dataPath + "/../../tools/AutoBuild/TmpAndroidProject";
        
        PackFile.ClearDir(path);

        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        // 设置需要打包的场景
        string[] scene = { "Assets/App/Scene/Start.unity",
            "Assets/App/Scene/Main.unity",
            "Assets/App/Scene/Racing.unity",
            "Assets/App/Scene/Login.unity" };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.locationPathName = path;
        buildPlayerOptions.options = BuildOptions.None;
        buildPlayerOptions.options |= BuildOptions.AcceptExternalModificationsToPlayer;
        buildPlayerOptions.scenes = scene;
        buildPlayerOptions.target = buildTarget;
        // 调用开始打包   
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}
