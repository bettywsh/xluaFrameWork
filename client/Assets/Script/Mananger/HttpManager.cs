using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HttpManager : MonoSingleton<HttpManager>
{


    /// <summary>
    /// 时间戳计时开始时间
    /// </summary>
    private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


    public void GetRequest(string url, string jsonString, string token,Action<string> callback)
    {

        //if (!String.IsNullOrEmpty(jsonString))
        {
            jsonString = EncrypUtil.MakeMd5(jsonString);

            url = url + "&" + jsonString.TrimStart('&');
        }
        StartCoroutine(Get(url, jsonString, token, callback));
    }

    public void PostRequest(string url, string jsonString, string token, Action<string> callback)
    {

        if (!String.IsNullOrEmpty(jsonString))
        {
            jsonString = EncrypUtil.MakeMd5(jsonString);
        }  
            
        StartCoroutine(Post(url, jsonString, token, callback));
    }

    /// <summary>
    /// DateTime转换为10位时间戳（单位：秒）
    /// </summary>
    /// <param name="dateTime"> DateTime</param>
    /// <returns>10位时间戳（单位：秒）</returns>
    public static long DateTimeToTimeStamp()
    {
        return GetCreatetime();
    }


    private static int GetCreatetime()
    {
        DateTime DateStart = new DateTime(1970, 1, 1, 8, 0, 0);
        return Convert.ToInt32((DateTime.UtcNow - DateStart).TotalSeconds);
    }


    public IEnumerator Get(string url, string jsonString, string token, Action<string> callback)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "GET"))
        {
            webRequest.timeout = 30;
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=gb2312");
            webRequest.SetRequestHeader("Time-Tamp", DateTimeToTimeStamp().ToString());
            webRequest.SetRequestHeader("Token", token);
            webRequest.SetRequestHeader("Time-Zone", System.TimeZone.CurrentTimeZone.GetUtcOffset(System.DateTime.Now).Hours.ToString());

            yield return webRequest.SendWebRequest();

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text + "   " + url);
                if (callback != null)
                {
                    callback("");
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(webRequest.downloadHandler.text);
                }
            }
        }

    }


    //判断网络的状态
    public bool JudgeNetState()
    {
        //当网络不可用时                
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }

        return true;
    }

    public string PackageJSON(string data)
    {
        return "enc=" + data;
    }


    public string PackageJSONString(string spliStr)
    {
        StringBuilder str = new StringBuilder();
        str.Append("{");
        string[] arrStr = spliStr.Split('&');

        foreach(string item in arrStr)
        {

            string[] spli = item.Split('=');

            if(spli.Length >= 2)
            {

                bool isNum = UnityUtil.IsNumeric(spli[1]);

                if (isNum)
                {
                    str.Append("\""+ spli[0] + "\":" + spli[1] + ",");
                }
                else
                {
                    str.Append("\"" + spli[0] + "\":\"" + spli[1] + "\",");
                }
            }
            
        }

        string endString = str.ToString().TrimEnd(',') + "}";

        return endString;
    }



    public IEnumerator Post(string url, string jsonString, string token, Action<string> callback)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            if (jsonString.EndsWith("&"))
            {
                jsonString = jsonString.Substring(0, jsonString.Length - 1);
            }

            if (!string.IsNullOrEmpty(jsonString))
            {
                byte[] bodyRaw;

                if (true)
                {
                    string key = EncrypUtil.EncryptRSAString(PackageJSONString(jsonString));

                    bodyRaw = Encoding.UTF8.GetBytes(UnityUtil.StringReplace(PackageJSON(key),"+", "%2B") );
                }
                else
                {
                    bodyRaw = Encoding.UTF8.GetBytes(jsonString);
                }

                webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            }
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=utf8");
            webRequest.SetRequestHeader("Time-Tamp", DateTimeToTimeStamp().ToString());
            webRequest.SetRequestHeader("Token", token);
            webRequest.SetRequestHeader("Time-Zone", System.TimeZone.CurrentTimeZone.GetUtcOffset(System.DateTime.Now).Hours.ToString());

            yield return webRequest.SendWebRequest();

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                if (callback != null)
                {
                    callback("");
                }
            }
            else
            {
                if (callback != null)
                {
                    if (true)
                    {
                        string jsonObj = EncrypUtil.DecryptRSAString(webRequest.downloadHandler.text);

                        callback(jsonObj);
                    }
                    else
                    {
                        callback(webRequest.downloadHandler.text);
                    }
                }
            }
        }
    }
}
