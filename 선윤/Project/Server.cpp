#include "stdafx.h"
#include "Server.h"
#include "Client.h"

Server::Server()
{
	std::wcout.imbue(std::locale("korean"));

	InitServer();
	StartServer();
	//CreateThread();
}

Server::~Server()
{

}

void Server::InitServer()
{
	std::wcout.imbue(std::locale("korean"));

	WSADATA wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0) return;

	server_socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (server_socket == INVALID_SOCKET) err_quit("InitServer() -> socket()");

	SOCKADDR_IN server_addr;
	ZeroMemory(&server_addr, sizeof(SOCKADDR_IN));
	server_addr.sin_family = AF_INET;
	server_addr.sin_addr.s_addr = htonl(INADDR_ANY);
	server_addr.sin_port = htons(SERVERPORT);
	int ret = bind(server_socket, (SOCKADDR*)&server_addr, sizeof(server_addr));
	if (ret == SOCKET_ERROR) err_quit("InitServer() -> bind()");

	ret = listen(server_socket, 5);
	if (ret == SOCKET_ERROR) {
		int err_no = WSAGetLastError();
		if (ERROR_IO_PENDING != err_no)
			err_display("InitServer() -> listen()", err_no);
	}

	// 네이글 알고리즘
	int opt = TRUE;
	setsockopt(server_socket, IPPROTO_TCP, TCP_NODELAY, (const char*)&opt, sizeof(opt));

	for (int i = 0; i < MAX_CLIENT; ++i)
		game_clients[i].SetConnect(false);

	std::cout << "Server Init OK!" << std::endl;
}

void Server::StartServer()
{
	SOCKADDR_IN client_addr;
	ZeroMemory(&client_addr, sizeof(SOCKADDR_IN));
	client_addr.sin_family = AF_INET;
	client_addr.sin_addr.s_addr = htonl(INADDR_ANY);
	client_addr.sin_port = htons(SERVERPORT);
	int addrlen = sizeof(client_addr);

	while (true) {
		SOCKET new_client_socket = accept(server_socket, (SOCKADDR*)&client_addr, &addrlen);
		if (new_client_socket == INVALID_SOCKET) {
			int err_no = WSAGetLastError();
			err_display("StartServer() -> accept()", err_no);
		}

		int new_client_id = -1;
		for (int i = 0; i < MAX_CLIENT; ++i) {
			if (game_clients[i].GetConnect() == false) {
#if (Debug == TRUE)
				game_clients[i].SetConnect(true);
				new_client_id = i;
				cout << "New Client : " << new_client_id << endl;
#endif
				break;
			}
		}

		if (new_client_id == -1) {
#if (Debug == TRUE)
			cout << "Max Client!" << endl;
#endif
			closesocket(new_client_socket);
			continue;
		}

		// InitClient
		game_clients[new_client_id].SetID(new_client_id);
		game_clients[new_client_id].SetConnect(true);
		game_clients[new_client_id].SetSocket(new_client_socket);
		game_clients[new_client_id].SetNick("NONE");
		game_clients[new_client_id].SetPW("NONE");
		game_clients[new_client_id].SetPos(1, 2, 3);
		game_clients[new_client_id].SetScore(10);
		Send_ID(new_client_id);

		LoginServer();
	}
}

void Server::Send_ID(int id)
{
	send(game_clients[id].GetSocket(), (char*)&id, sizeof(int), 0);
}

void Server::LoginServer()
{

}