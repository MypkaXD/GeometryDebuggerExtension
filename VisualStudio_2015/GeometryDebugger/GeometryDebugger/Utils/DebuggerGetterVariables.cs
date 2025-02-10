﻿using EnvDTE;
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
using Accessibility;
using System.Runtime.InteropServices;
using System.Windows.Automation;

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

            AutomationElement root = AutomationElement.FromHandle((IntPtr)customToolWindow.HWnd);
            Guid guidIAccessible = typeof(IAccessible).GUID;

            IAccessible window;

            if (AccessibleObjectFromWindow((IntPtr)customToolWindow.HWnd, OBJID_CLIENT, ref guidIAccessible, out window) == 0 && window != null)
            {
                GetChildrenFromIAccessible(window);
            }

        }

        void GetChildrenFromIAccessible(IAccessible accessible)
        {
            int childCount = accessible.accChildCount;

            if (childCount > 0)
            {
                object[] children = new object[childCount];
                int fetched = 0;
                if (AccessibleChildren(accessible, 0, childCount, children, out fetched) == 0)
                {
                    for (int i = 0; i < fetched; ++i)
                    {
                        if (children[i] is IAccessible)
                        {
                            IAccessible child = children[i] as IAccessible;
                            System.Diagnostics.Debug.WriteLine($"Дочерний элемент: {child.accName}");
                            GetChildrenFromIAccessible(child);
                        }
                        else if (children[i] is System.Int32)
                        {
                            System.Int32 childId = (System.Int32)(children[i]);

                            // Получаем свойства
                            string name = accessible.get_accName(childId); // 0 - это CHILDID_SELF
                            try
                            {
                                string value = accessible.get_accValue(childId); // 0 означает CHILDID_SELF (сам элемент)
                                System.Diagnostics.Debug.WriteLine(value);
                            }
                            catch
                            {

                            }
                            System.Diagnostics.Debug.WriteLine(name);

                        }
                    }
                }
            }
        }
        [DllImport("Oleacc.dll")]
        public static extern int WindowFromAccessibleObject(IAccessible pacc, out IntPtr phwnd);


        [DllImport("oleacc.dll")]
        private static extern int AccessibleObjectFromWindow(IntPtr hwnd, uint idObject, ref Guid riid, out IAccessible ppvObject);

        private const uint OBJID_CLIENT = 0xFFFFFFFC;

        [DllImport("oleacc.dll")]
        private static extern int AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren, [Out] object[] rgvarChildren, out int pcObtained);


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

            var rawViewWalker = TreeWalker.RawViewWalker;
            AutomationElement child = rawViewWalker.GetFirstChild(element);

            while (child != null)
            {
                if (child.Current.ClassName == "TreeGridItem")
                    GetElementsFromTreeGreed(child, ref variables);

                child = rawViewWalker.GetNextSibling(child);
            }

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
            

            if (element.Current.ClassName == "TREEGRID")
            {
                //var children = element.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
                //foreach (AutomationElement child in children)
                //    if (child.Current.ClassName == "TreeGridItem")
                //        GetElementsFromTreeGreed(child, ref variables);
                var rawViewWalker = TreeWalker.RawViewWalker;
                AutomationElement child = rawViewWalker.GetFirstChild(element);

                while (child != null)
                {
                    if (child.Current.ClassName == "TreeGridItem")
                        GetElementsFromTreeGreed(child, ref variables);

                    child = rawViewWalker.GetNextSibling(child);
                }
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
