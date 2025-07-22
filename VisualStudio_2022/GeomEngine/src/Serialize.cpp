#include <iostream>
#include <windows.h>
#include <string>
#include <vector>
#include <array>
#include <typeinfo>
#include <fstream>
#include <direct.h>

#include "SerializedMessages.h"

std::string nameOfMemorySection = "VariablesMemory";
std::string message = "";
std::string buffer = "";
std::string response = "";
std::string directory = "";

class Variable
{
public:

	std::string m_S_Addres;
	std::string m_S_Name;
	std::string m_S_Type;
	std::string m_S_Source;
	std::string m_S_index;
	float m_I_R;
	float m_I_G;
	float m_I_B;

	Variable(std::string name, std::string type,
		std::string addres, std::string source, float r, float g, float b, std::string index) :
		m_S_Name(name), m_S_Type(type), m_S_Addres(addres), m_S_Source(source),
		m_I_R(r), m_I_G(g), m_I_B(b), m_S_index(index) {
	}
};

enum statesOfGettingVariables
{
	GET_NAME,
	GET_TYPE,
	GET_ADDRES,
	GET_SOURCE,
	GET_COLOR,
	GET_INDEX
};

statesOfGettingVariables states = GET_NAME;
std::vector<Variable> m_VOV_Variables;

void readMemoryMappedFile()
{
	HANDLE handle;
	char* ptr = nullptr;
	int msgSize = 0;
	message = "";
	buffer = "";

	while (true)
	{
		handle = OpenFileMappingW(FILE_READ_ACCESS, false, L"VariablesMemory");

		if (handle == NULL)
		{
			std::cout << "CreateFileMapping error: \n" << GetLastError();
		}
		else
		{
			msgSize = *(int*)MapViewOfFile(handle, FILE_MAP_READ, 0, 0, sizeof(int));

			ptr = (char*)MapViewOfFile(handle, FILE_MAP_READ, 0, 0, msgSize * 2);
			ptr += sizeof(int);

			if (msgSize != 0) break;
		}

		std::cout << "error with reading from memory \n";
		Sleep(1000);
	}

	for (int i = 0; i < 2 * msgSize; i += 2)
	{
		message += ptr[i];
	}
	//std::cout << message << std::endl;
}


void tokenize(std::string& string) {

	std::vector<std::string> remove_strings = { "const", "class", "struct", "__ptr64" };

	string += " ";

	for (int i = 0; i < remove_strings.size(); ++i) {
		while (string.find(" " + remove_strings[i] + " ") != std::string::npos) {
			//while (string.find(remove_strings[i]) != std::string::npos) {
			size_t pos = string.find(" " + remove_strings[i] + " ");
			size_t offset = std::string(" " + remove_strings[i] + " ").size();
			string.erase(pos, offset);
		}
		while (string.find(" " + remove_strings[i]) != std::string::npos) {
			//while (string.find(remove_strings[i]) != std::string::npos) {
			size_t pos = string.find(" " + remove_strings[i]);
			size_t offset = std::string(" " + remove_strings[i]).size();
			string.erase(pos, offset);
		}
		while (true) {

			size_t pos = string.find(remove_strings[i] + " ");

			if (pos != std::string::npos) {
				if (pos == 0) {
					size_t offset = std::string(remove_strings[i] + " ").size();
					string.erase(pos, offset);
				}
				else {
					if (string[pos - 1] == '<' || string[pos - 1] == '>' || string[pos - 1] == ':' || string[pos - 1] == ',') {
						size_t offset = std::string(remove_strings[i] + " ").size();
						string.erase(pos, offset);
					}
				}
			}
			else {
				break;
			}
		}
	}

	while (string.find(" ") != std::string::npos) {
		size_t pos = string.find(" ");
		size_t offset = std::string(" ").size();
		string.erase(pos, offset);
	}
}

