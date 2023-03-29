using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public class TcpConnect : IConnect {
	IProtocolCodec protoCodec{
		get{
			if (_protoCodec == null){
				_protoCodec = new TcpProtocolCodec();
			}
			return _protoCodec;
		}
	}
		
	IObjectSerializer objSerializer{
		get{
			if (_objSerializer == null){
				_objSerializer = new ByteBufferSerializer();
			}
			return _objSerializer;
		}
	}
		
	IProtocolCodec _protoCodec;
	IObjectSerializer _objSerializer;	
	public ConnectEvent onConnected{get;set;}
	public ConnectEvent onLostConnect{get;set;}
	public ConnectEvent onDisconnected{get;set;}
	public ConnectDataEvent onReceiveData{get;set;}
	
	public enum DisconectReason {
		Exception,
		Disconnect,
	}
	
	TcpClient client;
	NetworkStream outStream;

    const int MAX_READ = 65536;//1024 * 1024;
	byte[] byteBuffer = new byte[MAX_READ];
	bool _isConnected;
	bool _lastConnected;
	DisconectReason _disReason;
    //-----------------------------------------
    string connectIp;
    int connectPort;
    public string name{get;set;}
	public string ip{get{return this.connectIp;}}
	public int port{get{return this.connectPort;}}
	    
	public bool isConnected{get{
//			Debug.Log("OK");
		//return client != null && client.Connected;
		return _isConnected;
	}}
	    
	public void connectTo(string ip, int port) {
		if (_isConnected && connectIp == ip && connectPort == port)
		{
			Debug.LogWarning(ip + ":"+port+" already connected!");
			return;
		}
		IPAddress[] address = Dns.GetHostAddresses(ip);
		if (address.Length == 0)
		{
			Debug.LogError("host invalid");
			return;
		}
		if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
		{
			client = new TcpClient(AddressFamily.InterNetworkV6);
		}
		else
		{
			client = new TcpClient(AddressFamily.InterNetwork);
		}
		//client = new TcpClient();
	    client.SendTimeout = 1000;
	    client.ReceiveTimeout = 1000;
	    client.NoDelay = true;
		connectIp = ip;
		connectPort = port;
	    try {		
			client.BeginConnect(address, port, new AsyncCallback(doOnConnected), null);
		} catch (Exception e) {
			Debug.LogError("Connect error:"+e.Message);
			disconnect();
	    }
	}
	    
	public void disconnect() {
		_isConnected = false;
		if (client != null) {
			if (client.Connected) 
				//client.Client.Disconnect(false); //addby dliu 某些情况会导致u3d卡死
				client.Close();
			client = null;
		}
	}
		
	public void send(int packetId,object obj){
		byte[] bytes = objSerializer.objToData(packetId,obj);
        
		if (bytes != null)
			send(packetId,bytes);
		else
		{
			Debug.LogError("send error:" + packetId);
		}
	}
	    
	public void send(int packetId,byte[] data)
	{
//			if (client == null || !client.Connected)
		if (!_isConnected)
		{
			Debug.LogWarning("client.connected----->>false, can't send data:"+packetId);
			return;
		}
		//RC4.Instance().RC4EncryptTo(ref data);
		byte[] wdata = protoCodec.encode(packetId, data);
		//add by dliu 异步发消息会导致ios发包报错和安卓断开socket时缓存出错
		//outStream.BeginWrite(wdata, 0, wdata.Length, new AsyncCallback(doOnWrite), null);
		try
		{
			outStream.Write(wdata, 0, wdata.Length);
			outStream.Flush();
		}
		catch (Exception e)
		{
			disconnect();
		}
	}

    public void update(){
		if (!_lastConnected && _isConnected){
			_lastConnected = true;
			if (onConnected != null)
				onConnected(this);
		}
		if (_lastConnected && !_isConnected){
			_lastConnected = false;
			if (_disReason == DisconectReason.Exception){
				if (onLostConnect != null) {
					Debug.LogError("onLostConnect");
					onLostConnect(this);
				}
			}else{
				if (onDisconnected != null)
					onDisconnected(this);
			}
		}
	}
	
	void doOnConnected(IAsyncResult asr) {
		try
		{
			outStream = client.GetStream();
			while (!outStream.CanRead)
			{
				System.Threading.Thread.Sleep(10);
			}
			outStream.BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(doOnRead), null);
			_isConnected = true;
		}
		catch(Exception e)
		{
			Debug.LogError("Connect error:" + e.Message);
			disconnect();
		}
	        
	}
	
	void doOnRead(IAsyncResult asr) {
        int bytesRead = 0;
        try
		{
			lock (client.GetStream())
			{         //读取字节流到缓冲区
				bytesRead = client.GetStream().EndRead(asr);
			}
			if (bytesRead < 1)
			{                //包尺寸有问题，断线处理
				Debug.LogWarning("bytesRead < 1 包尺寸有问题，已经断开连接");
				doOnDisconnected(DisconectReason.Disconnect, "bytesRead < 1");
				return;
			}
			doOnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层
			lock (client.GetStream())
			{         //分析完，再次监听服务器发过来的新消息
				Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
				while (!outStream.CanRead)
				{
					System.Threading.Thread.Sleep(10);
				}
				client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(doOnRead), null);
			}
			_isConnected = true;
		}
		catch (Exception ex)
		{
			//PrintBytes();
			Debug.Log("doOnRead error in Connect("  +this.name+ "),next doOnDisconnected"); 
			doOnDisconnected(DisconectReason.Exception, ex.Message);
			_isConnected = true;
		}
	}

    void doOnDisconnected(DisconectReason disReason, string msg) {
		Debug.Log("====失联:" + msg);
		//disconnect();   //关掉客户端链接
		_isConnected = false;
		_disReason = disReason;			
	}
	
	void doOnWrite(IAsyncResult r) {
	    try {
	        outStream.EndWrite(r);
	    } catch (Exception ex) {
	        Debug.LogError("OnWrite--->>>" + ex.Message);
	    }
	}
	
	//byte[] _buffer = null;
	List<KeyValuePair<int,byte[]>> _tmpDatas = new List<KeyValuePair<int,byte[]>>();
    void doOnReceive(byte[] bytes, int length) {
		_tmpDatas.Clear();
		protoCodec.decode(bytes, length, _tmpDatas);
		for (int i = 0; i < _tmpDatas.Count; i++)
		{
			int packetId = _tmpDatas[i].Key;
			byte[] data = _tmpDatas[i].Value;
			if (packetId != -1 && data != null)
			{
				if (onReceiveData != null)
				{
					object obj = objSerializer.dataToObj(packetId, data);
					onReceiveData(this, packetId, data);
                }
			}
		}
	}
}