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
using VSIXProjectHelloWorld.Utils;

namespace VSIXProjectHelloWorld
{
    /// <summary>
    /// Interaction logic for AddMenu.xaml
    /// </summary>
    public partial class AddMenu : System.Windows.Controls.UserControl
    {

        private ObservableCollection<Variable> m_OBOV_variablseFromCurrentStackFrame = new ObservableCollection<Variable>();
        private ObservableCollection<Variable> m_OBOV_variablesFromWathList = new ObservableCollection<Variable>();
        private ObservableCollection<Variable> m_OBOV_variablseFromMyselfAdded = new ObservableCollection<Variable>();

        private ObservableCollection<Variable> m_OBOV_Variables = new ObservableCollection<Variable>();

        private DebuggerGetterVariables m_DGV_debugger;

        public AddMenu(DebuggerGetterVariables debugger)
        {
            InitializeComponent();

            m_DGV_debugger = debugger;
        }

        public void BreakModDetected()
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
            Variable variable = m_DGV_debugger.GetElemetFromExpression(MySelfAddedVariables.Text);

            if (variable != null)
                m_OBOV_variablseFromMyselfAdded.Add(variable);
            else
                return;

            UpdateData();
        }

        private void SetOnlyAddedVariable()
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
        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            SetOnlyAddedVariable();
            System.Windows.Window.GetWindow(this).Close();
        }
        public ObservableCollection<Variable> GetVariables()
        {
            SetOnlyAddedVariable();
            return m_OBOV_Variables;
        }

        private void Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ColorDisplay_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;

            System.Windows.Media.Brush brush = button.Background;
            System.Windows.Media.Color color = (brush as SolidColorBrush).Color;


            ColorPicker colorPicker = new ColorPicker();
            // Create a new window to host the color picker
            System.Windows.Window pickerWindow = new System.Windows.Window
            {
                Title = "Pick a Color",
                Content = colorPicker,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            // Set the current color values
            colorPicker.RedSlider.Value = color.R;
            colorPicker.GreenSlider.Value = color.G;
            colorPicker.BlueSlider.Value = color.B;
            
            // Show the color picker window
            pickerWindow.ShowDialog();

            color.R = (byte)colorPicker.RedSlider.Value;
            color.G = (byte)colorPicker.GreenSlider.Value;
            color.B = (byte)colorPicker.BlueSlider.Value;

            button.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
        }

        private void MenuItemAddForIsntImport_Click(object sender, RoutedEventArgs e)
        {
            if (dgAddVariables.SelectedItems.Count == 0)
                return;

            foreach (var item in dgAddVariables.SelectedItems)
            {
                if (item is Variable dataItem)
                {
                    if (dataItem.m_B_IsAdded)
                    {
                        dataItem.m_B_IsAdded = false;
                    }
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
                if (item is Variable dataItem)
                {
                    if (!dataItem.m_B_IsAdded)
                    {
                        dataItem.m_B_IsAdded = true;
                    }
                }
            }

            dgAddVariables.Items.Refresh();
        }
    }
}
