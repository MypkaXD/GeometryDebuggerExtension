using EnvDTE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GeometryDebugger.Utils;

namespace GeometryDebugger.UI
{
    /// <summary>
    /// Interaction logic for AddMenu.xaml
    /// </summary>
    public partial class AddMenu : System.Windows.Controls.UserControl
    {

        private ObservableCollection<Variable> m_OBOV_variablseFromCurrentStackFrame = new ObservableCollection<Variable>();
        private ObservableCollection<Variable> m_OBOV_variablesFromWathList = new ObservableCollection<Variable>();
        private ObservableCollection<Variable> m_OBOV_variablseFromMyselfAdded = new ObservableCollection<Variable>();

        public ObservableCollection<Variable> m_OBOV_Variables = new ObservableCollection<Variable>();

        private DebuggerGetterVariables m_DGV_debugger;

        public AddMenu(DebuggerGetterVariables debugger)
        {
            InitializeComponent();

            m_DGV_debugger = debugger;
        }

        public void BreakModDetected()
        {
            ObservableCollection<Variable> variables = new ObservableCollection<Variable>(m_OBOV_Variables); // новая коллекция из старой коллекции (сохранили её грубо говоря)

            UpdateDataFromCurrentStackFrame(); // Сохраняем в m_OBOV_variablseFromCurrentStackFrame новые переменные (их получают заново)
            UpdateDataFromWatchList(); // ..// m_OBOV_variablesFromWathList
            UpdateDataFromMySelf(); // обновляем данные о переменных, которые добавил пользователь руками

            m_OBOV_Variables = new ObservableCollection<Variable>(
                m_OBOV_variablesFromWathList
                .Union(m_OBOV_variablseFromCurrentStackFrame)
                .Union(m_OBOV_variablseFromMyselfAdded)); // объединяем их

            foreach (var variable in variables) // проходим по старым переменным, чтобы найти те, которые еще остались живы и синхронизировать их с новыми (цвет, isSelected, isAdded)
            {
                foreach (var variableInTable in m_OBOV_Variables) // проходим каждый раз по новым переменным
                {
                    if (variableInTable.m_S_Addres == variable.m_S_Addres &&
                        variableInTable.m_S_Name == variable.m_S_Name &&
                        variableInTable.m_S_Type == variable.m_S_Type &&
                        variableInTable.m_S_Source == variable.m_S_Source)
                    {
                        variableInTable.m_C_Color = variable.m_C_Color;
                        variableInTable.m_B_IsAdded = variable.m_B_IsAdded;
                        variableInTable.m_B_IsSelected = variable.m_B_IsSelected;
                        break;
                    }
                }
            }

            dgAddVariables.ItemsSource = m_OBOV_Variables; // обновляем визуальную состовляющую таблицы
            //SetOnlyAddedVariable();

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
        private void UpdateDataFromMySelf()
        {
            ObservableCollection<Variable> variables = new ObservableCollection<Variable>(); // создаем новую коллекцию

            foreach (var variable in m_OBOV_variablseFromMyselfAdded) // проходимся по переменным, которые уже есть и проверяем их на валидность
            {
                Variable currentVariable = m_DGV_debugger.GetElemetFromExpression(variable.m_S_Name);

                if (currentVariable != null)
                {
                    currentVariable.m_B_IsAdded = variable.m_B_IsAdded;
                    currentVariable.m_B_IsSelected = variable.m_B_IsSelected;
                    currentVariable.m_C_Color = variable.m_C_Color;

                    variables.Add(currentVariable);
                }
                else
                    continue;
            }

            m_OBOV_variablseFromMyselfAdded = new ObservableCollection<Variable>(variables); // обновляем данные
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
            Variable variable = m_DGV_debugger.GetElemetFromExpression(MySelfAddedVariables.Text);

            ObservableCollection<Variable> variables = new ObservableCollection<Variable>(
                m_OBOV_variablesFromWathList
                .Union(m_OBOV_variablseFromCurrentStackFrame)
                .Union(m_OBOV_variablseFromMyselfAdded));

            if (variable != null)
            {
                if (!isContainVariable(variable, variables))
                    m_OBOV_variablseFromMyselfAdded.Add(variable);
                else
                    System.Windows.MessageBox.Show("ERROR: A variable with this address: " + variable.m_S_Addres + " is already in the table.\nIt will not be added to it.");
            }
            else
            {
                System.Windows.MessageBox.Show("ERROR: A variable with name: " + MySelfAddedVariables.Text + " isn't find.");
                return;
            }


            UpdateData();
        }

        private bool isContainVariable(Variable variable, ObservableCollection<Variable> variables)
        {
            foreach (var currentVariable in variables)
            {
                if (currentVariable.m_S_Addres == variable.m_S_Addres)
                    return true;
            }
            return false;
        }

        private void SetOnlyAddedVariable()
        {
            m_OBOV_Variables = new ObservableCollection<Variable>();

            ObservableCollection<Variable> temp = new ObservableCollection<Variable>(
                m_OBOV_variablesFromWathList
                .Union(m_OBOV_variablseFromCurrentStackFrame)
                .Union(m_OBOV_variablseFromMyselfAdded));

            foreach (var variable in temp)
            {
                if (variable.m_B_IsAdded && !isContainVariable(variable, m_OBOV_Variables))
                {
                    m_OBOV_Variables.Add(variable);
                }
            }
        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window.GetWindow(this).Close();
        }
        public ObservableCollection<Variable> GetVariables()
        {
            SetOnlyAddedVariable(); // изменяем коллекцию только на те, которые isAdded
            return m_OBOV_Variables;
        }

        private void Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ColorDisplay_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            Variable variable = button.DataContext as Variable;

            if (button != null && variable != null)
            {
                ColorPicker colorPicker = new ColorPicker();
                System.Windows.Window pickerWindow = new System.Windows.Window
                {
                    Title = "ColorPicker",
                    Content = colorPicker,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                colorPicker.RedValue.Text = ((byte)variable.m_C_Color.m_i_R).ToString();
                colorPicker.GreenValue.Text = ((byte)variable.m_C_Color.m_i_G).ToString();
                colorPicker.BlueValue.Text = ((byte)variable.m_C_Color.m_i_B).ToString();

                pickerWindow.ShowDialog();

                // Update Variable's color
                variable.m_C_Color = new Utils.Color(
                    (int)colorPicker.m_RGB.m_Byte_R,
                    (int)colorPicker.m_RGB.m_Byte_G,
                    (int)colorPicker.m_RGB.m_Byte_B
                );

                // Update the button background
                button.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
                    (byte)variable.m_C_Color.m_i_R,
                    (byte)variable.m_C_Color.m_i_G,
                    (byte)variable.m_C_Color.m_i_B));
            }
        }


