#pragma once
#include "stdafx.h"
#include "protocol.h"

class Client
{
private:
	int			m_id;
	SOCKET		m_client_socket;

	string		m_NICK;
	string		m_PW;

	Vector3		m_client_pos {0.0, 0.0, 0.0};
	Vector3		m_client_rot {0.0, 0.0, 0.0};
	int			m_score = 0;

public:
	Client(SOCKET socket, int id);
	~Client();

	int GetID() { return m_id; }
	SOCKET GetSocket() { return m_client_socket; }
	string GetNick() { return m_NICK; }
	string GetPW() { return m_PW; }
	Vector3 GetPos() { return m_client_pos; }
	Vector3 GetRot() { return m_client_rot; }
	int GetScore() { return m_score; }

	void SetID(int id) { m_id = id; }
	void SetSocket(SOCKET socket) { m_client_socket = socket; }
	void SetNick(string nick) { m_NICK = nick; }
	void SetPW(string pw) { m_PW = pw; }
	void SetPos(float x, float y, float z) { m_client_pos.x = x, m_client_pos.y = y, m_client_pos.z = z; }
	void SetRot(float x, float y, float z) { m_client_rot.x = x, m_client_rot.y = y, m_client_rot.z = z; }
	void SetScore(int score) { m_score = score; }
};