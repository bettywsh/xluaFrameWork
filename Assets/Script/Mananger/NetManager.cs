using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XLua;

[LuaCallCSharp]
public class NetManager : MonoSingleton<NetManager>
{
    Dictionary<string, IConnect> connects = new Dictionary<string, IConnect>();
    Dictionary<string, IConnect> tempConnects = new Dictionary<string, IConnect>();
    List<KeyValuePair<int, object>> tmpEvents = new List<KeyValuePair<int, object>>();
    Queue sEvents = new Queue();
    NetworkInfo _netWorkInfo;
    IPacketIDParser _packetIdParser;

    public void Init() {
        //SocketClient.OnRegister();

        _netWorkInfo = new NetworkInfo();

        _packetIdParser = new PacketIDParser();
        
    }


    public void Update()
    {
        tmpEvents.Clear();
        lock (sEvents.SyncRoot)
        {
            while (sEvents.Count > 0)
            {
                KeyValuePair<int, object> _event = (KeyValuePair<int, object>)sEvents.Dequeue();
                tmpEvents.Add(_event);
            }
        }
        for (int i = 0; i < tmpEvents.Count; i++)
        {
            KeyValuePair<int, object> _event = tmpEvents[i];

            MessageManager.Instance.EventNotify(MessageConst.MsgNetData,_event.Key, _event.Value); //send message ex: "OnNetMsg_GCConnect"
        }

        // 不能迭代修改Dictionary，用temp暂存
        tempConnects.Clear();
        foreach (KeyValuePair<string, IConnect> kvp in connects)
        {
            tempConnects.Add(kvp.Key, kvp.Value);
        }

        foreach (IConnect connect in tempConnects.Values)
        {
            connect.update();
        }


    }

    public bool GetConnectInfo(string targetName, out string ip, out int port)
    {
        if (string.IsNullOrEmpty(targetName))
        {
            if (connects.Count == 1)
            {
                foreach (KeyValuePair<string, IConnect> kv in connects)
                {
                    ip = kv.Value.ip;
                    port = kv.Value.port;
                    return true;
                }
            }
            else
                targetName = MessageConst.DefaultConnectName;
        }
        IConnect con;
        if (connects.TryGetValue(targetName, out con))
        {
            ip = con.ip;
            port = con.port;
            return true;
        }
        else
        {
            ip = "";
            port = 0;
            return false;
        }
    }

    
    /// <summary>
    /// 析构函数
    /// </summary>
    void OnDestroy() {
        DisConnectAll();
        Debug.Log("~NetworkManager was destroy");
    }

    public void DisConnect()
    {
        DisConnectFrom(MessageConst.DefaultConnectName);
    }

    public bool IsConnected()
    {
        return IsConnectedTo(MessageConst.DefaultConnectName);
    }


    public void OnConnected(IConnect connect)
    {
        if (MessageConst.DebugNetworkIO)
            Debug.Log("Network:" + connect.name + " onConnected " + connect.isConnected);
        MessageManager.Instance.EventNotify(MessageConst.MsgConnected, connect);
    }

    public void OnLostConnect(IConnect connect)
    {
        if (MessageConst.DebugNetworkIO)
            Debug.Log(connect.name + " onLostConnect " + connect.isConnected);
        MessageManager.Instance.EventNotify(MessageConst.MsgLostConnect, connect);
    }

    public void OnDisconnected(IConnect connect)
    {
        if (MessageConst.DebugNetworkIO)
            Debug.Log(connect.name + " onDisconnected " + connect.isConnected);
        MessageManager.Instance.EventNotify(MessageConst.MsgDisconnected, connect);
    }

    public void OnReceiveData(IConnect connect, int id, object obj)
    {
        lock (sEvents.SyncRoot)
        {
            sEvents.Enqueue(new KeyValuePair<int, object>(id, obj));
        }
    }

    public void ConnectTo(string targetName, string targetIP, int targetPort)
    {
        IConnect con = null;
        if (!connects.TryGetValue(targetName, out con))
        {
            con = new SimpleConnect();
            con.name = targetName;
            con.onConnected = OnConnected;
            con.onLostConnect = OnLostConnect;
            con.onDisconnected = OnDisconnected;
            con.onReceiveData = OnReceiveData;
            connects.Add(targetName, con);
        }
        con.connectTo(targetIP, targetPort);
    }

    public void DisConnectFrom(string targetName)
    {
        IConnect con = null;
        if (connects.TryGetValue(targetName, out con))
        {
            con.disconnect();
        }
    }

    public void DisConnectAll()
    {
        foreach (IConnect connect in connects.Values)
        {
            connect.disconnect();
        }
        connects.Clear();
    }

    public bool IsConnectedTo(string targetName)
    {
        IConnect con = null;
        if (connects.TryGetValue(targetName, out con))
        {
            return con.isConnected;
        }
        return false;
    }

    public void SendTo(string targetName, int packetId, object data)
    {
        IConnect con = null;
        if (connects.TryGetValue(targetName, out con))
        {
            con.send(packetId, data);
            if (MessageConst.DebugNetworkIO)
            {
                Debug.Log("Send Packet (:" + packetId + ") to " + con.name);
            }
        }
        else
        {
            Debug.LogWarning(targetName + " not connected,can not send:" + packetId);
        }
    }

    public void SendTo(string targetName, int packetId, byte[] data)
    {
        IConnect con = null;
        if (connects.TryGetValue(targetName, out con))
        {
            con.send(packetId, data);
            if (MessageConst.DebugNetworkIO)
            {
                Debug.Log("Send Packet (:" + packetId + ") to " + con.name);
            }
        }
        else
        {
            Debug.LogWarning(targetName + " not connected,can not send:" + packetId);
        }
    }

    public void OnApplicationQuit()
    {
        //发送通知给lua层(可能链接的是不同的链接)
        MessageManager.Instance.EventNotify("OnApplicationQuit");
    }
}
