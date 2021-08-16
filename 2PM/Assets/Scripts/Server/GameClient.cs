using System;
using System.Collections.Generic;
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
public class FireEventArgs : EventArgs
{
    public Vector3 position;
    public Vector3 targetPosition;
}

public class AIPositionUpdateEventArgs : EventArgs
{
    public int AIID;
    public Vector3 position;
    public Vector3 rotation;
}

public class AIFireEventArgs : EventArgs
{
    public int AIID;
    public Vector3 position;
    public Vector3 targetPosition;
}

public class GameStateChangedEventArgs : EventArgs
{
    public bool gameState;
}

public class GameSceneChangedEventArgs : EventArgs
{
    public int scene;
}

public class ReceiveMessageEventArgs : EventArgs
{
    public string msg;
}

public class RoomPlayer : EventArgs
{
    public string nick1;
    public string nick2;
    public string nick3;
    public string nick4;
}

public class RoomInfo : EventArgs
{
    public int RoomID;
    public bool IsPlaying;
    public int PlayerNum;
    public string RoomName;
}

public class RoomListMessageEventArgs : EventArgs
{
    public List<RoomInfo> roomInfo;
}

public class PlaceItemBoxMessageEventArgs : EventArgs
{
    public int ItemID;
    public Vector3 Position;
}

public class RemoveItemBoxMessageEventArgs : EventArgs
{
    public int ItemID;
}

public class ItemMessageEventArgs : EventArgs
{
    //0: 한명만 시야차단, 1: 나빼고 다 시야차단, 2: 이속 저하
    public byte ItemType;
}

public class MakeCarMessageEventArgs : EventArgs
{
    public int ID;
    public byte CarType;
    public Vector3 Position;
}

public class DestroyCarMessageEventArgs : EventArgs
{
    public int ID;
}

public class MakeTreeMessageEventArgs : EventArgs
{
    public byte Type;
    public Vector3 Position;
}

public class AIMessageEventArgs : EventArgs
{
    public int ID;
}

public class GameClient
{
    public const byte SC_LOGIN  = 100;
    public const byte SC_LOGOUT = 101;
    public const byte SC_SIGNUP = 102;

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
    public const byte SC_INIT = 14;
    public const byte SC_ROOM_LIST_INFO = 15;
    public const byte SC_ROOM_INFO = 16;
    public const byte SC_ROOM_PLAYER_INOUT = 17;
    public const byte SC_ROOM_READY = 18;

    public const byte SC_PLACE_ITEM   = 21;
    public const byte SC_REMOVE_ITEM = 22;
    public const byte SC_USE_ITEM = 23;

    public const byte SC_AI_MOVE = 24;
    public const byte SC_AI_FIRE = 25;

    public const byte SC_MAKE_CAR = 26;
    public const byte SC_DESTROY_CAR = 27;

    public const byte SC_AI_ADD = 28;
    public const byte SC_AI_REMOVE = 29;

    public const byte SC_MAKE_TREE = 30;

    //------------------------------------------------------------

    public const byte CS_LOGIN  = 100;
    public const byte CS_LOGOUT = 101;
    public const byte CS_SIGNUP = 102;

    public const byte CS_MOVE   = 3;
    public const byte CS_CHAT   = 4;
    public const byte CS_ITEM   = 5;
    public const byte CS_SCORE  = 6;
    public const byte CS_FIRE   = 8;
    public const byte CS_MESH     = 10;
    public const byte CS_SET_MESH = 11;
    public const byte CS_ROAD     = 12;
    public const byte CS_SET_ROAD = 13;

    public const byte CS_MAKE_ROOM = 14;
    public const byte CS_ENTER_ROOM = 15;
    public const byte CS_EXIT_ROOM = 16;
    public const byte CS_GAMESTATE = 17;
    public const byte CS_ROOM_LIST_INFO = 18;
    public const byte CS_ROOM_INFO = 19;
    public const byte CS_PLAYER_READ = 20;

    public const byte CS_PLACE_ITEM   = 21;
    public const byte CS_REMOVE_ITEM = 22;
    public const byte CS_USE_ITEM = 23;

    public const byte CS_AI_MOVE = 24;
    public const byte CS_AI_FIRE = 25;

