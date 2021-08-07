#include "stdafx.h"
#include "Server.h"
#include "Client.h"
#include "protocol.h"

const wchar_t* PACKET_NAME[] = { L"Login"
                            ,L"Logout"
                            ,L"Singup"
                            ,L"Move"
                            ,L"Chat"
                            ,L"Item"
                            ,L"Score"
                            ,L"Game State"
                            ,L"Fire"
                            ,L"??"
                            ,L"Mesh"
                            ,L"Set Mesh"
                            ,L"Road"
                            ,L"Set Road"
                            ,L"Init Mode"
                        };

Server::Server() :
	db(ODBC_NAME, DBUSER, DBPASSWORD)
{
	ZeroMemory(meshVertices, sizeof(meshVertices));
	ZeroMemory(meshTriangles, sizeof(meshTriangles));

	ZeroMemory(roadVertices, sizeof(roadVertices));
	ZeroMemory(roadTriangles, sizeof(roadTriangles));

	ZeroMemory(isRoad, sizeof(isRoad));
	ZeroMemory(isBuildingPlace, sizeof(isBuildingPlace));
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
	int ret = ::bind(server_socket, (SOCKADDR*)&server_addr, sizeof(server_addr));
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
#ifndef TEST
	std::cout << "Connecting DB..." << std::endl;
	int dbretcode = db.connect();
	if (!(dbretcode == SQL_SUCCESS || dbretcode == SQL_SUCCESS_WITH_INFO)) {
		err_quit(L"DB Connection failure (" + to_wstring(dbretcode) + L")");
	}
	std::cout << "DB Connected!" << std::endl;
#endif
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

    HANDLE server_thread = CreateThread(NULL, 0, this->ServerThread, (LPVOID)this, 0, NULL);
    if (server_thread == NULL) {
        err_display("StartServer() -> ServerThread()", ::GetLastError());
        return;
    }

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
		
		int new_client_id = clients.size();
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
	BYTE* buffer = new BYTE[MAX_PACKET_SIZE];

	while (TRUE) {
		int read = recv(socket, (char*) buffer, MAX_PACKET_SIZE, 0);

		if (read <= 0) {
			break;
		}
        
        if (buffer[0] >= 0 && buffer[0] < PACKET_CMD_MAX) {
            //wprintf(L"[Recv From %d] Cmd: %s, %d bytes\n", socket, PACKET_NAME[(int)buffer[0]], read);
        }
        else {
            wprintf(L"[Recv From %d] Cmd: %d(Unknown), %d bytes\n", socket, (int)buffer[0], read);
        }

		if (buffer[0] == CS_LOGIN) {
			User* user = ClientLogin(reinterpret_cast<Packet_Login*>(buffer));

			Packet_Login_SC* scPacket = new Packet_Login_SC;
			scPacket->success = user != NULL;
			if (user != NULL) {
				cout << "user logged on: " << user->GetName() << endl;
				scPacket->clientId = client->GetID();
				client->SetNick(user->GetName());

				if (CanStartGame()) {
					isGameStarted = true;
					gameStartedAt = time(NULL);
                    cout << "-- START GAME -- " << endl;

					Packet_GameState_SC* packet = new Packet_GameState_SC;
					packet->state = 1;
					for (int i = 0; i < clients.size(); i++) {
                        SendTo(clients[i]->GetSocket(), (char*)packet, sizeof(*packet));
					}
					delete packet;
				}
			}

            SendTo(socket, (char*) scPacket, sizeof(*scPacket));
			delete scPacket;
		}
		if (buffer[0] == CS_SIGNUP) {
			BOOL success = ClientSignUp(reinterpret_cast<Packet_SignUp*>(buffer));

			if (success) {
				cout << "new user created" << endl;
			}

			Packet_SignUp_SC* scPacket = new Packet_SignUp_SC;
			scPacket->success = success;
            SendTo(socket, (char*) scPacket, sizeof(*scPacket));
			delete scPacket;
		}
		if (buffer[0] == CS_MESH) {
            /*if (!isGameStarted) {
                continue;
            }*/
			Packet_Request_Mesh_SC* scPacket = new Packet_Request_Mesh_SC;
			scPacket->ready = meshReady;
			memcpy(scPacket->vertices, meshVertices, sizeof(meshVertices));
			memcpy(scPacket->triangles, meshTriangles, sizeof(meshTriangles));
            cout << "CS_MESH: " << meshReady << endl;
            SendTo(socket, (char*) scPacket, sizeof(*scPacket));
			delete scPacket;
		}
		if (buffer[0] == CS_SET_MESH) {
            /*if (!isGameStarted) {
                continue;
            }*/
			if (meshReady) {
                cout << "CS_SET_MESH: continue "<< endl;
				continue;
			}
			Packet_Set_Mesh* packet = reinterpret_cast<Packet_Set_Mesh*>(buffer);
			meshReady = true;
			memcpy(meshVertices, packet->vertices, sizeof(meshVertices));
			memcpy(meshTriangles, packet->triangles, sizeof(meshTriangles));
            cout << "CS_SET_MESH: meshReady true " << endl;
			Packet_Set_Mesh_SC* scPacket = new Packet_Set_Mesh_SC;
			scPacket->ready = meshReady;
			memcpy(scPacket->vertices, meshVertices, sizeof(meshVertices));
			memcpy(scPacket->triangles, meshTriangles, sizeof(meshTriangles));

#if (Debug == TRUE)
				cout << sizeof(*scPacket) << endl;
#endif
			for (int i = 0; i < clients.size(); i++) {
                SendTo(socket, (char*) scPacket, sizeof(*scPacket));
			}
			delete scPacket;
		}
		if (buffer[0] == CS_ROAD) {
            /*if (!isGameStarted) {
                continue;
            }*/
			Packet_Request_Road_SC* scPacket = new Packet_Request_Road_SC;
			scPacket->ready = roadReady;
            cout << "CS_ROAD: " << roadReady << endl;
			memcpy(scPacket->vertices, roadVertices, sizeof(roadVertices));
			memcpy(scPacket->triangles, roadTriangles, sizeof(roadTriangles));
			memcpy(scPacket->isRoad, isRoad, sizeof(isRoad));
			memcpy(scPacket->isBuildingPlace, isBuildingPlace, sizeof(isBuildingPlace));

            SendTo(socket, (char*) scPacket, sizeof(*scPacket));
			delete scPacket;
		}
		if (buffer[0] == CS_SET_ROAD) {
            /*if (!isGameStarted) {
                continue;
            }*/
			if (roadReady) {
                cout << "CS_SET_ROAD continue" << endl;
				continue;
			}
			Packet_Set_Road* packet = reinterpret_cast<Packet_Set_Road*>(buffer);
			roadReady = true;
			memcpy(roadVertices, packet->vertices, sizeof(roadVertices));
			memcpy(roadTriangles, packet->triangles, sizeof(roadTriangles));
			memcpy(isRoad, packet->isRoad, sizeof(isRoad));
			memcpy(isBuildingPlace, packet->isBuildingPlace, sizeof(isBuildingPlace));
            cout << "CS_SET_ROAD roadReady true" << endl;
			Packet_Set_Road_SC* scPacket = new Packet_Set_Road_SC;
			scPacket->ready = roadReady;
			memcpy(scPacket->vertices, roadVertices, sizeof(roadVertices));
			memcpy(scPacket->triangles, roadTriangles, sizeof(roadTriangles));
			memcpy(scPacket->isRoad, isRoad, sizeof(isRoad));
			memcpy(scPacket->isBuildingPlace, isBuildingPlace, sizeof(isBuildingPlace));

#if (Debug == TRUE)
			cout << sizeof(*scPacket) << endl;
#endif
			for (int i = 0; i < clients.size(); i++) {
                SendTo(clients[i]->GetSocket(), (char*) scPacket, sizeof(*scPacket));
			}
			delete scPacket;
		}
		if (buffer[0] == CS_SCORE) {
			if (!isGameStarted) {
				continue;
			}

			client->SetScore(client->GetScore() + 50);

			Packet_Score_SC* scPacket = new Packet_Score_SC;
			scPacket->players = MAX_CLIENT;
			memset(scPacket->scores, -1, sizeof(int32_t) * MAX_CLIENT);

			for (int i = 0; i < clients.size(); i++) {
				scPacket->scores[i] = clients[i]->GetScore();
			}

			for (int i = 0; i < clients.size(); i++) {
                SendTo(clients[i]->GetSocket(), (char*)scPacket, sizeof(*scPacket));
			}
		} 
		if (buffer[0] == CS_MOVE) {
            if (!isGameStarted) {
                continue;
            }
			Packet_Move* packet = reinterpret_cast<Packet_Move*>(buffer);

			client->SetPos(
				packet->position.x,
				packet->position.y,
				packet->position.z
			);
			client->SetRot(
				packet->rotation.x,
				packet->rotation.y,
				packet->rotation.z
			);

			Packet_Move_SC* scPacket = new Packet_Move_SC;
			memset(scPacket->position, 0x00, sizeof(scPacket->position));
			memset(scPacket->rotation, 0x00, sizeof(scPacket->rotation));
			scPacket->players = MAX_CLIENT;

			for (int i = 0; i < clients.size(); i++) {
				scPacket->position[i] = clients[i]->GetPos();
				scPacket->rotation[i] = clients[i]->GetRot();
				std::cout << "player " << i << ": " << scPacket->position[i].x << "," << scPacket->position[i].y << "," << scPacket->position[i].z << endl;
			}

			for (int i = 0; i < clients.size(); i++) {
                SendTo(clients[i]->GetSocket(), (char*)scPacket, sizeof(*scPacket));
			}

			// 타이머 시간에 따라 '180'위치의 숫자 초 단위로 조절
			// gameTimer.cs의 limitTime 숫자도 함께 조절
#ifdef TEST
            if (isGameStarted && (time(NULL) - gameStartedAt >= 10)) {
#else
			if (isGameStarted && (time(NULL) - gameStartedAt >= 100)) {
#endif
				cout << "-- END GAME --" << endl;
				if (_mutex.try_lock()) {
					isGameStarted = false;
                    gameFinishedAt = time(NULL);
                    isCountdownStarted = true;

					Packet_GameState_SC* packet = new Packet_GameState_SC;
					packet->state = 0;
					for (int i = 0; i < clients.size(); i++) {
                        SendTo(clients[i]->GetSocket(), (char*)packet, sizeof(*packet));
					}
					delete packet;

					_mutex.unlock();
				}
			}
		}
		if (buffer[0] == CS_FIRE) {
            if (!isGameStarted) {
                continue;
            }
			Packet_Fire* packet = reinterpret_cast<Packet_Fire*>(buffer);

			Packet_Fire_SC* scPacket = new Packet_Fire_SC;
			scPacket->position = packet->position;
			scPacket->targetPosition = packet->targetPosition;

			for (int i = 0; i < clients.size(); i++) {
                SendTo(clients[i]->GetSocket(), (char*)scPacket, sizeof(*scPacket));
			}
			delete scPacket;
		}
	}

	closesocket(socket);
	delete buffer;

	for (int i = 0; i < clients.size(); i++) {
		if (clients[i] == client) {
			// 클라이언트 목록에서 지웁니다
			// https://www.cplusplus.com/reference/vector/vector/erase/
			clients.erase(clients.begin() + i);
		}
	}

	cout << "disconnected : " << client->GetID() << std::endl;

	if (clients.size() == 0) {
		// 클라이언트가 0명이 되면 맵 초기화
		ZeroMemory(meshVertices, sizeof(meshVertices));
		ZeroMemory(meshTriangles, sizeof(meshTriangles));

		ZeroMemory(roadVertices, sizeof(roadVertices));
		ZeroMemory(roadTriangles, sizeof(roadTriangles));

		ZeroMemory(isRoad, sizeof(isRoad));
		ZeroMemory(isBuildingPlace, sizeof(isBuildingPlace));

		meshReady = false;
		roadReady = false;
	}
}

void Server::ServerMain()
{
    while (TRUE) {
        Sleep(1000);

        // 게임종료 후 10초뒤에 로그인화면으로 이동
        if (isCountdownStarted && (time(NULL) - gameFinishedAt >= 10)) {
            if (_mutex.try_lock()) {
                isCountdownStarted = false;

                Packet_GameInit_SC* packet = new Packet_GameInit_SC;
                packet->scene = 1;
                for (int i = 0; i < clients.size(); i++) {
                    SendTo(clients[i]->GetSocket(), (char*)packet, sizeof(*packet));
                }
                delete packet;
                _mutex.unlock();
            }

            ZeroMemory(meshVertices, sizeof(meshVertices));
            ZeroMemory(meshTriangles, sizeof(meshTriangles));

            ZeroMemory(roadVertices, sizeof(roadVertices));
            ZeroMemory(roadTriangles, sizeof(roadTriangles));

            ZeroMemory(isRoad, sizeof(isRoad));
            ZeroMemory(isBuildingPlace, sizeof(isBuildingPlace));

            meshReady = false;
            roadReady = false;
        }
    }
}

int Server::SendTo(SOCKET sock, char * packet, int packetSize)
{
    int sendLen = send(sock, packet, packetSize, 0);
    //wprintf(L"[Send To   %d] Cmd: %s, %d bytes\n", sock, PACKET_NAME[(int)packet[0]], sendLen);
    return sendLen;
}

User* Server::ClientLogin(Packet_Login* loginPacket)
{
#ifdef TEST
    static int testId = 1;
    return new User(testId++, string("User") + std::to_string(testId));
#else
	return db.login(c2ws(loginPacket->username), c2ws(loginPacket->password));
#endif
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

BOOL Server::CanStartGame()
{
	if (clients.size() != MAX_CLIENT) {
		return false;
	}

	BOOL flag = true;

	for (int i = 0; i < clients.size(); i++) {
		if (clients[i]->GetNick() == "") {
			flag = false;
			break;
		}
	}

	return flag;
}

DWORD WINAPI Server::NewClientThread(LPVOID args) 
{
	ThreadArgs* thread_args = reinterpret_cast<ThreadArgs*>(args);
	thread_args->server->ClientMain(thread_args->client);

	return 0;
}

DWORD WINAPI Server::ServerThread(LPVOID args)
{
    Server* pThis = reinterpret_cast<Server*>(args);
    pThis->ServerMain();

    return 0;
}
