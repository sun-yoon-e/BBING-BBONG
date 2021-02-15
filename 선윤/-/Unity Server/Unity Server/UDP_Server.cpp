#pragma comment(lib, "ws2_32")
#include <iostream>
#include <string>
#include <winsock2.h>
using namespace std;

#define PORT_NUM 13531

SOCKET socket_;
char buffer[1024]; // ���� ������ ��
int addrlen = 0;
SOCKADDR_IN clientAddr;

void bufferManager(string b, SOCKADDR_IN s);
void sendBuffer(string buff, SOCKADDR_IN s);

int main()
{
    SOCKADDR_IN addr;
    WSADATA wsaData;

    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != NO_ERROR)
    {
        cout << "error...1";
        return 1;
    }

    if ((socket_ = socket(AF_INET, SOCK_DGRAM, 0)) == -1)
    {
        cout << "error...2";
        return 1;
    }

    memset((void*)&addr, 0x00, sizeof(addr));
    addr.sin_family = AF_INET;
    addr.sin_addr.s_addr = htonl(INADDR_ANY);
    addr.sin_port = htons(PORT_NUM);
    addrlen = sizeof(addr);

    if (bind(socket_, (SOCKADDR*)&addr, addrlen) == -1)
    {
        cout << "error...3";
        return 1;
    }
    cout << "start server !";

    while (1)
    {
        //Ŭ���̾�Ʈ���� ���� ��Ŷ�� üũ
        memset((void*)&buffer, 0x00, sizeof(buffer));
        addrlen = sizeof(clientAddr);
        recvfrom(socket_, (char*)&buffer, sizeof(buffer), 0, (SOCKADDR*)&clientAddr, &addrlen);
        bufferManager(buffer, clientAddr);
    }
}

// ���� ó�� , ���⼭ ���� ���� ������ ó���Ͽ� ĳ���͸� �����̵� �������� ȹ�� �ϵ� ����� ó��
void bufferManager(string b, SOCKADDR_IN s)
{
    cout << "cleint - >" << b << endl;  // Ŭ���̾�Ʈ���� b ��� ������ �����Դ�.
    // ���� �������� b ��� ������ �����Ͽ� ���� Ŭ���̾�Ʈ���� �ʿ��� ������ �ְų� ����
    string n = "Ŭ���̾�Ʈ�� " + b + " ���� �� �޾Ҵ� ����"; // �̷� ������ ���� �Ͽ� �������
    sendBuffer(n, s); // ������ ������ Ŭ���̾�Ʈ�� �ٽ� ������. SOCKADDR_IN �� ��Ŷ�� ������ Ŭ���̾�Ʈ�� ip�� ����ִ�. 
}

void sendBuffer(string buff, SOCKADDR_IN s) // Ŭ���̾�Ʈ�� ��Ŷ ������
{
    sendto(socket_, buff.c_str(), buff.length(), 0, (SOCKADDR*)&s, addrlen); // Ŭ���̾�Ʈ�� ������ ���۸� ��Ŷ���� ������
    cout << "Server-> " << buff << endl;
}

//���� Thread �� ��� �Ѵٸ� 33������  if (bind(socket_, (SOCKADDR*)&addr, addrlen) == -1) ���⿡�� 
//if (::bind(socket_, (SOCKADDR*)&addr, addrlen) == -1)  bind �տ� :: �� �� ���̵��� ���� �ƴϸ� ������