#include <iostream>
#include <windows.h>
#include <string>
#include <memory>
#include <vector>
#include <sstream>
#include <typeinfo>
#include "boost/type_index.hpp"

std::string nameOfMemorySection = "VariablesMemory";
std::string message = "";

class Vertex {
private:
	double m_x;
	double m_y;
	double m_z;
public:
	Vertex() {
		m_x = 0;
		m_y = 0;
		m_z = 0;
	}
	Vertex(double x, double y, double z) :
		m_x(x), m_y(y), m_z(z) {

	}

	double getX() {
		return m_x;
	}
	double getY() {
		return m_y;
	}double getZ() {
		return m_z;
	}
};

void readMemoryMappedFile() {
	HANDLE handle;
	char* ptr = nullptr;
	int msgSize = 0;
	message = "";

	while (true)
	{
		handle = OpenFileMappingW(FILE_READ_ACCESS, false, L"VariablesMemory");

		if (handle == NULL)
			std::cout << "CreateFileMapping error: \n" << GetLastError();
		else
		{
			msgSize = *(int*)MapViewOfFile(handle, FILE_MAP_READ, 0, 0, sizeof(int)); std::cout << "opened mapped file with handle " << handle << ", mapped view to " << (void*)ptr << '\n';
			ptr = (char*)MapViewOfFile(handle, FILE_MAP_READ, 0, 0, msgSize * 2);
			ptr += sizeof(int);
			if (msgSize != 0) break;
		}
		std::cout << "error with read from memory \n";
		Sleep(1000);
	}

	std::cout << "there are " << msgSize << " letters\n";
	std::cout << "the message is: ";
	for (int i = 0; i < 2 * msgSize; i += 2)
	{
		message += ptr[i];
	}
	std::cout << '\n';
}

void printMessage() {
	std::cout << "MESSAGE IS: " << std::endl;
	std::cout << message << std::endl;
}

class Variable {
public:

	void* m_S_Addres;
	std::string m_S_Name;
	std::string m_S_Type;
	float m_I_R;
	float m_I_G;
	float m_I_B;


	Variable(std::string name, std::string type,
		void* addres, float r, float g, float b) :
		m_S_Name(name), m_S_Type(type), m_S_Addres(addres),
		m_I_R(r), m_I_G(g), m_I_B(b)
	{
	}
};

std::vector<Variable> m_VOV_Variables;

enum statesOfGettingVariables {
	GET_NAME,
	GET_TYPE,
	GET_ADDRES,
	GET_COLOR
};

statesOfGettingVariables states = statesOfGettingVariables::GET_NAME;

void parser() {

	m_VOV_Variables.clear();
	states = statesOfGettingVariables::GET_NAME;
	size_t pos = 0;

	std::string name;
	std::string type;
	void* addres = nullptr;
	int R, G, B = 255;

	for (int i = 0; i < message.size(); ++i) {
		switch (states)
		{
		case GET_NAME:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() - 1 : message.find('|', i);
			std::cout << pos << std::endl;
			name = message.substr(i, pos - i);
			i += name.size();
			states = statesOfGettingVariables::GET_TYPE;
			std::cout << "NAME: " << name << std::endl;
			break;
		}
		case GET_TYPE:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() - 1 : message.find('|', i);
			std::cout << pos << std::endl;
			type = message.substr(i, pos - i);
			i += type.size();
			states = statesOfGettingVariables::GET_ADDRES;
			std::cout << "TYPE: " << type << std::endl;
			break;
		}
		case GET_ADDRES:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			std::cout << pos << std::endl;
			std::string currentAddres = message.substr(i, pos - i);
			i += currentAddres.size();
			states = statesOfGettingVariables::GET_COLOR;
			uint64_t number = strtoull(currentAddres.c_str(), nullptr, 16);
			void* ptr = reinterpret_cast<void*>(number);
			std::cout << "”казатель: " << ptr << std::endl;

			addres = ptr;

			break;
		}
		case GET_COLOR:
		{
			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			R = std::stoi(message.substr(i, pos - i));
			i += message.substr(i, pos - i).size() + 1;

			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			G = std::stoi(message.substr(i, pos - i));
			i += message.substr(i, pos - i).size() + 1;

			pos = message.find('|', i) == std::string::npos ? message.size() : message.find('|', i);
			B = std::stoi(message.substr(i, pos - i));
			i += message.substr(i, pos - i).size();

			m_VOV_Variables.push_back(Variable(name, type, addres, (float)R / 255.0f, (float)G / 255.0f, (float)B / 255.0f));

			states = statesOfGettingVariables::GET_NAME;
			break;
		}
		default:
			break;
		}
	}

}

void serialize(const std::string* value, std::string name) {
	std::cout << "STRING" << std::endl;
}
void serialize(const int* value, std::string name) {
	std::cout << "INT" << std::endl;
	std::cout << *value << std::endl;
}
void serialize(Vertex* value, std::string name) {

	std::cout << "VERTEX" << std::endl;
	std::cout << (value->getX()) << std::endl;
	std::cout << (value->getY()) << std::endl;
	std::cout << (value->getZ()) << std::endl;
}

void serialize(std::vector<Variable>* value, std::string name) {

	std::cout << "Vector of Variable" << std::endl;
}

template<typename T>
void RegisterType(const Variable& o) {
	int hashCode = typeid(T).hash_code();
	std::string typeIdName = typeid(T).name();
	std::string typeName = boost::typeindex::type_id<T>().pretty_name();
	std::string type2 = boost::typeindex::type_id_with_cvr<T>().pretty_name();
	std::cout << "TypeId: " << typeIdName << std::endl;
	std::cout << "Type1: " << typeName << std::endl;
	std::cout << "Type2: " << type2 << std::endl;
	std::cout << "HashCode: " << hashCode << std::endl;
	if (typeName == o.m_S_Type) {
		T* ptr = static_cast<T*>(o.m_S_Addres);
		serialize(ptr, o.m_S_Name);
	}
}

std::string SerializeObjects(const std::vector<Variable>& objects) {
	std::string buffer;

	for (const auto& o : objects) {
		RegisterType<int>(o);
		RegisterType<std::string>(o);
		RegisterType<Vertex>(o);
		RegisterType<std::vector<Variable>>(o);

	}

	return buffer;
}

void Serialize() {
	readMemoryMappedFile();
	printMessage();
	parser();

	for (int i = 0; i < m_VOV_Variables.size(); ++i) {
		std::cout << "NAME: " << m_VOV_Variables[i].m_S_Name << " TYPE: " << m_VOV_Variables[i].m_S_Type << " ADDRESS: " << m_VOV_Variables[i].m_S_Addres << std::endl;
	}

	SerializeObjects(m_VOV_Variables);
}


Vertex Global = Vertex(3, 4, 1);
int globalInt = 1000;

int main() {

	int a = 5;
	Vertex local = Vertex(1, 12, 3);

	int b = 4;
	int b1 = 4;
	int b2 = 4;
	int b3 = 4;
	
	Serialize();

	return 0;
}