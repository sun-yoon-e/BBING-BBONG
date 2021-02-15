#include "stdafx.h"
#include "Server.h"
#include "Client.h"

SOCKET server_socket;
Client game_clients[MAX_CLIENT];

int main()
{
	InitServer();

	vector<thread> worker_threads;
	for (int i = 0; i < 6; ++i)
		worker_threads.emplace_back( Worker_Thread );

	thread accept_thread{ Accept_Thread };
	accept_thread.join();

	for (auto& p : worker_threads)
		p.join();

	closesocket(server_socket);
	WSACleanup();
}

void InitServer()
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
		game_clients[i].m_connect = false;

	std::cout << "Server Init OK!" << std::endl;
}

void Worker_Thread()
{
	while (true) {

	}
}

void Accept_Thread()
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
			err_display("Accept_Thread() -> accept()", err_no);
		}

		int new_client_id = -1;
		for (int i = 0; i < MAX_CLIENT; ++i) {
			if (game_clients[i].m_connect == false) {
#if (Debug == TRUE)
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
		game_clients[new_client_id].m_connect = true;
		game_clients[new_client_id].m_client_socket = new_client_socket;
		game_clients[new_client_id].m_NICK = "NONE";
		game_clients[new_client_id].m_PW = "NONE";
		game_clients[new_client_id].m_client_pos.x = 0;
		game_clients[new_client_id].m_client_pos.y = 0;
		game_clients[new_client_id].m_client_pos.z = 0;
		game_clients[new_client_id].m_score = 0;
		Send_ID(new_client_id);
	}
}

void Send_ID(int)
{

}
