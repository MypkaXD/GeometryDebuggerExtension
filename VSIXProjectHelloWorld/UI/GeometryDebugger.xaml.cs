using EnvDTE;
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

        private DebuggerGetterVariables m_DGV_debugger;
        private AddMenu addMenu;
        private System.Windows.Window addWindow;
        private EnvDTE.DebuggerEvents m_DE_events;

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
        public GeometryDebugger()
        {
            InitializeComponent();

            m_DGV_debugger = new DebuggerGetterVariables();
            
            addMenu = new AddMenu(m_DGV_debugger);

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
            addWindow.Loaded += OnWindowLoaded;

            if (m_DGV_debugger.GetDTE() != null)
            {
                m_DE_events = m_DGV_debugger.GetDTE().Events.DebuggerEvents;
                m_DE_events.OnEnterBreakMode += OnEnterBreakMode; // subscribe on Enter Break mode or press f10 or press f5
            }
        }
        private void btnOpenAddMenu_Click(object sender, RoutedEventArgs e)
        {
            if (m_DGV_debugger.IsDebugMode())
            {
                if (addWindow == null || addWindow.IsVisible == false)
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
                    addWindow.Loaded += OnWindowLoaded;
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

            dgObjects.ItemsSource = addMenu.GetVariables();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
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
    }
}
