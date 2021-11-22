#include "Server.h"

#include <ctime>

int main()
{
    _wsetlocale(LC_ALL, L"korean");

	// 맵 크기 증가로 Stack overflow 발생하여 힙에 객체 생성하도록 변경
	auto* server = new Server();
	server->InitServer();
	server->StartServer();
	delete server;

	return 0;
}