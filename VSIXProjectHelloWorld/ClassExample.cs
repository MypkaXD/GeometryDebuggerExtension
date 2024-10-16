using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Automation;

namespace VSIXProjectHelloWorld
{
    internal class ClassExample
    {

        public string message { get; set; }
        public bool isSet = false;
        public DTE m_DTE_Dte;
        public EnvDTE.DebuggerEvents m_DE_events;
        public UserControl1 ui;

        public ClassExample()
        {
            message = "";

            if (isSet)
                return;
            else
            {
                m_DTE_Dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SDTE)) as EnvDTE.DTE;

                if (m_DTE_Dte != null)
                {
                    m_DE_events = m_DTE_Dte.Events.DebuggerEvents;
                    m_DE_events.OnEnterBreakMode += OnEnterBreakMode;
                }

                init();
                isSet = true;
            }
        }

        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction action)
        {
            UpdateData();
        }

        private void getVariblesFromWatchList()
        {
            
            EnvDTE.Window customToolWindow;
            
            try
            {
                customToolWindow = m_DTE_Dte.Windows.Item("{90243340-BD7A-11D0-93EF-00A0C90F2734}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Watch window didn't find");
                return;
            }

            IntPtr hWndOfWatchWindow = customToolWindow.HWnd;  // Пример HWND (замени на актуальный)

            AutomationElement mainWindow = AutomationElement.FromHandle(hWndOfWatchWindow);

            PrintAutomationElement(mainWindow, 0);
        }

        private void UpdateData()
        {
            message = "";

            ObservableCollection<MyObject> objects = new ObservableCollection<MyObject>();

            EnvDTE80.DTE2 dte2 = (EnvDTE80.DTE2)m_DTE_Dte;
            EnvDTE90.Debugger3 debugger = (EnvDTE90.Debugger3)dte2.Debugger;
            EnvDTE.StackFrame currentFrame = debugger.CurrentStackFrame;
            // Iterate over Locals (variables in the current scope)
            foreach (EnvDTE.Expression localVariable in currentFrame.Locals)
            {

                var expression = m_DTE_Dte.Debugger.GetExpression("&" + localVariable.Name, true, 1);

                System.Diagnostics.Debug.WriteLine(expression.Type);

                objects.Add(new MyObject() { m_B_IsSelected = false, m_S_Name = localVariable.Name, m_S_Type = localVariable.Type, m_S_Addres = expression.Value, m_S_Source="LocalFrame"});
            }

            ui.dgObjects.ItemsSource = objects;

            getVariblesFromWatchList();

        }

        void GetElementsFromTreeGreed(AutomationElement element, int indent)
        {
            // Вывести имя и тип элемента
            string indentString = new string(' ', indent * 2);
            string name = element.Current.Name;
            string controlType = element.Current.ControlType.ProgrammaticName;
            string helpText = element.Current.HelpText;

            if (element.Current.ClassName == "TreeGridItem")
            {
                var expression = m_DTE_Dte.Debugger.GetExpression(name, true, 1);
                if (expression.IsValidValue)
                {
                    System.Diagnostics.Debug.Write($"Name: {name} ");
                    System.Diagnostics.Debug.WriteLine($"Type: {expression.Type}");
                }
                var children = element.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
                foreach (AutomationElement child in children)
                {
                    GetElementsFromTreeGreed(child, indent + 1); // Рекурсивный вызов для дочерних элементов
                }

            }
            else
                return;

        }

        void PrintAutomationElement(AutomationElement element, int indent)
        {
            // Вывести имя и тип элемента
            string indentString = new string(' ', indent * 2);
            string name = element.Current.Name;
            string controlType = element.Current.ControlType.ProgrammaticName;

            if (element.Current.ClassName == "TreeGrid")
            {
                System.Diagnostics.Debug.WriteLine("Find TreeGrid");
                var children = element.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
                foreach (AutomationElement child in children)
                {
                    if (child.Current.ClassName == "TreeGridItem")
                        GetElementsFromTreeGreed(child, indent + 1);
                }

                GetElementsFromTreeGreed(children[0], indent + 1);
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine($"{indentString}- Name: {name}, Control Type: {controlType}");

                // Получить все дочерние элементы
                var children = element.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
                foreach (AutomationElement child in children)
                {
                    PrintAutomationElement(child, indent + 1); // Рекурсивный вызов для дочерних элементов
                }
            }
        }

        private void init()
        {
            var window = new System.Windows.Window();
            ui = new UserControl1();
            window.Content = ui;
            window.Show();

            if (m_DTE_Dte.Debugger.CurrentMode == dbgDebugMode.dbgRunMode || m_DTE_Dte.Debugger.CurrentMode == dbgDebugMode.dbgBreakMode)
            {
                UpdateData();
            }
        }


    }
}
