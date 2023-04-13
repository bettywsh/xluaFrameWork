using com.bochsler.protocol;
using LuaFramework;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

public class ProtobufPacker : IMessagePacker
{
	private int sendMsgIndex = 0;
	public byte[] SerializeTo(int opcode, byte[] data)
	{
		sendMsgIndex += 1;
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            uint msglen = (uint)(data.Length + 6);

            ushort msgIndex = (ushort)(Interlocked.Increment(ref TcpProtocolCodec.msgIndex) & 0xFFFF);

            writer.Write(BitConverter.GetBytes(Converter.GetBigEndian((uint)(msglen))), 0, 4);
            //发送的包索引
            writer.Write(BitConverter.GetBytes(Converter.GetBigEndian((ushort)sendMsgIndex)), 0, 2);
            //发送的协议ID
            writer.Write(BitConverter.GetBytes(Converter.GetBigEndian((ushort)opcode)), 0, 2);
            //writer
            writer.Write(BitConverter.GetBytes(Converter.GetBigEndian((ushort)0)), 0, 2);
            //协议数据
            writer.Write(data, 0, data.Length);
            writer.Flush();
            return ms.ToArray();
        }
	}

	public object DeserializeFrom(MemoryStream memStream, int packetLength)
	{
        BinaryReader reader = new BinaryReader(memStream);
        //byte[] totalLengthBytes = reader.ReadBytes(4);
        //int totalLength = Converter.GetBigEndian(BitConverter.ToInt32(totalLengthBytes, 0));
        byte[] msgidexBytes = reader.ReadBytes(4);
        int msgidex = Converter.GetBigEndian(BitConverter.ToInt32(msgidexBytes, 0));
        byte[] packetIdBytes = reader.ReadBytes(2);
        ushort packetId = Converter.GetBigEndian(BitConverter.ToUInt16(packetIdBytes, 0));
        byte[] aaBytes = reader.ReadBytes(4);
        int aa = Converter.GetBigEndian(BitConverter.ToInt32(aaBytes, 0));
        byte[] packetArray = reader.ReadBytes(packetLength);

        if (packetId > 10000)
        {
            //lua
            MessageManager.Instance.EventNotify(MessageConst.MsgNetData, packetId, packetArray);
        }
        else
        {
            //c#
            SCMessageEnum scMsg = (SCMessageEnum)packetId;
            MessageManager.Instance.EventNotify(MessageConst.MsgNetMsg + scMsg.ToString(), packetId, packetArray);
        }
        return null;
	}
}