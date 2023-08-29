using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConst
{
    /// <summary>
    /// 是否是打包模式
    /// </summary>
    /// 
#if UNITY_EDITOR
    public const bool IsABMode = false;
#else
    public const bool IsABMode = true;
#endif


    /// <summary>
    /// 热更新模式
    /// </summary>
    public const bool UpdateModel = false;

    /// <summary>
    /// 打印Log模式
    /// </summary>
    public const bool DebugLog = true;

    /// <summary>
    /// 游戏帧频
    /// </summary>
    public const int GameFrameRate = 30;

    /// <summary>
    /// 游戏ID
    /// </summary>
    public const int GameID = 10001;

    /// <summary>
    /// 渠道ID
    /// </summary>
    public const int ChannelID = 10001;


    /// <summary>
    /// 网络类型
    /// </summary>
    public static NetworkProtocol NetProtocol = NetworkProtocol.TCP;

}
