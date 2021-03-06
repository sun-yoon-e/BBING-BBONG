﻿#pragma once
#include <time.h>
#include <mutex>

#include "stdafx.h"
#include "User.h"
#include "Client.h"
#include "DB.h"
#include "protocol.h"

class Server;

class ThreadArgs {
public:
	Server* server;
	Client* client;
};

class Server 
{
private:
	DB db;
	SOCKET server_socket;
	vector<Client*> clients;
	vector<HANDLE> client_threads;

	BOOL meshReady = false;
	Vector3 meshVertices[(XSIZE + 1) * (ZSIZE + 1)];
	int32_t meshTriangles[XSIZE * ZSIZE * 6];

	BOOL roadReady = false;
	Vector3 roadVertices[(XSIZE + 1) * (ZSIZE + 1)];
	int32_t roadTriangles[XSIZE * ZSIZE * 6];
	bool isRoad[(XSIZE + 1) * (ZSIZE + 1)];
	int16_t isBuildingPlace[(XSIZE + 1) * (ZSIZE + 1)];

	int client_id_counter = 0;

	BOOL isGameStarted = false;
	BOOL isCountdownStarted = false;
	time_t gameStartedAt = 0;
	time_t gameFinishedAt = 0;
	mutex _mutex;

public:
	Server();
	~Server();

	void InitServer();
	void StartServer();

	void ClientMain(Client* client);
	void ServerMain();
    int SendTo(SOCKET sock, char* packet, int packetSize);

	User* ClientLogin(Packet_Login* loginPacket);
	BOOL ClientSignUp(Packet_SignUp* signUpPacket);

	BOOL CanStartGame();

	static DWORD WINAPI NewClientThread(LPVOID);
	static DWORD WINAPI ServerThread(LPVOID);

	// https://stackoverflow.com/questions/10737644/convert-const-char-to-wstring 를 참고하여 수정함
	wstring c2ws(const char* cstr)
	{
		string str(cstr);
		int size_needed = MultiByteToWideChar(CP_UTF8, 0, &str[0], (int)str.size(), NULL, 0);
		std::wstring wstrTo(size_needed, 0);
		MultiByteToWideChar(CP_UTF8, 0, &str[0], (int)str.size(), &wstrTo[0], size_needed);
		return wstrTo;
	}

	void err_quit(const wstring msg)
	{
		MessageBox(NULL, (LPCWSTR) msg.c_str(), NULL, MB_ICONERROR);
		exit(1);
	}

	void err_display(const char* msg, int err_no = 0)
	{
		std::cout << msg;

		if (err_no != 0) {
			WCHAR* lpMsgBuf;
			FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM, NULL, err_no,
				MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPTSTR)&lpMsgBuf, 0, NULL);
			std::wcout << L"에러 " << lpMsgBuf << std::endl;
			LocalFree(lpMsgBuf);
		}
	}

	int recvn(SOCKET s, char* buf, int len, int flags)
	{
		int received;
		char* ptr = buf;
		int left = len;
	
		while (left > 0) {
			received = recv(s, ptr, left, flags);
			if (received == SOCKET_ERROR) return SOCKET_ERROR;
			else if (received == 0)	break;
			left -= received;
			ptr += received;
		}
		return (len - left);
	}
};