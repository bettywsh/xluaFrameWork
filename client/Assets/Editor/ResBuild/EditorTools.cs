using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EditorTools
{
    public static void FocusUnityGameWindow()
    {
        System.Type T = Assembly.Load("UnityEditor").GetType("UnityEditor.GameView");
        EditorWindow.GetWindow(T, false, "GameView", true);
    }


    public static void SaveCurrentShaderVariantCollection(string savePath)
    {
        EditorTools.InvokeNonPublicStaticMethod(typeof(ShaderUtil), "SaveCurrentShaderVariantCollection", savePath);
    }

    public static void ClearCurrentShaderVariantCollection()
    {
        EditorTools.InvokeNonPublicStaticMethod(typeof(ShaderUtil), "ClearCurrentShaderVariantCollection");
    }

    /// <summary>
    /// 调用私有的静态方法
    /// </summary>
    /// <param name="type">类的类型</param>
    /// <param name="method">类里要调用的方法名</param>
    /// <param name="parameters">调用方法传入的参数</param>
    public static object InvokeNonPublicStaticMethod(System.Type type, string method, params object[] parameters)
    {
        var methodInfo = type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static);
        if (methodInfo == null)
        {
            UnityEngine.Debug.LogError($"{type.FullName} not found method : {method}");
            return null;
        }
        return methodInfo.Invoke(null, parameters);
    }


    /// <summary>
    /// 显示进度框
    /// </summary>
    public static void DisplayProgressBar(string tips, int progressValue, int totalValue)
    {
        EditorUtility.DisplayProgressBar("进度", $"{tips} : {progressValue}/{totalValue}", (float)progressValue / totalValue);
    }

    /// <summary>
    /// 隐藏进度框
    /// </summary>
    public static void ClearProgressBar()
    {
        EditorUtility.ClearProgressBar();
    }
}
