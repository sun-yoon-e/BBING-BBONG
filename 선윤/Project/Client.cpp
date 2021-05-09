#include "Client.h"

Client::Client(SOCKET socket, int id)
{
	this->m_client_socket = socket;
	this->m_id = id;
}

Client::~Client()
{

}