using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetworkInfo
{
    //网络是否可用
    bool isAvaliable { get; }
    //网络信号强度(wifi可用就是Wifi信号强度，否则是流量信号强度)0..1
    float signalStrength { get; }
    //是否正在使用Wifi
    bool isWifi { get; }
    //网卡地址
    string macAddress { get; }
    //本地IP地址
    string localIP { get; }
}
