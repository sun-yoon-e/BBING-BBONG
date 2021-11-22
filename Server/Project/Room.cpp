#include "Room.h"

Room::Room(unsigned int id, char* name)
{
	InitVariables();

	clientSem = CreateSemaphore(NULL, 1, 1, NULL);
	roomId = id;
	memcpy_s(roomName, MAX_ROOM_NAME_SIZE, name, MAX_ROOM_NAME_SIZE);

	for (int i = MAX_PLAYER_NUMBER; i > 0; i--)
	{
		playerIndex.push(i);
	}
}

Room::~Room()
{
	CloseHandle(clientSem);
}

void Room::InitVariables()
{
	ZeroMemory(meshVertices, sizeof(meshVertices));
	ZeroMemory(meshTriangles, sizeof(meshTriangles));

	ZeroMemory(roadVertices, sizeof(roadVertices));
	ZeroMemory(roadTriangles, sizeof(roadTriangles));

	ZeroMemory(isRoad, sizeof(isRoad));
	ZeroMemory(isBuildingPlace, sizeof(isBuildingPlace));

	meshReady = false;
	roadReady = false;
}

bool Room::AddUser(Client* user)
{
	auto isSuccess = false;
	WaitForSingleObject(clientSem, 0xffff);

	if (!playerIndex.empty() && !isGameStarted)
	{
		InsertIfNotExist(players, user->GetID(), user);
		user->SetRoom(this);
		if (user->GetRoomPlayerIndex()== 0)
		{
			auto idx = playerIndex.top();
			playerIndex.pop();
			user->SetRoomPlayerIndex(idx);
		}
		isSuccess = true;
	}
	else
	{
		user->SetRoomPlayerIndex(0);
	}

	ReleaseSemaphore(clientSem, 1, NULL);

	return isSuccess;
}

bool Room::RemoveUser(Client* user)
{
	WaitForSingleObject(clientSem, 0xffff);

	RemoveIfExist(players, user->GetID());
	user->SetRoom(nullptr);

	if (user->GetRoomPlayerIndex() > 0)
	{
		auto idx = user->GetRoomPlayerIndex();
		user->SetRoomPlayerIndex(0);
		playerIndex.push(idx);
	}
	
	auto leftPlayerNumber = players.size();

	ReleaseSemaphore(clientSem, 1, NULL);
	return leftPlayerNumber == 0;
}

void Room::SendMessageToOtherPlayers(Client* sendUser, char* msg, int packetSize)
{
	WaitForSingleObject(clientSem, 0xffff);
	for (const auto& kv : players)
	{
		if (sendUser != nullptr && kv.second->GetID() == sendUser->GetID())
		{
			continue;
		}
		send(kv.second->GetSocket(), msg, packetSize, 0);
	}
	ReleaseSemaphore(clientSem, 1, NULL);
}

bool Room::Execute_Cs_Mesh(Packet_Request_Mesh_SC* scPacket)
{
	scPacket->type = SC_MESH;
	scPacket->ready = meshReady;
	memcpy(scPacket->vertices, meshVertices, sizeof(meshVertices));
	memcpy(scPacket->triangles, meshTriangles, sizeof(meshTriangles));
	return meshReady;
}

Packet_Set_Mesh_SC* Room::Execute_Cs_Set_Mesh(Packet_Set_Mesh* packet)
{
	meshReady = true;
	memcpy(meshVertices, packet->vertices, sizeof(meshVertices));
	memcpy(meshTriangles, packet->triangles, sizeof(meshTriangles));

	auto* ppp = new BYTE[MAX_PACKET_SIZE];
	Packet_Set_Mesh_SC* scPacket = reinterpret_cast<Packet_Set_Mesh_SC*>(ppp);
	scPacket->type = SC_SET_MESH;
	scPacket->ready = meshReady;
	memcpy(scPacket->vertices, meshVertices, sizeof(meshVertices));
	memcpy(scPacket->triangles, meshTriangles, sizeof(meshTriangles));

	return scPacket;
}

bool Room::Execute_Cs_Road(Packet_Request_Road_SC* scPacket)
{
	scPacket->type = SC_ROAD;
	scPacket->ready = roadReady;
	memcpy(scPacket->vertices, roadVertices, sizeof(roadVertices));
	memcpy(scPacket->triangles, roadTriangles, sizeof(roadTriangles));
	memcpy(scPacket->isRoad, isRoad, sizeof(isRoad));
	memcpy(scPacket->isBuildingPlace, isBuildingPlace, sizeof(isBuildingPlace));

	return roadReady;
}

Packet_Set_Road_SC* Room::Execute_Cs_Set_Road(Packet_Set_Road* packet)
{
	roadReady = true;

	memcpy(roadVertices, packet->vertices, sizeof(roadVertices));
	memcpy(roadTriangles, packet->triangles, sizeof(roadTriangles));
	memcpy(isRoad, packet->isRoad, sizeof(isRoad));
	memcpy(isBuildingPlace, packet->isBuildingPlace, sizeof(isBuildingPlace));

	auto* ppp = new BYTE[MAX_PACKET_SIZE];
	Packet_Set_Road_SC* scPacket = reinterpret_cast<Packet_Set_Road_SC*>(ppp);
	scPacket->type = SC_SET_ROAD;
	scPacket->ready = roadReady;
	memcpy(scPacket->vertices, roadVertices, sizeof(roadVertices));
	memcpy(scPacket->triangles, roadTriangles, sizeof(roadTriangles));
	memcpy(scPacket->isRoad, isRoad, sizeof(isRoad));
	memcpy(scPacket->isBuildingPlace, isBuildingPlace, sizeof(isBuildingPlace));

	return scPacket;
}

