#pragma once
#include "stdafx.h"

#pragma pack (push, 1)

#define SC_LOGIN			  100
#define SC_LOGOUT			  101
#define SC_SIGNUP			  102 

#define SC_MOVE					3
#define SC_CHAT					4
#define SC_ITEM					5
#define SC_SCORE				6
#define SC_GAMESTATE			7
#define SC_FIRE					8

#define SC_MESH					10
#define SC_SET_MESH				11
#define SC_ROAD					12
#define SC_SET_ROAD				13
#define SC_INIT					14
#define SC_ROOM_LIST_INFO		15
#define SC_ROOM_INFO			16
#define SC_ROOM_PLAYER_INOUT	17
#define SC_PLAYER_READY			18

#define SC_PLACE_ITEM			21
#define SC_REMOVE_ITEM			22
#define SC_USE_ITEM				23

#define SC_AI_MOVE				24
#define SC_AI_FIRE				25
#define SC_MAKE_CAR				26
#define SC_MOVE_CAR				31
#define SC_DESTROY_CAR			27
#define SC_AI_ADD				28
#define SC_AI_REMOVE			29
#define SC_MAKE_TREE			30
#define SC_MAKE_BUILDING		32
#define SC_MAKE_PIZZASTORE		33

// -----------------------------------

#define CS_LOGIN			  100
#define CS_LOGOUT			  101
#define CS_SIGNUP			  102

#define CS_MOVE					3
#define CS_CHAT					4
#define CS_CLICK				5
#define CS_SCORE				6

#define CS_FIRE					8

#define CS_MESH					10
#define CS_SET_MESH				11
#define CS_ROAD					12
#define CS_SET_ROAD				13

#define CS_MAKE_ROOM			14
#define CS_ENTER_ROOM			15
#define CS_EXIT_ROOM			16
#define CS_GAMESTATE			17
#define CS_ROOM_LIST_INFO		18
#define CS_ROOM_INFO			19
#define CS_PLAYER_READY			20

#define CS_PLACE_ITEM			21
#define CS_REMOVE_ITEM			22
#define CS_USE_ITEM				23

#define CS_AI_MOVE				24
#define CS_AI_FIRE				25
#define CS_MAKE_CAR				26
#define CS_MOVE_CAR				31
#define CS_DESTROY_CAR			27
#define CS_AI_ADD				28
#define CS_AI_REMOVE			29
#define CS_MAKE_TREE			30
#define CS_MAKE_BUILDING		32
#define CS_MAKE_PIZZASTORE		33

#define PACKET_CMD_MAX		   200

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
	int id;
};

struct Packet_Score_SC {
	BYTE type = SC_SCORE;
	int id;
	int score;
};
//struct Packet_Score_SC {
//	BYTE type = SC_SCORE;
//	int32_t players;
//	int32_t scores[MAX_CLIENT];
//};

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

// 클라이언트에서 전송하는 게임 시작 명령
struct Packet_GameState_CS
{
	BYTE type = CS_GAMESTATE;
	BYTE state;
};

struct Packet_Fire {
	BYTE TYPE = CS_FIRE;
	Vector3 position;
	Vector3 targetPosition;
	int playerIndex;
};

struct Packet_Fire_SC {
	BYTE TYPE = SC_FIRE;
	Vector3 position;
	Vector3 targetPosition;
	int playerIndex;
};

struct Packet_GameInit_SC {
    BYTE type = SC_INIT;
    int32_t scene;
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
	BYTE nickName[MAX_NICKNAME_SIZE];
	BYTE message[MAX_CHAT_SIZE];

	static BYTE* GetChatPacket(BYTE* buffer, BYTE* pNickName)
	{
		auto* packet = new BYTE[sizeof(sc_packet_chat)];

		packet[0] = SC_CHAT;

		memcpy_s(packet + 1 , MAX_NICKNAME_SIZE, pNickName, MAX_NICKNAME_SIZE);
		memcpy_s(packet + 1 + MAX_NICKNAME_SIZE, MAX_CHAT_SIZE, buffer + 1, MAX_CHAT_SIZE);

		return packet;
	}
};

// 로비에 있는 현재 페이지 방 정보 요청
struct cs_packet_room_list
{
	BYTE type = CS_ROOM_LIST_INFO;
	// 방 정보를 불러올 페이지 번호 입력(0 부터 시작)
	BYTE page;
};

// 특정 아이디의 방 정보 요청
struct cs_packet_room_info
{
	BYTE type = CS_ROOM_INFO;
	int roomId;
};

// 방 정보
struct sc_packet_room_info
{
	BYTE type = SC_ROOM_INFO;
	// 로비에 있는 방 번호(1번 부터 시작, 0번은 없는 방)
	int roomId;
	// 게임중인지
	bool inPlaying;
	// 방에 있는 플레이어 숫자(1~4)
	BYTE playerNum;
	// 로비에 있는 방 이름
	char roomName[MAX_ROOM_NAME_SIZE];
};

// 로비 현재 페이지의 방 정보들
struct sc_packet_room_list
{
	BYTE type = SC_ROOM_LIST_INFO;
	// 요청한 페이지에 있는 방 정보(최대 6개)
	sc_packet_room_info roomInfo[MAX_ROOM_LIST_PER_PAGE];
};

// 방에 입장
struct cs_packet_enter_room
{
	BYTE type = CS_ENTER_ROOM;
	int roomId;
};

// 방에서 퇴장
struct cs_packet_exit_room
{
	BYTE type = CS_EXIT_ROOM;
	int roomId;
};

// 방에 플레이어 입장
struct sc_packet_room_player
{
	BYTE type = SC_ROOM_PLAYER_INOUT;
	char nickName[MAX_NICKNAME_SIZE * 4];
};

// 방 생성
struct cs_packet_make_room
{
	BYTE type = CS_MAKE_ROOM;
	char roomName[MAX_ROOM_NAME_SIZE];
};

// 플레이어 준비 클릭
struct cs_packet_player_ready {
	BYTE type = CS_PLAYER_READY;
	bool flag;
};

// 플레이어 준비상태 전송
struct sc_packet_player_ready
{
	BYTE type = SC_PLAYER_READY;
	bool flag;
	int id;
};

struct packet_place_item
{
	BYTE type;
	int itemId;
	float x;
	float y;
	float z;
};

struct packet_remove_item
{
	BYTE type;
	int itemId;
};

struct packet_use_item
{
	BYTE type;
	int itemType;
	int targetPlayerId;
};

struct packet_ai_move
{
	BYTE type;
	int aiId;
	Vector3 pos;
	Vector3 rot;
};

struct packet_ai_fire
{
	BYTE type;
	int aiId;
	Vector3 pos;
	Vector3 tar;
};

struct cs_packet_bot_add
{
	BYTE type = CS_AI_ADD;
};

struct sc_packet_bot_add
{
	BYTE type = SC_AI_ADD;
	// 빈 자리 채우고 그 자리 index 를 넣어서 클라이언트로 보내도록
	int aiId;
};

struct sc_packet_bot_remove
{
	BYTE type = SC_AI_REMOVE;
	// 지운 bot ID
	int aiId;
};

struct cs_packet_bot_remove
{
	BYTE type;
	// 지울 bot ID
};

struct sc_packet_item {
	BYTE type;
	BYTE size;
	int id;
};

struct cs_packet_make_car
{
	BYTE type;
	int id;
	Vector3 pos;
};

struct cs_packet_destroy_car
{
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