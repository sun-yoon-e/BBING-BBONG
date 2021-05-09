#pragma once

#pragma pack (push, 1)

#define SC_LOGIN	0
#define SC_LOGOUT	1
#define SC_SIGNUP   2 
#define SC_MOVE		3
#define SC_CHAT		4
#define SC_ITEM		5

#define CS_LOGIN	0
#define CS_LOGOUT	1
#define CS_SIGNUP   2
#define CS_MOVE		3
#define CS_CHAT		4
#define CS_CLICK	5

struct Packet_Login {
	BYTE type = CS_LOGIN;
	char username[32];
	char password[32];
};

struct Packet_Login_SC {
	BYTE type = SC_LOGIN;
	BYTE success;
};

struct Packet_SignUp {
	BYTE type = CS_SIGNUP;
	char username[32];
	char password[32];
};

struct Packet_SignUp_SC {
	BYTE type = SC_SIGNUP;
	BYTE success;
};

struct sc_packet_enter {
	BYTE type;
	BYTE size;
	int id;
};

struct sc_packet_login {
	BYTE type;
	BYTE size;
	int id;

	char nick[16];
	char pw[16];
};

struct sc_packet_logout {
	BYTE type;
	BYTE size;
	int id;
};

struct sc_packet_move {
	BYTE type;
	BYTE size;
	int id;

	float x;
	float y;
	float z;
};

struct sc_packet_chat {
	BYTE type;
	BYTE size;
	int id;

	string message;
};

struct sc_packet_item {
	BYTE type;
	BYTE size;
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