    public const byte CS_MAKE_CAR = 26;
    public const byte CS_DESTROY_CAR = 27;

    public const byte CS_AI_ADD = 28;
    public const byte CS_AI_REMOVE = 29;

    public const byte CS_MAKE_TREE = 30;

    //public const string SERVER_IP = "127.0.0.1";
    public const string SERVER_IP = "14.38.227.223";
    public const int SERVER_PORT = 13531;

    // 싱글톤 패턴
    // https://tech.lonpeach.com/2017/02/04/unity3d-singleton-pattern-example/
    // 씬이 변경될 때 소켓이 끊기고 재접속하는 것을 방지하기 위해 GameClient를 하나만 만들어놓고 여기저기서 가져다 씁니다.
    private static GameClient _instance = null;
    private static readonly object padlock = new object();
    private static UnityMainThreadDispatcher dispinstance = null;

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
    // 소켓을 접속하고 처리하는 부분은 이 GameClient.cs에 포함되어 있습니다.
    // 그런데, 문제가 발생했을 때 '오류가 발생했습니다' 같은 메시지를 사용자에게 보여주는
    // 부분은 각 씬의 스크립트에서 처리해야 합니다. 
    // 각 씬의 스크립트에서 GameClient에 있는 소켓에 오류가 생겼는지 등의 정보를 받기 위해 이벤트 핸들러를 씁니다.
    public event EventHandler OnSocketError; // 소켓 오류 발생 시
    public event EventHandler<LoginEventArgs> OnLogin;
    public event EventHandler<SignupEventArgs> OnSignup;
    public event EventHandler<MeshEventArgs> OnMeshChanged;
    public event EventHandler<RoadEventArgs> OnRoadChanged;
    public event EventHandler<ScoreUpdateEventArgs> OnScoreUpdated;

    public event EventHandler<PositionUpdateEventArgs> OnPositionUpdated;
    public event EventHandler<FireEventArgs> OnFired;
    public event EventHandler<AIPositionUpdateEventArgs> OnAIPositionUpdated;
    public event EventHandler<AIFireEventArgs> OnAIFired;

    public event EventHandler<GameStateChangedEventArgs> OnGameStateChanged;
    public event EventHandler<GameSceneChangedEventArgs> OnGameSceneChanged;

    public event EventHandler<ReceiveMessageEventArgs> OnReceivedMessage;
    public event EventHandler<RoomListMessageEventArgs> OnRoomList;
    public event EventHandler<RoomInfo> OnRoomInfo;
    public event EventHandler<RoomPlayer> OnRoomNewPlayer;

    public event EventHandler<PlaceItemBoxMessageEventArgs> OnPlaceItemBox;
    public event EventHandler<RemoveItemBoxMessageEventArgs> OnRemoveItemBox;
    public event EventHandler<ItemMessageEventArgs> OnUseItem;

    public event EventHandler<MakeCarMessageEventArgs> OnMakeCar;
    public event EventHandler<DestroyCarMessageEventArgs> OnDestroyCar;

    public event EventHandler<MakeTreeMessageEventArgs> OnMakeTree;

    public event EventHandler<AIMessageEventArgs> OnAddAI;
    public event EventHandler<AIMessageEventArgs> OnRemoveAI;

    public int clientId { get; private set; } = -1;

    public string client_nick1 = "";
    public string client_nick2 = "";
    public string client_nick3 = "";
    public string client_nick4 = "";

    public bool[] ai = new bool[4];

    public int client_roomNum = -1;

    public bool isGameStarted = false;
    public bool isReadyToControl = false;

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
                            
                        var maxBufferSize = 512000 * 4;
                        var totalRead = 0;
                        byte[] buffer = new byte[maxBufferSize];

