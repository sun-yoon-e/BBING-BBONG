using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class LoginEventArgs : EventArgs
{
    public bool success;
}

public class SignupEventArgs : EventArgs
{
    public bool success;
}

public class MeshEventArgs : EventArgs
{
    public bool ready;
    public Vector3[] vertices;
    public int[] triangles;
}

public class RoadEventArgs : EventArgs
{
    public bool ready;
    public Vector3[] vertices;
    public int[] triangles;
    public bool[] isRoad;
    public int[] isBuildingPlace;
}

public class ScoreUpdateEventArgs : EventArgs
{
    public int[] scores;
}

public class PositionUpdateEventArgs : EventArgs
{
    public int players;
    public Vector3[] position;
    public Vector3[] rotation;
}

public class GameStateChangedEventArgs : EventArgs
{
    public bool gameState;
}

public class FireEventArgs : EventArgs
{
    public Vector3 position;
    public Vector3 targetPosition;
}

public class GameClient
{
    public const byte SC_LOGIN  = 0;
    public const byte SC_LOGOUT = 1;
    public const byte SC_SIGNUP = 2;
    public const byte SC_MOVE   = 3;
    public const byte SC_CHAT   = 4;
    public const byte SC_ITEM   = 5;
    public const byte SC_SCORE  = 6;
    public const byte SC_GAMESTATE = 7;
    public const byte SC_FIRE   = 8;
    public const byte SC_MESH     = 10;
    public const byte SC_SET_MESH = 11;
    public const byte SC_ROAD     = 12;
    public const byte SC_SET_ROAD = 13;
    
    public const byte CS_LOGIN  = 0;
    public const byte CS_LOGOUT = 1;
    public const byte CS_SIGNUP = 2;
    public const byte CS_MOVE   = 3;
    public const byte CS_CHAT   = 4;
    public const byte CS_ITEM   = 5;
    public const byte CS_SCORE  = 6;
    public const byte CS_FIRE   = 8;
    public const byte CS_MESH     = 10;
    public const byte CS_SET_MESH = 11;
    public const byte CS_ROAD     = 12;
    public const byte CS_SET_ROAD = 13;
    
    public const string SERVER_IP = "";
    public const int SERVER_PORT = 13531;
    

    // 싱글톤 패턴
    // https://tech.lonpeach.com/2017/02/04/unity3d-singleton-pattern-example/
    // :wishket: 씬이 변경될 때 소켓이 끊기고 재접속하는 것을 방지하기 위해 GameClient를 하나만 만들어놓고 여기저기서 가져다 씁니다.
    private static GameClient _instance = null;
    private static readonly object padlock = new object();
    
    public static GameClient Instance {
        get
        {
            lock (padlock)
            {
                if (_instance == null)
                {
                    _instance = new GameClient();
                }

                return _instance;
            }
        }
    }
    
    private Socket socket = null;
    private Thread recvThread = null;

    // 이벤트 핸들러
    // :wishket: 소켓을 접속하고 처리하는 부분은 이 GameClient.cs에 포함되어 있습니다.
    //           그런데, 문제가 발생했을 때 '오류가 발생했습니다' 같은 메시지를 사용자에게 보여주는
    //           부분은 각 씬의 스크립트에서 처리해야 합니다. 
    //           각 씬의 스크립트에서 GameClient에 있는 소켓에 오류가 생겼는지 등의 정보를 받기 위해 이벤트 핸들러를 씁니다.
    public event EventHandler OnSocketError; // 소켓 오류 발생 시
    public event EventHandler<LoginEventArgs> OnLogin;
    public event EventHandler<SignupEventArgs> OnSignup;
    public event EventHandler<MeshEventArgs> OnMeshChanged;
    public event EventHandler<RoadEventArgs> OnRoadChanged;
    public event EventHandler<ScoreUpdateEventArgs> OnScoreUpdated;
    public event EventHandler<PositionUpdateEventArgs> OnPositionUpdated;
    public event EventHandler<GameStateChangedEventArgs> OnGameStateChanged;
    public event EventHandler<FireEventArgs> OnFired;
    
    
    public int clientId { get; private set; } = -1;

    public bool isGameStarted = false;

    private GameClient() 
    {
        
    }

