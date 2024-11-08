#include <iostream>
#include <windows.h>
#include <string>
#include <memory>
#include <vector>
#include <sstream>
#include <typeinfo>
#include "boost/type_index.hpp"
#include <fstream>

#define _USE_MATH_DEFINES
#include <math.h>
#include <tuple>

#include "Vector.h"
#include "Circle.h"


std::string nameOfMemorySection = "VariablesMemory";
std::string message = "";
std::string buffer = "";

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
			std::cout << "opened mapped file with handle " << handle << ", mapped view to " << (void*)ptr << '\n';

			// Отображаем память для чтения
			ptr = (char*)MapViewOfFile(handle, FILE_MAP_READ, 0, 0, msgSize * 2);
			ptr += sizeof(int);  // Пропускаем первый int (размер сообщения)

			if (msgSize != 0) break;
		}

		std::cout << "error with reading from memory \n";
		Sleep(1000);  // Повторная попытка через 1 секунду
	}

	std::cout << "There are " << msgSize << " letters\n";
	std::cout << "The message is: ";

	// Чтение и вывод сообщения
	for (int i = 0; i < 2 * msgSize; i += 2)
	{
		message += ptr[i];
	}
	std::cout << message << '\n';
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
	int m_I_R;
	int m_I_G;
	int m_I_B;


	Variable(std::string name, std::string type,
		void* addres, int r, int g, int b) :
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
			std::cout << "Указатель: " << ptr << std::endl;

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

			m_VOV_Variables.push_back(Variable(name, type, addres, R, G, B));

			states = statesOfGettingVariables::GET_NAME;
			break;
		}
		default:
			break;
		}
	}

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

	std::cout << typeIdName << std::endl;

	if (typeIdName == o.m_S_Type) {
		T* ptr = static_cast<T*>(o.m_S_Addres);
		buffer += serialize(ptr, o.m_S_Type, o.m_S_Name, o.m_I_R, o.m_I_G, o.m_I_B);
	}
}


std::string SerializeObjects(const std::vector<Variable>& objects) {

	for (const auto& o : objects) {
		RegisterType<Vector>(o);
		RegisterType<Circle>(o);
	}

	std::cout << buffer << std::endl;

	return buffer;
}

std::string mmfName = "ReadyPath";

void writeMemoryMappedFile() {

	std::string path = "true";

	std::cout << path << std::endl;

	size_t dataSize = path.size() + 1;

	// Создаем или открываем MMF
	HANDLE hMapFile = CreateFileMapping(
		INVALID_HANDLE_VALUE,    // использование файла подкачки
		NULL,                 // безопасность по умолчанию
		PAGE_READWRITE,          // доступ для чтения и записи
		0,                       // максимальный размер (старшее слово)
		static_cast<DWORD>(dataSize), // максимальный размер (младшее слово)
		L"VariablesMemory"
	);

	if (hMapFile == nullptr) {
		std::cerr << "Не удалось создать MMF. Код ошибки: " << GetLastError() << std::endl;
		return;
	}

	// Спроецируем представление в адресное пространство процесса
	LPVOID pBuf = MapViewOfFile(
		hMapFile,          // дескриптор MMF
		FILE_MAP_ALL_ACCESS, // доступ для чтения и записи
		0,
		0,
		dataSize
	);

	if (pBuf == NULL) {
		std::cerr << "Не удалось спроецировать представление. Код ошибки: " << GetLastError() << std::endl;
		CloseHandle(hMapFile);
		return;
	}

	// Копируем строку в проецированную память
	memcpy(pBuf, path.c_str(), dataSize);

	// Освобождаем ресурсы
	UnmapViewOfFile(pBuf);
	CloseHandle(hMapFile);

	std::cout << "SUCCESS" << std::endl;

}

void Serialize() {
	readMemoryMappedFile();
	printMessage();
	parser();

	for (int i = 0; i < m_VOV_Variables.size(); ++i) {
		std::cout << "NAME: " << m_VOV_Variables[i].m_S_Name << " TYPE: " << m_VOV_Variables[i].m_S_Type << " ADDRESS: " << m_VOV_Variables[i].m_S_Addres << std::endl;
	}

	SerializeObjects(m_VOV_Variables);
	std::cout << "END SER" << std::endl;
	std::fstream file;
	file.open("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\ReadMemoryMappedFile\\out.txt", std::ios::out);
	if (file.is_open()) {
		file << buffer;
	}
	file.close();
	writeMemoryMappedFile();

}



Vector Global = Vector(3, 4, 1);
int globalInt = 1000;

int main() {

	int a = 5;
	Circle circle = Circle(5, Vector(0, 0, 0), Vector(0, 0, 1));
	Vector local = Vector(5, 0, 0);

	Circle circle2 = Circle(15, Vector(0, 0, 0), Vector(0, 1, 1));
	Circle circle3 = Circle(10, Vector(0, 0, 0), Vector(1, 1, 1));

	double step = 1;

	while (true) {
		circle.setRadius(circle.getRadius() + step);
		circle2.setRadius(circle.getRadius() - step);
		circle3.setRadius(circle.getRadius() + step);
		step += 0.5;
	}

	int b = 4;
	int b1 = 4;
	int b2 = 4;
	int b3 = 4;

	Serialize();

	return 0;
}