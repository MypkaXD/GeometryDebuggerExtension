using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GeometryDebugger.Utils;

namespace GeometryDebugger.UI
{
    /// <summary>
    /// Interaction logic for AddMenu.xaml
    /// </summary>
    public partial class AddMenu : System.Windows.Controls.UserControl
    {

        private ObservableCollection<Variable> m_OBOV_variablseFromCurrentStackFrame = new ObservableCollection<Variable>(); // переменные из CF
        private ObservableCollection<Variable> m_OBOV_variablesFromWathList = new ObservableCollection<Variable>(); // переменные из WL
        private ObservableCollection<Variable> m_OBOV_variablseFromMyselfAdded = new ObservableCollection<Variable>(); // переменные MyS

        public ObservableCollection<Variable> m_OBOV_Variables = new ObservableCollection<Variable>(); // все переменные

        private DebuggerGetterVariables m_DGV_debugger; // объект для получения переменных из CF, WL и MyS

        public AddMenu(DebuggerGetterVariables debugger)
        {
            InitializeComponent(); // инициализируем компоненты

            m_DGV_debugger = debugger; // сохраняем объект для получения переменных из CF, WL и MyS
        }

        private void UpdateData()
        {

            m_OBOV_Variables = new ObservableCollection<Variable>(
                m_OBOV_variablesFromWathList
                .Union(m_OBOV_variablseFromCurrentStackFrame)
                .Union(m_OBOV_variablseFromMyselfAdded));

            dgAddVariables.ItemsSource = m_OBOV_Variables;
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
            Variable variable = m_DGV_debugger.GetElementFromExpression(MySelfAddedVariables.Text, "AddedMySelf", new Utils.Color(0, 0, 255), true);

            if (variable != null)
            {
                foreach (var currentVariable in m_OBOV_variablseFromMyselfAdded)
                {
                    if (currentVariable.m_S_Name == variable.m_S_Name && currentVariable.m_S_Type == variable.m_S_Type &&
                        currentVariable.m_S_Addres == variable.m_S_Addres && currentVariable.m_S_Source == variable.m_S_Source)
                    {
                        MessageBox.Show("Error: A variable with name: " + variable.m_S_Name + " contains in table.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                m_OBOV_variablseFromMyselfAdded.Add(variable);
            }
            else
            {
                MessageBox.Show("Error: A variable with name: " + MySelfAddedVariables.Text + " isn't find.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UpdateData();
        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window.GetWindow(this).Close();
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
        //////////
        ////////////////////////////////////////////////////////////
        //////////
        // method for breakMod (f5, f10, f11)
        private void UpdateDataFromCurrentStackFrame() // обновляем переменные из CurrentStackFrame
        {
            if (this.CF.IsChecked == true) // если стоит флаг на доблавение переменных из CF
            {
                m_DGV_debugger.GetVariablesFromCurrentStackFrame(ref m_OBOV_variablseFromCurrentStackFrame);
            }
            else
            {
                m_OBOV_variablseFromCurrentStackFrame = new ObservableCollection<Variable>(); // иначе просто обнуляем
            }
        }
        private void UpdateDataFromWatchList() // обновляем переменные из WatchList
        {
            if (this.WL.IsChecked == true) // если стоит флаг на доблавение переменных из WL
            {
                m_DGV_debugger.GetVariablesFromWatchList(ref m_OBOV_variablesFromWathList);
            }
            else // иначе просто обнуляем
            {
                m_OBOV_variablesFromWathList = new ObservableCollection<Variable>();
            }
        }
        private void UpdateDataFromMySelf(bool isShowNotification = true)
        {
            ObservableCollection<Variable> variables = new ObservableCollection<Variable>(); // создаем новую коллекцию

            foreach (var variable in m_OBOV_variablseFromMyselfAdded) // проходимся по переменным, которые уже есть и проверяем их на валидность
            {
                Variable currentVariable = m_DGV_debugger.GetElementFromExpression(variable.m_S_Name, "AddedMySelf", new Utils.Color(0, 0, 255), true);

                if (currentVariable != null)
                {
                    currentVariable.m_B_IsAdded = variable.m_B_IsAdded;
                    currentVariable.m_B_IsSelected = variable.m_B_IsSelected;
                    currentVariable.m_C_Color = variable.m_C_Color;

                    if (m_OBOV_variablseFromMyselfAdded.Contains(currentVariable))
                    {
                        if (isShowNotification)
                            MessageBox.Show("Error: A variable with name: " + currentVariable.m_S_Name + " contains in table.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                    else
                        variables.Add(currentVariable);
                }
                else
                    continue;
            }

            m_OBOV_variablseFromMyselfAdded = new ObservableCollection<Variable>(variables); // обновляем данные
        }
        public void BreakModDetected()
        {
            ObservableCollection<Variable> variables = new ObservableCollection<Variable>(m_OBOV_Variables); // новая коллекция из старой коллекции (сохранили её грубо говоря), ведь GetVariables вернул только те переменные, которые isSelected

            UpdateDataFromCurrentStackFrame(); // Сохраняем в m_OBOV_variablseFromCurrentStackFrame новые переменные (их получают заново)
            UpdateDataFromWatchList(); // ..// m_OBOV_variablesFromWathList
            UpdateDataFromMySelf(); // обновляем данные о переменных, которые добавил пользователь руками

            m_OBOV_Variables = new ObservableCollection<Variable>(
                m_OBOV_variablesFromWathList
                .Union(m_OBOV_variablseFromCurrentStackFrame)
                .Union(m_OBOV_variablseFromMyselfAdded)); // объединяем их

            foreach (var variable in variables) // проходим по старым переменным, чтобы найти те, которые еще остались живы и синхронизировать их с новыми (цвет, isSelected, isAdded)
            {
                // число элементов в m_OBOV_Variables <= чем в variables
                foreach (var variableInTable in m_OBOV_Variables) // проходим каждый раз по новым переменным
                {
                    if (variableInTable.m_S_Name == variable.m_S_Name &&
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
        }
        public ObservableCollection<Variable> UpdateVariableAfterBreakMod(ObservableCollection<Variable> variables)
        {

            ObservableCollection<Variable> newVariables = new ObservableCollection<Variable>();

            foreach (var variable in variables)
            {
                Variable currentVariable = m_DGV_debugger.GetElementFromExpression(variable.m_S_Name, variable.m_S_Source, variable.m_C_Color, variable.m_B_IsAdded);

                if (currentVariable != null && variable.m_S_Type == currentVariable.m_S_Type) // проверка, чтобы не добавить ненужные переменные (если у них одинаковые названия, но разные типы)
                    // такого быть не может. Но вот разные адреса могут быть.
                {
                    currentVariable.m_B_IsAdded = variable.m_B_IsAdded;
                    currentVariable.m_B_IsSelected = variable.m_B_IsSelected;
                    currentVariable.m_B_IsSerialized = false;
                    currentVariable.m_C_Color = variable.m_C_Color;
                    currentVariable.m_i_NumberOfChilds = variable.m_i_NumberOfChilds;
                    currentVariable.m_OC_Childrens = variable.m_OC_Childrens;
                    newVariables.Add(currentVariable);
                }
                //else
                //    MessageBox.Show($"ERROR: Can't get variable {variable.m_S_Name}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return newVariables;
        }
        //
        //////////
        ////////////////////////////////////////////////////////////
        //////////
        //
        public ObservableCollection<Variable> GetVariables() // получаем переменные
        {
            SetOnlyAddedVariable(); // изменяем коллекцию только на те, которые isAdded
            return m_OBOV_Variables; // получаем все переменные
        }
        //
        //////////
        ////////////////////////////////////////////////////////////
        //////////
        //
        private void SetOnlyAddedVariable()
        {
            m_OBOV_Variables = new ObservableCollection<Variable>(); // очищаем текущие переменные

            // объединяем переменные
            ObservableCollection<Variable> temp = new ObservableCollection<Variable>(
                m_OBOV_variablesFromWathList
                .Union(m_OBOV_variablseFromCurrentStackFrame)
                .Union(m_OBOV_variablseFromMyselfAdded));

            foreach (var variable in temp)
            {
                if (variable.m_B_IsAdded)
                    m_OBOV_Variables.Add(variable); // сохраняем только те переменные, которые isAdded
            }
        }
        //
        //////////
        ////////////////////////////////////////////////////////////
    }
}
