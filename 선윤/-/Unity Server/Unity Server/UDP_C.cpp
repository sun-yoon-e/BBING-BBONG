#pragma comment(lib, "ws2_32")
#include <winsock2.h>
#include <stdio.h>

#define PORT_NUM 13531

void ClientFirstConnent(SOCKADDR_IN sock_in);
void Client_Play_Move(char buffer_[1024]);

SOCKADDR_IN client_addr[100];
int Client_MaxNum = 0;
int data_[1024];
int data_num = 0;
SOCKET socket_;
char buffer[1024];
int addrlen;

int main() {

    struct sockaddr_in addr, cliaddr;
    SOCKADDR_IN temp_clientAddr;
    WSADATA wsaData;

    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != NO_ERROR)
    {
        return 1;
    }

    if ((socket_ = socket(AF_INET, SOCK_DGRAM, 0)) == -1) {
        printf("server...2");
        return 1;
    }

    memset((void*)&addr, 0x00, sizeof(addr));
    addr.sin_family = AF_INET;
    addr.sin_addr.s_addr = htonl(INADDR_ANY);
    addr.sin_port = htons(PORT_NUM);
    addrlen = sizeof(addr);

    if (bind(socket_, (struct sockaddr*)&addr, addrlen) == -1)
    {
        return 1;
    }

    while (1)
    {
        //memset((void*)&xy_byte, 0x00, sizeof(xy_byte));
        memset((void*)&buffer, 0x00, sizeof(buffer));
        addrlen = sizeof(temp_clientAddr);
        recvfrom(socket_, (char*)&buffer, sizeof(buffer), 0, (struct sockaddr*)&temp_clientAddr, &addrlen);
        printf("IP : %s , Value :%s\n ", inet_ntoa(temp_clientAddr.sin_addr), buffer);

        char temp_buffer[1024];
        memset((void*)&temp_buffer, 0x00, sizeof(temp_buffer));
        //*temp_buffer = buffer; 
        printf("strtok:  %s\n", temp_buffer);

        for (int i = 0; i < sizeof(buffer) / sizeof(buffer[0]); i++) {
            temp_buffer[i] = buffer[i];
        }

        char* p;
        p = strtok(temp_buffer, "_");

        while (p != NULL)
        {
            data_[data_num] = atoi(p);
            p = strtok(NULL, "_");
            //printf("data [%d] : %d\n",data_num,data_[data_num]);
            data_num++;
        }

        switch (data_[0])
        {
        case 0:
            ClientFirstConnent(temp_clientAddr);
            break;

        case 1:
            Client_Play_Move(buffer);
            printf("%s\n", buffer);
            break;
        }

        data_num = 0;
    }

    closesocket(socket_);
    WSACleanup();
    return 0;
}

void ClientFirstConnent(SOCKADDR_IN sock_in)
{
    Client_MaxNum++;
    client_addr[Client_MaxNum] = sock_in;
    printf("%d.connent : %s\n ", Client_MaxNum, inet_ntoa(client_addr[Client_MaxNum].sin_addr));
    itoa(Client_MaxNum, buffer, 10);
    sendto(socket_, (char*)&buffer, sizeof(buffer), 0, (struct sockaddr*)&sock_in, addrlen);

    if (Client_MaxNum > 1) {
        for (int i = 1; i < Client_MaxNum; i++) {
            char temp_c[1024] = "0_1";
            sendto(socket_, (char*)&temp_c, sizeof(temp_c), 0, (struct sockaddr*)&client_addr[i], addrlen);
        }
    }
}

void Client_Play_Move(char buffer_[1024]) 
{
    printf("Clinet_Player_Move Vlaue: %s\n", &buffer);
    for (int i = 0; i < Client_MaxNum; i++) {
        sendto(socket_, (char*)&buffer, sizeof(buffer), 0, (struct sockaddr*)&client_addr[i + 1], addrlen);
    }

}