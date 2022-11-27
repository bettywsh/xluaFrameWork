using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

public class ProtocolCodec 
{

    static int index;
    MemoryStream memStream;
    BinaryReader reader;

    public ProtocolCodec()
    {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
    }

    public void reset()
    {
        memStream.Close();
    }

    public byte[] encode(int id, byte[] data)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            int msglen = data.Length;
            			Debug.Log("msglen:"+data.Length);
            if (!NetPacketDefine.FirstLenOnlyMessageLen)
            {
                int addLen = 4;
                if (NetPacketDefine.LenPacketId == 2)
                    addLen = 2;
                if (NetPacketDefine.NeedPacketIndex)
                    addLen += 4;
                msglen += addLen;
            }
            if (NetPacketDefine.LenMsgLen != 2)
                writer.Write(msglen);
            else
                writer.Write((ushort)msglen);
            if (NetPacketDefine.NeedPacketIndex)
            {
                if (index == int.MaxValue)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }
                writer.Write(index);
            }
            if (NetPacketDefine.LenPacketId != 2)
                writer.Write(id);
            else
                writer.Write((ushort)id);
            writer.Write(data);
            //				FileUtil.SaveFileData("d:/msgPc.proto",data);
            writer.Flush();
            return ms.ToArray();
        }
    }

#pragma warning disable 219
    //public void decode(byte[] receiveData,int length,ref int id,ref byte[] data){
    public void decode(byte[] receiveData, int length, List<KeyValuePair<int, byte[]>> datas)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(receiveData, 0, length);
        //Reset to beginning
        memStream.Seek(0, SeekOrigin.Begin);
        int PacketHeadSize = (NetPacketDefine.LenMsgLen == 2 ? 2 : 4) + (NetPacketDefine.LenPacketId == 2 ? 2 : 4) + (NetPacketDefine.NeedPacketIndex ? 4 : 0);
        while (RemainingBytes() > PacketHeadSize)
        {
            int messageLen = NetPacketDefine.LenMsgLen == 2 ? (int)reader.ReadUInt16() : reader.ReadInt32();
            if (!NetPacketDefine.FirstLenOnlyMessageLen)
            {
                int addLen = 4;
                if (NetPacketDefine.LenPacketId == 2)
                    addLen = 2;
                if (NetPacketDefine.NeedPacketIndex)
                    addLen += 4;
                messageLen -= addLen;
            }
            int messageIndex = NetPacketDefine.NeedPacketIndex ? reader.ReadInt32() : 0;
            int packetId = NetPacketDefine.LenPacketId == 2 ? reader.ReadUInt16() : reader.ReadInt32();
            if (RemainingBytes() >= messageLen)
            {
                Debug.Log("rec msg len :" + messageLen);
                //					MemoryStream ms = new MemoryStream();
                //					BinaryWriter writer = new BinaryWriter(ms);
                //					writer.Write(reader.ReadBytes(messageLen));
                //					ms.Seek(0, SeekOrigin.Begin);
                //					BinaryReader r = new BinaryReader(ms);
                //					KeyValuePair<int,byte[]> newData = new KeyValuePair<int, byte[]>(packetId,r.ReadBytes((int)(ms.Length - ms.Position)));
                KeyValuePair<int, byte[]> newData = new KeyValuePair<int, byte[]>(packetId, reader.ReadBytes(messageLen));
                datas.Add(newData);
            }
            else
            {
                //Back up the position two bytes
                //memStream.Position = memStream.Position - 2;
                //memStream.Position = memStream.Position - sizeof(int)*3;
                memStream.Position = memStream.Position - PacketHeadSize;
                break;
            }
        }
        //Create a new stream with any leftover bytes
        byte[] leftover = reader.ReadBytes((int)RemainingBytes());
        memStream.SetLength(0);     //Clear
        memStream.Write(leftover, 0, leftover.Length);
    }

    long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }
}
	#pragma warning restore 219	



