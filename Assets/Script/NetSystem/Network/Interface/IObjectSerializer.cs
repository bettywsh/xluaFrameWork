public interface IObjectSerializer{
	byte[] objToData(int packetId,object obj);
	object dataToObj(int packetId,byte[] data);
}
