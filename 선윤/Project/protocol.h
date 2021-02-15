#pragma once

#define SC_LOGIN	1
#define SC_LOGOUT	2
#define SC_POS		3
#define SC_CHAT		4
#define SC_ITEM		5

#define CS_UP		1
#define	CS_DOWN		2
#define CS_LEFT		3
#define CS_RIGHT	4
#define CS_CHAT		5
#define CS_CLICK	6
#define	CS_MOVE		7

struct sc_packet_login {
	char size;
	char type;

	int id;
};

struct sc_packet_logout {
	char size;
	char type;

	int id;
};

struct sc_packet_pos {
	char size;
	char type;
	
	float x;
	float y;
	float z;
};

struct sc_packet_chat {
	char size;
	char type;

	int id;
	string message;
};

struct sc_packet_item {
	char size;
	char type;

	int id;
};

struct cs_packet_move {
	char size;
	char type;

	float x;
	float y;
	float z;
};