#pragma once
#include "stdafx.h"

struct xyz {
	float x;
	float y;
	float z;
};

class Client
{
private:
	int			m_id;
	SOCKET		m_client_socket;

	string		m_NICK;
	string		m_PW;

	xyz			m_client_pos;
	int			m_score;

public:
	Client(SOCKET socket, int id);
	~Client();

	int GetID() { return m_id; }
	SOCKET GetSocket() { return m_client_socket; }
	string GetNick() { return m_NICK; }
	string GetPW() { return m_PW; }
	xyz GetPos() { return m_client_pos; }
	int GetScore() { return m_score; }

	void SetID(int id) { m_id = id; }
	void SetSocket(SOCKET socket) { m_client_socket = socket; }
	void SetNick(string nick) { m_NICK = nick; }
	void SetPW(string pw) { m_PW = pw; }
	void SetPos(float x, float y, float z) { m_client_pos.x = x, m_client_pos.y = y, m_client_pos.z = z; }
	void SetScore(int score) { m_score = score; }
};