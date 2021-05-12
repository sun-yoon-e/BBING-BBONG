#include "User.h"

User::User(int id, string name)
{
	this->id = id;
	this->name = name;
}

int User::GetId()
{
	return id;
}

string User::GetName()
{
	return name;
}
