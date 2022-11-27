using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class UdpManager : MonoSingleton<UdpManager>
{
    UdpClient udpcClient = null;
    IPEndPoint localIpep = null;
    bool IsUdpcRecvStart = false;
    Thread thrRecv;
    public void StartReceive(string ip, int port)
    {
        if (!IsUdpcRecvStart) // 未监听的情况，开始监听
        {
            localIpep = new IPEndPoint(IPAddress.Parse(ip), port); // 本机IP和监听端口号
            udpcClient = new UdpClient(localIpep);
            thrRecv = new Thread(ReceiveMessage);
            thrRecv.Start();
            IsUdpcRecvStart = true;
        }
    }

    /// <summary>
    /// 发送信息
    /// </summary>
    /// <param name="obj"></param>
    public void SendMessage(byte[] bytes)
    {   
        try
        {
            udpcClient.Send(bytes, bytes.Length, localIpep);
        }
        catch { }
    }

    /// <summary>
    /// 接收数据
    /// </summary>
    /// <param name="obj"></param>
    public void ReceiveMessage(object obj)
    {
        while (IsUdpcRecvStart)
        {
            try
            {                
                byte[] bytRecv = udpcClient.Receive(ref localIpep);
            }
            catch 
            {

            }
        }
    }

    private void Update()
    {
        
    }

}
