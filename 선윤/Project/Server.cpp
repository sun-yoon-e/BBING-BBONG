#include "stdafx.h"
#include "Server.h"
#include "Client.h"

Server::Server() :
	db(ODBC_NAME, DBUSER, DBPASSWORD)
{
}

Server::~Server()
{

}

void Server::InitServer()
{
	// UTF8 출력 설정
	// http://egloos.zum.com/Lusain/v/3182581
	SetConsoleOutputCP(CP_UTF8);

	// 소켓 초기화
	WSADATA wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0) return;

	server_socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (server_socket == INVALID_SOCKET) err_quit(L"InitServer() -> socket()");

	// bind - 몇번 포트 사용할지 정함
	SOCKADDR_IN server_addr;
	ZeroMemory(&server_addr, sizeof(SOCKADDR_IN));
	server_addr.sin_family = AF_INET;
	server_addr.sin_addr.s_addr = htonl(INADDR_ANY);
	server_addr.sin_port = htons(SERVERPORT);
	int ret = bind(server_socket, (SOCKADDR*)&server_addr, sizeof(server_addr));
	if (ret == SOCKET_ERROR) err_quit(L"InitServer() -> bind()");

	// listen - 클라이언트 접속 수신함
	ret = listen(server_socket, 5);
	if (ret == SOCKET_ERROR) {
		int err_no = WSAGetLastError();
		if (ERROR_IO_PENDING != err_no)
			err_display("InitServer() -> listen()", err_no);
	}

	// 네이글 알고리즘
	int opt = TRUE;
	setsockopt(server_socket, IPPROTO_TCP, TCP_NODELAY, (const char*)&opt, sizeof(opt));

	std::cout << "Connecting DB..." << std::endl;
	int dbretcode = db.connect();
	if (!(dbretcode == SQL_SUCCESS || dbretcode == SQL_SUCCESS_WITH_INFO)) {
		err_quit(L"DB Connection failure (" + to_wstring(dbretcode) + L")");
	}
	std::cout << "DB Connected!" << std::endl;

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
		// 클라이언트한테서 접속을 받는다
		SOCKET new_client_socket = accept(server_socket, (SOCKADDR*)&client_addr, &addrlen);
		if (new_client_socket == INVALID_SOCKET) {
			int err_no = WSAGetLastError();
			err_display("StartServer() -> accept()", err_no);
		}

		// 클라이언트 수가 많으면 접속을 거부한다
		if (clients.size() >= MAX_CLIENT) {
			err_display("Max Client!");
			closesocket(new_client_socket);
			continue;
		}

		
		// 새 클라이언트 ID = 카운터 + 1
		int new_client_id = ++client_id_counter;
		cout << "New Client : " << new_client_id << std::endl;

		// 클라이언트 객체 초기화하고 배열에 넣음
		Client* new_client = new Client(new_client_socket, new_client_id);
		clients.push_back(new_client);

		ThreadArgs* args = new ThreadArgs;
		args->server = this;
		args->client = new_client;

		HANDLE thread = CreateThread(NULL, 0, this->NewClientThread, (LPVOID)args, 0, NULL);
		if (thread == NULL) {
			closesocket(new_client_socket);
			continue;
		}
	}
}

void Server::ClientMain(Client* client) 
{
	SOCKET socket = client->GetSocket();
	BYTE buffer[MAX_PACKET_SIZE];

	while (TRUE) {
		int read = recv(socket, (char*) buffer, MAX_PACKET_SIZE, 0);

		if (read == 0) {
			break;
		}

		if (buffer[0] == CS_LOGIN) {
			User* user = ClientLogin(reinterpret_cast<Packet_Login*>(buffer));

			if (user != NULL) {
				cout << "user logged on: " << user->GetName() << endl;
			}

			Packet_Login_SC scPacket;
			scPacket.success = user != NULL;
			send(socket, (char*) &scPacket, sizeof(scPacket), 0);
		}
		if (buffer[0] == CS_SIGNUP) {
			BOOL success = ClientSignUp(reinterpret_cast<Packet_SignUp*>(buffer));


			if (success) {
				cout << "new user created" << endl;
			}

			Packet_SignUp_SC scPacket;
			scPacket.success = success;
			send(socket, (char*) &scPacket, sizeof(scPacket), 0);
		}
	}

	closesocket(socket);

	for (int i = 0; i < clients.size(); i++) {
		if (clients[i] == client) {
			// 클라이언트 목록에서 지웁니다
			// https://www.cplusplus.com/reference/vector/vector/erase/
			clients.erase(clients.begin() + i);
		}
	}

	cout << "disconnected : " << client->GetID() << std::endl;
}

User* Server::ClientLogin(Packet_Login* loginPacket)
{
	return db.login(c2ws(loginPacket->username), c2ws(loginPacket->password));
}

BOOL Server::ClientSignUp(Packet_SignUp* signUpPacket)
{
	int retcode = db.signup(c2ws(signUpPacket->username), c2ws(signUpPacket->password));
	std::cout << retcode << std::endl;
	if (retcode == SQL_SUCCESS) {
		return TRUE;
	}
	else {
		std::cout << "signup failed: " << retcode << std::endl;
		return FALSE;
	}
}

DWORD WINAPI Server::NewClientThread(LPVOID args) 
{
	ThreadArgs* thread_args = reinterpret_cast<ThreadArgs*>(args);
	thread_args->server->ClientMain(thread_args->client);

	return 0;
}
