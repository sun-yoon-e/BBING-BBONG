#pragma once
#include "stdafx.h"

#include "User.h"
#include "Client.h"

#include "DB.h"

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

	int client_id_counter = 0;

public:
	Server();
	~Server();

	void InitServer();
	void StartServer();


	void LoginServer(int);

	void ClientMain(Client* client);

	User* ClientLogin(Packet_Login* loginPacket);
	BOOL ClientSignUp(Packet_SignUp* signUpPacket);

	void Send_Enter_Packet(int);
	void Send_Login_Packet();
	void Send_Logout_Packet();
	void Send_Move_Packet();
	void Send_Chat_Packet();
	void Send_Item_Packet();

	static DWORD WINAPI NewClientThread(LPVOID);
	static DWORD WINAPI LobbyServer(LPVOID);

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