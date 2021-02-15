#pragma comment(lib, "ws2_32")
#include <winsock2.h>
#include <iostream>
#include <string>
#include <list>
using namespace std;

#define PORT  13531
#define BUFSIZE     1024

struct Client {
	SOCKET cSocket;
	string cName;

	Client(SOCKET s) {
		cName = "Guest";
		cSocket = s;
	}
};

void err_quit(char* msg);
void err_display(char* msg);
int recvn(SOCKET s, char* buf, int len, int flags);
bool IsConnected(SOCKET s);

list<Client> clients;
list<Client> disconnectList;

list<Client>::iterator iter_c = clients.begin();
list<Client>::iterator iter_d = disconnectList.begin();

bool serverStarted;

int main()
{
	int ret;
	// ���� �ʱ�ȭ
	WSADATA wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0) return 1;

	// socket()
	SOCKET sock = socket(AF_INET, SOCK_STREAM, 0);
	if (sock == INVALID_SOCKET) err_quit((char*)"socket()");

	// bind()
	SOCKADDR_IN addr;
	ZeroMemory(&addr, sizeof(addr));
	addr.sin_family = AF_INET;
	addr.sin_addr.s_addr = htonl(INADDR_ANY);
	addr.sin_port = htons(PORT);
	ret = bind(sock, (SOCKADDR*)&addr, sizeof(addr));
	if (ret == SOCKET_ERROR) err_quit((char*)"bind()");

	ret = listen(sock, SOMAXCONN);
	if (ret == SOCKET_ERROR) err_quit((char*)"listen()");

	cout << "Listening..." << endl;
	serverStarted = true;

	SOCKET client_sock;
	SOCKADDR_IN client_addr;
	int addrlen;

	while (1) {
		// accept()
		addrlen = sizeof(client_addr);
		client_sock = accept(sock, (SOCKADDR*)&client_addr, &addrlen);
		if (client_sock == INVALID_SOCKET) {
			err_display((char*)"accept()");
			break;
		}

		// ������ Ŭ���̾�Ʈ ���� ���
		cout << "[TCP ����] Ŭ���̾�Ʈ ����: IP �ּ�=" << inet_ntoa(client_addr.sin_addr) << ", ��Ʈ ��ȣ=" << ntohs(client_addr.sin_port) << endl;

		if (!serverStarted) return;

		//for (Client c : clients) {
		//	if (!IsConnected(c.cSocket)) {
		//		closesocket(c.cSocket);
		//		disconnectList.emplace_back(c);
		//		continue;
		//	}
		//	else {
		//		// Ŭ���̾�Ʈ�κ��� üũ �޽����� �޴´�
		//	}
		//}

		for (int i = 0; i < disconnectList.size() - 1; i++) {
			
		}

		closesocket(client_sock);
		cout << "[TCP ����] Ŭ���̾�Ʈ ����: IP �ּ�=" << inet_ntoa(client_addr.sin_addr) << ", ��Ʈ ��ȣ=" << ntohs(client_addr.sin_port) << endl << endl;
	}
	// closesocket()
	closesocket(sock);

	// ���� ����
	WSACleanup();
	return 0;
}

bool IsConnected(SOCKET s)
{

}

// ���� �Լ� ���� ��� �� ����
void err_quit(char* msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	MessageBox(NULL, (LPCTSTR)lpMsgBuf, (LPCWSTR)msg, MB_ICONERROR);
	LocalFree(lpMsgBuf);
	exit(1);
}

// ���� �Լ� ���� ���
void err_display(char* msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	printf("[%s] %s\n", msg, (char*)lpMsgBuf);
	LocalFree(lpMsgBuf);
}

// ����� ���� ������ ���� �Լ�
int recvn(SOCKET s, char* buf, int len, int flags)
{
	int received;
	char* ptr = buf;
	int left = len;

	while (left > 0) {
		received = recv(s, ptr, left, flags);
		if (received == SOCKET_ERROR)
			return SOCKET_ERROR;
		else if (received == 0)
			break;
		left -= received;
		ptr += received;
	}

	return (len - left);
}