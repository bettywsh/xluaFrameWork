using System;
using System.IO;
using System.Net;
using UnityEngine;

public enum ChannelType
{
	Connect,
	Accept,
}

public abstract class AChannel
{
	public ChannelType ChannelType { get; }

	public AService Service { get; }

	public abstract MemoryStream Stream { get; }
		
	public int Error { get; set; }

	public IPEndPoint RemoteAddress { get; protected set; }

		
		
	private Action<AChannel, int> connectCallback;

	public event Action<AChannel, int> ConnectCallback
	{
		add
		{
			this.connectCallback += value;
		}
		remove
		{
			this.connectCallback -= value;
		}
	}

		
	private Action<AChannel, int> errorCallback;

	public event Action<AChannel, int> ErrorCallback
	{
		add
		{
			this.errorCallback += value;
		}
		remove
		{
			this.errorCallback -= value;
		}
	}
		
	private Action<MemoryStream, int> readCallback;

	public event Action<MemoryStream, int> ReadCallback
	{
		add
		{
			this.readCallback += value;
		}
		remove
		{
			this.readCallback -= value;
		}
	}

	public void OnConnect(int code)
	{
		Debug.Log("connect success");
		this.connectCallback.Invoke(this, code);
	}
		
	protected void OnRead(MemoryStream memoryStream, int packetLength)
	{
		this.readCallback.Invoke(memoryStream, packetLength);
	}

	protected void OnError(int e)
	{
		this.Error = e;
		this.errorCallback?.Invoke(this, e);
	}

	protected AChannel(AService service, ChannelType channelType)
	{
		this.ChannelType = channelType;
		this.Service = service;
	}

	public abstract void Start();
		
	public abstract void Send(byte[] data);
		
	public void Dispose()
	{
		this.Service.Dispose();
	}
}