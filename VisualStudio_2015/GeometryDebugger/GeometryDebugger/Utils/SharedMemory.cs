﻿using EnvDTE;
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
            if (m_variables.Count > 0) // если число переменных на сериализацию больше 0
            {
                m_S_message = "";

                for (int i = 0; i < m_variables.Count; i++)
                {
                    if (m_variables[i].m_B_IsSelected)
                    {
                        string R = ((float)m_variables[i].m_C_Color.m_i_R / 255).ToString().Replace(",", ".");
                        string G = ((float)m_variables[i].m_C_Color.m_i_G / 255).ToString().Replace(",", ".");
                        string B = ((float)m_variables[i].m_C_Color.m_i_B / 255).ToString().Replace(",", ".");

                        m_S_message += $"{m_variables[i].m_S_Name}|{m_variables[i].m_S_Type}|{m_variables[i].m_S_Source}|{m_variables[i].m_S_Addres}|{R}|{G}|{B}";
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
