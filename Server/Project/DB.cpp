#include "DB.h"

DB::DB(wstring server, wstring dbuser, wstring password)
{
	this->server = server;
	this->dbuser = dbuser;
	this->password = password;
}

int DB::connect()
{
	SQLAllocHandle(SQL_HANDLE_ENV, SQL_NULL_HANDLE, &henv);
	SQLSetEnvAttr(henv, SQL_ATTR_ODBC_VERSION, (SQLPOINTER*)SQL_OV_ODBC3, 0);
	SQLAllocHandle(SQL_HANDLE_DBC, henv, &hdbc);
	int retcode = SQLConnect(hdbc, (SQLWCHAR*)server.c_str(), SQL_NTS, (SQLWCHAR*)dbuser.c_str(), SQL_NTS, (SQLWCHAR*)password.c_str(), SQL_NTS);

	return retcode;
}

int DB::signup(wstring username, wstring password)
{
	wstring query = L"INSERT INTO dbo.user_data (name, password) VALUES ('" + wstring(username) + L"', '" + wstring(password) + L"');";
	SQLHSTMT hstmt;
	SQLAllocHandle(SQL_HANDLE_STMT, hdbc, &hstmt);
	int retcode = SQLExecDirectW(hstmt, (SQLWCHAR*) query.c_str(), SQL_NTS);
	SQLFreeHandle(SQL_HANDLE_STMT, hstmt);

	return retcode;
}

User* DB::login(wstring username, wstring password)
{
	wstring query = L"SELECT id, name FROM dbo.user_data WHERE name = '" + wstring(username) + L"' AND password = '" + wstring(password) + L"';";

	SQLUINTEGER id;
	SQLCHAR name[32];
	SQLLEN idInd, nameInd;

	SQLHSTMT hstmt;
	SQLAllocHandle(SQL_HANDLE_STMT, hdbc, &hstmt);

	SQLBindCol(hstmt, 1, SQL_C_ULONG, &id, 0, &idInd);
	SQLBindCol(hstmt, 2, SQL_C_CHAR, &name, sizeof(name), &nameInd);

	int retcode = SQLExecDirectW(hstmt, (SQLWCHAR*) query.c_str(), SQL_NTS);

	while (true) {
		retcode = SQLFetch(hstmt);
		if (retcode == SQL_NO_DATA || retcode == SQL_ERROR) {
			break;
		}

		User* user = new User(id, string((char *) name));
		SQLCloseCursor(hstmt);
		SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
		return user;
	}

	SQLCloseCursor(hstmt);
	SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
	return NULL;
}