bool Room::CanStartGame()
{
	if (currentPlayerNumber < MAX_PLAYER_NUMBER)
	{
		return false;
	}

	bool flag = true;
	for (const auto& player : players)
	{
		if (player.second->GetNick() == "")
		{
			flag = false;
			break;
		}
	}

	return flag;
}

unsigned int Room::GetPlayerNumber()
{
	return MAX_PLAYER_NUMBER - playerIndex.size();
}

sc_packet_room_player* Room::GetPlayerInfo()
{
	sc_packet_room_player* rtn = new sc_packet_room_player();

	for (const auto& player : players)
	{
		auto idx = player.second->GetRoomPlayerIndex();
		if (idx > 0)
		{
			memcpy_s(rtn->nickName + ((idx - 1) * MAX_NICKNAME_SIZE), MAX_NICKNAME_SIZE, player.second->GetNickPtr(), MAX_NICKNAME_SIZE);
		}
	}

	for (const auto& ai : aiPlayers)
	{
		auto idx = ai.id;
		if (idx > 0)
		{
			auto name = string("AI(") + std::to_string(idx) + string(")");			
			memcpy_s(rtn->nickName + ((idx - 1) * MAX_NICKNAME_SIZE), MAX_NICKNAME_SIZE, name.c_str(), name.size());
		}
	}

	return rtn;
}

void Room::CheckGameFinish()
{
#ifdef TEST
	if (isGameStarted && (time(NULL) - gameStartedAt >= 20)) {
#else
	if (isGameStarted && (time(NULL) - gameStartedAt >= 300 + 5)) {	// 딜레이 추가 gameTimer.cs
#endif
		// 게임 종료 관련 추가 수정 필요함
		cout << "-- END GAME --" << endl;
		FinishGame();
		gameFinishedAt = time(NULL);

		BYTE* buffer = new BYTE[OTHER_PACKET_SIZE_MAX];
		Packet_GameState_SC* packet = new Packet_GameState_SC();
		packet->state = 0;

		ZeroMemory(buffer, OTHER_PACKET_SIZE_MAX);
		memcpy_s(buffer, OTHER_PACKET_SIZE_MAX, (char*)packet, sizeof(Packet_GameState_SC));

		SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(buffer), OTHER_PACKET_SIZE_MAX);
		delete[] buffer;
		isCountdownStarted = true;
	}
}

void Room::ToGameRoom()
{
	if (isCountdownStarted && (time(NULL) - gameFinishedAt >= 10))	// 게임 종료 후 10초 대기시간
	{
		isCountdownStarted = false;
		BYTE* buffer = new BYTE[OTHER_PACKET_SIZE_MAX];
		Packet_GameInit_SC* packet = new Packet_GameInit_SC();
		packet->scene = 2;

		ZeroMemory(buffer, OTHER_PACKET_SIZE_MAX);
		memcpy_s(buffer, OTHER_PACKET_SIZE_MAX, (char*)packet, sizeof(Packet_GameState_SC));

		SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(buffer), OTHER_PACKET_SIZE_MAX);
		delete packet;

		// 맵을 초기화 하려면 아래 주석 해제
		InitVariables();
		RemoveAiAll();
	}
}


Client* Room::GetIndexedPlayer(int playerIndex)
{
	Client* ptr = nullptr;

	for (const auto& player : players)
	{
		if (playerIndex == player.second->GetRoomPlayerIndex())
		{
			ptr = player.second;
			break;
		}
	}

	return ptr;
}


int Room::AddAI()
{
	WaitForSingleObject(clientSem, 0xffff);
	auto idx = 0l;

	if (playerIndex.size() > 0)
	{
		idx = playerIndex.top();
		playerIndex.pop();
		AI ai;
		ai.id = idx;
		aiPlayers.push_back(ai);
	}

	ReleaseSemaphore(clientSem, 1, NULL);

	return idx;
}

int Room::RemoveAI()
{
	WaitForSingleObject(clientSem, 0xffff);
	auto id = 0l;
	if (aiPlayers.size() > 0)
	{
		auto ai = aiPlayers.back();
		playerIndex.push(ai.id);
		id = ai.id;
		aiPlayers.pop_back();
	}

	ReleaseSemaphore(clientSem, 1, NULL);

	return id;
}

void Room::RemoveAiAll()
{
	WaitForSingleObject(clientSem, 0xffff);


	ReleaseSemaphore(clientSem, 1, NULL);
}

Packet_Move_SC* Room::GetMovePacket()
{
	Packet_Move_SC* scPacket = new Packet_Move_SC;
	memset(scPacket->position, 0x00, sizeof(scPacket->position));
	memset(scPacket->rotation, 0x00, sizeof(scPacket->rotation));

	/*scPacket->players = GetPlayerNumber() + aiPlayers.size();*/
	scPacket->players = 4;
	int count = 0;
	for (const auto& player : players)
	{
		scPacket->position[count] = player.second->GetPos();
		scPacket->rotation[count] = player.second->GetRot();
		count++;
	}

	//for (const auto& ai : aiPlayers)
	//{
	//	auto idx = ai.id - 1;
	//	if (idx >= 0 && idx < MAX_PLAYER_NUMBER)
	//	{
	//		scPacket->position[idx] = ai.m_client_pos;
	//		scPacket->rotation[idx] = ai.m_client_rot;
	//	}
	//}

	return scPacket;
}

void Room::SetAIPosition(int aiId, Vector3 pos, Vector3 rot)
{
	for (auto& ai : aiPlayers)
	{
		if (ai.id == aiId)
		{
			ai.m_client_pos = pos;
			ai.m_client_rot = rot;
			break;
		}
	}
}