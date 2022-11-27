using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using XLua;

public class ByteBuffer {
    MemoryStream stream = null;
    BinaryWriter writer = null;
    BinaryReader reader = null;


    public static double LuaStringToInt64(byte[] strBuffer)
    {
        if (strBuffer.Length == 8)
        {
            return System.BitConverter.ToDouble(strBuffer, 0);
        }
        return 0;
    }

    public static String LuaInt64StringToString(byte[] strBuffer)
    {
        long num = 0;
        if (strBuffer.Length == 8)
        {
            num = System.BitConverter.ToInt64(strBuffer, 0);
        }
        return num.ToString();
    }
    public static byte[] StringToLuaInt64String(String str)
    {
        long num = Convert.ToInt64(str);
        byte[] data = System.BitConverter.GetBytes(num);
        return (data);
    }

    public static long LuaStringRightShift(long strBuffer, int BitNum, int BandNum)
    {
        long ShiftLong = (strBuffer) >> BitNum;
        if (BandNum > 0)
        {
            int Band = (int)Mathf.Pow(2, BandNum) - 1;
            ShiftLong = ShiftLong & Band;
        }
        return ShiftLong;
    }


    public static String Int64ToString(double lv)
    {
        var luaByteBuffer = Int64ToLuaString(lv);
        return LuaInt64StringToString(luaByteBuffer);
    }

    public static double StringToInt64(string str)
    {
        var luaByteBuffer = StringToLuaInt64String(str);
        return LuaStringToInt64(luaByteBuffer);
    }

    public static byte[] Int64ToLuaString(double lv)
    {
        byte[] data = System.BitConverter.GetBytes(lv);
        return data;
    }

    public ByteBuffer() {
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);
    }

    public ByteBuffer(byte[] data) {
        if (data != null) {
            stream = new MemoryStream(data);
            reader = new BinaryReader(stream);
        } else {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }
    }

    public void Close() {
        if (writer != null) writer.Close();
        if (reader != null) reader.Close();

        stream.Close();
        writer = null;
        reader = null;
        stream = null;
    }

    public void WriteByte(byte v) {
        writer.Write(v);
    }

    public void WriteInt(int v) {
        writer.Write((int)v);
    }

    public void WriteShort(ushort v) {
        writer.Write((ushort)v);
    }

    public void WriteLong(long v) {
        writer.Write((long)v);
    }

    public void WriteFloat(float v) {
        byte[] temp = BitConverter.GetBytes(v);
        Array.Reverse(temp);
        writer.Write(BitConverter.ToSingle(temp, 0));
    }

    public void WriteDouble(double v) {
        byte[] temp = BitConverter.GetBytes(v);
        Array.Reverse(temp);
        writer.Write(BitConverter.ToDouble(temp, 0));
    }

    public void WriteString(string v) {
        byte[] bytes = Encoding.UTF8.GetBytes(v);
        writer.Write((ushort)bytes.Length);
        writer.Write(bytes);
    }

    public void WriteBytes(byte[] v) {
        //writer.Write((int)v.Length);
        writer.Write(v);
    }

    public void WriteBuffer(byte[] strBuffer) {
        WriteBytes(strBuffer);
    }

    public byte ReadByte() {
        return reader.ReadByte();
    }

    public int ReadInt() {
        return (int)reader.ReadInt32();
    }

    public ushort ReadShort() {
        return (ushort)reader.ReadInt16();
    }

    public long ReadLong() {
        return (long)reader.ReadInt64();
    }

    public float ReadFloat() {
        byte[] temp = BitConverter.GetBytes(reader.ReadSingle());
        Array.Reverse(temp);
        return BitConverter.ToSingle(temp, 0);
    }

    public double ReadDouble() {
        byte[] temp = BitConverter.GetBytes(reader.ReadDouble());
        Array.Reverse(temp);
        return BitConverter.ToDouble(temp, 0);
    }

    public string ReadString() {
        ushort len = ReadShort();
        byte[] buffer = new byte[len];
        buffer = reader.ReadBytes(len);
        return Encoding.UTF8.GetString(buffer);
    }

    public byte[] ReadBytes() {
        int len = ReadInt();
        return reader.ReadBytes(len);
    }

    public byte[] ReadBuffer() {
        //byte[] bytes = ReadBytes();
        return stream.ToArray();
    }

    public byte[] ToBytes() {
        writer.Flush();
        return stream.ToArray();
    }

    public void Flush() {
        writer.Flush();
    }
}