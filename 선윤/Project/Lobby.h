#ifndef LOBBY_H
#define LOBBY_H

#include <map>
#include "Client.h"
#include "Room.h"

using namespace std;

/// <summary>
/// �κ� ���� Ŭ����
/// </summary>
/// <remarks>
/// ��ü life cycle ���� ����
/// </remakrs>
class Lobby
{
private:
	map<int, Client*> lobbyClients;
	map<int, Room*> rooms;

	HANDLE userSem;
	HANDLE roomSem;
	vector<int> roomIdStack;

public:
	Lobby();
	~Lobby();

	/// <summary>
	/// �κ� ����� �߰�
	/// </summary>
	/// <param name="user">�߰��� �κ� �����</param>
	void AddUser(Client* user);
	/// <summary>
	/// �κ񿡼� ����� ����
	/// </summary>
	/// <param name="user">������ �κ� �����</param>
	void RemoveUser(Client* user);

	void SendMessageToOtherPlayers(Client* sendUser, char* msg, int packetSize);

	/// <summary>
	/// ���ǵ��� ���� ���� Ȯ�� - �ð� ������ ���� ���� ��Ű�� ����
	/// </summary>
	void CheckGameFinish();
	/// <summary>
	/// ���� ���� �� ���� �ð��� ������ ���Ƿ� ������ �޽��� ����
	/// </summary>
	void ToGameRoom();

	/// <summary>
	/// ���� ����
	/// </summary>
	/// <param name="roomName">������ ���� �̸�</param>
	/// <returns>���� ���̵�, �����ϸ� 0</returns>
	int MakeRoom(char* roomName);
	/// <summary>
	/// ���� ����
	/// </summary>
	/// <param name="roomId">������ ���� ���̵�</param>
	/// <returns>������ ���� ���̵�</returns>
	int DestroyRoom(int roomId);
	/// <summary>
	/// ���ǿ� ����
	/// </summary>
	/// <param name="roomId">������ ���� ���̵�</param>
	/// <param name="user">������ �÷��̾�</param>
	/// <returns>���� ����, ����</returns>
	bool EnterToRoom(int roomId, Client* user);
	/// <summary>
	/// ���ǿ��� ����
	/// </summary>
	/// <param name="roomId">������ ���� ���̵�</param>
	/// <param name="user">������ �÷��̾�</param>
	/// <returns>false �� ���������� ����</returns>
	bool ExitFromRoom(int roomId, Client* user);

	/// <summary>
	/// ���� ����Ʈ ���� ���� ��Ŷ ����
	/// </summary>
	/// <param name="page">���� ������ �о�� ������(1�������� 6����)</param>
	/// <param name="packet">������ ���� ���� ��Ŷ</param>
	void GetRoomListInfoPacket(BYTE page, sc_packet_room_info* packet);

	/// <summary>
	/// ���� ���� ���� ��Ŷ ����
	/// </summary>
	/// <param name="roomId">������ ������ ���� ���̵�</param>
	/// <returns>���� ���� ��ü</returns>
	sc_packet_room_info* GetRoomInfoPacket(int roomId);
};

#endif
