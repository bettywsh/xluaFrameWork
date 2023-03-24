
public class ByteBufferSerializer : IObjectSerializer{
	public byte[] objToData(int packetId,object obj){
		if (obj is ByteBuffer){
			ByteBuffer bb = obj as ByteBuffer;
			byte[] data = bb.ToBytes();
				
			bb.Close();
			return data;
		}
        return null;
    }

    public object dataToObj(int packetId,byte[] data){
		return new ByteBuffer(data);
	}
}

