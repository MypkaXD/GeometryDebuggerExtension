#include <iostream>
#include <windows.h>
#include <winuser.h>
#include <fstream>
#include <iostream>
#include <windows.h>
#include <vector>

#include "GLtool.h"

#define DLL_EXPORT extern "C" __declspec(dllexport)

GLtool* tool = nullptr;

struct StringArrayData {
	int Count;
	const char** StringArray; // ”казатель на массив указателей на строки
	const bool* BoolArray; // ”казатель на массив булевых значений
};

DLL_EXPORT void destroyGLtoolWindow(HWND hwnd) {
	tool->close();
}

DLL_EXPORT HWND createGLtoolWindow(HWND hWndParent = 0)
{
	if (!tool)
		tool = new GLtool;
	tool->init(hWndParent);
	return tool->native;
}

DLL_EXPORT void reload(StringArrayData* data, bool resetCamera) {

	std::vector<std::pair<std::string, bool>> files(data->Count);

	for (int i = 0; i < data->Count; ++i) {
		files[i] = std::make_pair(std::string(data->StringArray[i]), data->BoolArray[i]);
	}

	tool->reload(files, resetCamera);
}

DLL_EXPORT void visibilities(StringArrayData* data) {

	std::string path = data->StringArray[0];
	bool isVisble = data->BoolArray[0];

	tool->visibilities(path, isVisble);
}