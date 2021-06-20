#include "Server.h"

int main()
{
    _wsetlocale(LC_ALL, L"korean");
	Server server;

	server.InitServer();
	server.StartServer();
	return 0;
}