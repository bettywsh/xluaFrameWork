using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OverrideCreateUI
{
    //[MenuItem("GameObject/UI/Text")]
    //static void CreatText()
    //{
    //    CreateUI(Text);
    //}

    //private static void CreateUI(System.Func<GameObject> callback)
    //{
    //    var canvasObj = SecurityCheck();

    //    if (!Selection.activeTransform)      // 在根目录创建的， 自动移动到 Canvas下
    //    {
    //        callback().transform.SetParent(canvasObj.transform);
    //    }
    //    else // (Selection.activeTransform)
    //    {
    //        if (!Selection.activeTransform.GetComponentInParent<Canvas>())    // 没有在UI树下
    //        {
    //            callback().transform.SetParent(canvasObj.transform);
    //        }
    //        else
    //        {
    //            callback();
    //        }
    //    }
    //}
}
