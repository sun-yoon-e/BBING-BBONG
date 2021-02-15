#pragma once
#include "stdafx.h"

struct xyz {
	float x;
	float y;
	float z;
};

struct Client
{
	bool		m_connect;

	int			m_id;
	SOCKET		m_client_socket;

	string		m_NICK;
	string		m_PW;
	
	xyz			m_client_pos;
	int			m_score;
};

extern Client game_clients[MAX_CLIENT];