#include <iostream>
#include <windows.h>
#include <winuser.h>
#include <GL/glew.h>
#include <GL/wglew.h>
#include <fstream>

/*
������ ������ � �������� ���� ��� Win32. (https://learn.microsoft.com/en-us/windows/win32/learnwin32/creating-a-window)
��� � �����, ��� �������� ���� ���������� ������� ��� � ��������� "�������" (�� �++ �����). ��� ��������� ������, 
����������� ������ ������������ ������� ��� ������������� � ��������� ���� (���� "�����" ��������� ���������� ����, ������
���:
	*����� ����
	*��������� ���� (���������� �������)
	*������, ������, ��� � ������ ���������.
���������� �� WNDCLASS.
�������� ��������� ������� (https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-wndclassa):
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

��� ����� ������ ��������� �������� ���������:

1) lpfnWndProc - ��������� �� ������� (��� �����, ��� ��������� ������ ����� ���� ��������), ������������� �����������, ���������� ������� ���������� 
	(window procedure / window proc)

	������ ���������� � ���� ������� ��������� 
	������ (https://learn.microsoft.com/en-us/windows/win32/learnwin32/writing-the-window-procedure)

	LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

	�� ���� ��������� 4 ���������:
		hwnd - ���������� ����
		uMsg - ��� ���������, ������ WM_SIZE (���� �������� ����������)
		wParam � lParam - ��� ������

	LRESULT - ������������� ��������, ������� ��������� �������� Windows (�������� ����� �� ������������ ���������)

2) hInstance - ���������� ���������� ����������. 
3) lpszClassName - �����, ���������������� ����� ����.

*/
/*
���������, ����������� � ���� (https://learn.microsoft.com/en-us/windows/win32/learnwin32/window-messages):

	��� ������� ������, ���������� ����, �� ������� ������� ��� ��������� ����.
	��� ������� �������� ��������� ��� ���� ����, ��������� � ���� ������.

	��� ������� ������� ������ ��������� �� ������ �������:
		MSG msg;
		GetMessage(&msg, NULL, 0, 0);

	����� ����� ����� �������:
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	, ��� TranslateMessage ������� � ������ � ����������.
	� DispatchMessage �������� ������������ ������� � ������������� ������� �������
		��������� ����, ����������� ����� ���������
	(� ��� ������� ��� �� �������, ������� LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);)
	�� ���� ���������� ���� � ����� ������� ����, ������� ��������� �������, ��������� � �����,
	� ������� �������.

	���������� ������, ������������ ����� ����� ������ ����, ������ ������� ��������� �������:
		1. �� �������� ��������� WM_LBUTTONDOWN � ������� ���������.
		2. ��������� �������� ������� GetMessage.
		3. GetMessage ��������� ��������� WM_LBUTTONDOWN �� ������� � ��������� ��������
			MSG
		4. ��������� �������� ������� TranslateMessgae � DispatchMessage.
		5. ������ DispatchMessage �� �������� ���� ������� ���������
		6. ������� ��������� ����� ����������� �� ��� ���������, ���� ���������������.

	������� ��� ���������� ����� ����, ������� ���������� ��������� ��������� �� ������� � ���������� ��

	��������:
		while (1)      
		{
				GetMessage(&msg, NULL, 0,  0);
				TranslateMessage(&msg); 
				DispatchMessage(&msg);
		}

	����� ���� ������� �� ����������, �������, ���� � ���� ����� �� ����, ��� ����������
		��������� PostQuitMessage(0);
	��� ������� �������� ��������� WM_QUIT � �������.
	��� ���� ��� ���������� ������� ���� ������� GetMessage, ������� ���� ����� ����������
	��������� �������:
	MSG msg = { };
	while (GetMessage(&msg, NULL, 0, 0) > 0)
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
	��� ���� ��������� WM_QUIT �� ����� ������������ � ������� ���������

	���� ��� ���� ���������, ��� "Posting" ��������� � "Sending" ���������, 
	��� "Posting" ������� ������������ � �������, ����� ����� ���� � GetMessage � 
	DispatchMessage
	��� "Sending" ������ ������� � �������� �� ������� ������� ���������.

	

���������� ���������� ���������� (https://learn.microsoft.com/en-us/windows/win32/learnwin32/managing-application-state-)

		��� ������������ ��������� ���� ����� ������������ ���������� ����������, �� ��� ������ ���� ���� 
	�����.
		������� CreateWindowEx ������������� ������ �������� ����� ��������� ������ � ����, �.�., ��� � �����,
	�� ����� ������� �����-�� ��������� ��������� � ��������� � � ��� �������, ������� ������� ����, 
	��� ����� �� �������� � ����� ���� ���������.
		����� ��� ������� ���������, ��� ���������� ��������� ��������� � ���� ������� ��������� (� ����. 
	�������):
		WM_NCCREATE
		WM_CREATE
	�� ��� �� ������������ ���������, ������������ �� ����� ���� �������, ���������, ������, ����� ���������������
		��� ��� ��������� ������������ �� ����, ��� ���� ���������� �������.
		��������� ������� CreateWindowEx - ��������� ���� void*. ������� � ���� ��������� ����� 
	�������� ����� �������� ���������. ��� ���� ��� ��������� ���� ��������� ������� ����������,
	�� ����� �������� ��� ���������.

	���������� ������, ����� ���� �������� ��������� ���� ����� ��������� ���:
		struct StateInfo {
			// ... (struct members not shown)
		};

		��������� �������� �� ��� ��������� ��� ������ CreateWindowEx � �������� ��������� void*
		
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

	��� ��������� ���� ���� ��������� lParam - ��������� �� ��������� CREATESTRUCT, � �������
	���� ��������� �� ���� �������� (�� ���� https://learn.microsoft.com/en-us/windows/win32/learnwin32/images/appstate01.png)

	����������:
		CREATESTRUCT *pCreate = reinterpret_cast<CREATESTRUCT*>(lParam);
		pState = reinterpret_cast<StateInfo*>(pCreate->lpCreateParams);
		SetWindowLongPtr(hwnd, GWLP_USERDATA, (LONG_PTR)pState);

		����� ����� ���������� ������ ������� �������� ���������� ��������� StateInfo � ������ 
	���������� ��� ����. ����� ����, ��� �� ��� ��������, �� ������ ������� �������� ��������� 
	������� �� ����, ������ GetWindowLongPtr :

	LONG_PTR ptr = GetWindowLongPtr(hwnd, GWLP_USERDATA);
	StateInfo *pState = reinterpret_cast<StateInfo*>(ptr);

	��� ������ ������ ������� ���������:
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
		glEnable(GL_DEPTH_TEST); // �������� ����� �������

		glLoadIdentity();

		//glPushMatrix();  // ���������� ������� �������

		// �������� ���� ������ ����
		glRotatef(angle, 0.0f, 1.0f, 0.0f);  // �������� ������ ��� X
		glRotatef(angle, 1.0f, 0.0f, 0.0f);  // �������� ������ ��� X
		glRotatef(angle, 0.0f, 0.0f, 1.0f);  // �������� ������ ��� X

		glBegin(GL_QUADS);

		// �������� �����
		glColor3f(1.0f, 0.0f, 0.0f); // �������
		glVertex3f(-0.5f, -0.5f, 0.5f);
		glVertex3f(0.5f, -0.5f, 0.5f);
		glVertex3f(0.5f, 0.5f, 0.5f);
		glVertex3f(-0.5f, 0.5f, 0.5f);

		// ������ �����
		glColor3f(0.0f, 1.0f, 0.0f); // ������
		glVertex3f(-0.5f, -0.5f, -0.5f);
		glVertex3f(-0.5f, 0.5f, -0.5f);
		glVertex3f(0.5f, 0.5f, -0.5f);
		glVertex3f(0.5f, -0.5f, -0.5f);

		// ������� �����
		glColor3f(0.0f, 0.0f, 1.0f); // �����
		glVertex3f(-0.5f, 0.5f, -0.5f);
		glVertex3f(-0.5f, 0.5f, 0.5f);
		glVertex3f(0.5f, 0.5f, 0.5f);
		glVertex3f(0.5f, 0.5f, -0.5f);

		// ������ �����
		glColor3f(1.0f, 1.0f, 0.0f); // Ƹ����
		glVertex3f(-0.5f, -0.5f, -0.5f);
		glVertex3f(0.5f, -0.5f, -0.5f);
		glVertex3f(0.5f, -0.5f, 0.5f);
		glVertex3f(-0.5f, -0.5f, 0.5f);

		// ������ �����
		glColor3f(1.0f, 0.0f, 1.0f); // ����������
		glVertex3f(0.5f, -0.5f, -0.5f);
		glVertex3f(0.5f, 0.5f, -0.5f);
		glVertex3f(0.5f, 0.5f, 0.5f);
		glVertex3f(0.5f, -0.5f, 0.5f);

		// ����� �����
		glColor3f(0.0f, 1.0f, 1.0f); // �������
		glVertex3f(-0.5f, -0.5f, -0.5f);
		glVertex3f(-0.5f, -0.5f, 0.5f);
		glVertex3f(-0.5f, 0.5f, 0.5f);
		glVertex3f(-0.5f, 0.5f, -0.5f);

		glEnd();

		//glPopMatrix();  // �������������� ���������� �������

		// ������ ������ (��� ������� �����������)
		SwapBuffers(m_hdc);

		angle += 0.01;

		//InvalidateRect(m_hwnd, NULL, TRUE); // �������� �� ���� ��� �����������
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
		return NULL; // ��� ��������� ����������
	}

	HWND m_hwnd = CreateWindowEx(
		dwExStyle, L"OpenGLWindow", lpWindowName, dwStyle, x, y,
		nWidth, nHeight, hWndParent, hMenu, GetModuleHandle(NULL), NULL);

	if (m_hwnd == NULL)
	{
		DWORD errorCode = GetLastError();
		// ����� ����� ���������� ������, ��������, ������� ���������
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