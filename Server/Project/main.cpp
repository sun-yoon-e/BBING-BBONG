#include "Server.h"

#include <ctime>

int main()
{
    _wsetlocale(LC_ALL, L"korean");

	// �� ũ�� ������ Stack overflow �߻��Ͽ� ���� ��ü �����ϵ��� ����
	auto* server = new Server();
	server->InitServer();
	server->StartServer();
	delete server;

	return 0;
}