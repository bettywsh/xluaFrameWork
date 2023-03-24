using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;


    public class TcpProtocolCodec : IProtocolCodec
    {
        static int index;
        MemoryStream memStream;
        BinaryReader reader;

        public TcpProtocolCodec()
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
                uint msglen = (uint)data.Length;
                uint sid = (uint)id << 19;
                sid = sid | (uint)msglen;
                byte[] aa = BitConverter.GetBytes(sid);
                writer.Write(sid);
                writer.Write(data);
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
            int PacketHeadSize = 4;
            while (RemainingBytes() >= PacketHeadSize)
            {
                uint h = reader.ReadUInt32();
                uint packetId = h >> 19;
                uint messageLen = h << 13 >> 13;
                if (RemainingBytes() >= messageLen)
                {
                    byte[] packetArray = reader.ReadBytes((int)messageLen);
                    //if (null != packetArray) {
                    //    RC4.Instance().RC4DecryptTo(ref packetArray);
                    //}
                    KeyValuePair<int, byte[]> newData = new KeyValuePair<int, byte[]>((int)packetId, packetArray);
                    datas.Add(newData);
                    // KeyValuePair<int, byte[]> newData = new KeyValuePair<int, byte[]>((int)packetId, reader.ReadBytes((int)messageLen));
                    // datas.Add(newData);
                }
                else
                {
                    memStream.Position = memStream.Position - PacketHeadSize;
                    break;
                }
            }
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
