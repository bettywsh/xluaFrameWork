using System.Collections.Generic;

public class PacketIDParser : IPacketIDParser{
	Dictionary<int,string> typeDic = new Dictionary<int, string>();
	Dictionary<string,int> idDic = new Dictionary<string, int>();
	public PacketIDParser(){
		System.Type t = System.Type.GetType("ProtoMsgs.PACKET_ID");
		if (t != null){
			foreach (object obj in System.Enum.GetValues(t)){
				int pid = (int)obj;
				string typeName = obj.ToString();
				int idx = typeName.LastIndexOf("_");
				if (idx >-1){
					typeName = typeName.Substring(idx+1);
					typeDic.Add(pid,typeName);
					idDic.Add(typeName,pid);
				}else{
					UnityEngine.Debug.LogError("Bad Packet Name: "+typeName);
				}
					
			}
		}
	}
	
	public System.Type PacketIDtoType (int packetId)
	{
		string tn;
		if (typeDic.TryGetValue(packetId,out tn)){
			System.Type t = System.Type.GetType(string.Format("ProtoMsgs.{0}",tn));
			return t;
		}
		return null;
	}
		
	public int TypeToPacketID (System.Type type)
	{
		int id;
		if (idDic.TryGetValue(type.Name,out id)){
			return id;
		}
		return 0;
	}	
}
