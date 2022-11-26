using System.Collections.Generic;

	
public interface IPacketIDParser{
	System.Type PacketIDtoType(int packetId);
	int TypeToPacketID(System.Type type);
}
