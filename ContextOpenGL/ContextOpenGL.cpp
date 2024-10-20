#include <iostream>
#include <windows.h>
#include <winuser.h>
#include <GL/glew.h>
#include <GL/wglew.h>

/*
Начнем разбор с создания окна для Win32. (https://learn.microsoft.com/en-us/windows/win32/learnwin32/creating-a-window)
Как я понял, для создания окна необходимо связать его с некоторым "классом" (не с++ класс). Это структура данных, 
исользуемая внутри операционной системы для идентификации и настрокий окон (этот "класс" управляет свойствами окон, такими
как:
	*стиль окна
	*процедура окна (обработчик событий)
	*иконка, курсор, фон и другие параметры.
Называется он WNDCLASS.
Выглядит следующим образом (https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-wndclassa):
typedef struct tagWNDCLASSA {
  UINT      style;
  WNDPROC   lpfnWndProc;
  int       cbClsExtra;
  int       cbWndExtra;
  HINSTANCE hInstance;
  HICON     hIcon;
  HCURSOR   hCursor;
  HBRUSH    hbrBackground;
  LPCSTR    lpszMenuName;
  LPCSTR    lpszClassName;
} WNDCLASSA, *PWNDCLASSA, *NPWNDCLASSA, *LPWNDCLASSA;

Нам нужно задать следующие элементы структуры:

1) lpfnWndProc - указатель на функцию, опеределенную приложением, называемой оконной процедурой 
	(window procedure / window proc)

	Найдем информацию о этой оконной процедуре 
	Ссылка (https://learn.microsoft.com/en-us/windows/win32/learnwin32/writing-the-window-procedure)

	LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

	На вход принимаем 4 параметра:
		hwnd - дескриптор окна
		uMsg - код сообщения, пример WM_SIZE (окно поменяло разрешение)
		wParam и lParam - доп данные

	LRESULT - целочисленное значение, которое программа отсылает Windows (содержит ответ на определенное сообщение)

2) hInstance - дескриптор экземпляра приложения. 
3) lpszClassName - стока, идентифицирующая класс окна.

*/

/*
Сообщения, поступающие в окно (https://learn.microsoft.com/en-us/windows/win32/learnwin32/window-messages):

	Для каждого потока, создающего окно, ОС создаст очередь для сообщений окна.
	Эта очередь содержит сообщения ДЛЯ ВСЕХ окон, созданных в этом потоке.

	Эта функция удаляет первое сообщение из головы очереди:
		MSG msg;
		GetMessage(&msg, NULL, 0, 0);

	После этого нужно вызвать:
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	, где TranslateMessage связана с вводом с клавиатуры.
	а DispatchMessage сообщает операционной системе о необходимости вызвать оконную
		процедуру окна, являющегося целью сообщения
	(я так понимаю это та функция, которая LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);)
	ОС ищет дескриптор окна в своей таблице окон, находит указатель функции, связанный с окном,
	и вызвает функцию.

	Рассмотрим пример, пользователь нажал левую кнопку мыши, отсюда цепочка следующих событий:
		1. ОС помещает сообщение WM_LBUTTONDOWN в очередь сообщений.
		2. Программа вызывает функцию GetMessage.
		3. GetMessage извлекает сообщение WM_LBUTTONDOWN из очереди и заполняет стрктуру
			MSG
		4. Программа вызывает функции TranslateMessgae и DispatchMessage.
		5. Внутри DispatchMessage ОС вызывает вашу оконную процедуру
		6. Оконная процедура может реагировать на это сообщение, либо проигнорировать.

	Поэтому это необходимо иметь цикл, который непрерывно извлекает сообщения из очереди и отправляет их

	Например:
		while (1)      
		{
				GetMessage(&msg, NULL, 0,  0);
				TranslateMessage(&msg); 
				DispatchMessage(&msg);
		}

	Такой цикл никогда не закончится, поэтому, если я хочу выйти из него, мне необходимо
		выполнить PostQuitMessage(0);
	Эта функция помещает сообщение WM_QUIT в очередь.

*/

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	return;
}

int main() {

		// Register the window class.
	const wchar_t CLASS_NAME[]  = L"Sample Window Class";

	WNDCLASS wc = { };

	wc.lpfnWndProc   = WindowProc;
	wc.hInstance     = hInstance;
	wc.lpszClassName = CLASS_NAME;

	RegisterClass(&wc);

	// Create the window.

	HWND hwnd = CreateWindowEx(
		0,                              // Optional window styles.
		CLASS_NAME,                     // Window class
		L"Learn to Program Windows",    // Window text
		WS_OVERLAPPEDWINDOW,            // Window style

		// Size and position
		CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,

		NULL,       // Parent window    
		NULL,       // Menu
		hInstance,  // Instance handle
		NULL        // Additional application data
		);

	if (hwnd == NULL)
	{
		return 0;
	}

	ShowWindow(hwnd);
}