using EnvDTE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VSIXProjectHelloWorld.Utils;

namespace VSIXProjectHelloWorld
{
    /// <summary>
    /// Interaction logic for AddMenu.xaml
    /// </summary>
    public partial class AddMenu : UserControl
    {

        private ObservableCollection<Variable> m_OBOV_variablseFromCurrentStackFrame = new ObservableCollection<Variable>();
        private ObservableCollection<Variable> m_OBOV_variablesFromWathList = new ObservableCollection<Variable>();
        private ObservableCollection<Variable> m_OBOV_variablseFromMyselfAdded = new ObservableCollection<Variable>();

        private ObservableCollection<Variable> m_OBOV_Variables = new ObservableCollection<Variable>();

        private DebuggerGetterVariables m_DGV_debugger;
        private EnvDTE.DebuggerEvents m_DE_events;

        public AddMenu(DebuggerGetterVariables debugger)
        {
            InitializeComponent();

            m_DGV_debugger = debugger;

            if (m_DGV_debugger.GetDTE() != null)
            {
                m_DE_events = m_DGV_debugger.GetDTE().Events.DebuggerEvents;
                m_DE_events.OnEnterBreakMode += OnEnterBreakMode; // subscribe on Enter Break mode or press f10 or press f5
            }
        }

        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction action)
        {
            UpdateDataFromCurrentStackFrame();
            UpdateDataFromWatchList();

            UpdateData();
        }

        private void UpdateData()
        {

            m_OBOV_Variables = new ObservableCollection<Variable>(
                m_OBOV_variablesFromWathList
                .Union(m_OBOV_variablseFromCurrentStackFrame)
                .Union(m_OBOV_variablseFromMyselfAdded));

            dgAddVariables.ItemsSource = m_OBOV_Variables;
        }

        private void UpdateDataFromCurrentStackFrame()
        {
            if (this.CF.IsChecked == true)
            {
                m_DGV_debugger.GetVariablesFromCurrentStackFrame(ref m_OBOV_variablseFromCurrentStackFrame);
            }
            else
            {
                m_OBOV_variablseFromCurrentStackFrame = new ObservableCollection<Variable>();
            }
        }
        private void UpdateDataFromWatchList()
        {
            if (this.WL.IsChecked == true)
            {
                m_DGV_debugger.GetVariablesFromWatchList(ref m_OBOV_variablesFromWathList);
            }
            else
            {
                m_OBOV_variablesFromWathList = new ObservableCollection<Variable>();
            }
        }

        private void ButtonCurrentStackFrame_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataFromCurrentStackFrame();
            UpdateData();
        }
        private void ButtonWatchList_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataFromWatchList();
            UpdateData();
        }
        private void ButtonMyselfAdded_Click(object sender, RoutedEventArgs e)
        {
            Variable variable = m_DGV_debugger.GetElemetFromExpression(this.TextForExpression.Text);

            if (variable != null)
                m_OBOV_variablseFromMyselfAdded.Add(variable);
            else
                return;

            UpdateData();
        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            m_OBOV_Variables = new ObservableCollection<Variable>();

            foreach (var variable in m_OBOV_variablesFromWathList
                .Union(m_OBOV_variablseFromCurrentStackFrame)
                .Union(m_OBOV_variablseFromMyselfAdded))
            {
                if (variable.m_B_IsAdded)
                {
                    m_OBOV_Variables.Add(variable);
                }
            }
            System.Windows.Window.GetWindow(this).Close();
        }
        public ObservableCollection<Variable> GetVariables()
        {
            return m_OBOV_Variables;
        }
    }
}