        private void MenuItemAddForIsntImport_Click(object sender, RoutedEventArgs e)
        {
            if (dgAddVariables.SelectedItems.Count == 0)
                return;

            foreach (var item in dgAddVariables.SelectedItems)
            {
                if (item is Variable)
                {
                    Variable variable = (Variable)item;

                    if (variable.m_B_IsAdded)
                        variable.m_B_IsAdded = false;
                }
            }

            dgAddVariables.Items.Refresh();
        }

        private void MenuItemAddForImport_Click(object sender, RoutedEventArgs e)
        {
            if (dgAddVariables.SelectedItems.Count == 0)
                return;

            foreach (var item in dgAddVariables.SelectedItems)
            {
                if (item is Variable)
                {
                    Variable variable = (Variable)item;

                    if (!variable.m_B_IsAdded)
                        variable.m_B_IsAdded = true;
                }
            }

            dgAddVariables.Items.Refresh();
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgAddVariables.SelectedItems.Count == 0)
                return;

            foreach (var item in dgAddVariables.SelectedItems)
            {
                if (item is Variable)
                {
                    Variable variable = (Variable)item;

                    switch (variable.m_S_Source)
                    {
                        case "LocalStackFrame":
                            {
                                m_OBOV_variablseFromCurrentStackFrame.Remove(variable);
                                break;
                            }
                        case "AddedMySelf":
                            {
                                m_OBOV_variablseFromMyselfAdded.Remove(variable);
                                break;
                            }
                        case "WatchWindow":
                            {
                                m_OBOV_variablesFromWathList.Remove(variable);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }

            UpdateData();
        }
    }
}
