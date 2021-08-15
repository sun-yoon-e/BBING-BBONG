#ifndef ROOM_H
#define ROOM_H

#include <map>
#include "Client.h"

using namespace std;

/// <summary>
/// AI �÷��̾� ��ġ �� ���� ����
/// </summary>
struct AI
{
	int id;
	Vector3		m_client_pos{ 0.0, 0.0, 0.0 };
	Vector3		m_client_rot{ 0.0, 0.0, 0.0 };
};

/// <summary>
/// ���� ��� �� �÷��� �����ϴ� ��
/// </summary>
class Room
{
private:
	const unsigned int MAX_PLAYER_NUMBER = 4;
	unsigned int currentPlayerNumber = 0;

	HANDLE clientSem;

	stack<int> playerIndex;
	vector<AI> aiPlayers;
	
	map<int, Client*> players;
	bool isGameStarted = false;
	bool isCountdownStarted = false;

	BOOL meshReady = false;
	Vector3 meshVertices[(XSIZE + 1) * (ZSIZE + 1)];
	int32_t meshTriangles[XSIZE * ZSIZE * 6];

	BOOL roadReady = false;
	Vector3 roadVertices[(XSIZE + 1) * (ZSIZE + 1)];
	int32_t roadTriangles[XSIZE * ZSIZE * 6];
	bool isRoad[(XSIZE + 1) * (ZSIZE + 1)];
	int16_t isBuildingPlace[(XSIZE + 1) * (ZSIZE + 1)];

	unsigned int roomId;
	char roomName[MAX_ROOM_NAME_SIZE];

	/// <summary>
	/// �� ���� �ʱ�ȭ
	/// </summary>
	void InitVariables();
	void RemoveAiAll();

public:
	Room(unsigned int id, char* name);
	~Room();

	bool CanStartGame();
	bool IsGameStarted() { return isGameStarted; }
	void StartGame() { isGameStarted = true; }
	void FinishGame() { isGameStarted = false; }
	bool IsMeshReady() { return meshReady; }
	bool IsRoadReady() { return roadReady; }
	unsigned int GetID() { return roomId; }
	char* GetRoomName() { return roomName; }
	unsigned int GetPlayerNumber();
	void CheckGameFinish();

	/// <summary>
	/// ���� ���� �� ���Ƿ� �̵��ϵ��� �ϴ� �ڵ�
	///  <para>- ���Ƿ� �̵��ϴ��� �� �ʱ�ȭ�� ���ϵ��� �Ǿ�����</para>
	/// </summary>
	void ToGameRoom();

	void SetAIPosition(int aiId, Vector3 pos, Vector3 rot);

	/// <summary>
	/// �濡 �÷��̾� �߰�
	/// </summary>
	/// <param name="user">�߰��� �÷��̾�</param>
	/// <returns>true �� �߰� ����, false �� �߰� �ȵ�(�ο��� ����)</returns>
	bool AddUser(Client* user);
	/// <summary>
	/// �濡�� �÷��̾� ����
	/// </summary>
	/// <param name="user">������ �÷��̾�</param>
	/// <returns>true �� �濡 �÷��̾� ����, false�� �濡 �÷��̾� ����</returns>
	bool RemoveUser(Client* user);

	/// <summary>
	/// �÷��̾� �̵� ���� ���� ��Ŷ ����
	/// <para>AI �����ӿ� ���� ��ġ ������ ����</para>
	/// </summary>
	/// <returns>�÷��̾� ��ġ ���� ��ü</returns>
	Packet_Move_SC* GetMovePacket();

	/// <summary>
	/// AI �߰�
	/// </summary>
	/// <returns>�߰��� AI �� �÷��̾� �ε���(�濡���� ����, 1~4)</returns>
	int AddAI();
	/// <summary>
	/// AI �÷��̾� ����
	/// </summary>
	int RemoveAI();

	/// <summary>
	/// ���� �ٸ� �÷��̾�� �޽��� ����
	/// </summary>
	/// <param name="sendUser">�����ϰ� ���� client ��ü, nullptr �̸� ���� ���� ��ο��� ����</param>
	/// <param name="msg">������ �޽���</param>
	/// <param name="packetSize">�޽��� ũ��</param>
	void SendMessageToOtherPlayers(Client* sendUser, char* msg, int packetSize);

	bool Execute_Cs_Mesh(Packet_Request_Mesh_SC* scPacket);
	Packet_Set_Mesh_SC* Execute_Cs_Set_Mesh(Packet_Set_Mesh* packet);
	bool Execute_Cs_Road(Packet_Request_Road_SC* scPacket);
	Packet_Set_Road_SC* Execute_Cs_Set_Road(Packet_Set_Road* packet);

	/// <summary>
	/// �÷��̾� ��ü ��ȯ
	/// </summary>
	/// <param name="playerIndex">ã�� �÷��̾��� �ε���</param>
	/// <returns>�÷��̾� ��ü, �ε����� ��ġ�ϴ� �÷��̾ ������ nullptr</returns>
	Client* GetIndexedPlayer(int playerIndex);

	/// <summary>
	/// �÷��̾� ���� �޽��� ����(���� ���� ������� �г���)
	/// </summary>
	/// <returns>�÷��̾� ���� ��ü</returns>
	sc_packet_room_player* GetPlayerInfo();

	time_t gameStartedAt = 0;
	time_t gameFinishedAt = 0;
};
#endif