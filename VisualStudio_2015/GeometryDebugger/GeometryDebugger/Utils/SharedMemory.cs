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
using System.Diagnostics;

namespace GeometryDebugger.Utils
{
    public class SharedMemory
    {
        private const string m_S_MemoryFileName = "VariablesMemory"; // Общее имя памяти
        private MemoryMappedFile m_MMF_mmf;
        private string m_S_message = "";
        private DTE m_DTE;
        private List<Variable> m_variables;
        private bool isReadyMessages = false;

        public SharedMemory(List<Variable> variables, DTE dTE)
        {
            m_variables = variables;
            m_DTE = dTE;

            isReadyMessages = false;
        }
        public void CreateMessages()
        {
            if (m_variables.Count > 0)
            {
                m_S_message = "";

                for (int i = 0; i < m_variables.Count; i++)
                {
                    if (m_variables[i].m_B_IsSelected)
                    {
                        string R = ((float)m_variables[i].m_C_Color.m_i_R / 255).ToString().Replace(",", ".");
                        string G = ((float)m_variables[i].m_C_Color.m_i_G / 255).ToString().Replace(",", ".");
                        string B = ((float)m_variables[i].m_C_Color.m_i_B / 255).ToString().Replace(",", ".");

                        m_S_message += $"{m_variables[i].m_S_Name}|{m_variables[i].m_S_Type}|{m_variables[i].m_S_Addres}|{R}|{G}|{B}";
                        if (i < m_variables.Count - 1)
                            m_S_message += "|";
                    }
                }
            }
        }
        public void WriteToMemory()
        {
            if (m_S_message.Length > 0)
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

                    isReadyMessages = true;
                }
            }
        }
        public void DoSerialize()
        {
            if (isReadyMessages)
                m_DTE.Debugger.GetExpression("Serialize()", true, 1);
        }
        public string getResult()
        {
            if (isReadyMessages)
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

                                if (!result.ToString().Contains(":"))
                                {
                                    continue;
                                }
                                else
                                    return result.ToString();
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
            return "";
        }
    }
}
