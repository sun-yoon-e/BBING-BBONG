#pragma comment(lib, "ws2_32")
#include <iostream>
#include <string>
#include <winsock2.h>
using namespace std;

#define PORT_NUM 13531

SOCKET socket_;
char buffer[1024]; // 담을 버퍼의 양
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
        //클라이언트에서 오는 패킷을 체크
        memset((void*)&buffer, 0x00, sizeof(buffer));
        addrlen = sizeof(clientAddr);
        recvfrom(socket_, (char*)&buffer, sizeof(buffer), 0, (SOCKADDR*)&clientAddr, &addrlen);
        bufferManager(buffer, clientAddr);
    }
}

// 버퍼 처리 , 여기서 받은 버퍼 정보를 처리하여 캐릭터를 움직이든 아이템을 획득 하든 모든지 처리
void bufferManager(string b, SOCKADDR_IN s)
{
    cout << "cleint - >" << b << endl;  // 클라이언트에서 b 라는 정보를 보내왔다.
    // 이제 서버에서 b 라는 정보를 정제하여 쓰고 클라이언트에게 필요한 정보를 주거나 하자
    string n = "클라이언트야 " + b + " 정보 잘 받았다 고맙소"; // 이런 식으로 정제 하여 사용하자
    sendBuffer(n, s); // 정제한 정보를 클라이언트로 다시 보낸다. SOCKADDR_IN 은 패킷을 보내온 클라이언트의 ip가 들어있다. 
}

void sendBuffer(string buff, SOCKADDR_IN s) // 클라이언트로 패킷 보내기
{
    sendto(socket_, buff.c_str(), buff.length(), 0, (SOCKADDR*)&s, addrlen); // 클라이언트로 정제된 버퍼를 패킷으로 보낸다
    cout << "Server-> " << buff << endl;
}

//만약 Thread 를 써야 한다면 33번줄의  if (bind(socket_, (SOCKADDR*)&addr, addrlen) == -1) 여기에서 
//if (::bind(socket_, (SOCKADDR*)&addr, addrlen) == -1)  bind 앞에 :: 를 꼭 붙이도록 하자 아니면 오류남