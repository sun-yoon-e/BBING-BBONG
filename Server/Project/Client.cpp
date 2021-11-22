#include "Client.h"

Client::Client(SOCKET socket, int id)
{
	this->m_client_socket = socket;
	this->m_id = id;
	this->m_participatedRoom = nullptr;
	this->m_roomPlayerIndex = 0;
	
	ZeroMemory(m_NickPtr, MAX_NICKNAME_SIZE);
}

Client::~Client()
{

}

void Client::SetNick(string nick) 
{
	m_NICK = nick; 
	auto len = nick.size() < MAX_NICKNAME_SIZE ? nick.size() : MAX_NICKNAME_SIZE;
	memcpy_s(m_NickPtr, MAX_NICKNAME_SIZE, nick.c_str(), len);
}