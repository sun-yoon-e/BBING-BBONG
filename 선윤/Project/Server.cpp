#include "stdafx.h"
#include "Server.h"
#include "Client.h"
#include "protocol.h"
#include "Room.h"

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

Server::Server() : db(ODBC_NAME, DBUSER, DBPASSWORD)
{
	lobby = new Lobby();

	serverSem = CreateSemaphore(NULL, 1, 1, NULL);
}

Server::~Server()
{
	delete lobby;
	CloseHandle(serverSem);
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
	
	auto totalRead = 0ull;	

	while (TRUE) {		
		int read = recv(socket, (char*) (buffer + totalRead), (MAX_PACKET_SIZE - totalRead), 0);

		if (read <= 0) {
			cout << read << endl;
			break;
		}

		// 큰 패킷의 경우 recv 한번에 안받아 질 수 있음
		// SET_MESH와 SET_ROAD 가 패킷 사이즈가 크므로
		// 두 개만 받은 데이터 양 체크해서 부족하면 이어서 받을 수 있도록 수정
		if (buffer[0] == CS_SET_MESH || buffer[0] == CS_SET_ROAD)
		{
			totalRead += read;
			if (totalRead < MAX_PACKET_SIZE)
			{
				continue;
			}
		}

		totalRead = 0;        
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

				lobby->AddUser(client);
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
			auto* room = client->GetRoom();
			if (room)
			{
				auto* packet = new BYTE[MAX_PACKET_SIZE];
				Packet_Request_Mesh_SC* scPacket = reinterpret_cast<Packet_Request_Mesh_SC*>(packet);
				scPacket->ready = room->Execute_Cs_Mesh(scPacket);
				cout << "CS_MESH: " << client->GetID() << ", " << scPacket->ready << endl;
				
				SendTo(socket, (char*)packet, MAX_PACKET_SIZE);

				delete [] packet;
			}
			else
			{
				cout << "CS_MESH: " << client->GetID() << " " << "room not exist" << endl;
			}
		}
		if (buffer[0] == CS_SET_MESH) {
			auto* room = client->GetRoom();
			if (room)
			{	
				if (room->IsMeshReady()) {
					cout << "CS_SET_MESH: continue " << client->GetID() << endl;
					continue;
				}

				Packet_Set_Mesh* packet = reinterpret_cast<Packet_Set_Mesh*>(buffer);
				auto* scPacket = room->Execute_Cs_Set_Mesh(packet);
				cout << "CS_SET_MESH: " << client->GetID() << " meshReady true " << endl;

#if (Debug == TRUE)
				cout << sizeof(*scPacket) << endl;
#endif
				room->SendMessageToOtherPlayers(nullptr, (char*)scPacket, MAX_PACKET_SIZE);
				delete [] (BYTE*)scPacket;
			}
			else
			{
				cout << "CS_SET_MESH: " << client->GetID() << " " << "room not exist" << endl;
			}
		}
		if (buffer[0] == CS_ROAD) {
			auto* room = client->GetRoom();
			if (room)
			{
				auto* packet = new BYTE[MAX_PACKET_SIZE];
				Packet_Request_Road_SC* scPacket = reinterpret_cast<Packet_Request_Road_SC*>(packet);
				scPacket->ready = room->Execute_Cs_Road(scPacket);
				cout << "CS_ROAD: " << client->GetID() << " , " << scPacket->ready << endl;
				SendTo(socket, (char*)packet, MAX_PACKET_SIZE);
				delete [] packet;
			}
			else
			{
				cout << "CS_ROAD: " << client->GetID() << " " << "room not exist" << endl;
			}
		}
		if (buffer[0] == CS_SET_ROAD) {
			auto* room = client->GetRoom();
			
			if (room)
			{
				if (room->IsRoadReady()) {
					cout << "CS_SET_ROAD: continue " << client->GetID() << endl;
					continue;
				}

				Packet_Set_Road* packet = reinterpret_cast<Packet_Set_Road*>(buffer);
				auto* scPacket = room->Execute_Cs_Set_Road(packet);
				cout << "CS_SET_ROAD: roadready true " << client->GetID() << endl;

#if (Debug == TRUE)
				cout << sizeof(*scPacket) << endl;
#endif
				room->SendMessageToOtherPlayers(nullptr, (char*)scPacket, MAX_PACKET_SIZE);
				delete [] (BYTE*)scPacket;
			}
			else
			{
				cout << "CS_SET_ROAD: " << client->GetID() << " " << "room not exist" << endl;
			}
		}
		if (buffer[0] == CS_MAKE_CAR) {
			auto* room = client->GetRoom();
			if (room)
			{
				buffer[1] = SC_MAKE_CAR;
				room->SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(buffer), sizeof(cs_packet_make_car));
			}
		}
		if (buffer[0] == CS_DESTROY_CAR) {
			auto* room = client->GetRoom();
			if (room)
			{
				auto* room = client->GetRoom();
				if (room)
				{
					buffer[1] = SC_DESTROY_CAR;
					room->SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(buffer), sizeof(cs_packet_destroy_car));
				}
			}
		}
		if (buffer[0] == CS_SCORE) {
			auto* room = client->GetRoom();
			if (room && room->IsGameStarted())
			{
				client->SetScore(client->GetScore() + 50);

				Packet_Score_SC* scPacket = new Packet_Score_SC;
				scPacket->players = MAX_CLIENT;
				memset(scPacket->scores, -1, sizeof(int32_t)* MAX_CLIENT);

				for (int i = 0; i < clients.size(); i++) {
					scPacket->scores[i] = clients[i]->GetScore();
				}

				room->SendMessageToOtherPlayers(client, reinterpret_cast<char*>(scPacket), sizeof(*scPacket));
				delete scPacket;
			}
		} 
		if (buffer[0] == CS_MOVE) {
			auto* room = client->GetRoom();
			Packet_Move* packet = reinterpret_cast<Packet_Move*>(buffer);
			if (room && room->IsGameStarted())
			{
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

				Packet_Move_SC* scPacket = room->GetMovePacket();
				room->SendMessageToOtherPlayers(client, reinterpret_cast<char*>(scPacket), sizeof(Packet_Move_SC));
				delete scPacket;
			}
		}
		if (buffer[0] == CS_FIRE) {
			auto* room = client->GetRoom();
			Packet_Fire* packet = reinterpret_cast<Packet_Fire*>(buffer);
			if (room && room->IsGameStarted())
			{
				packet->TYPE = SC_FIRE;
				room->SendMessageToOtherPlayers(client, reinterpret_cast<char*>(packet), sizeof(Packet_Fire));
			}
		}
		if (buffer[0] == CS_AI_MOVE) {
			auto* room = client->GetRoom();
			if (room && room->IsGameStarted())
			{
				buffer[0] = SC_AI_MOVE;
				auto* p = reinterpret_cast<packet_ai_move*>(buffer);
				room->SetAIPosition(p->aiId, p->pos, p->rot);
				room->SendMessageToOtherPlayers(client, reinterpret_cast<char*>(buffer), sizeof(packet_ai_move));
			}
		}
		if (buffer[0] == CS_AI_FIRE) {
			auto* room = client->GetRoom();
			if (room && room->IsGameStarted())
			{
				buffer[0] = SC_AI_FIRE;
				room->SendMessageToOtherPlayers(client, reinterpret_cast<char*>(buffer), sizeof(packet_ai_fire));
			}
		}
		if (buffer[0] == CS_AI_ADD) {
			auto* room = client->GetRoom();
			if (room)
			{
				auto aiIndex = room->AddAI();
				auto* packet = new sc_packet_bot_add();
				packet->aiId = aiIndex;
				room->SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(packet), sizeof(sc_packet_bot_add));
				delete packet;

				auto* scPacket = room->GetPlayerInfo();
				room->SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(scPacket), sizeof(sc_packet_room_player));
				delete scPacket;
			}
		}
		if (buffer[0] == CS_AI_REMOVE) {
			auto* room = client->GetRoom();
			if (room)
			{
				auto* packet = reinterpret_cast<cs_packet_bot_remove*>(buffer);				
				auto id = room->RemoveAI();
				packet->type = SC_AI_REMOVE;
				packet->aiId = id;
				room->SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(packet), sizeof(sc_packet_bot_remove));			

				auto* scPacket = room->GetPlayerInfo();
				room->SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(scPacket), sizeof(sc_packet_room_player));
				delete scPacket;
			}
		}
		if (buffer[0] == CS_CHAT) {
			auto* room = client->GetRoom();
			auto* packet = sc_packet_chat::GetChatPacket(buffer, client->GetNickPtr());

			if (room)
			{
				room->SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(packet), sizeof(sc_packet_chat));
			}

			delete [] packet;
		}
		if (buffer[0] == CS_MAKE_ROOM) {
			// 클라이언트가 방에 속해있지 않을 때만 방 생성
			auto* room = client->GetRoom();
			if (!room)
			{
				auto roomId = lobby->MakeRoom(reinterpret_cast<char*>(&buffer[1]));
				if (roomId < 0)
				{
					cout << "make room fail" << endl;
				}
				else
				{
					lobby->EnterToRoom(roomId, client);
					lobby->RemoveUser(client);
				}
			}

			// 방을 성공적으로 생성했으면
			room = client->GetRoom();
			if (room)
			{	
				auto* scPacket = room->GetPlayerInfo();
				room->SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(scPacket), sizeof(sc_packet_room_player));
				delete scPacket;
			}
		}
		if (buffer[0] == CS_ENTER_ROOM) {
			auto* room = client->GetRoom();
			auto success = false;
			if (!room)
			{
				success = lobby->EnterToRoom(*reinterpret_cast<int*>(&buffer[1]), client);
				if (success)
				{
					lobby->RemoveUser(client);
				}
			}

			// 방에 입장하면 모두에게 방에 있는 플레이어 정보 전달
			room = client->GetRoom();
			if (room)
			{
				auto* scPacket = room->GetPlayerInfo();
				room->SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(scPacket), sizeof(sc_packet_room_player));
				delete scPacket;
			}

			// 방 입장에 실패하면 입장 요청한 클라이언트에 방 정보 전달
			if (!success)
			{
				auto roomId = *reinterpret_cast<int*>(buffer + 1);
				auto* packet = lobby->GetRoomInfoPacket(roomId);
				SendTo(client->GetSocket(), reinterpret_cast<char*>(packet), sizeof(sc_packet_room_info));
				delete packet;
			}
		}
		if (buffer[0] == CS_EXIT_ROOM) {
			auto* room = client->GetRoom();
			if (room)
			{
				lobby->AddUser(client);

				// 클라이언트가 룸에서 마지막으로 나오면 룸 삭제
				if (room->RemoveUser(client))
				{
					lobby->DestroyRoom(room->GetID());
				}
				else
				{
					auto* scPacket = room->GetPlayerInfo();
					room->SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(scPacket), sizeof(sc_packet_room_player));;
					delete scPacket;
				}
			}
		}
		if (buffer[0] == CS_ROOM_LIST_INFO) {
			auto page = buffer[1];
			sc_packet_room_list rList;
			ZeroMemory(&rList, sizeof(sc_packet_room_list));
			rList.type = SC_ROOM_LIST_INFO;
			lobby->GetRoomListInfoPacket(page, rList.roomInfo);
			SendTo(client->GetSocket(), reinterpret_cast<char*>(&rList), sizeof(sc_packet_room_list));
		}
		if (buffer[0] == CS_ROOM_INFO) {
			auto roomId = *reinterpret_cast<int*>(&buffer[1]);

			if (roomId < 0)
			{
				auto* room = client->GetRoom();
				if (room)
				{
					roomId = room->GetID();
				}
			}			
			auto*  packet = lobby->GetRoomInfoPacket(roomId);
			SendTo(client->GetSocket(), reinterpret_cast<char*>(packet), sizeof(sc_packet_room_info));
			delete packet;
		}
		if (buffer[0] == CS_GAMESTATE) {
			auto* room = client->GetRoom();
			if(room)
			{
				room->StartGame();
				room->gameStartedAt = time(NULL);
				cout << "-- START GAME @" << room->GetID() << " --" << endl;

				Packet_GameState_SC* packet = new Packet_GameState_SC;
				packet->state = buffer[1];
				room->SendMessageToOtherPlayers(nullptr, (char*)packet, sizeof(*packet));
				delete packet;
			}
		}
		if (buffer[0] == CS_PLACE_ITEM) {
			buffer[0] = SC_PLACE_ITEM;
			auto* room = client->GetRoom();
			if (room)
			{
				room->SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(buffer), sizeof(packet_place_item));
			}
		}
		if (buffer[1] == CS_REMOVE_ITEM) {
			buffer[0] = SC_REMOVE_ITEM;
			auto* room = client->GetRoom();
			if (room)
			{
				room->SendMessageToOtherPlayers(nullptr, reinterpret_cast<char*>(buffer), sizeof(packet_remove_item));
			}
		}
		if (buffer[1] == CS_USE_ITEM) {
			auto* room = client->GetRoom();
			if (room)
			{
				auto* msg = reinterpret_cast<packet_use_item*>(buffer);
				msg->type = SC_USE_ITEM;
				auto targetId = msg->targetPlayerId;

				if (targetId > 0)
				{
					auto* target = room->GetIndexedPlayer(msg->targetPlayerId);
					if (target)
					{
						SendTo(target->GetSocket(), reinterpret_cast<char*>(buffer), sizeof(packet_use_item));
					}
				}
				else
				{
					room->SendMessageToOtherPlayers(client, reinterpret_cast<char*>(buffer), sizeof(packet_use_item));
				}
			}
		}
	}

	closesocket(socket);
	delete [] buffer;

	auto* room = client->GetRoom();
	if (room)
	{
		if (room->RemoveUser(client))
		{
			lobby->DestroyRoom(room->GetID());
		}
	}
	lobby->RemoveUser(client);

	for (int i = 0; i < clients.size(); i++) {
		if (clients[i] == client) {
			// 클라이언트 목록에서 지웁니다
			// https://www.cplusplus.com/reference/vector/vector/erase/
			clients.erase(clients.begin() + i);
		}
	}

	cout << "disconnected : " << client->GetID() << std::endl;
}

void Server::ServerMain()
{
    while (TRUE) {
        Sleep(1000);

		lobby->CheckGameFinish();
		lobby->ToGameRoom();
#if false
        // 게임종료 후 10초뒤에 로그인화면으로 이동
		// To-Do 게임종료 후 방 별로 맵 초기화 하도록 코드 수정필요		
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
#endif
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