
// 소켓을 사용하기 위해서 라이브러리 참조해야 한다.	
#pragma comment(lib, "ws2_32")	
// inet_ntoa가 deprecated가 되었는데.. 사용하려면 아래 설정을 해야 한다.	
#pragma warning(disable:4996)	
#include <stdio.h>	
#include <iostream>	
#include <vector>	
#include <thread>	
// 소켓을 사용하기 위한 라이브러리	
#include <WinSock2.h>	
// 수신 버퍼 사이즈	
#define BUFFERSIZE 1024	
using namespace std;
// 클라이언트로 부터 오는 데이터 수신 함수	
// 수신 순서가 데이터 사이즈, 데이터 순으로 수신된다.	
unsigned char* receive(SOCKET clientSock, int* size)
{
    // 처음에는 데이터의 사이즈가 온다. C++과 C#은 기본적으로 빅 엔디안이기 때문에 엔디안 변환은 필요없다.	
    // char*를 4바이트로 받아버리면 int형이 된다.	
    if (recv(clientSock, (char*)size, 4, 0) == SOCKET_ERROR)
    {
        cout << "error" << endl;
        return nullptr;
    }
    // 데이터를 unsigned char형식으로 받는다. =byte	
    unsigned char* buffer = new unsigned char[*size];
    if (recv(clientSock, (char*)buffer, *size, 0) == SOCKET_ERROR)
    {
        cout << "error" << endl;
        return nullptr;
    }
    // 받은 데이터를 리턴한다.	
    return buffer;
}
// 접속되는 client별 쓰레드	
void client(SOCKET clientSock, SOCKADDR_IN clientAddr, vector<thread*>* clientlist)
{
    // 접속 정보를 콘솔에 출력한다.	
    cout << "Client connected IP address = " << inet_ntoa(clientAddr.sin_addr) << ":" << ntohs(clientAddr.sin_port) << endl;
    // 데이터의 사이즈 변수	
    int size;
    // 저장할 디렉토리	
    wchar_t buffer[BUFFERSIZE] = L"d:\\work\\";
    // 저장할 파일명 변수	
    wchar_t filename[BUFFERSIZE];
    // 데이터를 받는다.	
    unsigned char* data = receive(clientSock, &size);
    // 첫번째는 파일명이다. 이번에는 C#에서 unicode식이 아닌 utf8형식으로 파일명을 보냈다.	
    // c++에서는 unicode를 다루기 때문에 변환이 필요하다. (MB_ERR_INVALID_CHARS)	
    MultiByteToWideChar(CP_UTF8, 0, (const char*)data, size, filename, BUFFERSIZE);
    // 수신 데이터를 메모리에서 삭제	
    delete data;
    // 디렉토리 + 파일명	
    wcscat(buffer, filename);
    // 데이터를 다시 받는다. 이번엔 업로드하는 파일 데이터이다.	
    data = receive(clientSock, &size);
    // 저장할 파일 객체를 받는다.	
    FILE* fp = _wfopen(buffer, L"wb");
    if (fp != NULL)
    {
        // 파일 저장  	
        fwrite(data, 1, size, fp);
        // 파일 닫기	
        fclose(fp);
    }
    else
    {
        // 파일 객체 취득에 실패할 경우 콘솔 에러 표시	
        cout << "File open failed" << endl;
    }
    // 수신 데이터를 메모리에서 삭제	
    delete data;
    // 송신 데이터 선언 byte=1를 보내면 송신 완료.	
    char ret[1] = { 1 };
    // 클라이언트로 완료 패킷을 보낸다.	
    send(clientSock, ret, 1, 0);
    // 소켓을 닫는다.	
    closesocket(clientSock);
    // 접속 종료 정보를 콘솔에 출력한다.	
    cout << "Client disconnected IP address = " << inet_ntoa(clientAddr.sin_addr) << ":" << ntohs(clientAddr.sin_port) << endl;
    // 쓰레드에서 쓰레드를 제거한다.	
    for (auto ptr = clientlist->begin(); ptr < clientlist->end(); ptr++)
    {
        if ((*ptr)->get_id() == this_thread::get_id())
        {
            clientlist->erase(ptr);
            break;
        }
    }
}
// 실행 함수	
int main()
{
    // 클라이언트 접속 중인 client list	
    vector<thread*> clientlist;
    // 소켓 정보 데이터 설정	
    WSADATA wsaData;
    // 소켓 실행.	
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
    {
        return 1;
    }
    // Internet의 Stream 방식으로 소켓 생성	
    SOCKET serverSock = socket(PF_INET, SOCK_STREAM, 0);
    // 소켓 주소 설정	
    SOCKADDR_IN addr;
    // 구조체 초기화	
    memset(&addr, 0, sizeof(addr));
    // 소켓은 Internet 타입	
    addr.sin_family = AF_INET;
    // 서버이기 때문에 local 설정한다.	
    // Any인 경우는 호스트를 127.0.0.1로 잡아도 되고 localhost로 잡아도 되고 양쪽 다 허용하게 할 수 있따. 그것이 INADDR_ANY이다.	
    addr.sin_addr.s_addr = htonl(INADDR_ANY);
    // 서버 포트 설정...저는 9090으로 설정함.	
    addr.sin_port = htons(9090);
    // 설정된 소켓 정보를 소켓에 바인딩한다.	
    if (bind(serverSock, (SOCKADDR*)&addr, sizeof(SOCKADDR_IN)) == SOCKET_ERROR)
    {
        // 에러 콘솔 출력	
        cout << "error" << endl;
        return 1;
    }
    // 소켓을 대기 상태로 기다린다.	
    if (listen(serverSock, SOMAXCONN) == SOCKET_ERROR)
    {
        // 에러 콘솔 출력	
        cout << "error" << endl;
        return 1;
    }
    // 서버를 시작한다.	
    cout << "Server Start" << endl;
    // 다중 접속을 위해 while로 소켓을 대기한다.	
    while (1)
    {
        // 접속 설정 구조체 사이즈	
        int len = sizeof(SOCKADDR_IN);
        // 접속 설정 구조체	
        SOCKADDR_IN clientAddr;
        // client가 접속을 하면 SOCKET을 받는다.	
        SOCKET clientSock = accept(serverSock, (SOCKADDR*)&clientAddr, &len);
        // 쓰레드를 실행하고 쓰레드 리스트에 넣는다.	
        clientlist.push_back(new thread(client, clientSock, clientAddr, &clientlist));
    }
    // 종료가 되면 쓰레드 리스트에 남아 있는 쓰레드를 종료할 때까지 기다린다.	
    if (clientlist.size() > 0)
    {
        for (auto ptr = clientlist.begin(); ptr < clientlist.end(); ptr++)
        {
            (*ptr)->join();
        }
    }
    // 서버 소켓 종료	
    closesocket(serverSock);
    // 소켓 종료	
    WSACleanup();
    return 0;
}