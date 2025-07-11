using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using static GeometryDebugger.Utils.ControlHost;

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

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallbackDelegate(StringArrayData data);
        private CallbackDelegate _callback;

        [DllImport("GLTool.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr createGLtoolWindow(CallbackDelegate callback, IntPtr hWndParent = default(IntPtr));

        [DllImport("GLTool.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void destroyGLtoolWindow(IntPtr hwnd);

        [DllImport("GLTool.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void reload(ref StringArrayData data, bool resetCamera);

        [DllImport("GLTool.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void visibilities(ref StringArrayData data);

        [DllImport("GLTool.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void highlight(ref StringArrayData data);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        internal static extern bool DestroyWindow(IntPtr hwnd);

        GeometryDebugger.UI.GeometryDebuggerToolWindow window = null;

        // Callback метод
        public void MyCallBack(StringArrayData data)
        {
            IntPtr stringPtr = Marshal.ReadIntPtr(data.StringArray, 0);
            string str = Marshal.PtrToStringAnsi(stringPtr);
            int depth_of_selected_variable = -1;
            int index_of_selected_variable = -1;

            if (window != null)
            {
                string name_of_selected_variable = str.Substring(str.IndexOf("vis_dbg_") + "vis_dbg_".Length, str.LastIndexOf("_depth") - str.IndexOf("vis_dbg_") - "vis_dbg_".Length);

                for (int j = 0; j < window.m_OBOV_Variables.Count; ++j)
                {
                    string current_string = Util.getPathOfVariable("", window.m_OBOV_Variables[j]);
                    string name_of_variable = current_string.Substring(0, current_string.LastIndexOf("_depth"));

                    if (name_of_variable == name_of_selected_variable)
                    {
                        depth_of_selected_variable = System.Convert.ToInt32(current_string.Substring(current_string.LastIndexOf("_depth") + "_depth".Length, current_string.Length - current_string.LastIndexOf("_depth") - "_depth".Length));
                        index_of_selected_variable = j;
                        break;
                    }
                }
            }

            if (depth_of_selected_variable != -1 && index_of_selected_variable != -1)
            {
                if (window != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.window.dgObjects.SelectedIndex = index_of_selected_variable;
                    });
                }
            }

        }


        private IntPtr m_Hwnd = IntPtr.Zero;

        public ControlHost(GeometryDebugger.UI.GeometryDebuggerToolWindow window)
        {
            this.window = window;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            _callback = MyCallBack;
            IntPtr hwndControl = createGLtoolWindow(_callback, hwndParent.Handle);
            m_Hwnd = hwndControl;

            return new HandleRef(this, hwndControl);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            DestroyWindow(hwnd.Handle);
        }

        public void destroyOpenGLWindow()
        {
            DestroyWindowCore(new HandleRef(this, m_Hwnd));
        }

        public void visibilityGeomView(string path, string globalPath,  bool isVisible)
        {
            IntPtr[] stringPtrs = new IntPtr[1];
            bool[] bools = new bool[1];

            stringPtrs[0] = Marshal.StringToHGlobalAnsi(globalPath + "\\\\" + path + ".txt");
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

        public void reloadGeomView(List<Tuple<string, bool>> files, string globalPath, bool isResetCamera = false)
        {
            IntPtr[] stringPtrs = new IntPtr[files.Count];
            bool[] bools = new bool[files.Count];

            for (int i = 0; i < files.Count; i++)
            {
                stringPtrs[i] = Marshal.StringToHGlobalAnsi(globalPath + "\\\\" + files[i].Item1 + ".txt");
                bools[i] = files[i].Item2;
            }

            StringArrayData data = new StringArrayData
            {
                Count = files.Count,
                StringArray = Marshal.UnsafeAddrOfPinnedArrayElement(stringPtrs, 0),
                BoolArray = Marshal.UnsafeAddrOfPinnedArrayElement(bools, 0)
            };

            reload(ref data, isResetCamera);

            foreach (IntPtr ptr in stringPtrs)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public void highlightGeomView(List<Tuple<String, bool>> files)
        {
            IntPtr[] stringPtrs = new IntPtr[files.Count];
            bool[] bools = new bool[files.Count];

            for (int i = 0; i < files.Count; i++)
            {
                stringPtrs[i] = Marshal.StringToHGlobalAnsi(files[i].Item1);
                bools[i] = files[i].Item2;
            }

            StringArrayData data = new StringArrayData
            {
                Count = files.Count,
                StringArray = Marshal.UnsafeAddrOfPinnedArrayElement(stringPtrs, 0),
                BoolArray = Marshal.UnsafeAddrOfPinnedArrayElement(bools, 0)
            };

            highlight(ref data);

            foreach (IntPtr ptr in stringPtrs)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
