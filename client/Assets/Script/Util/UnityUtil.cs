using DG.Tweening;
using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UnityUtil
{
    public static void SetGameObjectVisable(GameObject go, bool va)
    {
        go.SetActive(va);
    }

    public static void SetMonoBehaviourEnabled(MonoBehaviour mono, bool va)
    {
        mono.enabled = va;
    }

    //禁用并变灰
    public static void SetBtnEnabled(Button btn, bool va)
    {
        btn.interactable = va;
    }


    public static void AddBox(GameObject go, Vector3 center, Vector3 size)
    {
        var box = go.AddComponent<BoxCollider>();
        box.size = size;
        box.center = center;
    }

    public static Tweener DOTween_FillAmount(Image img, float from, float to, float duration)
    {
        return DOTween.To(x => img.fillAmount = x, from, to, duration);
    }

    public static void Destroy(GameObject go)
    {
        GameObject.Destroy(go);
    }

    //获取字节长度
    public static int GetByteCount(string str)
    {
        return System.Text.Encoding.GetEncoding("utf-8").GetByteCount(str);
    }

    //立即刷新物体格式大小
    public static void LayoutRebuilderRefre(RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    public static string GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds).ToString();
    }

    /// <summary>
    /// 判断该字符串是否为数字
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>

    public static bool IsNumeric(string value)
    {
        return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
    }


    /// <summary>  
    ///   
    /// </summary>  
    /// <param name="str">待处理的字符串</param>  
    /// <param name="toRep">要替换的字符串中的子串</param>  
    /// <param name="strRep">用来替换toRep字符串的字符串</param>  
    /// <returns>返回一个结果字符串</returns>  

    public static string StringReplace(string str, string toRep, string strRep)
    {
        StringBuilder sb = new StringBuilder();

        int np = 0, n_ptmp = 0;

        for (; ; )
        {
            string str_tmp = str.Substring(np);
            n_ptmp = str_tmp.IndexOf(toRep);

            if (n_ptmp == -1)
            {
                sb.Append(str_tmp);
                break;
            }
            else
            {
                sb.Append(str_tmp.Substring(0, n_ptmp)).Append(strRep);
                np += n_ptmp + toRep.Length;
            }
        }
        return sb.ToString();

    }


}
