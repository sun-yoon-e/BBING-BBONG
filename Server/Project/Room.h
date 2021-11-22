#ifndef ROOM_H
#define ROOM_H

#include <map>
#include "Client.h"

using namespace std;

/// <summary>
/// AI 플레이어 위치 및 방향 정보
/// </summary>
struct AI
{
	int id;
	Vector3		m_client_pos{ 0.0, 0.0, 0.0 };
	Vector3		m_client_rot{ 0.0, 0.0, 0.0 };
};

/// <summary>
/// 게임 대기 및 플레이 진행하는 방
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
	/// 맵 정보 초기화
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
	/// 게임 종료 후 대기실로 이동하도록 하는 코드
	///  <para>- 대기실로 이동하더라도 맵 초기화는 안하도록 되어있음</para>
	/// </summary>
	void ToGameRoom();

	void SetAIPosition(int aiId, Vector3 pos, Vector3 rot);

	/// <summary>
	/// 방에 플레이어 추가
	/// </summary>
	/// <param name="user">추가할 플레이어</param>
	/// <returns>true 면 추가 성공, false 면 추가 안됨(인원수 제한)</returns>
	bool AddUser(Client* user);
	/// <summary>
	/// 방에서 플레이어 제외
	/// </summary>
	/// <param name="user">제외할 플레이어</param>
	/// <returns>true 면 방에 플레이어 없음, false면 방에 플레이어 있음</returns>
	bool RemoveUser(Client* user);

	/// <summary>
	/// 플레이어 이동 정보 담은 패킷 생성
	/// <para>AI 움직임에 대한 위치 정보는 없음</para>
	/// </summary>
	/// <returns>플레이어 위치 정보 객체</returns>
	Packet_Move_SC* GetMovePacket();

	/// <summary>
	/// AI 추가
	/// </summary>
	/// <returns>추가한 AI 의 플레이어 인덱스(방에서의 순서, 1~4)</returns>
	int AddAI();
	/// <summary>
	/// AI 플레이어 삭제
	/// </summary>
	int RemoveAI();

	/// <summary>
	/// 방의 다른 플레이어에게 메시지 전달
	/// </summary>
	/// <param name="sendUser">제외하고 보낼 client 객체, nullptr 이면 본인 포함 모두에게 전달</param>
	/// <param name="msg">전달할 메시지</param>
	/// <param name="packetSize">메시지 크기</param>
	void SendMessageToOtherPlayers(Client* sendUser, char* msg, int packetSize);

	bool Execute_Cs_Mesh(Packet_Request_Mesh_SC* scPacket);
	Packet_Set_Mesh_SC* Execute_Cs_Set_Mesh(Packet_Set_Mesh* packet);
	bool Execute_Cs_Road(Packet_Request_Road_SC* scPacket);
	Packet_Set_Road_SC* Execute_Cs_Set_Road(Packet_Set_Road* packet);

	/// <summary>
	/// 플레이어 객체 반환
	/// </summary>
	/// <param name="playerIndex">찾을 플레이어의 인덱스</param>
	/// <returns>플레이어 객체, 인덱스와 일치하는 플레이어가 없으면 nullptr</returns>
	Client* GetIndexedPlayer(int playerIndex);

	/// <summary>
	/// 플레이어 정보 메시지 생성(대기실 입장 순서대로 닉네임)
	/// </summary>
	/// <returns>플레이어 정보 객체</returns>
	sc_packet_room_player* GetPlayerInfo();

	time_t gameStartedAt = 0;
	time_t gameFinishedAt = 0;
};
#endif