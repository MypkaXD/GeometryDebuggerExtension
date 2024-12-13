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
	bool isInitialized = false;
	GLFWwindow* window = nullptr;
	HWND native = 0;
	std::mutex windowMutex;
	std::condition_variable windowCV;
	double phi = .0;
	double theta = .0;
	double phi0 = .0;
	double theta0 = .0;
	double x = 0;
	double y = 0;
	double z = 0;
	double x0 = 0;
	double y0 = 0;
	double z0 = 0;
	double s = 1;
	bool LMB = false;
	bool RMB = false;
	double lastX = 0;
	double lastY = 0;
};
