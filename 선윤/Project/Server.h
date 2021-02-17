#pragma once
#include "stdafx.h"
#include "Client.h"

class Server 
{
private:
	SOCKET server_socket;
	HANDLE client_thread[MAX_CLIENT];

	static Client game_clients[MAX_CLIENT];
	//static unordered_map<int, Client> game_clients;

public:
	Server();
	~Server();

	void InitServer();
	void StartServer();
	void Send_Enter_Packet(int);
	void Send_Login_Packet();
	void Send_Logout_Packet();
	void Send_Move_Packet();
	void Send_Chat_Packet();
	void Send_Item_Packet();

	static DWORD WINAPI LoginServer(LPVOID);

	void err_quit(const char* msg)
	{
		LPVOID lpMsgBuf;
		FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM, NULL, WSAGetLastError(),					MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPTSTR)&lpMsgBuf, 0, NULL);
		MessageBox(NULL, (LPCTSTR)lpMsgBuf, (LPCWSTR)msg, MB_ICONERROR);
		LocalFree(lpMsgBuf);
		exit(1);
	}

	void err_display(const char* msg, int err_no)
	{
		WCHAR* lpMsgBuf;
		FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM, NULL, err_no, 
					  MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPTSTR)&lpMsgBuf, 0, NULL);
		std::cout << msg;
		std::wcout << L"¿¡·¯ " << lpMsgBuf << std::endl;
		while (true);
		LocalFree(lpMsgBuf);
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