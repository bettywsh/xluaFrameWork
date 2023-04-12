using com.bochsler.protocol;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.PackageManager;
using UnityEngine;
using ProtoBuf;
using com.bochsler.protocol;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Instance.Init(NetworkProtocol.TCP);
        NetworkManager.Instance.Connect("shiku.grandlink.net:8443");
        NetworkManager.Instance.MessagePacker = new ProtobufPacker();
        NetworkManager.Instance.MessageDispatcher = new OuterMessageDispatcher();
        NetworkManager.Instance.OnConnect += OnConnect;
        NetworkManager.Instance.OnError += OnError;
    }

    private void OnError(int e)
    {
        Debug.LogError("网络错误：" + e);
    }

    private void OnConnect(int c)
    {
        Debug.Log("连接成功");
        com.bochsler.protocol.LoginRequest loginRequest = new com.bochsler.protocol.LoginRequest();
        loginRequest.sceneType = com.bochsler.protocol.SceneType.SceneTypeGALLERY;
        loginRequest.Password = "";
        loginRequest.Username = "6422bf99fad65bd0a84c10ab";
        loginRequest.loginType = com.bochsler.protocol.LoginType.LoginVisitor;
        byte[] data = ProtobufHelper.Serialize(loginRequest);
        NetworkManager.Instance.Send((ushort)com.bochsler.protocol.CSMessageEnum.LoginRequest, data);
    }
}
