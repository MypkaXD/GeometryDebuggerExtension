using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace VSIXProjectHelloWorld.Utils
{
    public class SharedMemory
    {
        private const string m_S_MemoryFileName = "VariablesMemory"; // Общее имя памяти
        private MemoryMappedFile m_MMF_mmf;
        private string m_S_message = "";
        private DTE m_DTE;

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess,
        IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress,
        IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        [Flags]
        private enum ProcessAccessFlags : uint
        {
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000,
            All = 0x001F0FFF
        }

        public SharedMemory(ObservableCollection<Variable> variables, DTE dTE, ControlHost host)
        {
            m_S_message = "";
            m_DTE = dTE;

            for (int i = 0; i < variables.Count; ++i)
            {
                if (variables[i].m_B_IsSelected)
                {
                    m_S_message += $"{variables[i].m_S_Name}|{variables[i].m_S_Type}|{variables[i].m_S_Addres}|{variables[i].m_C_Color.m_i_R}|{variables[i].m_C_Color.m_i_G}|{variables[i].m_C_Color.m_i_B}";
                    if (i < variables.Count - 1)
                        m_S_message += "|";
                }
            }

            if (m_S_message.Length > 0)
                WriteToMemory();

            int processID = 0;
            foreach (EnvDTE.Process proc in m_DTE.Debugger.DebuggedProcesses)
                processID = proc.ProcessID;

            var expr = m_DTE.Debugger.GetExpression("&Serialize");
            IntPtr addressOfFunc = (IntPtr)(ulong)new System.ComponentModel.UInt64Converter().ConvertFromString(expr.Value.Split(' ').First());

            IntPtr hHandle = OpenProcess(ProcessAccessFlags.All, false, processID);
            IntPtr createThreadRes = CreateRemoteThread(hHandle, IntPtr.Zero, 0, addressOfFunc, IntPtr.Zero, 0, out createThreadRes);
            CloseHandle(hHandle);

            //Заморозить
            var currentThread = m_DTE.Debugger.CurrentThread;
            currentThread.Freeze();

            //континью
            m_DTE.Debugger.Go(false);

            WaitAnswer();

            try
            {
                //размораживаем мейн поток
                m_DTE.Debugger.Break();
                if (currentThread != null)
                {
                    currentThread.Thaw();
                    m_DTE.Debugger.CurrentThread = currentThread;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("the process was break by closing window");
            }

            host.reloadGeomView();
        }

        private void WaitAnswer()
        {
            string mmfName = "SharedMemory";
            int retryCount = 0;
            int maxRetries = 60; // максимальное количество попыток (например, 1 минута)

            while (retryCount < maxRetries)
            {
                try
                {
                    using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(m_S_MemoryFileName))
                    {
                        using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                        {
                            // Проверяем первый байт, чтобы узнать, есть ли данные
                            int firstByte = stream.ReadByte();

                            // Если первый байт пустой (0), продолжаем ожидание
                            if (firstByte == 0)
                            {
                                retryCount++;
                                System.Threading.Thread.Sleep(1000); // Пауза 1 секунда перед повторной проверкой
                                continue;
                            }

                            // Считываем данные до нуль-терминатора
                            StringBuilder result = new StringBuilder();
                            result.Append((char)firstByte); // добавляем первый байт

                            int b;
                            while ((b = stream.ReadByte()) > 0) // Чтение до '\0'
                            {
                                result.Append((char)b);
                            }

                            // Выводим результат и завершаем цикл ожидания
                            System.Diagnostics.Debug.WriteLine("Прочитано из MMF: " + result.ToString());
                            
                            if (result.ToString() != "true")
                            {
                                continue;
                            }
                            else
                                break;
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    System.Diagnostics.Debug.WriteLine("MemoryMappedFile не найден.");
                    retryCount++;
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Произошла ошибка: " + ex.Message);
                    retryCount++;
                    System.Threading.Thread.Sleep(1000);
                }
            }

            if (retryCount >= maxRetries)
            {
                System.Diagnostics.Debug.WriteLine("Превышено максимальное количество попыток.");
            }
        }


        private void SerializeObjects()
        {
            
        }

        private void WriteToMemory()
        {
            // Ввод выражения для записи в общую память
            char[] message = m_S_message.ToCharArray();
            // Размер введенного сообщения
            int size = message.Length;
            int totalSizeOfMessage = size * 2 + 4; // 2 байта на каждый символ (UTF-16) + 4 байта для хранения размера

            if (m_MMF_mmf == null || m_MMF_mmf.SafeMemoryMappedFileHandle.IsClosed)
                m_MMF_mmf = MemoryMappedFile.CreateOrOpen(m_S_MemoryFileName, totalSizeOfMessage, MemoryMappedFileAccess.ReadWrite);

            using (MemoryMappedViewAccessor writer = m_MMF_mmf.CreateViewAccessor(0, totalSizeOfMessage))
            {
                // Записываем сначала размер сообщения (int — 4 байта)
                writer.Write(0, size);
                // Записываем само сообщение начиная с 4 байта
                writer.WriteArray<char>(4, message, 0, size);
            }

            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
