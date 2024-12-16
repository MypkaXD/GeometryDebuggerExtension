using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;

namespace GeometryDebugger.Utils
{
    public class DebuggerGetterVariables
    {
        private DTE m_DTE_Dte;

        public DebuggerGetterVariables()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            m_DTE_Dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as EnvDTE.DTE;
        }
        public DTE GetDTE()
        {
            return m_DTE_Dte;
        }
        public bool IsDebugMode()
        {
            if (GetDTE() != null && GetDTE().Debugger.CurrentMode == dbgDebugMode.dbgBreakMode)
                return true;
            return false;
        }
        public void GetVariablesFromCurrentStackFrame(ref ObservableCollection<Variable> variables)
        {
            variables = new ObservableCollection<Variable>();

            EnvDTE.StackFrame currentFrame = m_DTE_Dte.Debugger.CurrentStackFrame;

            foreach (EnvDTE.Expression localVariable in currentFrame.Locals)
            {

                EnvDTE.Expression expression = m_DTE_Dte.Debugger.GetExpression("&(" + localVariable.Name + ")", true, 1); // get addres of variable

                System.Diagnostics.Debug.WriteLine(localVariable.Type.GetHashCode());

                if (expression.IsValidValue)
                {
                    Variable currentVariable = new Variable()
                    {
                        m_B_IsAdded = false,
                        m_B_IsSelected = false,
                        m_S_Name = localVariable.Name,
                        m_S_Source = "LocalStackFrame",
                        m_S_Type = localVariable.Type.Replace(" ", ""),
                        m_S_Addres = expression.Value.Split(' ')[0],
                        m_C_Color = new Utils.Color(255, 0, 0)
                    };

                    if (!isContainVariable(currentVariable, variables))
                        variables.Add(currentVariable);
                    else
                        System.Windows.MessageBox.Show("ERROR: A variable with this address: " + currentVariable.m_S_Addres + " is already in the table.\nIt will not be added to it.");
                }
                else
                    System.Diagnostics.Debug.WriteLine($"ERROR: Can't get addres for value {localVariable.Name}");
            }
        }
        public void GetVariablesFromWatchList(ref ObservableCollection<Variable> variables)
        {
            variables = new ObservableCollection<Variable>();

            EnvDTE.Window customToolWindow;

            try
            {
                customToolWindow = m_DTE_Dte.Windows.Item("{90243340-BD7A-11D0-93EF-00A0C90F2734}"); // get Watch window by GUID
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Watch window didn't find");
                return;
            }

            AutomationElement mainWindow = AutomationElement.FromHandle((IntPtr)customToolWindow.HWnd);
            PrintAutomationElement(mainWindow, ref variables);
        }

        private bool isContainVariable(Variable variable, ObservableCollection<Variable> variables)
        {
            foreach (var currentVariable in variables)
            {
                if (variable.m_S_Addres == currentVariable.m_S_Addres)
                    return true;
            }
            return false;
        }

        private void GetElementsFromTreeGreed(AutomationElement element, ref ObservableCollection<Variable> variables)
        {
            string name = element.Current.Name;

            if (element.Current.ClassName == "TreeGridItem")
            {
                var expressionForTypeAndName = m_DTE_Dte.Debugger.GetExpression(name, true, 1);
                var expressionForAddress = m_DTE_Dte.Debugger.GetExpression("&(" + name + ")", true, 1);

                if (expressionForTypeAndName.IsValidValue && expressionForAddress.IsValidValue)
                {
                    Variable variable = new Variable()
                    {
                        m_B_IsAdded = false,
                        m_B_IsSelected = false,
                        m_S_Name = expressionForTypeAndName.Name,
                        m_S_Source = "WatchWindow",
                        m_S_Type = expressionForTypeAndName.Type.Replace(" ", ""),
                        m_S_Addres = expressionForAddress.Value.Split(' ')[0],
                        m_C_Color = new Utils.Color(0, 255, 0)
                    };

                    if (!isContainVariable(variable, variables))
                        variables.Add(variable);
                    else
                        System.Windows.MessageBox.Show("ERROR: A variable with this address: " + variable.m_S_Addres + " is already in the table.\nIt will not be added to it.");
                }
            }
            else
                return;
        }
        private void PrintAutomationElement(AutomationElement element, ref ObservableCollection<Variable> variables)
        {
            if (element.Current.ClassName == "TreeGrid")
            {
                var children = element.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
                foreach (AutomationElement child in children)
                    if (child.Current.ClassName == "TreeGridItem")
                        GetElementsFromTreeGreed(child, ref variables);
            }
            else
            {
                var children = element.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
                foreach (AutomationElement child in children)
                    PrintAutomationElement(child, ref variables); // Рекурсивный вызов для дочерних элементов
            }
        }
        public Variable GetElemetFromExpression(string name)
        {
            var expressionForTypeAndName = m_DTE_Dte.Debugger.GetExpression(name, true, 1);
            var expressionForAddress = m_DTE_Dte.Debugger.GetExpression("&(" + name + ")", true, 1);

            if (expressionForTypeAndName.IsValidValue && expressionForAddress.IsValidValue)
            {
                Variable variable = new Variable()
                {
                    m_B_IsAdded = true,
                    m_B_IsSelected = false,
                    m_S_Name = expressionForTypeAndName.Name,
                    m_S_Source = "AddedMySelf",
                    m_S_Type = expressionForTypeAndName.Type.Replace(" ", ""),
                    m_S_Addres = expressionForAddress.Value.Split(' ')[0],
                    m_C_Color = new Utils.Color(0, 0, 255)
                };

                return variable;
            }
            return null;
        }
    }
}
