using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using GeometryDebugger.UI;

namespace GeometryDebugger.Utils
{
    [Guid("2af6a638-bbf3-4d1d-aab8-6ccf3491edbe")]
    public class GeometryDebuggerControl : ToolWindowPane
    {
        private GeometryDebuggerToolWindow m_ui;

        public GeometryDebuggerControl() : base(null)
        {
            this.Caption = "GeometryDebuggerTool";
            m_ui = new GeometryDebuggerToolWindow();
            this.Content = m_ui;
        }
    }
}
