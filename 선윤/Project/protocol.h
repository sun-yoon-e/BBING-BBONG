#pragma once
#include "stdafx.h"

#pragma pack (push, 1)

#define SC_LOGIN	0
#define SC_LOGOUT	1
#define SC_SIGNUP   2 
#define SC_MOVE		3
#define SC_CHAT		4
#define SC_ITEM		5
#define SC_SCORE    6
#define SC_GAMESTATE 7
#define SC_FIRE     8

#define SC_MESH     10
#define SC_SET_MESH 11
#define SC_ROAD     12
#define SC_SET_ROAD 13

#define CS_LOGIN	0
#define CS_LOGOUT	1
#define CS_SIGNUP   2
#define CS_MOVE		3
#define CS_CHAT		4
#define CS_CLICK	5
#define CS_SCORE    6
#define CS_FIRE     8
#define CS_MESH     10
#define CS_SET_MESH 11
#define CS_ROAD     12
#define CS_SET_ROAD 13

struct Vector3 {
	float x;
	float y;
	float z;
};

struct Packet_Login {
	BYTE type = CS_LOGIN;
	char username[32];
	char password[32];
};

struct Packet_Login_SC {
	BYTE type = SC_LOGIN;
	BYTE success;
	int clientId = -1;
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

struct Packet_Request_Mesh {
	BYTE type = CS_MESH;
};

struct Packet_Request_Mesh_SC {
	BYTE type = SC_MESH;
	BYTE ready;
	Vector3 vertices[(XSIZE + 1) * (ZSIZE + 1)];
	int32_t triangles[XSIZE * ZSIZE * 6];
};

struct Packet_Set_Mesh {
	BYTE type = CS_SET_MESH;
	Vector3 vertices[(XSIZE + 1) * (ZSIZE + 1)];
	int32_t triangles[XSIZE * ZSIZE * 6];
};

struct Packet_Set_Mesh_SC {
	BYTE type = SC_SET_MESH;
	BYTE ready;
	Vector3 vertices[(XSIZE + 1) * (ZSIZE + 1)];
	int32_t triangles[XSIZE * ZSIZE * 6];
};

struct Packet_Request_Road {
	BYTE type = CS_ROAD;
};

struct Packet_Request_Road_SC {
	BYTE type = SC_ROAD;
	BYTE ready;
	Vector3 vertices[(XSIZE + 1) * (ZSIZE + 1)];
	int32_t triangles[XSIZE * ZSIZE * 6];
	bool isRoad[(XSIZE + 1) * (ZSIZE + 1)];
	int16_t isBuildingPlace[(XSIZE + 1) * (ZSIZE + 1)];
};

struct Packet_Set_Road {
	BYTE type = CS_SET_ROAD;
	Vector3 vertices[(XSIZE + 1) * (ZSIZE + 1)];
	int32_t triangles[XSIZE * ZSIZE * 6];
	bool isRoad[(XSIZE + 1) * (ZSIZE + 1)];
	int16_t isBuildingPlace[(XSIZE + 1) * (ZSIZE + 1)];
};

struct Packet_Set_Road_SC {
	BYTE type = SC_SET_ROAD;
	BYTE ready;
	Vector3 vertices[(XSIZE + 1) * (ZSIZE + 1)];
	int32_t triangles[XSIZE * ZSIZE * 6];
	bool isRoad[(XSIZE + 1) * (ZSIZE + 1)];
	int16_t isBuildingPlace[(XSIZE + 1) * (ZSIZE + 1)];
};

struct Packet_Score {
	BYTE type = CS_SCORE;
};

struct Packet_Score_SC {
	BYTE type = SC_SCORE;
	int32_t players;
	int32_t scores[MAX_CLIENT];
};

struct Packet_Move {
	BYTE type = CS_MOVE;
	Vector3 position;
	Vector3 rotation;
};

struct Packet_Move_SC {
	BYTE type = SC_MOVE;
	int32_t players;
	Vector3 position[MAX_CLIENT];
	Vector3 rotation[MAX_CLIENT];
};

struct Packet_GameState_SC {
	BYTE type = SC_GAMESTATE;
	BYTE state;
};

struct Packet_Fire {
	BYTE TYPE = CS_FIRE;
	Vector3 position;
	Vector3 targetPosition;
};

struct Packet_Fire_SC {
	BYTE TYPE = SC_FIRE;
	Vector3 position;
	Vector3 targetPosition;
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