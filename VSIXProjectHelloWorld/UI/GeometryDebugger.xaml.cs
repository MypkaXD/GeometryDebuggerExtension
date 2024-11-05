using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json.Linq;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VSIXProjectHelloWorld
{
    /// <summary>
    /// Interaction logic for UserControl.xaml
    /// </summary>
    public partial class GeometryDebugger : System.Windows.Controls.UserControl
    {
        private ObservableCollection<Variable> m_OBOV_Variables;
        private string message { get; set; }
        private bool isSet = false;

        private DebuggerGetterVariables m_DGV_debugger;
        private AddMenu addMenu;
        private System.Windows.Window addWindow;
        private EnvDTE.DebuggerEvents m_DE_events;

        private void InitDebuggerComponent()
        {
            m_DGV_debugger = new DebuggerGetterVariables();

            if (m_DGV_debugger.GetDTE() != null)
            {
                m_DE_events = m_DGV_debugger.GetDTE().Events.DebuggerEvents;
                m_DE_events.OnEnterBreakMode += OnEnterBreakMode; // subscribe on Enter Break mode or press f10 or press f5
            }
        }
        private void InitAddWindowComponent()
        {
            addWindow = new System.Windows.Window
            {
                Title = "Add Variable",
                Content = addMenu,
                Height = 800,
                Width = 600,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            addWindow.Closing += OnWindowClosing;
        }

        public GeometryDebugger()
        {
            InitializeComponent();
            InitDebuggerComponent();

            addMenu = new AddMenu(m_DGV_debugger);

            InitAddWindowComponent();
        }
        private void btnOpenAddMenu_Click(object sender, RoutedEventArgs e)
        {
            if (m_DGV_debugger.IsDebugMode())
            {
                if (addWindow == null || addWindow.IsVisible == false)
                {
                    InitAddWindowComponent();
                }
                addMenu.BreakModDetected();
                m_OBOV_Variables = addMenu.GetVariables();
                dgObjects.ItemsSource = m_OBOV_Variables;
                addWindow.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("ERROR: You need to start Debug mode.");
            }
        }
        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            m_OBOV_Variables = addMenu.GetVariables();

            dgObjects.ItemsSource = m_OBOV_Variables;
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
                    Title = "Pick a Color",
                    Content = colorPicker,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                // Set the current color values
                colorPicker.RedSlider.Value = variable.m_C_Color.m_i_R;
                colorPicker.GreenSlider.Value = variable.m_C_Color.m_i_G;
                colorPicker.BlueSlider.Value = variable.m_C_Color.m_i_B;

                pickerWindow.ShowDialog();

                // Update Variable's color
                variable.m_C_Color = new Utils.Color(
                    (int)colorPicker.RedSlider.Value,
                    (int)colorPicker.GreenSlider.Value,
                    (int)colorPicker.BlueSlider.Value
                );

                // Update the button background
                button.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
                    (byte)variable.m_C_Color.m_i_R,
                    (byte)variable.m_C_Color.m_i_G,
                    (byte)variable.m_C_Color.m_i_B));
            }
        }

        private void GeometryDebuggerLoaded(object sender, RoutedEventArgs e)
        {
            // Создание нашего OpenGL Hwnd 'контроля'...
            ControlHost host = new ControlHost(450, 800);

            // ... и присоединяем его к контейнеру:
            if (ControlHostElement.Child == null)
                ControlHostElement.Child = host;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SharedMemory sharedMemory = new SharedMemory(m_OBOV_Variables, m_DGV_debugger.GetDTE());
        }
        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction action)
        {
            addMenu.BreakModDetected();
            m_OBOV_Variables = addMenu.GetVariables();
            dgObjects.ItemsSource = m_OBOV_Variables;
        }

        private void MenuItemAddForIsntDrawing_Click(object sender, RoutedEventArgs e)
        {
            if (dgObjects.SelectedItems.Count == 0)
                return;

            foreach (var item in dgObjects.SelectedItems)
            {
                if (item is Variable dataItem)
                {
                    if (dataItem.m_B_IsSelected)
                    {
                        dataItem.m_B_IsSelected = false;
                    }
                }
            }

            dgObjects.Items.Refresh();
        }

        private void MenuItemAddForDrawing_Click(object sender, RoutedEventArgs e)
        {
            if (dgObjects.SelectedItems.Count == 0)
                return;
            
            foreach (var item in dgObjects.SelectedItems)
            {
                if (item is Variable dataItem)
                {
                    if (!dataItem.m_B_IsSelected)
                    {
                        dataItem.m_B_IsSelected = true;
                    }
                }
            }

            dgObjects.Items.Refresh();
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgObjects.SelectedItems.Count == 0)
                return;

            foreach (var item in dgObjects.SelectedItems)
            {
                if (item is Variable dataItem)
                {
                    dataItem.m_B_IsAdded = false;
                    dataItem.m_B_IsSelected = false;
                }
            }

            
            ObservableCollection<Variable> variables = new ObservableCollection<Variable>();

            foreach (var variable in m_OBOV_Variables)
            {
                if (variable.m_B_IsAdded != false)
                    variables.Add(variable);
            }

            m_OBOV_Variables = new ObservableCollection<Variable>(variables);

            dgObjects.ItemsSource = m_OBOV_Variables;
        }
    }
}
