using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace GeometryDebugger.Utils
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
        [DllImport("C:\\dev\\Source\\LearningWPF\\VisualStudio_2015\\GeometryDebugger\\Release\\GLtool.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr createGLtoolWindow(IntPtr hWndParent = default(IntPtr));

        [DllImport("C:\\dev\\Source\\LearningWPF\\VisualStudio_2015\\GeometryDebugger\\Release\\GLtool.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void destroyGLtoolWindow(IntPtr hwnd);

        [DllImport("C:\\dev\\Source\\LearningWPF\\VisualStudio_2015\\GeometryDebugger\\Release\\GLtool.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void reload(ref StringArrayData data, bool resetCamera);

        [DllImport("C:\\dev\\Source\\LearningWPF\\VisualStudio_2015\\GeometryDebugger\\Release\\GLtool.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void visibilities(ref StringArrayData data);

        private bool isFirst = true;

        public ControlHost()
        {
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            IntPtr hwndControl = createGLtoolWindow(hwndParent.Handle);
            return new HandleRef(this, hwndControl);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            destroyOpenGLWindow(hwnd.Handle);
        }

        public void destroyOpenGLWindow(IntPtr hwnd)
        {
            destroyGLtoolWindow(hwnd);
        }

        public void visibilityGeomView(string path, bool isVisible)
        {
            IntPtr[] stringPtrs = new IntPtr[1];
            bool[] bools = new bool[1];

            stringPtrs[0] = Marshal.StringToHGlobalAnsi(path + ".txt");
            bools[0] = isVisible;

            // Создаем и заполняем структуру
            StringArrayData data = new StringArrayData
            {
                Count = 1,
                StringArray = Marshal.UnsafeAddrOfPinnedArrayElement(stringPtrs, 0),
                BoolArray = Marshal.UnsafeAddrOfPinnedArrayElement(bools, 0)
            };

            // Передаем структуру в C++
            visibilities(ref data);

            // Освобождаем память
            foreach (IntPtr ptr in stringPtrs)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public void reloadGeomView(List<Tuple<string, bool>> files, string globalPath)
        {
            // Создаем массив указателей на строки
            IntPtr[] stringPtrs = new IntPtr[files.Count];
            bool[] bools = new bool[files.Count];

            for (int i = 0; i < files.Count; i++)
            {
                stringPtrs[i] = Marshal.StringToHGlobalAnsi(globalPath + "\\" + files[i].Item1 + ".txt");
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
            reload(ref data, isFirst);
            isFirst = false;

            // Освобождаем память
            foreach (IntPtr ptr in stringPtrs)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