                        while (socket.Connected)
                        {
                            int read = socket.Receive(buffer, totalRead, maxBufferSize - totalRead, System.Net.Sockets.SocketFlags.None);

                            if (read == 0)
                            {
                                break;
                            }

                            if (buffer[0] == SC_ROAD || buffer[0] == SC_SET_ROAD || buffer[0] == SC_MESH || buffer[0] == SC_SET_MESH)
                            {
                                totalRead += read;
                                if (totalRead < 512000 * 4)
                                {
                                    continue;
                                }
                            }
                            totalRead = 0;

                            // if (buffer[0] == SC_ROAD || buffer[0] == SC_SET_ROAD)
                            //     Debug.Log("Road packet is received");
                            //Debug.Log(read + " bytes read");

                            if (dispinstance == null)
                            {
                                dispinstance = UnityMainThreadDispatcher.Instance();
                            }

                            var target = new byte[maxBufferSize];
                            Buffer.BlockCopy(buffer, 0, target, 0, maxBufferSize);
                            dispinstance.Enqueue(
                                () => ProcessPacket(target)
                            );
                            Array.Clear(buffer, 0, maxBufferSize);
                        }
                    }
                    catch (SocketException e)
                    {
                        Debug.Log(e);
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
            Debug.Log(e);
            if (OnSocketError != null)
                OnSocketError(this, EventArgs.Empty);
        }
    }

    public void DisconnectServer()
    {
        if (socket != null)
        {
            socket.Close();
            socket = null;
        }
    }

    public bool IsConnect()
    {
        if (socket == null || !socket.Connected) return false;
        return true;
    }

    private void ProcessPacket(byte[] buffer)
    {
        var reader = new BinaryReader(new MemoryStream(buffer));
        var header = reader.ReadByte();

        if(header == SC_CHAT)
        {
            // byte size = reader.ReadByte();
            // int id = reader.ReadInt32();
            var nick = reader.ReadBytes(20);
            string nickStr = System.Text.Encoding.Default.GetString(nick).Trim('\0');
            var byteChatMessage = reader.ReadBytes(83*3);
            string str = System.Text.Encoding.UTF8.GetString(byteChatMessage).Trim('\0');

            if (OnReceivedMessage != null)
            { 
                var eventArgs = new ReceiveMessageEventArgs();
                eventArgs.msg = $"{nickStr}:{str}";
                OnReceivedMessage(this, eventArgs);
            }
        }
        else if(header == SC_ROOM_PLAYER_INOUT)
        {
            RoomPlayer players = new RoomPlayer();
            byte[] nick1 = reader.ReadBytes(20);
            players.nick1 = System.Text.Encoding.Default.GetString(nick1).TrimEnd('\0');
            byte[] nick2 = reader.ReadBytes(20);
            players.nick2 = System.Text.Encoding.Default.GetString(nick2).TrimEnd('\0');
            byte[] nick3 = reader.ReadBytes(20);
            players.nick3 = System.Text.Encoding.Default.GetString(nick3).TrimEnd('\0');
            byte[] nick4 = reader.ReadBytes(20);
            players.nick4 = System.Text.Encoding.Default.GetString(nick4).TrimEnd('\0');

            if (players.nick1 != null) client_nick1 = players.nick1;
            if (players.nick2 != null) client_nick2 = players.nick2;
            if (players.nick3 != null) client_nick3 = players.nick3;
            if (players.nick4 != null) client_nick4 = players.nick4;

            if (OnRoomNewPlayer != null)
            {
                var eventArgs = new RoomPlayer();
                eventArgs = players;
                OnRoomNewPlayer(this, eventArgs);
            }
        }
        else if(header == SC_ROOM_INFO)
        {
            RoomInfo room = new RoomInfo();
            room.RoomID = reader.ReadInt32();
            room.IsPlaying = reader.ReadBoolean();
            room.PlayerNum = reader.ReadByte();
            byte[] bytes = reader.ReadBytes(30);
            room.RoomName = System.Text.Encoding.Default.GetString(bytes).TrimEnd('\0');
            
            if (OnRoomInfo != null)
            {
                var eventArgs = new RoomInfo();
                eventArgs = room;
                OnRoomInfo(this, eventArgs);
            }
        }
        else if(header == SC_ROOM_LIST_INFO)
        {
            if (OnRoomList != null)
            { 
                var eventArgs = new RoomListMessageEventArgs();
                eventArgs.roomInfo = new List<RoomInfo>();

                const int MAX_ROOM_NAME_SIZE = 30;
                for (int i =0; i<6; i++)
                {
                    byte type = reader.ReadByte();
                    RoomInfo room = new RoomInfo();
                    room.RoomID = reader.ReadInt32();
                    room.IsPlaying = reader.ReadBoolean();
                    room.PlayerNum = reader.ReadByte();
                    byte[] bytes = reader.ReadBytes(MAX_ROOM_NAME_SIZE);
                    room.RoomName = System.Text.Encoding.Default.GetString(bytes).TrimEnd('\0');
                    if(room.RoomID > 0)
                        eventArgs.roomInfo.Add(room);
                }
                OnRoomList(this, eventArgs);
            }
        }
        else if (header == SC_LOGIN)
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
            var vertices = new Vector3[201 * 201];
            var triangles = new int[200 * 200 * 6];

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
            else
            {
                Debug.Log("OnMeshChanged() is null");
            }
        }
        else if (header == SC_ROAD || header == SC_SET_ROAD)
        {
            var ready = reader.ReadBoolean();
            var vertices = new Vector3[201 * 201];
            var triangles = new int[200 * 200 * 6];
            var isRoad = new bool[201 * 201];
            var isBuildingPlace = new int[201 * 201];

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
        } 
        else if (header == SC_SCORE)
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
        } 
        else if (header == SC_GAMESTATE)
        {
            bool state = reader.ReadBoolean();
            isGameStarted = state;
            Debug.Log("게임시작여부? " + state);
            if (OnGameStateChanged != null)
            {
                var eventArgs = new GameStateChangedEventArgs();
                eventArgs.gameState = state;
                
                OnGameStateChanged(this, eventArgs);
            }
            else
            {
                Debug.Log("OnGameStateChanged is null");
            }
        } 
        else if (header == SC_MOVE)
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

                Debug.Log("OnPositionUpdate");
                OnPositionUpdated(this, eventArgs);
            }
        }
        else if (header == SC_FIRE)
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
        else if(header == SC_AI_ADD)
        {
            int aiID = reader.ReadInt32();

            if (OnAddAI != null)
            {
                var eventArgs = new AIMessageEventArgs();
                eventArgs.ID = aiID;
                OnAddAI(this, eventArgs);
            }
        }
        else if(header == SC_AI_REMOVE)
        {
            int aiID = reader.ReadInt32();

            if (OnRemoveAI != null)
            {
                var eventArgs = new AIMessageEventArgs();
                eventArgs.ID = aiID;
                OnRemoveAI(this, eventArgs);
            }
        }
        else if (header == SC_AI_MOVE)
        {
            int aiID = reader.ReadInt32();
            Vector3 position = new Vector3();
            Vector3 rotation = new Vector3();

            position.x = reader.ReadSingle();
            position.y = reader.ReadSingle();
            position.z = reader.ReadSingle();
            
            rotation.x = reader.ReadSingle();
            rotation.y = reader.ReadSingle();
            rotation.z = reader.ReadSingle();

            if (OnAIPositionUpdated != null)
            {
                var eventArgs = new AIPositionUpdateEventArgs();
                eventArgs.AIID = aiID;
                eventArgs.position = position;
                eventArgs.rotation = rotation;

                OnAIPositionUpdated(this, eventArgs);
            }
        }
        else if (header == SC_AI_FIRE)
        {
            int aiID = reader.ReadInt32();
            Vector3 position = new Vector3();
            Vector3 targetPosition = new Vector3();

            position.x = reader.ReadSingle();
            position.y = reader.ReadSingle();
            position.z = reader.ReadSingle();
            
            targetPosition.x = reader.ReadSingle();
            targetPosition.y = reader.ReadSingle();
            targetPosition.z = reader.ReadSingle();

            if (OnAIFired != null)
            {
                var eventArgs = new AIFireEventArgs();
                eventArgs.AIID = aiID;
                eventArgs.position = position;
                eventArgs.targetPosition = targetPosition;

                OnAIFired(this, eventArgs);
            }
        }
        else if (header == SC_INIT)
        {
            isGameStarted = false;
            int scene = reader.ReadInt32();
            Debug.Log("초기화");
            //SceneManager.LoadScene("Scenes/LoginScene");

            if (OnGameSceneChanged != null)
            {
                var eventArgs = new GameSceneChangedEventArgs();
                eventArgs.scene = scene;

                OnGameSceneChanged(this, eventArgs);
            }
            else
            {
                Debug.Log("OnGameSceneChanged is null");
            }
        }
        else if(header == SC_PLACE_ITEM)
        {
            Debug.Log("recvItem");
            int itemID = reader.ReadInt32();

            Vector3 position = new Vector3();
            position.x = reader.ReadSingle();
            position.y = reader.ReadSingle();
            position.z = reader.ReadSingle();

            if(OnPlaceItemBox != null)
            {
                var eventArgs = new PlaceItemBoxMessageEventArgs();
                eventArgs.ItemID = itemID;
                eventArgs.Position = position;

                Debug.Log("eventItem");
                OnPlaceItemBox(this, eventArgs);
            }
        }
        else if(header == SC_REMOVE_ITEM)
        {
            int itemID = reader.ReadInt32();

            if(OnRemoveItemBox != null)
            {
                var eventArgs = new RemoveItemBoxMessageEventArgs();
                eventArgs.ItemID = itemID;
                OnRemoveItemBox(this, eventArgs);
            }
        }
        else if(header == SC_USE_ITEM)
        {
            //0: 한명만 시야차단, 1: 나빼고 다 시야차단, 2: 이속 저하
            //Event를 받는 쪽에서 아이템종류에 맞춰 처리
            byte itemType = reader.ReadByte();
            if(OnUseItem != null)
            {
                var eventArgs = new ItemMessageEventArgs();
                eventArgs.ItemType = itemType;
                OnUseItem(this, eventArgs);
            }
        }
        else if (header == SC_MAKE_CAR)
        {
            Debug.Log("recvCar");
            int id = reader.ReadInt32();
            byte carType = reader.ReadByte();

            Vector3 position = new Vector3();
            position.x = reader.ReadSingle();
            position.y = reader.ReadSingle();
            position.z = reader.ReadSingle();

            if (OnMakeCar != null)
            {
                var eventArgs = new MakeCarMessageEventArgs();
                eventArgs.ID = id;
                eventArgs.CarType = carType;
                eventArgs.Position = position;

                Debug.Log("eventCar");
                OnMakeCar(this, eventArgs);
            }
        }
        else if (header == SC_DESTROY_CAR)
        {
            int id = reader.ReadInt32();

            if (OnDestroyCar != null)
            {
                var eventArgs = new DestroyCarMessageEventArgs();
                eventArgs.ID = id;
                OnDestroyCar(this, eventArgs);
            }
        }
        else if (header == SC_MAKE_TREE)
        {
            byte type = reader.ReadByte();
            Vector3 position = new Vector3();
            position.x = reader.ReadSingle();
            position.y = reader.ReadSingle();
            position.z = reader.ReadSingle();

            if (OnMakeTree != null)
            {
                var eventArgs = new MakeTreeMessageEventArgs();
                eventArgs.Type = type;
                eventArgs.Position = position;
                OnMakeTree(this, eventArgs);
            }
        }
    }

