#include <iostream>
#include <windows.h>
#include <winuser.h>
#include <GL/glew.h>
#include <GL/wglew.h>

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

1) lpfnWndProc - ��������� �� �������, ������������� �����������, ���������� ������� ���������� 
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