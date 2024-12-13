using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace LearningWPF
{
    public class ControlHost : HwndHost
    {
        //        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\ContextOpenGL.dll",
        //    CallingConvention = CallingConvention.Cdecl,
        //    CharSet = CharSet.Unicode)]
        //        public static extern IntPtr createOpenGLWindow(
        //    [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
        //    uint dwStyle,
        //    uint dwExStyle = 0,
        //    int x = -1,
        //    int y = -1,
        //    int nWidth = -1,
        //    int nHeight = -1,
        //    IntPtr hWndParent = default,
        //    IntPtr hMenu = default
        //);
        //        // Импорт метода destroyOpenGLWindow
        //        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\ContextOpenGL.dll",
        //            CallingConvention = CallingConvention.Cdecl)]
        //        public static extern void destroyOpenGLWindow(IntPtr hwnd);

        //        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\ContextOpenGL.dll",
        //    CallingConvention = CallingConvention.Cdecl)]
        //        public static extern IntPtr HandleMessage(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        //        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\ContextOpenGL.dll")]
        //        public static extern void getMessages();

        //        public void getMessagesInHost()
        //        {
        //            getMessages();
        //        }

        //        int hostHeight, hostWidth;

        //        internal const int
        //              WS_CHILD = 0x40000000,
        //              WS_VISIBLE = 0x10000000,
        //              LBS_NOTIFY = 0x00000001,
        //              HOST_ID = 0x00000002,
        //              LISTBOX_ID = 0x00000001,
        //              WS_VSCROLL = 0x00200000,
        //              WS_BORDER = 0x00800000;

        //        public ControlHost(double height, double width)
        //        {
        //            hostHeight = (int)height;
        //            hostWidth = (int)width;
        //        }
        //        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //        {
        //            //Console.WriteLine($"HWND: {hwnd}; MSG: {msg}");
        //            uint uintValue = Convert.ToUInt32(msg);
        //            return HandleMessage(hwnd, uintValue, wParam, lParam);
        //        }
        //        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        //        {
        //            // Используем твою функцию для создания OpenGL окна
        //            IntPtr hwndControl = createOpenGLWindow(
        //                "My OpenGL Window",                      // Заголовок окна
        //                WS_CHILD | WS_VISIBLE,                    // Стиль окна (дочернее окно)
        //                0,                                        // Расширенный стиль (не используется)
        //                0,                                        // Координаты X
        //                0,                                        // Координаты Y
        //                hostWidth,                                // Ширина
        //                hostHeight,                               // Высота
        //                hwndParent.Handle,                        // Родительское окно
        //                IntPtr.Zero                               // Меню (не используется)
        //            );

        //            // Возвращаем HandleRef, который WPF будет использовать для управления окном
        //            return new HandleRef(this, hwndControl);
        //        }

        //        protected override void DestroyWindowCore(HandleRef hwnd)
        //        {
        //            // Уничтожение OpenGL окна и освобождение ресурсов
        //            destroyOpenGLWindow(hwnd.Handle);
        //        }
        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\ContextOpenGL.dll",
       CallingConvention = CallingConvention.Cdecl,
       CharSet = CharSet.Unicode)]
        public static extern bool glfwWindow(IntPtr hWndParent = default(IntPtr));

        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\ContextOpenGL.dll",
CallingConvention = CallingConvention.Cdecl,
CharSet = CharSet.Unicode)]
        public static extern IntPtr getNative();

        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\ContextOpenGL.dll",
    CallingConvention = CallingConvention.Cdecl,
    CharSet = CharSet.Unicode)]
        public static extern IntPtr createOpenGLWindow(
    [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
    uint dwStyle,
    uint dwExStyle = 0,
    int x = -1,
    int y = -1,
    int nWidth = -1,
    int nHeight = -1,
    IntPtr hWndParent = default(IntPtr),
    IntPtr hMenu = default(IntPtr)
);
        // Импорт метода destroyOpenGLWindow
        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\ContextOpenGL.dll",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void destroyOpenGLWindow(IntPtr hwnd);
        int hostHeight, hostWidth;

        internal const int
              WS_CHILD = 0x40000000,
              WS_VISIBLE = 0x10000000,
              LBS_NOTIFY = 0x00000001,
              HOST_ID = 0x00000002,
              LISTBOX_ID = 0x00000001,
              WS_VSCROLL = 0x00200000,
              WS_BORDER = 0x00800000;

        public ControlHost(double height, double width)
        {
            hostHeight = (int)height;
            hostWidth = (int)width;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            /*
            // Используем твою функцию для создания OpenGL окна
            IntPtr hwndControl = createOpenGLWindow(
                "My OpenGL Window",                      // Заголовок окна
                WS_CHILD | WS_VISIBLE,                    // Стиль окна (дочернее окно)
                0,                                        // Расширенный стиль (не используется)
                0,                                        // Координаты X
                0,                                        // Координаты Y
                hostWidth,                                // Ширина
                hostHeight,                               // Высота
                hwndParent.Handle,                        // Родительское окно
                IntPtr.Zero                               // Меню (не используется)
            );
            */
            glfwWindow(hwndParent.Handle);
            IntPtr hwndControl = getNative();
            // Возвращаем HandleRef, который WPF будет использовать для управления окном
            return new HandleRef(this, hwndControl);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            // Уничтожение OpenGL окна и освобождение ресурсов
            destroyOpenGLWindow(hwnd.Handle);
        }
    }
}
