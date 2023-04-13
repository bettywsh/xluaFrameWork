using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Assertions.Must;

public sealed class Session
{
	private AChannel channel;


	public NetworkManager Network
	{
		get { return NetworkManager.Instance; }
	}

	public int Error
	{
		get
		{
			return this.channel.Error;
		}
		set
		{
			this.channel.Error = value;
		}
	}

	public Session(AChannel aChannel)
	{
		this.channel = aChannel;

		channel.ConnectCallback += OnConnect;
		channel.ErrorCallback += OnError;
		channel.ReadCallback += OnRead;
	}
		
	private void OnConnect(AChannel channel, int code)
	{
		if (Network.OnConnect != null)
		{
			Network.OnConnect.Invoke(0);
		}
        MessageManager.Instance.EventNotify(MessageConst.MsgConnected, code);
        Debug.Log("OnConnect" + code);
	}
		
	private void OnError(AChannel channel, int code)
	{
		if (Network.OnError != null)
		{
			Network.OnError.Invoke(code);
		}
		Debug.LogError("OnError:" + code);
		this.Dispose();
	}

	public void Dispose()
	{
		int error = this.channel.Error;
		if (this.channel.Error != 0)
		{
			Debug.LogError($"session dispose: ErrorCode: {error}, please see ErrorCode.cs!");
		}
			
		this.channel.Dispose();
	}

	public void Start()
	{
		this.channel.Start();
	}

	public IPEndPoint RemoteAddress
	{
		get
		{
			return this.channel.RemoteAddress;
		}
	}

	public ChannelType ChannelType
	{
		get
		{
			return this.channel.ChannelType;
		}
	}

	public MemoryStream Stream
	{
		get
		{
			return this.channel.Stream;
		}
	}
		
	public void OnRead(MemoryStream memoryStream, int packetLength)
	{
		try
		{
            Network.Recv(memoryStream, packetLength);
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
	}

	public void Send(byte[] buffers)
	{
		channel.Send(buffers);
    }
}