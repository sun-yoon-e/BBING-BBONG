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
#define MAX_CLIENT		4					// 클라이언트 수
#define MAX_ROOM_LIST_PER_PAGE 6			// 한 페이지에 표시하는 방 개수
#define MAX_ROOM_NAME_SIZE 30				// 한 글자에 2바이트 해서 15글자, UTF-8로 하면 한 글자에 3바이트인데 그럼 10글자네?
#else
#define SERVERIP		""		// GameClient.cs 에 IP 복붙
#define SERVERPORT		13531
#define ODBC_NAME       L"Pizza"
#define DBUSER          L"Pizza"
#define DBPASSWORD      L"sun0818"

#define BUF_SIZE		1024
#define MAX_BUF_SIZE	4096
#define MAX_PACKET_SIZE 512000 * 4
#define MAX_CLIENT		10					// 클라이언트 수
#define MAX_ROOM_LIST_PER_PAGE 6			// 한 페이지에 표시하는 방 개수
#define MAX_ROOM_NAME_SIZE 30				// 한 글자에 2바이트 해서 15글자
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