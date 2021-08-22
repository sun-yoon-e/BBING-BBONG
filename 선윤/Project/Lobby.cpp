#include "Lobby.h"

Lobby::Lobby()
{
	userSem = CreateSemaphore(NULL, 1, 1, NULL);
	roomSem = CreateSemaphore(NULL, 1, 1, NULL);

	for (auto i = MAX_ROOM_NUMBER; i > 0; i--)
	{
		roomIdStack.push_back(i);
	}
}

Lobby::~Lobby()
{
	CloseHandle(userSem);
	CloseHandle(roomSem);
}

void Lobby::AddUser(Client* client)
{
	WaitForSingleObject(userSem, 0xffff);

	InsertIfNotExist(lobbyClients, client->GetID(), client);

	ReleaseSemaphore(userSem, 1, NULL);
}

void Lobby::RemoveUser(Client* client)
{
	WaitForSingleObject(userSem, 0xffff);

	RemoveIfExist(lobbyClients, client->GetID())

	ReleaseSemaphore(userSem, 1, NULL);
}

void Lobby::SendMessageToOtherPlayers(Client* sendUser, char* msg, int packetSize)
{
	WaitForSingleObject(userSem, 0xffff);
	for (const auto& kv : lobbyClients)
	{
		if (sendUser != nullptr && kv.first == sendUser->GetID())
		{
			continue;
		}

		if(kv.second->GetRoom() != nullptr)
		{
			send(kv.second->GetSocket(), msg, packetSize, 0);
		}
	}
	ReleaseSemaphore(userSem, 1, NULL);
}


int Lobby::MakeRoom(char* roomName)
{
	WaitForSingleObject(roomSem, 0xffff);
	auto rtn = 0l;

	if (!roomIdStack.empty())
	{
		rtn = roomIdStack.back();
		roomIdStack.pop_back();
		InsertIfNotExist(rooms, rtn, new Room(rtn, roomName));
	}
	
	ReleaseSemaphore(roomSem, 1, NULL);

	return rtn;
}

int Lobby::DestroyRoom(int roomId)
{
	WaitForSingleObject(roomSem, 0xffff);
	int rtn = -1;

	auto iter = rooms.find(roomId);
	if (iter != rooms.end())
	{
		roomIdStack.push_back(roomId);
		sort(roomIdStack.begin(), roomIdStack.end(), greater<int>());
		auto* room = rooms[roomId];
		rooms.erase(roomId);
		delete room;
		rtn = roomId;
	}
	ReleaseSemaphore(roomSem, 1, NULL);
	return rtn;
}

bool Lobby::EnterToRoom(int roomId, Client* user)
{
	bool rtn = false;

	WaitForSingleObject(roomSem, 0xffff);

	auto iter = rooms.find(roomId);
	if (iter != rooms.end())
	{
		rtn = iter->second->AddUser(user);
		if (rtn)
		{
			user->SetRoom(iter->second);
		}
	}

	ReleaseSemaphore(roomSem, 1, NULL);

	return rtn;
}

bool Lobby::ExitFromRoom(int roomId, Client* user)
{
	bool rtn = false;

	WaitForSingleObject(roomSem, 0xffff);

	auto iter = rooms.find(roomId);
	if (iter != rooms.end())
	{
		rtn = iter->second->RemoveUser(user);
		if (rtn)
		{
			user->SetRoom(nullptr);
		}
	}

	ReleaseSemaphore(roomSem, 1, NULL);

	return rtn;
}

void Lobby::GetRoomListInfoPacket(BYTE page, sc_packet_room_info* packet)
{
	auto startIdx = page * MAX_ROOM_LIST_PER_PAGE;
	auto lastIdx = startIdx + MAX_ROOM_LIST_PER_PAGE;
	auto currIdx = 0;
	auto roomIdx = 0;
	WaitForSingleObject(roomSem, 0xffff);

	for (const auto& elem : rooms)
	{
		currIdx++;

		if (currIdx > startIdx)
		{
			memcpy_s(packet[roomIdx].roomName, MAX_ROOM_NAME_SIZE, elem.second->GetRoomName(), MAX_ROOM_NAME_SIZE);
			packet[roomIdx].roomId = elem.second->GetID();
			packet[roomIdx].playerNum = elem.second->GetPlayerNumber();
			packet[roomIdx].inPlaying = elem.second->IsGameStarted();
			roomIdx++;
		}

		if (currIdx >= lastIdx)
		{
			break;
		}
	}
	ReleaseSemaphore(roomSem, 1, NULL);
}

sc_packet_room_info* Lobby::GetRoomInfoPacket(int roomId)
{
	WaitForSingleObject(roomSem, 0xffff);

	auto* packet = new sc_packet_room_info();

	auto iter = rooms.find(roomId);
	if (iter != rooms.end())
	{
		packet->roomId = iter->second->GetID();
		packet->playerNum = iter->second->GetPlayerNumber();
		packet->inPlaying = iter->second->IsGameStarted();
		memcpy_s(packet->roomName, MAX_ROOM_NAME_SIZE, iter->second->GetRoomName(), MAX_ROOM_NAME_SIZE);
	}

	ReleaseSemaphore(roomSem, 1, NULL);

	return packet;
}

void Lobby::CheckGameFinish()
{
	WaitForSingleObject(roomSem, 0xffff);

	for (const auto& room : rooms)
	{
		room.second->CheckGameFinish();
	}

	ReleaseSemaphore(roomSem, 1, NULL);
}

void Lobby::ToGameRoom()
{
	WaitForSingleObject(roomSem, 0xffff);

	for (const auto& room : rooms)
	{
		room.second->ToGameRoom();
	}

	ReleaseSemaphore(roomSem, 1, NULL);
}