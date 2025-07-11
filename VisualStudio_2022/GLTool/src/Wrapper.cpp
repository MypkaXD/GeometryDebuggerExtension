#include <iostream>
#include <windows.h>
#include <winuser.h>
#include <fstream>
#include <iostream>
#include <windows.h>
#include <vector>
#include <fstream>

#include "GLtool.h"

#define DLL_EXPORT extern "C" __declspec(dllexport)

GLtool* tool = nullptr;

struct StringArrayData {
	int Count;
	const char** StringArray;
	const bool* BoolArray;
};

typedef void(__stdcall* CallbackFunction)(StringArrayData data);

CallbackFunction g_callback;


DLL_EXPORT void destroyGLtoolWindow(HWND hwnd) {
	tool->close();
}

DLL_EXPORT void select_control(void* callback_data,
    const std::vector<std::tuple<std::vector<std::string>, size_t, float>>& data) {
    geom_view& gv = *static_cast<geom_view*>(callback_data);

    /*std::fstream file("C:\\geom_view\\file.txt", std::ios::app);
    for (int i = 0; i < data.size(); ++i) {
        for (int j = 0; j < std::get<0>(data[i]).size(); ++j) {
            file << (std::get<0>(data[i]))[j] << "\n";
        }
    }
    file.close();*/

    std::string last_select_object;

    if (data.size() != 0) {
        if (std::get<0>(data[0]).size() != 0) {
            last_select_object = (std::get<0>(data[0])[1]);
        }
    }

    if (g_callback) {

        StringArrayData data;
        const char* temp = last_select_object.c_str();
        data.StringArray = &temp;

        g_callback(data);
    }
}


DLL_EXPORT HWND createGLtoolWindow(CallbackFunction callback, HWND hWndParent = 0) {
    if (!tool)
        tool = new GLtool();
    tool->init(hWndParent);

    g_callback = callback;

    // Используем обёртку, которая будет вызывать C# callback через глобальную переменную
    tool->gv.setSelectCallBack((void*)&tool->gv, select_control);

    return tool->native;
}

DLL_EXPORT void reload(StringArrayData* data, bool resetCamera) {

	std::vector<std::pair<std::string, bool>> files(data->Count);

	for (int i = 0; i < data->Count; ++i)
		files[i] = std::make_pair(std::string(data->StringArray[i]), data->BoolArray[i]);

	tool->reload(files, resetCamera);
}

DLL_EXPORT void highlight(StringArrayData* data) {

    bool is_highlight = data->BoolArray[0];
    std::string file_path = data->StringArray[0];

    tool->gv.highlight({ "root", file_path }, is_highlight);
}

DLL_EXPORT void visibilities(StringArrayData* data) {

	std::string path = data->StringArray[0];
	bool isVisble = data->BoolArray[0];

	tool->visibilities(path, isVisble);
}

DLL_EXPORT void* get_addres_of_select() {
	return (void*)select_control;
}