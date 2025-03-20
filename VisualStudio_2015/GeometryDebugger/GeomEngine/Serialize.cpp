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
std::string response = "";

class Variable {
public:

	std::string m_S_Addres;
	std::string m_S_Name;
	std::string m_S_Type;
	std::string m_S_Source;
	float m_I_R;
	float m_I_G;
	float m_I_B;

	Variable() {

	}

	Variable(std::string name, std::string type,
		std::string addres, std::string source, float r, float g, float b) :
		m_S_Name(name), m_S_Type(type), m_S_Addres(addres), m_S_Source(source),
		m_I_R(r), m_I_G(g), m_I_B(b)
	{
	}
};
enum statesOfGettingVariables {
	GET_NAME,
	GET_TYPE,
	GET_ADDRES,
	GET_SOURCE,
	GET_COLOR
};

statesOfGettingVariables states = statesOfGettingVariables::GET_NAME;
Variable variable;

void readMemoryMappedFile() {
	HANDLE handle;
	char* ptr = nullptr;
	int msgSize = 0;
	message = "";
	buffer = "";

	while (true)
	{
		// Открытие MMF
		handle = OpenFileMappingW(FILE_READ_ACCESS, false, L"VariablesMemory");

		if (handle == NULL)
		{
			std::cout << "CreateFileMapping error: \n" << GetLastError();
		}
		else
		{
			// Получаем размер сообщения (первый int в MMF)
			msgSize = *(int*)MapViewOfFile(handle, FILE_MAP_READ, 0, 0, sizeof(int));

			// Отображаем память для чтения
			ptr = (char*)MapViewOfFile(handle, FILE_MAP_READ, 0, 0, msgSize * 2);
			ptr += sizeof(int);  // Пропускаем первый int (размер сообщения)

			if (msgSize != 0) break;
		}

		std::cout << "error with reading from memory \n";
		Sleep(1000);  // Повторная попытка через 1 секунду
	}

	// Чтение и вывод сообщения
	for (int i = 0; i < 2 * msgSize; i += 2)
	{
		message += ptr[i];
	}
	std::cout << message << std::endl;
	std::cout << "END READ MMF" << std::endl;
}

void parser() {

	std::cout << "PARSER START" << std::endl;

	std::cout << "m_VOV_Variables.clear();" << std::endl;
	states = statesOfGettingVariables::GET_NAME;
	size_t pos = 0;

	std::string name;
	std::string type;
	std::string addres;
	std::string source;
	float R, G, B = 1;

	for (int i = 0; i < message.size(); ++i) {
		switch (states)
		{
		case GET_NAME:
		{
			std::cout << "Name" << std::endl;
			pos = message.find('|', i) == std::string::npos ? message.size() - 1 : message.find('|', i);
			name = message.substr(i, pos - i);
			i += name.size();
			states = statesOfGettingVariables::GET_TYPE;
			break;
		}
		case GET_TYPE:
		{
			std::cout << "Type" << std::endl;
			pos = message.find('|', i) == std::string::npos ? message.size() - 1 : message.find('|', i);
			type = message.substr(i, pos - i);
			i += type.size();
			states = statesOfGettingVariables::GET_SOURCE;
			break;
		}
		case GET_SOURCE:
		{
			std::cout << "Source" << std::endl;
			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			source = message.substr(i, pos - i);
			i += source.size();
			states = statesOfGettingVariables::GET_ADDRES;
			break;
		}
		case GET_ADDRES:
		{
			std::cout << "Addres" << std::endl;
			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			addres = message.substr(i, pos - i);
			i += addres.size();
			states = statesOfGettingVariables::GET_COLOR;
			break;
		}
		case GET_COLOR:
		{
			std::cout << "Color" << std::endl;
			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			R = std::stof(message.substr(i, pos - i));
			i += message.substr(i, pos - i).size() + 1;
			//std::cout << R << std::endl;

			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			G = std::stof(message.substr(i, pos - i));
			i += message.substr(i, pos - i).size() + 1;
			//std::cout << G << std::endl;

			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			B = std::stof(message.substr(i, pos - i));
			i += message.substr(i, pos - i).size();
			//std::cout << B << std::endl;

			variable = Variable(name, type, addres, source, R, G, B);

			states = statesOfGettingVariables::GET_NAME;
			break;
		}
		default:
			break;
		}
	}


	std::cout << "END PARSER" << std::endl;
}

