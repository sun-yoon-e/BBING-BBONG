using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class TcpSocket : MonoBehaviour
{
    private Socket client_socket;
    private string IP = "127.0.0.1";
    private int Port = 13531;

    private int SenddataLength;                         // Send Data Length. (byte)
    private int ReceivedataLength;                      // Receive Data Length. (byte)

    private byte[] Sendbyte;                            // Data encoding to send. ( to Change bytes)
    private byte[] Receivebyte = new byte[2000];        // Receive data by this array to save.
    private string ReceiveString;                       // Receive bytes to Change string.

    // Start is called before the first frame update
    void Start()
    {
        InitClient();
    }

    // Update is called once per frame
    void Update()
    {
        Recv();
        Send();
    }

    void OnApplicationQuit()
    {
        client_socket.Close();
        client_socket = null;
    }

    void InitClient()
    {
        client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Socket option.
        //client_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
        //client_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);

        // Socket connect.
        try
        {
            IPAddress ipAddr = System.Net.IPAddress.Parse(IP);
            IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, Port);
            client_socket.Connect(ipEndPoint);
        }
        catch (SocketException e)
        {
            Debug.Log("Socket connect error! : " + e.ToString());
            return;
        }
    }

    void Recv()
    {
        try
        {
            client_socket.Receive(Receivebyte);
            ReceiveString = Encoding.Default.GetString(Receivebyte);
            ReceivedataLength = Encoding.Default.GetByteCount(ReceiveString.ToString());
            Debug.Log("Receive Data : " + ReceiveString + "(" + ReceivedataLength + ")");
        }
        catch (SocketException e)
        {
            Debug.Log("Socket recv error! : " + e.ToString());
        }
    }

    void Send()
    {
        StringBuilder sb = new StringBuilder(); // String Builder Create
        sb.Append("Test 1");
        sb.Append("Test 2");

        try
        {
            SenddataLength = Encoding.Default.GetByteCount(sb.ToString());
            Sendbyte = Encoding.Default.GetBytes(sb.ToString());
            client_socket.Send(Sendbyte, Sendbyte.Length, 0);
        }
        catch (SocketException e)
        {
            Debug.Log("Socket send error! : " + e.ToString());
        }
    }
}
