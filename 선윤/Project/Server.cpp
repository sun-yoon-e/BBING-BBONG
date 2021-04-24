#include "stdafx.h"
#include "Server.h"
#include "Client.h"

Client Server::game_clients[MAX_CLIENT];

Server::Server()
{
	std::wcout.imbue(std::locale("korean"));

	InitServer();
	StartServer();
}

Server::~Server()
{

}

void Server::InitServer()
{
	std::wcout.imbue(std::locale("korean"));	// 한글

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
				game_clients[i].SetConnect(true);
				new_client_id = i;
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
		game_clients[new_client_id].SetID(new_client_id);
		game_clients[new_client_id].SetConnect(true);
		game_clients[new_client_id].SetSocket(new_client_socket);
		/*game_clients[new_client_id].SetNick("NONE");
		game_clients[new_client_id].SetPW("NONE");
		game_clients[new_client_id].SetPos(0, 0, 0);
		game_clients[new_client_id].SetScore(0);*/
		Send_Enter_Packet(new_client_id);

		LoginServer(new_client_id);

		// 스레드
		/*client_thread[new_client_id] = CreateThread(NULL, 0, this->LobbyServer, (LPVOID)game_clients[new_client_id].GetID(), 0, NULL);
		if (client_thread[new_client_id] == NULL) closesocket(game_clients[new_client_id].GetSocket());
		else CloseHandle(client_thread[new_client_id]);*/
	}
}

void Server::LoginServer(int id)
{
	// 아이디 패스워드 클라이언트와 패킷 주고받기
	cs_packet_login_nk login_packet_nk;
	ZeroMemory(&login_packet_nk, sizeof(cs_packet_login_nk));
	recv(game_clients[id].GetSocket(), (char*)&login_packet_nk, sizeof(cs_packet_login_nk), 0);
	cout << "id : " << login_packet_nk.id << ", nick : " << login_packet_nk.nick << endl;

	cs_packet_login_pw login_packet_pw;
	ZeroMemory(&login_packet_pw, sizeof(cs_packet_login_pw));
	recv(game_clients[id].GetSocket(), (char*)&login_packet_pw, sizeof(cs_packet_login_pw), 0);
	cout << "id : " << login_packet_pw.id << ", pw : " << login_packet_pw.pw << endl;

	// 추가할 것 : DB서버 연동해서 정보 확인하기

	// 로그인 완료하면 빠져나가기
}

void Server::Send_Enter_Packet(int id)
{
	sc_packet_enter enter_packet;
	ZeroMemory(&enter_packet, sizeof(sc_packet_enter));

	enter_packet.id = id;
	enter_packet.type = SC_ENTER;
	enter_packet.size = sizeof(sc_packet_enter);

	int ret = send(game_clients[id].GetSocket(), (char*)&enter_packet, sizeof(sc_packet_enter), 0);
	if (ret == SOCKET_ERROR) {
		int err_no = WSAGetLastError();
		if (ERROR_IO_PENDING != err_no)
			err_display("Send_Enter_Packet() -> send()", err_no);
	}
	//cout << "send_enter_packet OK" << endl;
}

DWORD __stdcall Server::LobbyServer(LPVOID arg)
{
	int id = reinterpret_cast<int>(arg);

	SOCKADDR_IN addr;
	int addrlen = sizeof(addr);
	getpeername(game_clients[id].GetSocket(), (SOCKADDR*)&addr, &addrlen);

	bool Login = false;
	while (!Login) {

	}

	return 0;
}
