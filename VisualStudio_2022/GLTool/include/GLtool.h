#pragma once

#include <iostream>
#include <windows.h>
#include <thread>
#include <condition_variable>
#include <vector>

#include "geom_view.h"

struct GLFWwindow;

struct __declspec (dllexport) GLtool {
	bool init(HWND = 0);
	void close();
	void reload(const std::vector<std::pair<std::string, bool>>& files, bool resetCamera);
	void visibilities(std::string path, bool isVisible);
	void draw();
	geom_view gv;
	HWND native = 0;
};