    public void ConnectServer(string serverIP, int serverPort)
    {
        // https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket?view=net-5.0
        Debug.Log($"서버 접속 중: {serverIP}:{serverPort}");

        // string 형식의 ipv4 주소를 32비트 int로 변환한다
        // https://docs.microsoft.com/en-us/dotnet/api/system.net.ipaddress.parse?view=net-5.0
        var address = IPAddress.Parse(serverIP);
        var ipe = new IPEndPoint(address, serverPort);

        Socket tmpSocket = new Socket(
            ipe.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        try
        {
            tmpSocket.Connect(ipe);

            if (tmpSocket.Connected)
            {
                socket = tmpSocket;
                recvThread = new Thread(new ThreadStart(() =>
                {
                    Debug.Log("recvThread started");
                    try
                    {
                        while (socket.Connected)
                        {
                            byte[] buffer = new byte[512000];
                            int read = socket.Receive(buffer);

                            if (read == 0)
                            {
                                break;
                            }
                            Debug.Log(read + " bytes read");

                            UnityMainThreadDispatcher.Instance().Enqueue(
                                () => ProcessPacket(buffer)
                            );
                        }
                    }
                    catch (SocketException e)
                    {
                        if (OnSocketError != null)
                        {
                            OnSocketError(this, EventArgs.Empty);
                        }
                    }
                }));
                recvThread.Start();
            }
            else
            {
                if (OnSocketError != null)
                    OnSocketError(this, EventArgs.Empty);
            }
        }
        catch (SocketException e)
        {
            if (OnSocketError != null)
                OnSocketError(this, EventArgs.Empty);
        }
    }

    public void DisconnectServer()
    {
        
    }

    private void ProcessPacket(byte[] buffer)
    {
        var reader = new BinaryReader(new MemoryStream(buffer));
        var header = reader.ReadByte();

        if (header == SC_LOGIN)
        {
            var success = reader.ReadBoolean();
            clientId = reader.ReadInt32();
            
            if (OnLogin != null)
            {
                var eventArgs = new LoginEventArgs();
                eventArgs.success = success;
                OnLogin(this, eventArgs);
            }
        }
        else if (header == SC_SIGNUP)
        {
            var success = reader.ReadBoolean();
            if (OnSignup != null)
            {
                var eventArgs = new SignupEventArgs();
                eventArgs.success = success;
                OnSignup(this, eventArgs);
            }
        }
        else if (header == SC_MESH || header == SC_SET_MESH)
        {
            var ready = reader.ReadBoolean();
            var vertices = new Vector3[101 * 101];
            var triangles = new int[100 * 100 * 6];

            for (int i = 0; i < vertices.Length; i++)
            {
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                var z = reader.ReadSingle();
                vertices[i] = new Vector3(x, y, z);
            }

            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = reader.ReadInt32();
            }

            if (OnMeshChanged != null)
            {
                var eventArgs = new MeshEventArgs();
                eventArgs.ready = ready;
                eventArgs.vertices = vertices;
                eventArgs.triangles = triangles;
                OnMeshChanged(this, eventArgs);
            }
        } else if (header == SC_ROAD || header == SC_SET_ROAD)
        {
            var ready = reader.ReadBoolean();
            var vertices = new Vector3[101 * 101];
            var triangles = new int[100 * 100 * 6];
            var isRoad = new bool[101 * 101];
            var isBuildingPlace = new int[101 * 101];

            for (int i = 0; i < vertices.Length; i++)
            {
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                var z = reader.ReadSingle();
                vertices[i] = new Vector3(x, y, z);
            }

            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = reader.ReadInt32();
            }

            for (int i = 0; i < isRoad.Length; i++)
            {
                isRoad[i] = reader.ReadBoolean();
            }

            for (int i = 0; i < isBuildingPlace.Length; i++)
            {
                isBuildingPlace[i] = reader.ReadInt16();
            }

            if (OnRoadChanged != null)
            {
                var eventArgs = new RoadEventArgs();
                eventArgs.ready = ready;
                eventArgs.vertices = vertices;
                eventArgs.triangles = triangles;
                eventArgs.isRoad = isRoad;
                eventArgs.isBuildingPlace = isBuildingPlace;
                OnRoadChanged(this, eventArgs);
            }
        } else if (header == SC_SCORE)
        {
            int players = reader.ReadInt32();
            int[] scores = new int[players];

            for (int i = 0; i < players; i++)
            {
                scores[i] = reader.ReadInt32();
            }

            if (OnScoreUpdated != null)
            {
                var eventArgs = new ScoreUpdateEventArgs();
                eventArgs.scores = scores;
                OnScoreUpdated(this, eventArgs);
            }
        } else if (header == SC_MOVE)
        {
            int players = reader.ReadInt32();
            Vector3[] position = new Vector3[players];
            Vector3[] rotation = new Vector3[players];

            for (int i = 0; i < players; i++)
            {
                position[i].x = reader.ReadSingle();
                position[i].y = reader.ReadSingle();
                position[i].z = reader.ReadSingle();
            }
            
            for (int i = 0; i < players; i++)
            {
                rotation[i].x = reader.ReadSingle();
                rotation[i].y = reader.ReadSingle();
                rotation[i].z = reader.ReadSingle();
            }

            if (OnPositionUpdated != null)
            {
                var eventArgs = new PositionUpdateEventArgs();
                eventArgs.players = players;
                eventArgs.position = position;
                eventArgs.rotation = rotation;

                OnPositionUpdated(this, eventArgs);
            }
        } else if (header == SC_GAMESTATE)
        {
            bool state = reader.ReadBoolean();
            isGameStarted = state;
            
            if (OnGameStateChanged != null)
            {
                var eventArgs = new GameStateChangedEventArgs();
                eventArgs.gameState = state;
                
                OnGameStateChanged(this, eventArgs);
            }
        } else if (header == SC_FIRE)
        {
            Vector3 position = new Vector3();
            Vector3 targetPosition = new Vector3();

            position.x = reader.ReadSingle();
            position.y = reader.ReadSingle();
            position.z = reader.ReadSingle();
            
            targetPosition.x = reader.ReadSingle();
            targetPosition.y = reader.ReadSingle();
            targetPosition.z = reader.ReadSingle();

            if (OnFired != null)
            {
                var eventArgs = new FireEventArgs();
                eventArgs.position = position;
                eventArgs.targetPosition = targetPosition;

                OnFired(this, eventArgs);
            }
        }
    }

    public void Login(string username, string password)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));
        char[] byteUsername = new char[32];
        char[] bytePassword = new char[32];
        username.CopyTo(0, byteUsername, 0, Math.Min(username.Length, 31));
        password.CopyTo(0, bytePassword, 0, Math.Min(password.Length, 31));
        
        writer.Write(CS_LOGIN);
        writer.Write(byteUsername);
        writer.Write(bytePassword);

        socket.Send(buffer);
    }

    public void SignUp(string username, string password)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));
        char[] byteUsername = new char[32];
        char[] bytePassword = new char[32];
        username.CopyTo(0, byteUsername, 0, Math.Min(username.Length, 31));
        password.CopyTo(0, bytePassword, 0, Math.Min(password.Length, 31));
        
        writer.Write(CS_SIGNUP);
        writer.Write(byteUsername);
        writer.Write(bytePassword);

        socket.Send(buffer);
    }

    public void GetMesh()
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_MESH);
        socket.Send(buffer);
    }

    public void SetMesh(Vector3[] vertices, int[] triangles)
    {
        var buffer = new byte[512000];
        var writer = new BinaryWriter(new MemoryStream(buffer));
        
        writer.Write(CS_SET_MESH);
        for (int i = 0; i < vertices.Length; i++)
        {
            writer.Write(vertices[i].x);
            writer.Write(vertices[i].y);
            writer.Write(vertices[i].z);
        }

        for (int i = 0; i < triangles.Length; i++)
        {
            writer.Write(triangles[i]);
        }

        socket.Send(buffer);
    }
    
    public void GetRoad()
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_ROAD);
        socket.Send(buffer);
    }

    public void SetRoad(Vector3[] vertices, int[] triangles, bool[] isRoad, int[] isBuildingPlace)
    {
        var buffer = new byte[512000];
        var writer = new BinaryWriter(new MemoryStream(buffer));
        
        writer.Write(CS_SET_ROAD);
        for (int i = 0; i < vertices.Length; i++)
        {
            writer.Write(vertices[i].x);
            writer.Write(vertices[i].y);
            writer.Write(vertices[i].z);
        }

        for (int i = 0; i < triangles.Length; i++)
        {
            writer.Write(triangles[i]);
        }

        for (int i = 0; i < isRoad.Length; i++)
        {
            writer.Write(isRoad[i]);
        }

        for (int i = 0; i < isBuildingPlace.Length; i++)
        {
            writer.Write((short) isBuildingPlace[i]);
        }

        socket.Send(buffer);
    }

    public void UpdateScore()
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));
        writer.Write(CS_SCORE);

        socket.Send(buffer);
    }

    public void UpdatePosition(Vector3 pos, Vector3 rot)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));
        writer.Write(CS_MOVE);
        
        writer.Write(pos.x);
        writer.Write(pos.y);
        writer.Write(pos.z);
        
        
        writer.Write(rot.x);
        writer.Write(rot.y);
        writer.Write(rot.z);

        socket.Send(buffer);
    }

    public void FirePizza(Vector3 pos, Vector3 targetPos)
    {
         var buffer = new byte[255];
         var writer = new BinaryWriter(new MemoryStream(buffer));
         writer.Write(CS_FIRE);
        
         writer.Write(pos.x);
         writer.Write(pos.y);
         writer.Write(pos.z);
         
         
         writer.Write(targetPos.x);
         writer.Write(targetPos.y);
         writer.Write(targetPos.z);
 
         socket.Send(buffer);       
    }
}
