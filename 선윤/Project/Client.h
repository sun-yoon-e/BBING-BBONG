#pragma once
#include "stdafx.h"
#include "protocol.h"

class Room;

class Client
{
private:
	int			m_id;
	int			m_roomPlayerIndex;
	SOCKET		m_client_socket;

	string		m_NICK;
	string		m_PW;

	Vector3		m_client_pos {0.0, 0.0, 0.0};
	Vector3		m_client_rot {0.0, 0.0, 0.0};
	int			m_score = 0;

	Room*		m_participatedRoom;
	BYTE		m_NickPtr[MAX_NICKNAME_SIZE];

public:
	Client(SOCKET socket, int id);
	~Client();

	int GetID() { return m_id; }
	SOCKET GetSocket() { return m_client_socket; }
	string GetNick() { return m_NICK; }
	BYTE* GetNickPtr() { return m_NickPtr; }
	string GetPW() { return m_PW; }
	Vector3 GetPos() { return m_client_pos; }
	Vector3 GetRot() { return m_client_rot; }
	int GetScore() { return m_score; }
	Room* GetRoom() { return m_participatedRoom; }

	/// <summary>
	/// 대기실에서 플레이어의 순서(1~4번) 반환
	/// </summary>
	/// <returns>대기실에서의 플레이어 순서</returns>
	int GetRoomPlayerIndex() { return m_roomPlayerIndex; }
	/// <summary>
	/// 플레이어의 대기실에서의 순서 설정(1~4번)
	/// </summary>
	/// <param name="roomPlayerIndex">대기실 순서</param>
	void SetRoomPlayerIndex(int roomPlayerIndex) { m_roomPlayerIndex = roomPlayerIndex; }

	void SetID(int id) { m_id = id; }
	void SetSocket(SOCKET socket) { m_client_socket = socket; }
	void SetNick(string nick);
	void SetPW(string pw) { m_PW = pw; }
	void SetPos(float x, float y, float z) { m_client_pos.x = x, m_client_pos.y = y, m_client_pos.z = z; }
	void SetRot(float x, float y, float z) { m_client_rot.x = x, m_client_rot.y = y, m_client_rot.z = z; }
	void SetScore(int score) { m_score = score; }

	/// <summary>
	/// 플레이어가 참여하고 있는 대기실 객체 설정
	/// </summary>
	/// <param name="room">플레이어가 들어가 있는 대기실 객체</param>
	void SetRoom(Room* room) { m_participatedRoom = room; }
};