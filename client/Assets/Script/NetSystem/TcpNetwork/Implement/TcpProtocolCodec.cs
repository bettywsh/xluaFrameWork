using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using LuaFramework;
using UnityEditor.Sprites;
using UnityEditor.Experimental.GraphView;


    public class TcpProtocolCodec : IProtocolCodec
    {
        public static volatile int msgIndex = 0;
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
                uint msglen = (uint)(data.Length + 6);
            //uint sid = (uint)id << 19;
            //sid = sid | (uint)msglen;
                ushort msgIndex = (ushort)(Interlocked.Increment(ref TcpProtocolCodec.msgIndex) & 0xFFFF);

                //writer.Write((uint)msglen);
                //writer.Write((ushort)msgIndex);
                //writer.Write((ushort)id);
                //writer.Write((ushort)0);
                //writer.Write(data);

                writer.Write(BitConverter.GetBytes(Converter.GetBigEndian((uint)(msglen))), 0, 4);
                //发送的包索引
                writer.Write(BitConverter.GetBytes(Converter.GetBigEndian((ushort)msgIndex)), 0, 2);
                //发送的协议ID
                writer.Write(BitConverter.GetBytes(Converter.GetBigEndian((ushort)id)), 0, 2);
                //writer
                writer.Write(BitConverter.GetBytes(Converter.GetBigEndian((ushort)0)), 0, 2);
                //协议数据
                writer.Write(data, 0, data.Length);
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
            int PacketHeadSize = 14;
            while (RemainingBytes() >= PacketHeadSize)
            {
                byte[] totalLengthBytes = reader.ReadBytes(4);
                int totalLength = Converter.GetBigEndian(BitConverter.ToInt32(totalLengthBytes, 0));
                byte[] msgidexBytes = reader.ReadBytes(4);
                int msgidex = Converter.GetBigEndian(BitConverter.ToInt32(msgidexBytes, 0));
                byte[] packetIdBytes = reader.ReadBytes(2);
                ushort packetId = Converter.GetBigEndian(BitConverter.ToUInt16(packetIdBytes, 0));
                byte[] aaBytes = reader.ReadBytes(4);
                int aa = Converter.GetBigEndian(BitConverter.ToInt32(aaBytes, 0));

                //ushort msgidex = reader.ReadUInt16();
                //uint packetId = reader.ReadUInt16();
                //uint aa = reader.ReadUInt32();
                int messageLen = totalLength;
                if (RemainingBytes() >= messageLen)
                {
                    byte[] packetArray = reader.ReadBytes((int)messageLen);
                //Array.Reverse(packetArray);
                //byte[] proto = null;
                //    Array.Copy(packetArray, 0, proto, 0, messageLen);
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
