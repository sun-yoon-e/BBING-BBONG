#pragma once
#include "stdafx.h"

// https://docs.microsoft.com/en-us/sql/odbc/reference/install/odbc-header-files?view=sql-server-ver15
#include <odbcinst.h>
#include <sql.h>
#include <sqlext.h>
#include <sqltypes.h>
#include <sqlucode.h>
#include <msdasql.h>
#include <msdadc.h>

#include "User.h"

class DB
{
private:
	wstring server;
	wstring dbuser;
	wstring password;

	SQLHANDLE henv;
	SQLHANDLE hdbc;

public:
	DB(wstring server, wstring dbuser, wstring password);

	int connect();

	int signup(wstring username, wstring password);
	User* login(wstring username, wstring password);

	/************************************************************************
	/* HandleDiagnosticRecord : display error/warning information
	/*
	/* Parameters:
	/* hHandle ODBC handle
	/* hType Type of handle (SQL_HANDLE_STMT, SQL_HANDLE_ENV, SQL_HANDLE_DBC)
	/* RetCode Return code of failing command
	/************************************************************************/

	void HandleDiagnosticRecord(SQLHANDLE hHandle, SQLSMALLINT hType, RETCODE RetCode)
	{
		SQLSMALLINT iRec = 0;
		SQLINTEGER iError;
		WCHAR wszMessage[1000];
		WCHAR wszState[SQL_SQLSTATE_SIZE + 1];
		if (RetCode == SQL_INVALID_HANDLE) {
			fwprintf(stderr, L"Invalid handle!\n");
			return;
		}
		while (SQLGetDiagRec(hType, hHandle, ++iRec, wszState, &iError, wszMessage,
			(SQLSMALLINT)(sizeof(wszMessage) / sizeof(WCHAR)), (SQLSMALLINT*)NULL) == SQL_SUCCESS) {
			// Hide data truncated..
			if (wcsncmp(wszState, L"01004", 5)) {
				fwprintf(stderr, L"[%5.5s] %s (%d)\n", wszState, wszMessage, iError);
			}
		}
	}
};