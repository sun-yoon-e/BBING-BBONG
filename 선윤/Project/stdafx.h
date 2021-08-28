#pragma once

#pragma comment(lib, "ws2_32")

#include <winsock2.h>
#include <windows.h>
#include <iostream>
#include <string>
#include <thread>
#include <mutex>
#include <vector>
#include <stack>
#include <unordered_map>
#include <random>

using namespace std;

#define Debug			FALSE

//#define TEST        

#ifdef TEST
#define SERVERIP		"127.0.0.1"
#define SERVERPORT		13531
#define ODBC_NAME       L"Pizza"
#define DBUSER          L"Pizza"
#define DBPASSWORD      L"sun0818"

#define BUF_SIZE		1024
#define MAX_BUF_SIZE	4096
#define MAX_PACKET_SIZE 512000 * 4
#define MAX_CLIENT		4					// Ŭ���̾�Ʈ ��
#define MAX_ROOM_LIST_PER_PAGE 6			// �� �������� ǥ���ϴ� �� ����
#define MAX_ROOM_NAME_SIZE 30				// �� ���ڿ� 2����Ʈ �ؼ� 15����, UTF-8�� �ϸ� �� ���ڿ� 3����Ʈ�ε� �׷� 10���ڳ�?
#else
#define SERVERIP		""		// GameClient.cs �� IP ����
#define SERVERPORT		13531
#define ODBC_NAME       L"Pizza"
#define DBUSER          L"Pizza"
#define DBPASSWORD      L"sun0818"

#define BUF_SIZE		1024
#define MAX_BUF_SIZE	4096
#define MAX_PACKET_SIZE 512000 * 4
#define MAX_CLIENT		10					// Ŭ���̾�Ʈ ��
#define MAX_ROOM_LIST_PER_PAGE 6			// �� �������� ǥ���ϴ� �� ����
#define MAX_ROOM_NAME_SIZE 30				// �� ���ڿ� 2����Ʈ �ؼ� 15����
#endif
#define XSIZE			200
#define ZSIZE			200
#define MAX_ROOM_NUMBER 1000
#define MAX_CHAT_SIZE	231
#define MAX_NICKNAME_SIZE 20			
#define OTHER_PACKET_SIZE_MAX 255

#define InsertIfNotExist(m, k, v) \
	if(m.find(k) == m.end()) {	\
		m[k] = v; \
	}

#define RemoveIfExist(m, k) \
	if(m.find(k) != m.end()) { \
		m.erase(k);\
	}