std::string getCurrentDir() {

	const size_t size = 1024;
	char buffer[size];

	if (_getcwd(buffer, size) != NULL)
		return buffer;
	else
		return "";
}

template<typename T>
bool RegisterType(const Variable& o) {

	std::string typeIdName = typeid(T).name();

	std::cout << "REG TYPE" << std::endl;

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

	while (typeIdName.find("const") != std::string::npos) {

		size_t pos = typeIdName.find("const");
		size_t offset = std::string("const").size();
		typeIdName.erase(pos, offset);
	}

	while (typeIdName.find(" ") != std::string::npos) {

		size_t pos = typeIdName.find(" ");
		size_t offset = std::string(" ").size();
		typeIdName.erase(pos, offset);
	}

	std::cout << "End delete trash" << std::endl;

	if (typeIdName == o.m_S_Type) {

		std::string message = "";

		std::cout << "start strtoull" << std::endl;

		uint64_t number = strtoull(o.m_S_Addres.c_str(), nullptr, 16);
		void* ptrOfVariable = reinterpret_cast<void*>(number);
		std::cout << "End strtoull" << std::endl;

		// Проверка, что ptrOfVariable не равен nullptr
		if (ptrOfVariable == nullptr) {
			std::cout << "Invalid address: nullptr" << std::endl;
			return false;
		}

		// Приведение void* к T*
		T* ptr = static_cast<T*>(ptrOfVariable);
		std::cout << "Static cast work" << std::endl;

		// Проверка, что ptr не равен nullptr
		if (ptr == nullptr) {
			std::cout << "Invalid pointer: nullptr" << std::endl;
			return false;
		}

		// Создание shared_ptr из сырого указателя (если это необходимо)
		std::shared_ptr<T> sharedPtr(ptr, [](T*) {}); // Пустой deleter, так как память управляется вручную

													  // Проверка, что sharedPtr не равен nullptr (хотя это избыточно, так как ptr уже проверен)
		if (!sharedPtr) {
			std::cout << "Shared pointer is invalid" << std::endl;
			return false;
		}

		// Вызов serialize
		try {
			message = serialize(ptr, o.m_S_Name, o.m_I_R, o.m_I_G, o.m_I_B);
		}
		catch (const std::exception& e) {
			std::cout << "Error in serialize: " << e.what() << std::endl;
			return false;
		}

		std::cout << "Serialize completed" << std::endl;

		std::cout << "end serialize" << std::endl;
		if (message.size() != 0) {

			std::fstream file;
			file.open("vis_dbg_" + o.m_S_Name + "_" + o.m_S_Source + "_" + o.m_S_Addres + ".txt", std::ios::out);

			if (file.is_open()) {
				file << message;

				return true;
			}
		}
	}
	return false;
}

std::string SerializeObjects(const Variable& object) {

	std::cout << "start serialize" << std::endl;

	std::string serializingVariables = "";

	bool isSerialized = false;
	try {
		isSerialized |= RegisterType<Point>(object);
		isSerialized |= RegisterType<Edge>(object);
		isSerialized |= RegisterType<Vector>(object);
		isSerialized |= RegisterType<CustomPlane>(object);
		isSerialized |= RegisterType<Plane>(object);
		isSerialized |= RegisterType<Sphere>(object);
		isSerialized |= RegisterType<Cylinder>(object);
		isSerialized |= RegisterType<Face>(object);
		isSerialized |= RegisterType<std::vector<Edge>>(object);
		isSerialized |= RegisterType<std::vector<Point>>(object);
	}
	catch (...) {}

	serializingVariables += isSerialized ? "1" : "0";

	return serializingVariables;
}

std::string Serialize() {

	readMemoryMappedFile();
	parser();
	response = SerializeObjects(variable);

	return response + "|" + getCurrentDir();
}

