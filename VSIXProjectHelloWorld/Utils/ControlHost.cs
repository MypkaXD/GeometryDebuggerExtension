using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace VSIXProjectHelloWorld.Utils
{

    [StructLayout(LayoutKind.Sequential)]
    public struct StringArrayData
    {
        public int Count;
        public IntPtr StringArray; // Указатель на массив строк
        public IntPtr BoolArray; // Указатель на массив булевых значений
    }
    public class ControlHost : HwndHost
    {
        [DllImport("C:\\Users\\MypkaXD\\Desktop\\wpfOpenGL\\x64\\Release\\GLtool.dll",
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Unicode)]
        public static extern IntPtr createGLtoolWindow(IntPtr hWndParent = default(IntPtr));

        // Импорт метода destroyOpenGLWindow
        [DllImport("C:\\Users\\MypkaXD\\Desktop\\wpfOpenGL\\x64\\Release\\GLtool.dll",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void destroyGLtoolWindow(IntPtr hwnd);

        [DllImport("C:\\Users\\MypkaXD\\Desktop\\wpfOpenGL\\x64\\Release\\GLtool.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void reload(ref StringArrayData data);

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
            IntPtr hwndControl = createGLtoolWindow(hwndParent.Handle);
            return new HandleRef(this, hwndControl);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            destroyGLtoolWindow(hwnd.Handle);
        }

        public void reloadGeomView(List<Tuple<string, bool>> files)
        {
            // Создаем массив указателей на строки
            IntPtr[] stringPtrs = new IntPtr[files.Count];
            bool[] bools = new bool[files.Count];

            for (int i = 0; i < files.Count; i++)
            {
                stringPtrs[i] = Marshal.StringToHGlobalAnsi("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\ReadMemoryMappedFile\\" + files[i].Item1 + ".txt");
                bools[i] = files[i].Item2;
            }

            // Создаем и заполняем структуру
            StringArrayData data = new StringArrayData
            {
                Count = files.Count,
                StringArray = Marshal.UnsafeAddrOfPinnedArrayElement(stringPtrs, 0),
                BoolArray = Marshal.UnsafeAddrOfPinnedArrayElement(bools, 0)
            };

            // Передаем структуру в C++
            reload(ref data);

            // Освобождаем память
            foreach (IntPtr ptr in stringPtrs)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
