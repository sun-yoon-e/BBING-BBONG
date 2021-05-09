#pragma once
#include "stdafx.h"

class User
{
private:
	int id;
	string name;

public:
	User(int id, string name);

	int GetId();
	string GetName();
};

