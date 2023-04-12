using System.Net;


public static class NetworkHelper
{
	public static IPEndPoint ToIPEndPoint(string host, int port)
	{
        IPAddress[] addresses = Dns.GetHostAddresses(host);
        return new IPEndPoint(addresses[0], port);
	}

	public static IPEndPoint ToIPEndPoint(string address)
	{
		int index = address.LastIndexOf(':');
		string host = address.Substring(0, index);
		string p = address.Substring(index + 1);
		int port = int.Parse(p);
		return ToIPEndPoint(host, port);
	}
}
