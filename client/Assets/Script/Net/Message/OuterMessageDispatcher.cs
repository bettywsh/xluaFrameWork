using System;
using UnityEngine;


public class OuterMessageDispatcher: IMessageDispatcher
{
	public void Dispatch(Session session, byte[] buffer)
	{
		Debug.LogError(buffer.Length);
		//ushort opcode = BitConverter.ToUInt16(buffer, Packet.OpcodeIndex);
		//object message = NetworkManager.Instance.MessagePacker.DeserializeFrom(null, memoryStream);
		//Test01.Receive();
		//Debug.Log("receive msg：" + opcode);
	}
}
