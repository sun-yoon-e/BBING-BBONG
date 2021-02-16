#pragma comment(lib, "ws2_32")
#include <winsock2.h>
#include <iostream>
using namespace std;

struct packet {
	//char size;
	//char type;
	int  score;
	float xyz;
	bool connect;
	string name;
};
#define LOGIN 1

#define SERVERPORT  13531
#define BUFSIZE     1024

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
void err_display(const char* msg, int err_no)
{
	WCHAR* lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER |
		FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, err_no,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	std::cout << msg;
	std::wcout << L"에러 " << lpMsgBuf << std::endl;
	while (true);
	LocalFree(lpMsgBuf);
}

int main()
{
	std::wcout.imbue(std::locale("korean"));

	int retval;

	// 윈속 초기화
	WSADATA wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
		return 1;

	// socket()
	SOCKET sock = socket(AF_INET, SOCK_STREAM, 0);
	if (sock == INVALID_SOCKET) err_quit((char*)"socket()");
	
	SOCKADDR_IN serveraddr;
	ZeroMemory(&serveraddr, sizeof(serveraddr));
	serveraddr.sin_family = AF_INET;
	serveraddr.sin_addr.s_addr = htonl(INADDR_ANY);
	serveraddr.sin_port = htons(SERVERPORT);
	retval = bind(sock, (SOCKADDR*)&serveraddr, sizeof(serveraddr));
	if (retval == SOCKET_ERROR) err_quit((char*)"bind()");

	// listen()
	retval = listen(sock, SOMAXCONN);
	if (retval == SOCKET_ERROR) err_quit((char*)"listen()");

	cout << "Server Start!" << endl;

	// 데이터 통신에 사용할 변수
	SOCKET client_sock;
	SOCKADDR_IN clientaddr;
	int addrlen;

	while (1) {
		// accept()
		addrlen = sizeof(clientaddr);
		client_sock = accept(sock, (SOCKADDR*)&clientaddr, &addrlen);
		if (client_sock == INVALID_SOCKET) {
			int err_no = WSAGetLastError();
			if (ERROR_IO_PENDING != err_no)
				err_display("accept() : ", err_no);
			break;
		}

		// 접속한 클라이언트 정보 출력
		cout << "[TCP 서버] 클라이언트 접속: IP 주소=" << inet_ntoa(clientaddr.sin_addr) << ", 포트 번호=" << ntohs(clientaddr.sin_port) << endl;

		// 클라이언트와 데이터 통신
		while (1) {
			/*retval = send(client_sock, (char*)"Hello", sizeof("Hello"), 0);
			if (retval == SOCKET_ERROR) {
				int err_no = WSAGetLastError();
				if (ERROR_IO_PENDING != err_no)
					err_display("send() : ", err_no);
				break;
			}

			char* buf = new char[BUFSIZE];
			retval = recv(client_sock, buf, BUFSIZE, 0);
			if (retval == SOCKET_ERROR) {
				int err_no = WSAGetLastError();
				if (ERROR_IO_PENDING != err_no)
					err_display("recv() : ", err_no);
				break;
			}
			cout << buf << endl;*/

			//packet p;
			//p.connect = true;
			//p.name = "hello";
			//p.score = 10000;
			//p.xyz = 3.5f;
			////p.type = LOGIN;
			////p.size = sizeof(packet);
			//send(client_sock, (char*)&p, sizeof(p), 0);

			packet t;
			recv(client_sock, (char*)&t, sizeof(t), 0);
			cout << boolalpha << t.connect << endl;
			cout << t.name << endl;
			cout << t.score << endl;
			//cout << t.size << endl;
			//cout << t.type << endl;
			cout << t.xyz << endl;
		}

		// closesocket()
		closesocket(client_sock);
		cout << "[TCP 서버] 클라이언트 종료: IP 주소=" << inet_ntoa(clientaddr.sin_addr) << ", 포트 번호=" << ntohs(clientaddr.sin_port) << endl << endl;
	}

	// closesocket()
	closesocket(sock);

	// 윈속 종료
	WSACleanup();

	return 0;
}