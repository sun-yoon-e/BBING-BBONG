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

#define Debug			TRUE

#define SERVERIP		"14.38.247.250"
#define SERVERPORT		13531
#define ODBC_NAME       L"Pizza"
#define DBUSER          L"Pizza"
#define DBPASSWORD      L"sun0818"

#define BUF_SIZE		1024
#define MAX_BUF_SIZE	4096
#define MAX_PACKET_SIZE 512000
#define MAX_CLIENT		10

#define XSIZE 100
#define ZSIZE 100