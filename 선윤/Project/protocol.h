#pragma once

#pragma pack (push, 1)

#define SC_ENTER	0
#define SC_LOGIN	1
#define SC_LOGOUT	2
//#define SC_MOVE		3
#define SC_CHAT		4
#define SC_ITEM		5

#define CS_LOGIN	0
#define CS_LOGOUT	1
#define CS_MOVE		2
#define CS_CHAT		3
#define CS_CLICK	4

struct sc_packet_enter {
	BYTE size;
	BYTE type;
	int id;
};

struct sc_packet_login {
	BYTE size;
	BYTE type;
	int id;

	char nick[16];
	char pw[16];
};

struct sc_packet_logout {
	BYTE size;
	BYTE type;
	int id;
};

struct sc_packet_move {
	BYTE size;
	BYTE type;
	int id;

	float x;
	float y;
	float z;
};

struct sc_packet_chat {
	BYTE size;
	BYTE type;
	int id;

	string message;
};

struct sc_packet_item {
	BYTE size;
	BYTE type;
	int id;
};

struct cs_packet_login_nk {
	int id;
	char nick[16];
	BYTE size;
	BYTE type;
};

struct cs_packet_login_pw {
	int id;
	char pw[16];
	BYTE size;
	BYTE type;
};

struct cs_packet_move {
	BYTE size;
	BYTE type;
	int id;

	float x;
	float y;
	float z;
};

#pragma pack (pop)