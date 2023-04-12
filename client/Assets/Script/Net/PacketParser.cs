using LuaFramework;
using System;
using System.IO;


public enum ParserState
{
	PacketSize,
	PacketBody
}
	
public static class Packet
{
	public const int PacketSizeLength2 = 2;
	public const int PacketSizeLength4 = 4;
	public const int MinPacketSize = 2;
	public const int OpcodeIndex = 0;
	public const int MessageIndex = 2;
}

public class PacketParser
{
	private readonly CircularBuffer buffer;
    public int packetSize;
	private ParserState state;
    private readonly byte[] cache = new byte[8];
    public MemoryStream memoryStream;
	private bool isOK;
	private readonly int packetSizeLength;

	public PacketParser(int packetSizeLength, CircularBuffer buffer, MemoryStream memoryStream)
	{
		this.packetSizeLength = packetSizeLength;
		this.buffer = buffer;
		this.memoryStream = memoryStream;
	}

	public bool Parse()
	{
		if (this.isOK)
		{
			return true;
		}

		bool finish = false;
		while (!finish)
		{
			switch (this.state)
			{
				case ParserState.PacketSize:
					if (this.buffer.Length < this.packetSizeLength)
					{
						finish = true;
					}
					else
					{
						this.buffer.Read(this.cache, 0, this.packetSizeLength);
							
						switch (this.packetSizeLength)
						{
							case Packet.PacketSizeLength4:
								this.packetSize = Converter.GetBigEndian(BitConverter.ToInt32(this.cache, 0));
								if (this.packetSize > ushort.MaxValue * 16 || this.packetSize < Packet.MinPacketSize)
								{
									throw new Exception($"recv packet size error, 可能是外网探测端口: {this.packetSize}");
								}
								break;
							case Packet.PacketSizeLength2:
								this.packetSize = BitConverter.ToUInt16(this.cache, 0);
								if (this.packetSize > ushort.MaxValue || this.packetSize < Packet.MinPacketSize)
								{
									throw new Exception($"recv packet size error:, 可能是外网探测端口: {this.packetSize}");
								}
								break;
							default:
								throw new Exception("packet size byte count must be 2 or 4!");
						}
						this.state = ParserState.PacketBody;
					}
					break;
				case ParserState.PacketBody:
					if (this.buffer.Length < this.packetSize + 10)
					{
						finish = true;
					}
					else
					{
                        memoryStream = new MemoryStream(this.packetSize + 10);                        
						this.buffer.Read(memoryStream, this.packetSize + 10);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        this.isOK = true;
						this.state = ParserState.PacketSize;
						finish = true;
					}
					break;
			}
		}
		return this.isOK;
	}

	public MemoryStream GetPacket()
	{
		this.isOK = false;
		return this.memoryStream;
	}
}