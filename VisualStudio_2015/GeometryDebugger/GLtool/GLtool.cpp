#include <iostream>
#include <windows.h>
#include <winuser.h>
#include <fstream>
#include <iostream>
#include <thread>
#include <vector>
#include <condition_variable>
#include <clocale>

#include <GLFW/glfw3.h>
#define GLFW_EXPOSE_NATIVE_WIN32
#define GLFW_EXPOSE_NATIVE_WGL
#define GLFW_NATIVE_INCLUDE_NONE
#include <GLFW/glfw3native.h>

#include "GLtool.h"

bool GLtool::init(HWND _hWndParent)
{
	std::setlocale(LC_NUMERIC, "en_US.UTF-8");

	gv.setParentWin32Handler(_hWndParent);
	gv.init("");
	native = gv.getNativeWin32Handler();

	return true;
}

void GLtool::close() {
	gv.close();
}

void GLtool::reload(const std::vector<std::pair<std::string, bool>>& files, bool resetCamera) {
	gv.reload(files, resetCamera);
}

void GLtool::visibilities(std::string path, bool isVisble) {

	std::vector<std::pair<std::string, bool>> pairs = { std::make_pair(path, isVisble) };

	gv.visibilities(pairs);
}
