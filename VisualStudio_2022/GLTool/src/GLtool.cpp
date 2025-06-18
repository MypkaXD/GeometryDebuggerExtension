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

#include <mutex>

bool GLtool::init(HWND _hWndParent)
{
	std::setlocale(LC_NUMERIC, "en_US.UTF-8");

	gv.setParentWin32Handler(_hWndParent);
	gv.init("");
	native = gv.getNativeWin32Handler();
	gv.appearance.imgui_cam_control = false;
	gv.appearance.imgui_object_control = false;

	return true;
}

void GLtool::close() {
	gv.close();
}

void GLtool::reload(const std::vector<std::pair<std::string, bool>>& files, bool resetCamera) {

	std::setlocale(LC_NUMERIC, "en_US.UTF-8");

	gv.reload(files, resetCamera);
}

void GLtool::visibilities(std::string path, bool isVisble) {

	std::setlocale(LC_NUMERIC, "en_US.UTF-8");

	std::vector<std::pair<std::string, bool>> pairs = { std::make_pair(path, isVisble) };

	gv.visibilities(pairs);
}
