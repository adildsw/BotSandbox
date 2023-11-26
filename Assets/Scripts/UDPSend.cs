using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPSend : MonoBehaviour
{
    public string IP = "127.0.0.1";
    public int port = 8080;

    private Socket socket;
    private IPEndPoint endPoint;

    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        endPoint = new IPEndPoint(IPAddress.Parse(IP), port);
    }

    public void SendData(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        socket.SendTo(data, endPoint);
    }

    void OnApplicationQuit()
    {
        if (socket != null)
        {
            socket.Close();
        }
    }
}