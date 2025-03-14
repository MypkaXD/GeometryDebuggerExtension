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
        private string m_S_response = "";
        private DTE m_DTE;
        private bool isReadyMessages = false;

        public SharedMemory(DTE dTE)
        {
            m_DTE = dTE;

            isReadyMessages = false;
        }
        public void CreateMessages(Variable variable)
        {
            m_S_message = "";

            if (variable.m_B_IsSelected && !variable.m_B_IsSerialized)
            {
                string R = ((float)variable.m_C_Color.m_i_R / 255).ToString().Replace(",", ".");
                string G = ((float)variable.m_C_Color.m_i_G / 255).ToString().Replace(",", ".");
                string B = ((float)variable.m_C_Color.m_i_B / 255).ToString().Replace(",", ".");

                m_S_message += $"{variable.m_S_Name}|{variable.m_S_Type}|{variable.m_S_Source}|{variable.m_S_Addres}|{R}|{G}|{B}";
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
            {
                EnvDTE.Expression expr = m_DTE.Debugger.GetExpression($"Serialize()", true, -1);
                m_S_response = expr.Value;
            }
            else
                m_S_response =  "";
        }
        public string getPath()
        {
            return m_S_response.Substring(m_S_response.IndexOf('|') + 1, m_S_response.IndexOf('\"', m_S_response.IndexOf('|') + 1) - m_S_response.IndexOf('|') - 1);
        }
        public string getResponse()
        {
            return m_S_response;
        }
    }
}