#region LoginSceneMessage
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

        client_nick1 = username;
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
#endregion
#region LobbySceneMessage
    public void RequestRoomList(int pageNum)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_ROOM_LIST_INFO);
        writer.Write((byte)pageNum);

        socket.Send(buffer);
    }
    public void MakeRoom(string roomName)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));
        char[] byteRoomName = new char[32];
        roomName.CopyTo(0, byteRoomName, 0, Math.Min(roomName.Length, 31));

        writer.Write(CS_MAKE_ROOM);
        writer.Write(byteRoomName);

        socket.Send(buffer);
    }
    public void EnterRoom(int roomID)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_ENTER_ROOM);
        writer.Write(roomID);

        socket.Send(buffer);
    }
    #endregion
#region WaitingRoomSceneMessage
    public void RoomInfo(int roomID)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_ROOM_INFO);
        writer.Write(roomID);

        socket.Send(buffer);
    }
    public void StartGame()
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_GAMESTATE);
        writer.Write(true);

        socket.Send(buffer);
    }
    public void ExitRoom()
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_EXIT_ROOM);
        socket.Send(buffer);
    }
#endregion
#region GamePlaySceneMessage
    public void SendMessage(string str)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));
        var byteChatMessage = new byte[251];
        var utf8 = System.Text.Encoding.UTF8.GetBytes(str.Substring(0, Math.Min(83, str.Length)));
        utf8.CopyTo(byteChatMessage, 0);

        string newStr = System.Text.Encoding.UTF8.GetString(byteChatMessage).Trim('\0');

        writer.Write(SC_CHAT);
        writer.Write(byteChatMessage);

        socket.Send(buffer);
    }
    public void UpdateScore()
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));
        writer.Write(CS_SCORE);

        socket.Send(buffer);
    }

