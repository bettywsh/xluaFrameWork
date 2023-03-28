public delegate void ConnectEvent(IConnect connect);
public delegate void ConnectDataEvent(IConnect connect,int id,byte[] data);
	
public interface IConnect{
	ConnectEvent onConnected{get;set;}
	ConnectEvent onLostConnect{get;set;}
	ConnectEvent onDisconnected{get;set;}
	ConnectDataEvent onReceiveData{get;set;}
		
	string name{get;set;}
	string ip{get;}
	int port{get;}
	bool isConnected{get;}
	void connectTo(string ip,int port);
	void disconnect();
	void send(int packetId,object obj);
	void send(int packetId,byte[] data);
	void update();
}
