using System;
using System.Diagnostics;
using System.Windows.Controls;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Windows;

namespace GeometryDebugger.UI
{
    public partial class GeometryDebuggerToolWindow : UserControl
    {
        private DebuggerEvents m_DE_debuggerEvents;

        public GeometryDebuggerToolWindow()
        {
            InitializeComponent();

            Loaded += GeometryDebuggerToolWindowLoaded;
        }

        public void GeometryDebuggerToolWindowLoaded(object sender, RoutedEventArgs e)
        {
            SubscribeOnDebugEvents();
        }


        public void SubscribeOnDebugEvents()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte == null)
            {
                Debug.WriteLine("DTE сервис не найден.");
                return;
            }

            m_DE_debuggerEvents = dte.Events.DebuggerEvents;
            m_DE_debuggerEvents.OnEnterBreakMode += OnEnterBreakMode;
            Debug.WriteLine("Подписка на события DebuggerEvents выполнена.");
        }

        public void UnsubscribeFromDebugEvents()
        {
            if (m_DE_debuggerEvents != null)
            {
                m_DE_debuggerEvents.OnEnterBreakMode -= OnEnterBreakMode;
                Debug.WriteLine("Отписка от событий DebuggerEvents выполнена.");
            }
        }

        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction action)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Debug.WriteLine($"EnterBreakMode: reason={reason}");
        }
    }
}
