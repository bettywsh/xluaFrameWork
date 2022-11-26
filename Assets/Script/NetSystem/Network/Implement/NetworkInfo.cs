using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInfo : INetworkInfo
{
    //网络是否可用
    public bool isAvaliable
    {
        get
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return false;
            return true;
        }
    }

    //网络信号强度(wifi可用就是Wifi信号强度，否则是流量信号强度)
    public float signalStrength
    {
        get
        {
#if UNITY_ANDROID
            if (isWifi)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    if (jc != null)
                    {
                        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                        AndroidJavaObject wifiMgr = jo.Call<AndroidJavaObject>("getSystemService", "wifi");
                        AndroidJavaObject winfo = wifiMgr.Call<AndroidJavaObject>("getConnectionInfo");
                        if (winfo.Call<string>("getBSSID") != null)
                        {
                            int rssi = winfo.Call<int>("getRssi");
                            int lv = wifiMgr.CallStatic<int>("calculateSignalLevel", rssi, 5);
                            return lv;
                        }
                    }
                }
            }
#endif
            return 0f;
        }
    }

    //是否正在使用Wifi
    public bool isWifi
    {
        get
        {
            if (isAvaliable)
            {
                if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                    return true;
                return false;
            }
            return false;
        }
    }

    //网卡地址
    public string macAddress
    {
        get
        {
#if UNITY_ANDROID
            if (isWifi)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    if (jc != null)
                    {
                        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                        AndroidJavaObject wifiMgr = jo.Call<AndroidJavaObject>("getSystemService", "wifi");
                        AndroidJavaObject winfo = wifiMgr.Call<AndroidJavaObject>("getConnectionInfo");
                        if (winfo != null)
                        {
                            return winfo.Call<string>("getMacAddress");
                        }
                    }
                }
            }
#endif
            return "";
        }
    }

    //本地IP地址
    public string localIP
    {
        get
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass jc = new AndroidJavaClass("java.net.NetworkInterface");
                for (AndroidJavaObject en = jc.CallStatic<AndroidJavaObject>("getNetworkInterfaces"); en.Call<bool>("hasMoreElements");)
                {
                    AndroidJavaObject intf = en.Call<AndroidJavaObject>("nextElement");
                    for (AndroidJavaObject enumIpaddr = intf.Call<AndroidJavaObject>("getInetAddresses"); enumIpaddr.Call<bool>("hasMoreElements");)
                    {
                        AndroidJavaObject inetAddress = enumIpaddr.Call<AndroidJavaObject>("nextElement");
                        if (!inetAddress.Call<bool>("isLoopbackAddress") && !inetAddress.Call<bool>("isLinkLocalAddress"))
                        {
                            return inetAddress.Call<string>("getHostAddress");
                        }
                    }
                }

            }
#endif
            return "";
        }

    }
}
