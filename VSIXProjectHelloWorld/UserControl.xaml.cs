using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VSIXProjectHelloWorld
{
    /// <summary>
    /// Interaction logic for UserControl.xaml
    /// </summary>
    public partial class UserControl1 : System.Windows.Controls.UserControl
    {
        public ObservableCollection<MyObject> m_OBC_Objects;
        public DTE m_DTE_Dte;
        public EnvDTE.DebuggerEvents m_DE_events;

        public string txtbEnterExpressionText = "";
        public string message { get; set; }
        public bool isSet = false;

        double alpha = 0.0d;
        int[] maj;
        int[] mino;

        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\GLutility.dll")] public static extern int GetGLMinor();
        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\GLutility.dll")] public static extern int GetGLMajor();
        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\GLutility.dll")] public static extern int Get3();
        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\GLutility.dll")] public static extern void GetGLver();
        [DllImport("C:\\Users\\MypkaXD\\source\\repos\\LearningWPF\\x64\\Debug\\GLutility.dll")] public static extern void drawTriangles();

        public UserControl1()
        {
            InitializeComponent();
            openGLControl.MouseLeftButtonUp += new MouseButtonEventHandler(_myCustomUserControl_MouseLeftButtonUp);

            initComponentOfDTE();
            UpdateAllData();

            maj = new int[1];
            mino = new int[1];
            maj[0] = -1;
            mino[0] = -1;
            int a = 4;
        }

        void _myCustomUserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            alpha += 0.3d;

            OpenGL gl = openGLControl.OpenGL;

            int major = GetGLMajor();
            int minor = Get3();// GLMinor();
            GetGLver();
            textBox.Content = "alpha =" + alpha.ToString() + " GL: " + major.ToString() + ":" + minor.ToString();
            e.Handled = true;
        }

        private void initComponentOfDTE()
        {
            m_OBC_Objects = new ObservableCollection<MyObject>();
            m_DTE_Dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SDTE)) as EnvDTE.DTE;

            if (m_DTE_Dte != null)
            {
                m_DE_events = m_DTE_Dte.Events.DebuggerEvents;
                m_DE_events.OnEnterBreakMode += OnEnterBreakMode; // subscribe on Enter Break mode or press f10 or press f5
            }
        }
        private void UpdateAllData()
        {
            m_OBC_Objects = new ObservableCollection<MyObject>();

            UpdateDataFromLocalFrame();
            UpdateDataFromWatchList();

            dgObjects.ItemsSource = m_OBC_Objects; // set our data for show on ui in DataGrid
        }
        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction action)
        {
            UpdateAllData();
        }
        private void UpdateDataFromLocalFrame()
        {
            EnvDTE.StackFrame currentFrame = m_DTE_Dte.Debugger.CurrentStackFrame;

            foreach (EnvDTE.Expression localVariable in currentFrame.Locals)
            {

                EnvDTE.Expression expression = m_DTE_Dte.Debugger.GetExpression("&(" + localVariable.Name + ")", true, 1); // get addres of variable

                if (expression.IsValidValue)
                {
                    MyObject myObject = new MyObject()
                    {
                        m_B_IsSelected = false,
                        m_S_Name = localVariable.Name,
                        m_S_Type = localVariable.Type,
                        m_S_Addres = expression.Value,
                        m_S_Source = "LocalFrame"
                    };

                    m_OBC_Objects.Add(myObject);
                }
                else
                    System.Diagnostics.Debug.WriteLine($"ERROR: Can't get addres for value {localVariable.Name}");

            }
        }

        private void UpdateDataFromWatchList()
        {
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

            AutomationElement mainWindow = AutomationElement.FromHandle(customToolWindow.HWnd);
            PrintAutomationElement(mainWindow);
        }

        void GetElementsFromTreeGreed(AutomationElement element)
        {
            string name = element.Current.Name;

            if (element.Current.ClassName == "TreeGridItem")
            {
                var expressionForTypeAndName = m_DTE_Dte.Debugger.GetExpression(name, true, 1);
                var expressionForAddress = m_DTE_Dte.Debugger.GetExpression("&(" + name + ")", true, 1);

                if (expressionForTypeAndName.IsValidValue && expressionForAddress.IsValidValue)
                {
                    MyObject myObject = new MyObject()
                    {
                        m_B_IsSelected = false,
                        m_S_Name = expressionForTypeAndName.Name,
                        m_S_Type = expressionForTypeAndName.Type,
                        m_S_Addres = expressionForAddress.Value,
                        m_S_Source = "WatchWindow"
                    };
                    m_OBC_Objects.Add(myObject);
                }
            }
            else
                return;
        }

        void PrintAutomationElement(AutomationElement element)
        {
            if (element.Current.ClassName == "TreeGrid")
            {
                var children = element.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
                foreach (AutomationElement child in children)
                    if (child.Current.ClassName == "TreeGridItem")
                        GetElementsFromTreeGreed(child);
            }
            else
            {
                var children = element.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
                foreach (AutomationElement child in children)
                    PrintAutomationElement(child); // Рекурсивный вызов для дочерних элементов
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            txtbEnterExpressionText = txtbEnterExpression.Text;
        }

        private void OpenGLControl_OpenGLDraw(object sender, RoutedEventArgs args)
        {
            drawTriangles();
        }


        private void OpenGLControl_OpenGLInitialized(object sender, RoutedEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.ClearColor(0.3f, 0.3f, 0.3f, 0.3f);
        }

        private void OpenGLControl_Resized(object sender, RoutedEventArgs args)
        {
        }
    }
    public class MyObject : INotifyPropertyChanged
    {
        private string name;
        private bool isSelected;
        private Color color;
        private string type;
        private string addres;
        private string source;

        public string m_S_Source
        {
            get => this.source;
            set
            {
                if (this.source != value)
                {
                    this.source = value;
                    this.OnPropertyChanged(nameof(m_S_Source));
                }
            }
        }

        public string m_S_Type
        {
            get => this.type;
            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.OnPropertyChanged(nameof(m_S_Type));
                }
            }
        }

        public string m_S_Addres
        {
            get => this.addres;
            set
            {
                if (this.addres != value)
                {
                    this.addres = value;
                    this.OnPropertyChanged(nameof(m_S_Addres));
                }
            }
        }

        public string m_S_Name
        {
            get => this.name;
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged(nameof(m_S_Name));
                }
            }
        }

        public bool m_B_IsSelected
        {
            get => this.isSelected;
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(nameof(m_B_IsSelected));
                }
            }
        }

        public Color m_C_Color
        {
            get => this.color;
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.OnPropertyChanged(nameof(m_C_Color));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
