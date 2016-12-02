// ConsoleApplication.cpp : Defines the entry point for the console application.
//
#pragma comment(lib, "Debug/MixedCode.lib")

#include "stdafx.h"
#include <iostream>
#include "MixedCode/CppService.h"

using namespace std;
using namespace MixedCode;

int main(int argc, char* argv[])
{
	CppService service;
	/*service.process(123);*/

	cout << "press any key..." << endl;
	getchar();
}