void parser() {

	m_VOV_Variables.clear();
	states = statesOfGettingVariables::GET_NAME;
	size_t pos = 0;

	//std::cout << message << std::endl;

	std::string name;
	std::string type;
	std::string addres;
	std::string source;
	std::string index;
	float R, G, B = 1;

	for (int i = 0; i < message.size(); ++i) {
		switch (states)
		{
		case GET_NAME:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			name = message.substr(i, pos - i);
			i += name.size();
			states = statesOfGettingVariables::GET_TYPE;
			break;
		}
		case GET_TYPE:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			type = message.substr(i, pos - i);
			i += type.size();
			tokenize(type);
			if ('&' == type[type.length() - 1])
				type.pop_back();
			states = statesOfGettingVariables::GET_SOURCE;
			break;
		}
		case GET_SOURCE:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			source = message.substr(i, pos - i);
			i += source.size();
			states = statesOfGettingVariables::GET_ADDRES;
			break;
		}
		case GET_ADDRES:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
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
			//std::cout << R << std::endl;

			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			G = std::stof(message.substr(i, pos - i));
			i += message.substr(i, pos - i).size() + 1;
			//std::cout << G << std::endl;

			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			B = std::stof(message.substr(i, pos - i));
			i += message.substr(i, pos - i).size();
			//std::cout << B << std::endl;

			states = statesOfGettingVariables::GET_INDEX;
			break;
		}
		case GET_INDEX:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			index = message.substr(i, pos - i);
			i += index.size();

			m_VOV_Variables.push_back(Variable(name, type, addres, source, R, G, B, index));

			states = statesOfGettingVariables::GET_NAME;
			break;
		}
		default:
			break;
		}
	}
}

std::string getCurrentDir(int time_created) {

	const size_t size = 1024;
	char buffer[size];
	std::string path;

	struct stat sb;

	if (_getcwd(buffer, size) != NULL) {

		path = buffer;
		path += "\\vis_dbg" + std::to_string(time_created);

		//std::cout << path << std::endl;

		if (stat(path.c_str(), &sb) != 0)
			mkdir(path.c_str());

		return path;
	}
	else
		return "";
}

std::string sanitizeFileName(const std::string& name) {
	std::string result = name;
	const std::string forbiddenChars = "<>:\"/\\|?*&";

	for (char& ch : result) {
		if (forbiddenChars.find(ch) != std::string::npos) {
			ch = '_';
		}
	}

	return result;
}


template<typename T>
bool RegisterType(const Variable& o) {

	std::string typeIdName = typeid(T).name();

	tokenize(typeIdName);

	if (typeIdName == o.m_S_Type) {

		std::string message = "";

		uint64_t number = strtoull(o.m_S_Addres.c_str(), nullptr, 16);
		void* ptrOfVariable = reinterpret_cast<void*>(number);

		T* ptr = static_cast<T*>(ptrOfVariable);
		message = serialize(ptr, o.m_S_Name, o.m_I_R, o.m_I_G, o.m_I_B);

		if (message.size() != 0) {

			std::string name = sanitizeFileName(o.m_S_Name);

			std::fstream file;
			file.open(directory + "\\vis_dbg_" + name + "_" + o.m_S_Source + "_" + o.m_S_Addres + "_depth" + o.m_S_index + ".txt", std::ios::out);

			if (file.is_open()) {
				file << message;
				file.close();
				return true;
			}
		}
	}
	return false;
}

std::string SerializeObjects(const std::vector<Variable>& objects) {

	std::string serializingVariables = "";

	for (const auto& object : objects) {

		bool isSerialized = false;

		isSerialized |= RegisterType<Point>(object);
		isSerialized |= RegisterType<Edge>(object);
		isSerialized |= RegisterType<Edge*>(object);
		isSerialized |= RegisterType<Vector>(object);
		isSerialized |= RegisterType<CustomPlane>(object);
		isSerialized |= RegisterType<Plate>(object);
		isSerialized |= RegisterType<Tree>(object);
		isSerialized |= RegisterType<Tree*>(object);
		isSerialized |= RegisterType<Node>(object);
		isSerialized |= RegisterType<std::vector<Plate>>(object);
		//isSerialized |= RegisterType<Sphere>(object);
		//isSerialized |= RegisterType<Cylinder>(object);
		//isSerialized |= RegisterType<Face>(object);
		isSerialized |= RegisterType<std::vector<Edge>>(object);
		isSerialized |= RegisterType<std::vector<Edge*>>(object);
		isSerialized |= RegisterType<std::vector<Point>>(object);
		isSerialized |= RegisterType<BoundingBox>(object);
		isSerialized |= RegisterType<std::array<Edge*, 2>>(object);
		//isSerialized |= RegisterType<Plate>(object);
		//isSerialized |= RegisterType<QuadTree>(object);
		//isSerialized |= RegisterType<Node>(object);
		//isSerialized |= RegisterType<BoundingBox>(object);

		serializingVariables += isSerialized ? "1" : "0";
	}

	return serializingVariables;
}

std::string Serialize(int time_created) {

	directory = getCurrentDir(time_created);

	readMemoryMappedFile();
	parser();

	response = SerializeObjects(m_VOV_Variables);

	return response + "|" + directory;
}