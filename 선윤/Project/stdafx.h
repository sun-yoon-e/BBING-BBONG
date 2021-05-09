#pragma once

#pragma comment(lib, "ws2_32")

#include <winsock2.h>
#include <windows.h>
#include <iostream>
#include <string>
#include <thread>
#include <mutex>
#include <vector>
#include <unordered_map>
#include <random>

using namespace std;

#include "protocol.h"

#define Debug			TRUE

#define SERVERPORT		13531
#define ODBC_NAME       L"SUN-Pizza"
#define DBUSER          L"sa"
#define DBPASSWORD      L"test1234"

#define BUF_SIZE		1024
#define MAX_BUF_SIZE	4096
#define MAX_PACKET_SIZE  255
#define MAX_CLIENT		  10