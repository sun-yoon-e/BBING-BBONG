using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class Tcp_Client : MonoBehaviour
{
    TcpClient client_socket;
    private string IP = "127.0.0.1";
    private int Port = 13531;

    byte[] recvB;

    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;

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
        try
        {
            client_socket = new TcpClient(IP, Port);
            stream = client_socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
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
            recvB = new byte[13];
            stream.Read(recvB, 0, recvB.Length);
            string msg = Encoding.UTF8.GetString(recvB, 0, recvB.Length);
            Debug.Log(msg);
        }
        catch (Exception e)
        {
            Debug.Log("Socket recv error! : " + e.ToString());
        }
       
    }

    void Send()
    {
        string message = "I'm Client!";
        byte[] sendB = Encoding.ASCII.GetBytes(message);

        try
        {
            stream.Write(sendB, 0, sendB.Length);
            string msg = Encoding.UTF8.GetString(recvB, 0, recvB.Length);
            Debug.Log(msg);
        }
        catch (Exception e)
        {
            Debug.Log("Socket send error! : " + e.ToString());
        }
    }
}
