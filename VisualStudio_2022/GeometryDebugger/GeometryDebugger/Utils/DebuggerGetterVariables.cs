using EnvDTE;
using System.Windows;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Shell;
using Accessibility;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.ComponentModel;

namespace GeometryDebugger.Utils
{
    public class DebuggerGetterVariables
    {
        [DllImport("Oleacc.dll")]
        public static extern int WindowFromAccessibleObject(IAccessible pacc, out IntPtr phwnd);
        [DllImport("oleacc.dll")]
        private static extern int AccessibleObjectFromWindow(IntPtr hwnd, uint idObject, ref Guid riid, out IAccessible ppvObject);
        [DllImport("oleacc.dll")]
        private static extern int AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren, [Out] object[] rgvarChildren, out int pcObtained);

        private const uint OBJID_CLIENT = 0xFFFFFFFC;

        private DTE m_DTE_Dte;

        public DebuggerGetterVariables()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            m_DTE_Dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as EnvDTE.DTE; // получаем EnvDTE
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

        public Variable GetElementFromExpression(string name, string source, Utils.Color color, bool isAdded)
        {
            var expressionForTypeAndName = m_DTE_Dte.Debugger.GetExpression(name, true, 1);
            var expressionForAddress = m_DTE_Dte.Debugger.GetExpression("&(" + name + ")", true, 1);

            if (expressionForTypeAndName.IsValidValue && expressionForAddress.IsValidValue)
            {
                Variable variable = new Variable()
                {
                    m_B_IsAdded = isAdded,
                    m_B_IsSelected = false,
                    m_B_IsSerialized = false,
                    m_S_Name = expressionForTypeAndName.Name,
                    m_S_Source = source,
                    m_S_Type = expressionForTypeAndName.Type,
                    m_S_Addres = expressionForAddress.Value.Split(' ')[0],
                    m_C_Color = color
                };


                if (variable.m_S_Type.Contains("{") || variable.m_S_Type.Contains("}"))
                {
                    int indexOfLeftBracket = variable.m_S_Type.IndexOf('{');
                    int indexOfRightBracket = variable.m_S_Type.IndexOf('}');
                    string removedString = variable.m_S_Type.Substring(indexOfLeftBracket, indexOfRightBracket - indexOfLeftBracket + 1);
                    variable.m_S_Type = variable.m_S_Type.Replace(removedString, "");
                }


                return variable;
            }
            return null;
        }
        //
        //////////
        ////////////////////////////////////////////////////////////
        //////////
        // method for get variables from CurrentStackFrame
        public void GetVariablesFromCurrentStackFrame(ref ObservableCollection<Variable> variables)
        {
            variables = new ObservableCollection<Variable>();

            EnvDTE.StackFrame currentFrame = m_DTE_Dte.Debugger.CurrentStackFrame;

            foreach (EnvDTE.Expression localVariable in currentFrame.Locals)
            {

                EnvDTE.Expression expression = m_DTE_Dte.Debugger.GetExpression("&(" + localVariable.Name + ")", true, 1); // get address of variable

                if (expression.IsValidValue)
                {
                    Variable currentVariable = new Variable()
                    {
                        m_B_IsAdded = false,
                        m_B_IsSelected = false,
                        m_B_IsSerialized = false,
                        m_S_Name = localVariable.Name,
                        m_S_Source = "LocalStackFrame",
                        m_S_Type = localVariable.Type,
                        m_S_Addres = expression.Value.Split(' ')[0],
                        m_C_Color = new Utils.Color(255, 0, 0)
                    };

                    if (currentVariable.m_S_Type.Contains("{") || currentVariable.m_S_Type.Contains("}"))
                    {
                        int indexOfLeftBracket = currentVariable.m_S_Type.IndexOf('{');
                        int indexOfRightBracket = currentVariable.m_S_Type.IndexOf('}');
                        string removedString = currentVariable.m_S_Type.Substring(indexOfLeftBracket, indexOfRightBracket - indexOfLeftBracket + 1);
                        currentVariable.m_S_Type = currentVariable.m_S_Type.Replace(removedString, "");
                    }

                    variables.Add(currentVariable);
                }
                else
                    MessageBox.Show($"ERROR: Can't get addres for value {localVariable.Name}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //
        //////////
        ////////////////////////////////////////////////////////////
        //////////
        // method for get variables from WatchWindow
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
                MessageBox.Show($"ERROR: \"Watch 1\" window didn't find. Try to open window \"Watch 1\"", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AutomationElement mainWindow = AutomationElement.FromHandle(customToolWindow.HWnd);

            if (mainWindow == null)
                MessageBox.Show($"ERROR: Treegrid wasn't find in window \"Watch 1\"", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                GetChildrenFromAutomationElement(mainWindow, ref variables);
        }
        void GetChildrenFromAutomationElement(AutomationElement element, ref ObservableCollection<Variable> variables)
        {
            if (element.Current.ClassName == "TreeGrid")
            {
                var children = element.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
                Queue<Tuple<string, int>> container = new Queue<Tuple<string, int>>();
                GetElementsFromTreeGrid(children, ref container);
                getVariablesFromQueue(ref container, ref variables);
            }
            else
            {
                var children = element.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
                foreach (AutomationElement child in children)
                    GetChildrenFromAutomationElement(child, ref variables); // Рекурсивный вызов для дочерних элементов
            }
        }
        void GetElementsFromTreeGrid(AutomationElementCollection treeGrid, ref Queue<Tuple<string, int>> container, int lvl = 1)
        {
            foreach (AutomationElement child in treeGrid)
            {
                if (child.Current.ClassName == "TreeGridItem")
                {
                    string currentName = child.Current.Name;
                    container.Enqueue(Tuple.Create(currentName, lvl));


                    var currentChildren = child.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);

                    if (currentChildren.Count > 0)
                    {
                        GetElementsFromTreeGrid(currentChildren, ref container, lvl + 1);
                    }
                }
            }
        }
        void getVariablesFromQueue(ref Queue<Tuple<string, int>> container, ref ObservableCollection<Variable> variables, int minLvl = 1, string prevName = "", string typeOfVariableFromPrevDepth = "", int helper = 0)
        {

            int count = container.Count; // число в очереди
            string prevValidType = new string(typeOfVariableFromPrevDepth.ToCharArray());

            for (int i = 0; i < count; ++i)
            {
                if (container.Count == 0) // если число переменных в очереде == 0, то return
                    return;
                Tuple<string, int> currentVariable = container.Peek(); // смотрим первый элемент на выход из очереди

                string currentNameOfVariable = currentVariable.Item1; // название переменной
                int currentLvlOfVariable = currentVariable.Item2; // глубина переменной

                if (currentLvlOfVariable == minLvl) // если глубина переменной совпадает с минимальным, то есть мы не опустилиь и не поднялись, то
                {

                    string generalName = prevName;

                    if (!currentNameOfVariable.Contains("["))
                    {
                        if (typeOfVariableFromPrevDepth != "")
                        {
                            if (typeOfVariableFromPrevDepth[typeOfVariableFromPrevDepth.Length - 1] != '*')
                                generalName += ".";
                            else
                                generalName += "->";
                        }
                    }

                    Variable variable = GetElementFromExpression(generalName + currentNameOfVariable, "WatchWindow", new Utils.Color(0, 255, 0), false); // получаем переменную

                    bool isFind = false;

                    if (variable != null)
                    {
                        foreach (var tempVariable in variables)
                        {
                            if (tempVariable.m_S_Name == variable.m_S_Name && tempVariable.m_S_Type == variable.m_S_Type &&
                                tempVariable.m_S_Addres == variable.m_S_Addres && tempVariable.m_S_Source == variable.m_S_Source)
                            {
                                MessageBox.Show("Error: the variable with name \"" + variable.m_S_Name + "\" contains in table.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                isFind = true;
                            }
                        }

                        if (!isFind)
                        {
                            variables.Add(variable);
                            prevValidType = variable.m_S_Type;
                        }
                    }

                    container.Dequeue(); // убираем этот элемент из очереди
                }
                else if (currentLvlOfVariable > minLvl) // если мы опустились глубже
                {
                    string nameOnNextLvl = "(" + variables[variables.Count - 1].m_S_Name + ")";

                    getVariablesFromQueue(ref container, ref variables, currentLvlOfVariable, nameOnNextLvl, prevValidType, helper);
                }
                else
                {
                    return;
                }
            }
        }

        //
        //////////
        ////////////////////////////////////////////////////////////
        //////////

    }
}