#region 자동차생성삭제연동
    public void MakeCar(int id, byte carType, Vector3 pos)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_MAKE_CAR);

        writer.Write(id);
        writer.Write(carType);
        writer.Write(pos.x);
        writer.Write(pos.y);
        writer.Write(pos.z);

        socket.Send(buffer);

        Debug.Log("sendCar");
    }
    public void DestroyCar(int id)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_DESTROY_CAR);
        writer.Write(id);

        socket.Send(buffer);
    }
    #endregion
#region 나무연동
    public void MakeTree(byte treeType, Vector3 pos)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_MAKE_TREE);

        writer.Write(treeType);
        writer.Write(pos.x);
        writer.Write(pos.y);
        writer.Write(pos.z);

        socket.Send(buffer);
    }
    #endregion
#region Player연동
    public void UpdatePosition(Vector3 pos, Vector3 rot)
    {
        Debug.Log("UpdatePosition");
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
#endregion
#region AI 플레이어 연동
    public void AddAI()
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));
        writer.Write(CS_AI_ADD);

        socket.Send(buffer);
    }
    public void RemoveAI(/*int id*/)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));
        writer.Write(CS_AI_REMOVE);
        //writer.Write(id);

        socket.Send(buffer);
    }
    public void UpdatePositionAI(int aiID, Vector3 pos, Vector3 rot)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));
        writer.Write(CS_AI_MOVE);

        writer.Write(aiID);
        
        writer.Write(pos.x);
        writer.Write(pos.y);
        writer.Write(pos.z);
        
        writer.Write(rot.x);
        writer.Write(rot.y);
        writer.Write(rot.z);

        socket.Send(buffer);
    }
    public void FirePizzaAI(int aiID, Vector3 pos, Vector3 targetPos)
    {
         var buffer = new byte[255];
         var writer = new BinaryWriter(new MemoryStream(buffer));
         writer.Write(CS_AI_FIRE);

         writer.Write(aiID);
        
         writer.Write(pos.x);
         writer.Write(pos.y);
         writer.Write(pos.z);
         
         writer.Write(targetPos.x);
         writer.Write(targetPos.y);
         writer.Write(targetPos.z);
 
         socket.Send(buffer);       
    }
