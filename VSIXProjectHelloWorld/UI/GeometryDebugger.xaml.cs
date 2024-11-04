using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;
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

        private ICommand DrawVariable { get; set; }

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

            this.DrawVariable = new DelegateCommand((o) =>
            {
                foreach (var item in dgObjects.SelectedItems)
                {
                    if (item is Variable variable)
                    {
                        variable.m_B_IsSelected = true;
                    }
                }
                dgObjects.ItemsSource = m_OBOV_Variables;
            });
        }
        private void btnOpenAddMenu_Click(object sender, RoutedEventArgs e)
        {
            if (m_DGV_debugger.IsDebugMode())
            {
                if (addWindow == null || addWindow.IsVisible == false)
                {
                    InitAddWindowComponent();
                }
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
            int processID = 0;
            EnvDTE.Processes processes = m_DGV_debugger.GetDTE().Debugger.DebuggedProcesses;
            foreach (EnvDTE.Process proc in processes)
                processID = proc.ProcessID;

            //узнать адрес функции
            var expr = m_DGV_debugger.GetDTE().Debugger.GetExpression("&Serialize");
            IntPtr intPtr = (IntPtr)(ulong)new System.ComponentModel.UInt64Converter().ConvertFromString(expr.Value.Split(' ').First());

            SharedMemory sharedMemory = new SharedMemory(m_OBOV_Variables, intPtr, processID);


        }
        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction action)
        {
            addMenu.BreakModDetected();

            ObservableCollection<Variable> TempVariables = new ObservableCollection<Variable>();

            foreach (var variable in m_OBOV_Variables)
            {
                if (m_DGV_debugger.GetDTE().Debugger.GetExpression(variable.m_S_Name).IsValidValue)
                    TempVariables.Add(variable);
                else
                    continue;
            }

            m_OBOV_Variables = TempVariables;

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
    }
}
