using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExtension
{
    internal class GeometryDebugger
    {
        DTE m_DTE_dte;

        EnvDTE.DebuggerEvents m_DE_events;
        
        string message = "";

        public bool isInit { get; set; }
    
        public GeometryDebugger()
        {
            isInit = false;
        }

        public void init()
        {
            if (m_DTE_dte != null)
                return;
            else
            {
                m_DTE_dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SDTE)) as EnvDTE.DTE;

                if (m_DTE_dte != null)
                {
                    m_DE_events = m_DTE_dte.Events.DebuggerEvents;
                    m_DE_events.OnEnterBreakMode += OnEnterBreakMode;

                    isInit = true;
                }
            }
        }

        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction action)
        {
            message = "";

            EnvDTE80.DTE2 dte2 = (EnvDTE80.DTE2)m_DTE_dte;
            EnvDTE90.Debugger3 debugger = (EnvDTE90.Debugger3)dte2.Debugger;
            // Get the current stack frame (to access Locals/Auto variables)
            EnvDTE.StackFrame currentFrame = debugger.CurrentStackFrame;
            // Iterate over Locals (variables in the current scope)
            foreach (EnvDTE.Expression localVariable in currentFrame.Locals)
            {
                string variableName = localVariable.Name;
                string variableValue = localVariable.Value;
                System.Diagnostics.Debug.WriteLine($"Local Variable - Name: {variableName}, Value: {variableValue}");
                message += $"{localVariable.Name}|{localVariable.Type}|{localVariable.Value}|";
            }

            WriteObjectsToList();
        }

        private void WriteObjectsToList()
        {
          
        }
    }
}
