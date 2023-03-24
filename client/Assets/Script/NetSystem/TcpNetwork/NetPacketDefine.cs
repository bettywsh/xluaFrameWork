public class NetPacketDefine
{
    public static bool FirstLenOnlyMessageLen = true; //包的第一个数值包长度是否只是表示真正的消息数据长度（不是所有数据长度）
    public static int LenMsgLen = 4; //only 4,2  (!=2 then as 4)  //see SimpleProtocolCocdeC.cs
    public static int LenPacketId = 4; //only 4,2 (!=2 then as 4)  //see SimpleProtocolCocdeC.cs
    public static bool NeedPacketIndex = true; //packet send index  //see SimpleProtocolCocdeC.cs
    public static bool NeedCSharpNetwork;  //是否需要C#收发消息
}
