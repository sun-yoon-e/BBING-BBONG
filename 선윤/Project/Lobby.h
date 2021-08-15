#ifndef LOBBY_H
#define LOBBY_H

#include <map>
#include "Client.h"
#include "Room.h"

using namespace std;

/// <summary>
/// 로비 관리 클래스
/// </summary>
/// <remarks>
/// 객체 life cycle 관리 안함
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
	/// 로비에 사용자 추가
	/// </summary>
	/// <param name="user">추가할 로비 사용자</param>
	void AddUser(Client* user);
	/// <summary>
	/// 로비에서 사용자 제거
	/// </summary>
	/// <param name="user">제거할 로비 사용자</param>
	void RemoveUser(Client* user);

	void SendMessageToOtherPlayers(Client* sendUser, char* msg, int packetSize);

	/// <summary>
	/// 대기실들의 게임 상태 확인 - 시간 지나면 게임 종료 시키기 위함
	/// </summary>
	void CheckGameFinish();
	/// <summary>
	/// 게임 종료 후 일정 시간이 지나면 대기실로 가도록 메시지 생성
	/// </summary>
	void ToGameRoom();

	/// <summary>
	/// 대기실 생성
	/// </summary>
	/// <param name="roomName">생성할 대기실 이름</param>
	/// <returns>대기실 아이디, 실패하면 0</returns>
	int MakeRoom(char* roomName);
	/// <summary>
	/// 대기실 삭제
	/// </summary>
	/// <param name="roomId">삭제할 대기실 아이디</param>
	/// <returns>삭제한 대기실 아이디</returns>
	int DestroyRoom(int roomId);
	/// <summary>
	/// 대기실에 입장
	/// </summary>
	/// <param name="roomId">입장할 대기실 아이디</param>
	/// <param name="user">입장한 플레이어</param>
	/// <returns>입장 성공, 실패</returns>
	bool EnterToRoom(int roomId, Client* user);
	/// <summary>
	/// 대기실에서 퇴장
	/// </summary>
	/// <param name="roomId">퇴장할 대기실 아이디</param>
	/// <param name="user">퇴장할 플레이어</param>
	/// <returns>false 면 마지막으로 퇴장</returns>
	bool ExitFromRoom(int roomId, Client* user);

	/// <summary>
	/// 대기실 리스트 정보 전달 패킷 생성
	/// </summary>
	/// <param name="page">대기실 정보를 읽어올 페이지(1페이지에 6개씩)</param>
	/// <param name="packet">생성한 대기실 정보 패킷</param>
	void GetRoomListInfoPacket(BYTE page, sc_packet_room_info* packet);

	/// <summary>
	/// 대기실 정보 전달 패킷 생성
	/// </summary>
	/// <param name="roomId">정보를 전달할 대기실 아이디</param>
	/// <returns>대기실 정보 객체</returns>
	sc_packet_room_info* GetRoomInfoPacket(int roomId);
};

#endif
