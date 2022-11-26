using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class MessageConst
{

    public const string MsgConnected = "OnConnected"; //连接到服务器
    public const string MsgLostConnect = "OnLostConnect"; //失去服务器
    public const string MsgDisconnected = "OnDisconnected"; //断开服务器

    public const string DefaultConnectName = "DefaultConnect";

    public const string LoginSeverConnect = "LoginSeverConnect";
    
    public static bool DebugNetworkIO = false;  //是否打印网络IO消息

    public const string MsgNetData = "OnNetData";//收到网络数据的消息（还未分出Command Id的数据，在这里处理网络消息分发）
    public const string MsgNetCmd = "OnNetCmd_";//收到网络数据的消息（已经分出Command Id的数据，在这里处理收到具体对应网络命令，比如：OnNetCmd_LC_Login,LC_Login是Login返回消息CommandId的定义）
    public const string MsgNetMsg = "OnNetMsg_";// for c# use

    //热更新代码
    //小版本更新
    public const string MsgUpdateLostConnect = "MsgUpdateLostConnect";
    //服务器连接失败
    public const string MsgUpdateSmallVersion = "MsgUpdateSmallVersion";
    //大版本更新
    public const string MsgUpdateBigVersion = "MsgUpdateBigVersion";
    //不更新
    public const string MsgUpdateNo = "MsgUpdateNo";
    //更新
    public const string MsgUpdateYes = "MsgUpdateYes";
    //第一次拷贝
    public const string MsgUpdateFristCopy = "MsgUpdateFristCopy";
    //拷贝进度
    public const string MsgUpdateFristProgress = "MsgUpdateFristProgress";
    //开始下载
    public const string MsgUpdateDownLoad = "MsgUpdateDownLoad";
    //开始下载更新数据
    public const string MsgUpdateDownLoadUpdate = "MsgUpdateDownLoadUpdate";
    //下载完成
    public const string MsgUpdateDownLoadComplete = "MsgUpdateDownLoadComplete";
    //下载错误
    public const string MsgUpdateDownLoadError = "MsgUpdateDownLoadError";

    //游戏切换后台
    public const string EventApplicationPause = "EventApplicationPause";
    //主界面点击
    public const string MsgMainClick = "MsgMainClick";

    //比赛碰撞事件
    public const string MsgRaceTrigger = "MsgRaceTrigger";
    //比赛冲刺事件s
    public const string MsgRaceSprint = "MsgRaceSprint";
    //比赛完成事件
    public const string MsgRaceComplete = "MsgRaceComplete";

    //新手引导点击事件
    public const string MsgGuideClickComplete = "MsgGuideClickComplete";

    //异常Log事件
    public const string MsgOnExceptionLogEvent = "MsgOnExceptionLogEvent";

    //unity退出事件
    public const string MsgOnApplicationQuit = "MsgOnApplicationQuit";
}
