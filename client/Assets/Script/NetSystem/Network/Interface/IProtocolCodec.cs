using System.Collections.Generic;

public interface IProtocolCodec{
	void reset();
	byte[] encode(int id,byte[] data);
	void decode(byte[] receiveData,int length,List<KeyValuePair<int,byte[]>> datas);
}
