
// ������ ����ϱ� ���ؼ� ���̺귯�� �����ؾ� �Ѵ�.	
#pragma comment(lib, "ws2_32")	
// inet_ntoa�� deprecated�� �Ǿ��µ�.. ����Ϸ��� �Ʒ� ������ �ؾ� �Ѵ�.	
#pragma warning(disable:4996)	
#include <stdio.h>	
#include <iostream>	
#include <vector>	
#include <thread>	
// ������ ����ϱ� ���� ���̺귯��	
#include <WinSock2.h>	
// ���� ���� ������	
#define BUFFERSIZE 1024	
using namespace std;
// Ŭ���̾�Ʈ�� ���� ���� ������ ���� �Լ�	
// ���� ������ ������ ������, ������ ������ ���ŵȴ�.	
unsigned char* receive(SOCKET clientSock, int* size)
{
    // ó������ �������� ����� �´�. C++�� C#�� �⺻������ �� ������̱� ������ ����� ��ȯ�� �ʿ����.	
    // char*�� 4����Ʈ�� �޾ƹ����� int���� �ȴ�.	
    if (recv(clientSock, (char*)size, 4, 0) == SOCKET_ERROR)
    {
        cout << "error" << endl;
        return nullptr;
    }
    // �����͸� unsigned char�������� �޴´�. =byte	
    unsigned char* buffer = new unsigned char[*size];
    if (recv(clientSock, (char*)buffer, *size, 0) == SOCKET_ERROR)
    {
        cout << "error" << endl;
        return nullptr;
    }
    // ���� �����͸� �����Ѵ�.	
    return buffer;
}
// ���ӵǴ� client�� ������	
void client(SOCKET clientSock, SOCKADDR_IN clientAddr, vector<thread*>* clientlist)
{
    // ���� ������ �ֿܼ� ����Ѵ�.	
    cout << "Client connected IP address = " << inet_ntoa(clientAddr.sin_addr) << ":" << ntohs(clientAddr.sin_port) << endl;
    // �������� ������ ����	
    int size;
    // ������ ���丮	
    wchar_t buffer[BUFFERSIZE] = L"d:\\work\\";
    // ������ ���ϸ� ����	
    wchar_t filename[BUFFERSIZE];
    // �����͸� �޴´�.	
    unsigned char* data = receive(clientSock, &size);
    // ù��°�� ���ϸ��̴�. �̹����� C#���� unicode���� �ƴ� utf8�������� ���ϸ��� ���´�.	
    // c++������ unicode�� �ٷ�� ������ ��ȯ�� �ʿ��ϴ�. (MB_ERR_INVALID_CHARS)	
    MultiByteToWideChar(CP_UTF8, 0, (const char*)data, size, filename, BUFFERSIZE);
    // ���� �����͸� �޸𸮿��� ����	
    delete data;
    // ���丮 + ���ϸ�	
    wcscat(buffer, filename);
    // �����͸� �ٽ� �޴´�. �̹��� ���ε��ϴ� ���� �������̴�.	
    data = receive(clientSock, &size);
    // ������ ���� ��ü�� �޴´�.	
    FILE* fp = _wfopen(buffer, L"wb");
    if (fp != NULL)
    {
        // ���� ����  	
        fwrite(data, 1, size, fp);
        // ���� �ݱ�	
        fclose(fp);
    }
    else
    {
        // ���� ��ü ��濡 ������ ��� �ܼ� ���� ǥ��	
        cout << "File open failed" << endl;
    }
    // ���� �����͸� �޸𸮿��� ����	
    delete data;
    // �۽� ������ ���� byte=1�� ������ �۽� �Ϸ�.	
    char ret[1] = { 1 };
    // Ŭ���̾�Ʈ�� �Ϸ� ��Ŷ�� ������.	
    send(clientSock, ret, 1, 0);
    // ������ �ݴ´�.	
    closesocket(clientSock);
    // ���� ���� ������ �ֿܼ� ����Ѵ�.	
    cout << "Client disconnected IP address = " << inet_ntoa(clientAddr.sin_addr) << ":" << ntohs(clientAddr.sin_port) << endl;
    // �����忡�� �����带 �����Ѵ�.	
    for (auto ptr = clientlist->begin(); ptr < clientlist->end(); ptr++)
    {
        if ((*ptr)->get_id() == this_thread::get_id())
        {
            clientlist->erase(ptr);
            break;
        }
    }
}
// ���� �Լ�	
int main()
{
    // Ŭ���̾�Ʈ ���� ���� client list	
    vector<thread*> clientlist;
    // ���� ���� ������ ����	
    WSADATA wsaData;
    // ���� ����.	
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
    {
        return 1;
    }
    // Internet�� Stream ������� ���� ����	
    SOCKET serverSock = socket(PF_INET, SOCK_STREAM, 0);
    // ���� �ּ� ����	
    SOCKADDR_IN addr;
    // ����ü �ʱ�ȭ	
    memset(&addr, 0, sizeof(addr));
    // ������ Internet Ÿ��	
    addr.sin_family = AF_INET;
    // �����̱� ������ local �����Ѵ�.	
    // Any�� ���� ȣ��Ʈ�� 127.0.0.1�� ��Ƶ� �ǰ� localhost�� ��Ƶ� �ǰ� ���� �� ����ϰ� �� �� �ֵ�. �װ��� INADDR_ANY�̴�.	
    addr.sin_addr.s_addr = htonl(INADDR_ANY);
    // ���� ��Ʈ ����...���� 9090���� ������.	
    addr.sin_port = htons(9090);
    // ������ ���� ������ ���Ͽ� ���ε��Ѵ�.	
    if (bind(serverSock, (SOCKADDR*)&addr, sizeof(SOCKADDR_IN)) == SOCKET_ERROR)
    {
        // ���� �ܼ� ���	
        cout << "error" << endl;
        return 1;
    }
    // ������ ��� ���·� ��ٸ���.	
    if (listen(serverSock, SOMAXCONN) == SOCKET_ERROR)
    {
        // ���� �ܼ� ���	
        cout << "error" << endl;
        return 1;
    }
    // ������ �����Ѵ�.	
    cout << "Server Start" << endl;
    // ���� ������ ���� while�� ������ ����Ѵ�.	
    while (1)
    {
        // ���� ���� ����ü ������	
        int len = sizeof(SOCKADDR_IN);
        // ���� ���� ����ü	
        SOCKADDR_IN clientAddr;
        // client�� ������ �ϸ� SOCKET�� �޴´�.	
        SOCKET clientSock = accept(serverSock, (SOCKADDR*)&clientAddr, &len);
        // �����带 �����ϰ� ������ ����Ʈ�� �ִ´�.	
        clientlist.push_back(new thread(client, clientSock, clientAddr, &clientlist));
    }
    // ���ᰡ �Ǹ� ������ ����Ʈ�� ���� �ִ� �����带 ������ ������ ��ٸ���.	
    if (clientlist.size() > 0)
    {
        for (auto ptr = clientlist.begin(); ptr < clientlist.end(); ptr++)
        {
            (*ptr)->join();
        }
    }
    // ���� ���� ����	
    closesocket(serverSock);
    // ���� ����	
    WSACleanup();
    return 0;
}