#endregion
#region Item연동
    //itemID는 각 아이템별로 고유한 값이어야한다
    public void PlaceItemBox(int itemID, Vector3 pos)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_PLACE_ITEM);
        writer.Write(itemID);
        writer.Write(pos.x);
        writer.Write(pos.y);
        writer.Write(pos.z);

        socket.Send(buffer);
        Debug.Log("sendItem");
    }
    public void RemoveItemBox(int itemID)
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_REMOVE_ITEM);
        writer.Write(itemID);

        socket.Send(buffer);
    }
    public void UseItem(int itemType, int playerID)
    {
        //0: 한명만 시야차단, 1: 나빼고 다 시야차단, 2: 이속 저하
        //나빼고 다 시야차단일 경우 playerID를 0으로 보내줄것
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_USE_ITEM);
        writer.Write((byte)itemType);
        writer.Write(playerID);

        socket.Send(buffer);
    }
#endregion
#region MeshSyncMessage
    public void GetMesh()
    {
        var buffer = new byte[255];
        var writer = new BinaryWriter(new MemoryStream(buffer));

        writer.Write(CS_MESH);
        socket.Send(buffer);
    }
    public void SetMesh(Vector3[] vertices, int[] triangles)
    {
        var buffer = new byte[512000*4];
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

        Debug.Log("Request to get road from server");
    }
    public void SetRoad(Vector3[] vertices, int[] triangles, bool[] isRoad, int[] isBuildingPlace)
    {
        var buffer = new byte[512000*4];
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
#endregion
#endregion
}