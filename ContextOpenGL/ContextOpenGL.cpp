#include <iostream>
#include <windows.h>
#include <winuser.h>
#include <GL/glew.h>
#include <GL/wglew.h>
#include <fstream>

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

1) lpfnWndProc - УКАЗАТЕЛЬ на функцию (это важно, при написании класса будут свои проблемы), опеределенную приложением, называемой оконной процедурой 
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
	При этом она заставляет вернуть ноль функцию GetMessage, поэтому цикл можно переделать
	следующим образом:
	MSG msg = { };
	while (GetMessage(&msg, NULL, 0, 0) > 0)
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
	При этом сообщение WM_QUIT не нужно обрабатывать в оконной процедуре

	Есть два типа сообщений, это "Posting" сообщения и "Sending" сообщения, 
	где "Posting" сначала отправляются в очередь, потом через цикл в GetMessage и 
	DispatchMessage
	где "Sending" минуют очередь и напрямую ОС вызвает оконную процедуру.

	

Управление состоянием приложения (https://learn.microsoft.com/en-us/windows/win32/learnwin32/managing-application-state-)

		Для отслеживания состояний окна можно использовать глобальные переменные, но что делать если окон 
	много.
		Функция CreateWindowEx предоставляет способ передачи любой структуры данных в окно, т.е., как я понял,
	мы можем создать какую-то рандомную стркутуру и отправить её в эту функцию, которая создает окно, 
	тем самым мы привяжем к этому окну структуру.
		Когда эта функция вызывется, она отправляет следующие сообщения в вашу оконную процедуру (в след. 
	порядке):
		WM_NCCREATE
		WM_CREATE
	НО ЭТО НЕ ЕДИНСТВЕННЫЕ сообщения, отправляемые во время этой функции, остальные, просто, можно проигнорировать
		Эти два сообщения отправляются до того, как окно становится видимым.
		Последний парамер CreateWindowEx - указатель типа void*. Поэтому в этом параметре можно 
	передать любое значение указателя. При этом при обработке этих сообщений оконной процедурой,
	мы можем вытащить эту структуру.

	Рассмотрим пример, пусть наша стрктура состояний окна имеет следующий вид:
		struct StateInfo {
			// ... (struct members not shown)
		};

		Передадим указатль на эту структуру при вызове CreateWindowEx в конечном параметре void*
		
			StateInfo *pState = new (std::nothrow) StateInfo;

			if (pState == NULL)
			{
				return 0;
			}

			// Initialize the structure members (not shown).

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
				pState      // Additional application data
				);

	При получении этих двух сообщений lParam - указатель на стркутуру CREATESTRUCT, в которой
	есть указатель на нашу стрктуру (см фото https://learn.microsoft.com/en-us/windows/win32/learnwin32/images/appstate01.png)

	Извлечение:
		CREATESTRUCT *pCreate = reinterpret_cast<CREATESTRUCT*>(lParam);
		pState = reinterpret_cast<StateInfo*>(pCreate->lpCreateParams);
		SetWindowLongPtr(hwnd, GWLP_USERDATA, (LONG_PTR)pState);

		Целью этого последнего вызова функции является сохранение указателя StateInfo в данных 
	экземпляра для окна. После того, как вы это сделаете, вы всегда сможете получить указатель 
	обратно из окна, вызвав GetWindowLongPtr :

	LONG_PTR ptr = GetWindowLongPtr(hwnd, GWLP_USERDATA);
	StateInfo *pState = reinterpret_cast<StateInfo*>(ptr);

	Для класса удобно сделать следующее:
		inline StateInfo* GetAppState(HWND hwnd)
		{
			LONG_PTR ptr = GetWindowLongPtr(hwnd, GWLP_USERDATA);
			StateInfo *pState = reinterpret_cast<StateInfo*>(ptr);
			return pState;
		}

*/

#include <iostream>
#include <windows.h>
#include <GL/glew.h>
#include <GL/wglew.h>

#define DLL_EXPORT extern "C" __declspec(dllexport)

std::ofstream of("C:\\out.txt");

HGLRC m_hGLRC;
HDC m_hdc;

float angle = 45.0f;

LRESULT CALLBACK FalseWndProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam){
	
	switch (uMsg)
	{
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hwnd, uMsg, wParam, lParam);
	}

	return 0;
}
HGLRC CreateFalseRenderingContext(HDC hDC)
{
	PIXELFORMATDESCRIPTOR pfd;

	// Choose a stub pixel format in order to get access to wgl functions.
	::SetPixelFormat(
		hDC,   // Device context.
		1,     // Index that identifies the pixel format to set. The various pixel formats supported by a device context are identified by one-based indexes.
		&pfd); // [out] Pointer to a PIXELFORMATDESCRIPTOR structure that contains the logical pixel format specification.

	// Create a fiction OpenGL rendering context.
	HGLRC hGLRC = wglCreateContext(hDC);

	wglMakeCurrent(hDC, hGLRC);

	return hGLRC;
}
void initGlewLibrary() {

	static bool isInit = false;

	if (isInit)
		return;

	isInit = true;
	HINSTANCE hInst = GetModuleHandle(NULL);

	// Create a struct describing window class.
	WNDCLASSEX wcex;
	wcex.cbSize = sizeof(WNDCLASSEX);                                 // struct size
	wcex.style = CS_OWNDC; // CS_HREDRAW | CS_VREDRAW |                  // window style
	wcex.lpfnWndProc = FalseWndProc;                                       // pointer to window function WndProc
	wcex.cbClsExtra = 0;                                                  // shared memory
	wcex.cbWndExtra = 0;                                                  // number of additional bytes
	wcex.hInstance = hInst;                                              // current application's handle
	wcex.hIcon = LoadIcon(hInst, MAKEINTRESOURCE(IDI_APPLICATION));  // icon handle
	wcex.hCursor = LoadCursor(NULL, IDC_CROSS);                        // cursor handle
	wcex.hbrBackground = (HBRUSH)(COLOR_MENU + 1);                             // background brush's handle
	wcex.lpszMenuName = NULL;                                               // pointer to a string - menu name
	wcex.lpszClassName = L"FalseWindow";                                       // pointer to a string - window class name
	wcex.hIconSm = LoadIcon(hInst, MAKEINTRESOURCE(IDI_APPLICATION));  // small icon's handle

	// Register window class for consequtive calls of CreateWindow or CreateWindowEx.
	RegisterClassEx(&wcex);

	// Create a FICTION window based on the previously registered window class.
	HWND hWnd = CreateWindow(L"FalseWindow",                  // window class name
		L"FalseWindow",                  // window title
		WS_OVERLAPPEDWINDOW,           // window type
		CW_USEDEFAULT, CW_USEDEFAULT,  // window's start position (x, y)
		100,                           // window's  width in pixels
		100,                           // window's  height in pixels
		NULL,                          // parent window
		NULL,                          // menu handle
		hInst,                         // application handle
		NULL);

	of << "SECOND" << hWnd << "\n";

	HDC hDC = GetDC(hWnd);

	// Create a fiction rendering context.
	HGLRC tempOpenGLContext = CreateFalseRenderingContext(hDC);

	glewInit();

	DestroyWindow(hWnd);

	MSG msg = { 0 };
	while (msg.message != WM_QUIT)
	{
		while (GetMessage(&msg, NULL, 0, 0))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	wglMakeCurrent(NULL, NULL);          // remove the temporary context from being active
	wglDeleteContext(tempOpenGLContext); // delete the temporary OpenGL context

}
void SetPixelFormat(HDC deviceContext, HWND m_hwnd)
{
	// Pixel format attributes array.
	int pixAttribs[] =
	{
		WGL_SUPPORT_OPENGL_ARB,   GL_TRUE,                   // nonzero value means "support OpenGL"
		WGL_DRAW_TO_WINDOW_ARB,   GL_TRUE,                   // true if the pixel format can be used with a window
		WGL_ACCELERATION_ARB,     WGL_FULL_ACCELERATION_ARB, // hardware acceleration through ICD driver
		WGL_DOUBLE_BUFFER_ARB,    GL_TRUE,                   // nonzero value means "double buffering"
		WGL_SAMPLE_BUFFERS_ARB,   GL_TRUE,                   // support multisampling
		WGL_PIXEL_TYPE_ARB,       WGL_TYPE_RGBA_ARB,         // color mode (either WGL_TYPE_RGBA_ARB or WGL_TYPE_COLORINDEX_ARB)
		WGL_COLOR_BITS_ARB,       32,                        // bits number in the color buffer for R, G and B channels
		WGL_DEPTH_BITS_ARB,       24,                        // bits number in the depth buffer
		WGL_STENCIL_BITS_ARB,     8,                         // bits number in the stencil buffer
		WGL_SAMPLES_ARB,          8,                         // multisampling factor
		0                                                    // "end of array" symbol
	};

	m_hdc = GetDC(m_hwnd);

	int numFormats = 0;
	int pixelFormat = -1;

	// Find the most relevant pixel format for the specified attributes.
	wglChoosePixelFormatARB(
		deviceContext,       // device context
		&pixAttribs[0],      // list of integer attributes
		NULL,                // list of float attributes
		1,                   // the maximum number of pixel formats to be obtained
		&pixelFormat,        // [out] pointer to the array of pixel formats
		(UINT*)&numFormats); // the number of appropriate pixel formats found

	// Set pixel format for the window device context.
	PIXELFORMATDESCRIPTOR pfd;
	::SetPixelFormat(
		deviceContext,
		pixelFormat,
		&pfd);
}
void CreateRenderingContext(HDC deviceContext)
{
	// Create OpenGL rendering context.
	m_hGLRC = wglCreateContextAttribsARB(
		deviceContext,
		0, NULL);

	// Make the OpenGL rendering context current.
	wglMakeCurrent(deviceContext, m_hGLRC);
}

void deleteContext() {
	wglMakeCurrent(NULL, NULL);          // remove the temporary context from being active
	wglDeleteContext(m_hGLRC); // delete the temporary OpenGL context
}

DLL_EXPORT LRESULT CALLBACK HandleMessage(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	switch (uMsg)
	{
	case WM_DESTROY:
		PostQuitMessage(0);
		return 0;

	case WM_PAINT:
	{

		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		glEnable(GL_DEPTH_TEST); // Включаем буфер глубины

		glLoadIdentity();

		//glPushMatrix();  // Сохранение текущей матрицы

		// Вращение куба вокруг осей
		glRotatef(angle, 0.0f, 1.0f, 0.0f);  // Вращение вокруг оси X
		glRotatef(angle, 1.0f, 0.0f, 0.0f);  // Вращение вокруг оси X
		glRotatef(angle, 0.0f, 0.0f, 1.0f);  // Вращение вокруг оси X

		glBegin(GL_QUADS);

		// Передняя грань
		glColor3f(1.0f, 0.0f, 0.0f); // Красная
		glVertex3f(-0.5f, -0.5f, 0.5f);
		glVertex3f(0.5f, -0.5f, 0.5f);
		glVertex3f(0.5f, 0.5f, 0.5f);
		glVertex3f(-0.5f, 0.5f, 0.5f);

		// Задняя грань
		glColor3f(0.0f, 1.0f, 0.0f); // Зелёная
		glVertex3f(-0.5f, -0.5f, -0.5f);
		glVertex3f(-0.5f, 0.5f, -0.5f);
		glVertex3f(0.5f, 0.5f, -0.5f);
		glVertex3f(0.5f, -0.5f, -0.5f);

		// Верхняя грань
		glColor3f(0.0f, 0.0f, 1.0f); // Синяя
		glVertex3f(-0.5f, 0.5f, -0.5f);
		glVertex3f(-0.5f, 0.5f, 0.5f);
		glVertex3f(0.5f, 0.5f, 0.5f);
		glVertex3f(0.5f, 0.5f, -0.5f);

		// Нижняя грань
		glColor3f(1.0f, 1.0f, 0.0f); // Жёлтая
		glVertex3f(-0.5f, -0.5f, -0.5f);
		glVertex3f(0.5f, -0.5f, -0.5f);
		glVertex3f(0.5f, -0.5f, 0.5f);
		glVertex3f(-0.5f, -0.5f, 0.5f);

		// Правая грань
		glColor3f(1.0f, 0.0f, 1.0f); // Фиолетовая
		glVertex3f(0.5f, -0.5f, -0.5f);
		glVertex3f(0.5f, 0.5f, -0.5f);
		glVertex3f(0.5f, 0.5f, 0.5f);
		glVertex3f(0.5f, -0.5f, 0.5f);

		// Левая грань
		glColor3f(0.0f, 1.0f, 1.0f); // Голубая
		glVertex3f(-0.5f, -0.5f, -0.5f);
		glVertex3f(-0.5f, -0.5f, 0.5f);
		glVertex3f(-0.5f, 0.5f, 0.5f);
		glVertex3f(-0.5f, 0.5f, -0.5f);

		glEnd();

		//glPopMatrix();  // Восстановление предыдущей матрицы

		// Меняем буферы (для двойной буферизации)
		SwapBuffers(m_hdc);

		angle += 0.01;

		//InvalidateRect(m_hwnd, NULL, TRUE); // Пометить всё окно для перерисовки
	}
	return 0;

	default:
		return DefWindowProc(hwnd, uMsg, wParam, lParam);
	}
	return TRUE;
}
DLL_EXPORT HWND createOpenGLWindow(PCWSTR lpWindowName, DWORD dwStyle, DWORD dwExStyle = 0, int x = CW_USEDEFAULT, int y = CW_USEDEFAULT, int nWidth = CW_USEDEFAULT, int nHeight = CW_USEDEFAULT, HWND hWndParent = 0, HMENU hMenu = 0) {

	initGlewLibrary();

	WNDCLASS wc = { 0 };

	wc.lpfnWndProc = HandleMessage;
	wc.hInstance = GetModuleHandle(NULL);
	wc.lpszClassName = L"OpenGLWindow";
	wc.style = CS_HREDRAW | CS_VREDRAW | CS_OWNDC;

	if (!RegisterClass(&wc)) {
		DWORD errorCode = GetLastError();
		of << "RegisterClass failed with error: " << errorCode << "\n";
		return NULL; // или выбросьте исключение
	}

	HWND m_hwnd = CreateWindowEx(
		dwExStyle, L"OpenGLWindow", lpWindowName, dwStyle, x, y,
		nWidth, nHeight, hWndParent, hMenu, GetModuleHandle(NULL), NULL);

	if (m_hwnd == NULL)
	{
		DWORD errorCode = GetLastError();
		// Здесь можно обработать ошибку, например, вывести сообщение
		of << "ERROR: " << errorCode << "\n";
	}

	of << "MAIN" << m_hwnd << "\n";
	of << "PARENT" << hWndParent << "\n";
	of.close();

	SetPixelFormat(GetDC(m_hwnd), m_hwnd);
	CreateRenderingContext(GetDC(m_hwnd));

	return m_hwnd;
}
DLL_EXPORT void destroyOpenGLWindow(HWND hwnd) {
	
	PostQuitMessage(0);

	DestroyWindow(hwnd);

	wglMakeCurrent(NULL, NULL);          // remove the temporary context from being active
	wglDeleteContext(m_hGLRC); // delete the temporary OpenGL context
}