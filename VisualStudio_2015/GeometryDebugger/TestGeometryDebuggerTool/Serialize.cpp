#pragma once

#include <iostream>
#include <windows.h>
#include <string>
#include <memory>
#include <vector>
#include <typeinfo>
#include <fstream>
#include <direct.h>

#include "SerializedMessages.h"

std::string nameOfMemorySection = "VariablesMemory";
std::string message = "";
std::string buffer = "";

class Variable {
public:

	std::string m_S_Addres;
	std::string m_S_Name;
	std::string m_S_Type;
	float m_I_R;
	float m_I_G;
	float m_I_B;


	Variable(std::string name, std::string type,
		std::string addres, int r, int g, int b) :
		m_S_Name(name), m_S_Type(type), m_S_Addres(addres),
		m_I_R(r), m_I_G(g), m_I_B(b)
	{
	}
};
enum statesOfGettingVariables {
	GET_NAME,
	GET_TYPE,
	GET_ADDRES,
	GET_COLOR
};

statesOfGettingVariables states = statesOfGettingVariables::GET_NAME;
std::vector<Variable> m_VOV_Variables;

void readMemoryMappedFile() {
	HANDLE handle;
	char* ptr = nullptr;
	int msgSize = 0;
	message = "";
	buffer = "";

	while (true)
	{
		// �������� MMF
		handle = OpenFileMappingW(FILE_READ_ACCESS, false, L"VariablesMemory");

		if (handle == NULL)
		{
			std::cout << "CreateFileMapping error: \n" << GetLastError();
		}
		else
		{
			// �������� ������ ��������� (������ int � MMF)
			msgSize = *(int*)MapViewOfFile(handle, FILE_MAP_READ, 0, 0, sizeof(int));

			// ���������� ������ ��� ������
			ptr = (char*)MapViewOfFile(handle, FILE_MAP_READ, 0, 0, msgSize * 2);
			ptr += sizeof(int);  // ���������� ������ int (������ ���������)

			if (msgSize != 0) break;
		}

		std::cout << "error with reading from memory \n";
		Sleep(1000);  // ��������� ������� ����� 1 �������
	}

	// ������ � ����� ���������
	for (int i = 0; i < 2 * msgSize; i += 2)
	{
		message += ptr[i];
	}
}
void parser() {

	m_VOV_Variables.clear();
	states = statesOfGettingVariables::GET_NAME;
	size_t pos = 0;

	std::string name;
	std::string type;
	std::string addres;
	float R, G, B = 1;

	for (int i = 0; i < message.size(); ++i) {
		switch (states)
		{
		case GET_NAME:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() - 1 : message.find('|', i);
			//std::cout << pos << std::endl;
			name = message.substr(i, pos - i);
			i += name.size();
			states = statesOfGettingVariables::GET_TYPE;
			//std::cout << "NAME: " << name << std::endl;
			break;
		}
		case GET_TYPE:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() - 1 : message.find('|', i);
			//std::cout << pos << std::endl;
			type = message.substr(i, pos - i);
			i += type.size();
			states = statesOfGettingVariables::GET_ADDRES;
			//std::cout << "TYPE: " << type << std::endl;
			break;
		}
		case GET_ADDRES:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			//std::cout << pos << std::endl;
			addres = message.substr(i, pos - i);
			i += addres.size();
			states = statesOfGettingVariables::GET_COLOR;
			break;
		}
		case GET_COLOR:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			R = std::stof(message.substr(i, pos - i));
			i += message.substr(i, pos - i).size() + 1;

			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			G = std::stof(message.substr(i, pos - i));
			i += message.substr(i, pos - i).size() + 1;

			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			B = std::stof(message.substr(i, pos - i));
			i += message.substr(i, pos - i).size();

			m_VOV_Variables.push_back(Variable(name, type, addres, R, G, B));

			states = statesOfGettingVariables::GET_NAME;
			break;
		}
		default:
			break;
		}
	}
}

std::string getCurrentDir() {

	const size_t size = 1024;
	char buffer[size];

	if (_getcwd(buffer, size) != NULL)
		return buffer;
	else
		return "";
}

void writeMemoryMappedFile() {

	std::string path = getCurrentDir();

	std::cout << path << std::endl;

	size_t dataSize = path.size() + 1;

	// ������� ��� ��������� MMF
	HANDLE hMapFile = CreateFileMapping(
		INVALID_HANDLE_VALUE,    // ������������� ����� ��������
		NULL,                 // ������������ �� ���������
		PAGE_READWRITE,          // ������ ��� ������ � ������
		0,                       // ������������ ������ (������� �����)
		static_cast<DWORD>(dataSize), // ������������ ������ (������� �����)
		L"VariablesMemory"
	);

	if (hMapFile == nullptr) {
		std::cerr << "�� ������� ������� MMF. ��� ������: " << GetLastError() << std::endl;
		return;
	}

	// ����������� ������������� � �������� ������������ ��������
	LPVOID pBuf = MapViewOfFile(
		hMapFile,          // ���������� MMF
		FILE_MAP_ALL_ACCESS, // ������ ��� ������ � ������
		0,
		0,
		dataSize
	);

	if (pBuf == NULL) {
		std::cerr << "�� ������� ������������� �������������. ��� ������: " << GetLastError() << std::endl;
		CloseHandle(hMapFile);
		return;
	}

	// �������� ������ � �������������� ������
	memcpy(pBuf, path.c_str(), dataSize);

	// ����������� �������
	UnmapViewOfFile(pBuf);
	CloseHandle(hMapFile);

}

template<typename T>
void RegisterType(const Variable& o) {

	std::string typeIdName = typeid(T).name();

	while (typeIdName.find("class") != std::string::npos) {

		size_t pos = typeIdName.find("class");
		size_t offset = std::string("class").size();
		typeIdName.erase(pos, offset);
	}

	while (typeIdName.find("struct") != std::string::npos) {

		size_t pos = typeIdName.find("struct");
		size_t offset = std::string("struct").size();
		typeIdName.erase(pos, offset);
	}

	while (typeIdName.find(" ") != std::string::npos) {

		size_t pos = typeIdName.find(" ");
		size_t offset = std::string(" ").size();
		typeIdName.erase(pos, offset);
	}

	if (typeIdName == o.m_S_Type) {

		std::cout << "FIND SERIALIZATOR FOR " << o.m_S_Type << std::endl;

		std::string message = "";

		uint64_t number = strtoull(o.m_S_Addres.c_str(), nullptr, 16);
		void* ptrOfVariable = reinterpret_cast<void*>(number);

		T* ptr = static_cast<T*>(ptrOfVariable);
		message = serialize(ptr, o.m_S_Name, o.m_I_R, o.m_I_G, o.m_I_B);

		std::cout << "ENDED SERIALIZE." << std::endl;

		if (message.size() != 0) {

			std::fstream file;
			file.open("vis_dbg_" + o.m_S_Addres + ".txt", std::ios::out);

			if (file.is_open()) {
				file << message;
			}
		}
	}
}

std::string SerializeObjects(const std::vector<Variable>& objects) {

	for (const auto& o : objects) {
		RegisterType<Point>(o);
		RegisterType<Vector>(o);
		RegisterType<Circle>(o);
		RegisterType<std::vector<Circle>>(o);
		RegisterType<Line>(o);
		RegisterType<CoordinateSystem>(o);
		RegisterType<Edge>(o);

	}

	return buffer;
}

void Serialize() {
	readMemoryMappedFile();
	parser();
	SerializeObjects(m_VOV_Variables);
	writeMemoryMappedFile();
}

