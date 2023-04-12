using System;
using System.IO;


public interface IMessagePacker
{
	byte[] SerializeTo(int opcode, byte[] data);
    object DeserializeFrom(MemoryStream stream, int packetLength);

}