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

            Guid guidIAccessible = typeof(IAccessible).GUID;

            IAccessible window;

            if (AccessibleObjectFromWindow((IntPtr)customToolWindow.HWnd, OBJID_CLIENT, ref guidIAccessible, out window) == 0 && window != null)
            {
                GetChildrenFromIAccessible(window, ref variables);
            }

        }

        void GetElementsFromTreeGridAccessibility(IAccessible treeGrid, ref ObservableCollection<Variable> variables)
        {
            int childCount = treeGrid.accChildCount; // кол-во детей

            if (childCount > 0)
            {
                object[] childrens = new object[childCount];
                int reciveChilds = 0; // число полученных детей

                if (AccessibleChildren(treeGrid, 0, childCount, childrens, out reciveChilds) == 0)
                {
                    Queue<Tuple<string, int>> container = new Queue<Tuple<string, int>>();

                    for (int i = 0; i < reciveChilds; ++i)
                    {
                        if (childrens[i] is System.Int32)
                        {
                            int childID = (System.Int32)childrens[i];

                            try
                            {
                                if (treeGrid.get_accValue(childID).Contains("@ tree depth"))
                                {
                                    string nameOfVariable = treeGrid.get_accValue(childID).Split(' ')[0];
                                    int lvlOfVariable = System.Convert.ToInt32(treeGrid.get_accValue(childID).Split(' ')[4]);

                                    container.Enqueue(Tuple.Create(nameOfVariable, lvlOfVariable));
                                }
                            }
                            catch { }
                        }
                    }

                    getVariablesFromQueue(ref container, ref variables);

                }
            }
        }

        void getVariablesFromQueue(ref Queue<Tuple<string, int>> container, ref ObservableCollection<Variable> variables, int minLvl = 1, string currentName = "")
        {

            int count = container.Count;

            for (int i = 0; i < count; ++i)
            {
                if (container.Count == 0)
                    return;
                Tuple<string, int> currentVariable = container.Peek();

                string currentNameOfVariable = currentVariable.Item1;
                int currentLvlOfVariable = currentVariable.Item2;

                if (currentLvlOfVariable == minLvl)
                {
                    getVariableFromString(currentName + currentNameOfVariable, ref variables);
                    container.Dequeue();
                }
                else if (currentLvlOfVariable > minLvl)
                {
                    if (currentNameOfVariable.Contains("["))
                        getVariablesFromQueue(ref container, ref variables, currentLvlOfVariable, variables[variables.Count - 1].m_S_Name);
                    else
                        getVariablesFromQueue(ref container, ref variables, currentLvlOfVariable, variables[variables.Count - 1].m_S_Name + ".");

                }
                else
                {
                    return;
                }
            }
        }

        void getVariableFromString(string value, ref ObservableCollection<Variable> variables)
        {
            var expressionForTypeAndName = m_DTE_Dte.Debugger.GetExpression(value, true, 1);
            var expressionForAddress = m_DTE_Dte.Debugger.GetExpression("&(" + value + ")", true, 1);

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

                variables.Add(variable);
            }
        }

        bool GetChildrenFromIAccessible(IAccessible accessible, ref ObservableCollection<Variable> variables)
        {
            try
            {
                string name = accessible.get_accName(0);

                if (name == "Treegrid Accessibility")
                {
                    GetElementsFromTreeGridAccessibility(accessible, ref variables);
                    return true;
                }
            }
            catch { }


            int childCount = accessible.accChildCount; // кол-во детей

            if (childCount > 0)
            {
                object[] childrens = new object[childCount];
                int reciveChilds = 0; // число полученных детей

                if (AccessibleChildren(accessible, 0, childCount, childrens, out reciveChilds) == 0)
                {
                    bool isFind = false;

                    for (int i = 0; i < reciveChilds; ++i)
                    {
                        if (childrens[i] is IAccessible)
                        {
                            IAccessible child = childrens[i] as IAccessible;

                            isFind = GetChildrenFromIAccessible(child, ref variables);
                        }

                        if (isFind)
                            return true;
                    }
                }
            }

            return false;
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
