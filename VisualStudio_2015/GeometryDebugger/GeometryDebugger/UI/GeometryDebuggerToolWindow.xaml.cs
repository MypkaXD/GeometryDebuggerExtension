using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GeometryDebugger.Utils;
using System.Collections.Specialized;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.PlatformUI;
using System.Windows.Media.Imaging;

namespace GeometryDebugger.UI
{
    public partial class GeometryDebuggerToolWindow : UserControl
    {
        private string m_S_PathForFile = "vis_dbg_";
        private string m_S_GlobalPath = "";
        private string m_S_Message { get; set; }

        private bool m_B_IsSet = false;
        private bool m_B_IsSubscribeOnBreakMod = false;
        private bool m_B_IsFirst = true;

        private DebuggerGetterVariables m_DGV_Debugger;
        private AddMenu m_AM_AddMenu;
        private System.Windows.Window m_W_AddWindow;
        private DebuggerEvents m_DE_DebuggerEvents;
        private ControlHost m_CH_Host;
        private Dictionary<string, Tuple<bool, bool>> m_L_Paths;

        private ResourceDictionary lightTheme = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/GeometryDebugger;component/Style/Light_Theme.xaml", UriKind.RelativeOrAbsolute)
        };

        //private ResourceDictionary darkTheme = new ResourceDictionary
        //{
        //    Source = new Uri("Dark_Theme.xaml", UriKind.RelativeOrAbsolute)
        //};

        private ObservableCollection<Variable> _m_OBOV_Variables;
        public ObservableCollection<Variable> m_OBOV_Variables
        {
            get
            {
                return _m_OBOV_Variables;

            }
            set
            {
                if (_m_OBOV_Variables != null)
                {
                    // Отписываемся от событий старой коллекции
                    _m_OBOV_Variables.CollectionChanged -= Variables_CollectionChanged;
                    foreach (var variable in _m_OBOV_Variables)
                        variable.PropertyChanged -= Variable_PropertyChanged;
                }

                // Устанавливаем новую коллекцию
                _m_OBOV_Variables = value;
                _m_OBOV_Variables.CollectionChanged += Variables_CollectionChanged;

                if (_m_OBOV_Variables != null)
                {
                    // Подписываемся на события новой коллекции
                    foreach (var variable in _m_OBOV_Variables)
                        variable.PropertyChanged += Variable_PropertyChanged;
                }
            }
        }

        public GeometryDebuggerToolWindow()
        {
            InitializeComponent();

            m_DGV_Debugger = new DebuggerGetterVariables();

            VSColorTheme.ThemeChanged += OnThemeChanged;
            Loaded += GeometryDebuggerToolWindowLoaded;
            Unloaded += GeometryDebuggerToolWindowUnloaded;

            m_AM_AddMenu = new AddMenu(m_DGV_Debugger);
            m_L_Paths = new Dictionary<string, Tuple<bool, bool>>();
            m_OBOV_Variables = new ObservableCollection<Variable>();

            InitAddWindowComponent();

            resetTheme();
        }

        private void InitAddWindowComponent()
        {
            m_W_AddWindow = new System.Windows.Window
            {
                Title = "Add Variable",
                Content = m_AM_AddMenu,
                Height = 800,
                Width = 600,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            m_W_AddWindow.Loaded += OnAddWindowLoaded;
            m_W_AddWindow.Closing += OnAddWindowClosing;
        }

        private void OnAddWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (lightTheme["CheckBoxStyle_Light"] is Style)
            {
                System.Diagnostics.Debug.WriteLine("CheckBoxStyle_Light найден в словаре.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("CheckBoxStyle_Light НЕ найден в словаре!");
            }


            if (m_AM_AddMenu.WL.Style == lightTheme["CheckBoxStyle_Light"])
            {
                System.Diagnostics.Debug.WriteLine("Стиль уже установлен.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Устанавливаем стиль...");
                m_AM_AddMenu.WL.Style = lightTheme["CheckBoxStyle_Light"] as Style;
            }


        }

        private void OnThemeChanged(ThemeChangedEventArgs e)
        {
            resetTheme();
        }

        public void resetTheme()
        {
            System.Drawing.Color newBackgroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundBrushKey);

            var menuItems = this.dgObjects.ContextMenu.Items.OfType<MenuItem>();

            foreach (var menuItem in menuItems)
            {
                string fileName = menuItem.Header.ToString();

                if (newBackgroundColor.R >= 155 || newBackgroundColor.G >= 155 || newBackgroundColor.G >= 155)
                    fileName += "_Light";
                else
                    fileName += "_Dark";

                var imageSource = new BitmapImage(new Uri("../Images/" + fileName + ".png", UriKind.Relative));

                var image = new Image
                {
                    Source = imageSource,
                    Width = 16,
                    Height = 16,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                menuItem.Icon = image;
            }

            var menuItemsAddMenu = this.m_AM_AddMenu.dgAddVariables.ContextMenu.Items.OfType<MenuItem>();

            foreach (var menuItem in menuItemsAddMenu)
            {
                string fileName = menuItem.Header.ToString();

                if (newBackgroundColor.R >= 155 || newBackgroundColor.G >= 155 || newBackgroundColor.B >= 155)
                    fileName += "_Light";
                else
                    fileName += "_Dark";

                var imageSource = new BitmapImage(new Uri("../Images/" + fileName + ".png", UriKind.Relative));

                var image = new Image
                {
                    Source = imageSource,
                    Width = 16,
                    Height = 16,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                menuItem.Icon = image;
            }

        }

        public void GeometryDebuggerToolWindowLoaded(object sender, RoutedEventArgs e)
        {
            VSColorTheme.ThemeChanged += OnThemeChanged;

            m_CH_Host = new ControlHost();
            if (ControlHostElement.Child == null)
                ControlHostElement.Child = m_CH_Host;

            if (!m_B_IsSubscribeOnBreakMod)
            {
                SubscribeOnDebugEvents();
                m_B_IsSubscribeOnBreakMod = true;
            }
        }

        private void ClearGeomViewWindow()
        {
            m_OBOV_Variables.Clear();
            dgObjects.ItemsSource = m_OBOV_Variables;

            m_CH_Host.reloadGeomView(new List<Tuple<string, bool>> { }, m_S_GlobalPath);
        }

        public void SubscribeOnDebugEvents()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (m_DGV_Debugger.GetDTE() == null)
                return;

            m_DE_DebuggerEvents = m_DGV_Debugger.GetDTE().Events.DebuggerEvents;
            m_DE_DebuggerEvents.OnEnterBreakMode += OnEnterBreakMode;
        }

        public void UnsubscribeFromDebugEvents()
        {
            if (m_DE_DebuggerEvents != null)
            {
                m_DE_DebuggerEvents.OnEnterBreakMode -= OnEnterBreakMode;
            }
        }

        private void GeometryDebuggerToolWindowUnloaded(object sender, EventArgs e)
        {
            VSColorTheme.ThemeChanged -= OnThemeChanged;

            ClearGeomViewWindow();
            if (m_B_IsSubscribeOnBreakMod)
            {
                UnsubscribeFromDebugEvents();
                m_B_IsSubscribeOnBreakMod = false;
            }
        }

        private void Variable_PropertyChanged(object sender, PropertyChangedEventArgs e) // срабатывает, если какой-то элемент в таблице изменил своё свойство (пример, CheckBox на m_B_IsSelected)
        {
            var variable = sender as Variable;

            if (variable != null)
            {
                // Здесь можно обработать изменения конкретных свойств
                if (e.PropertyName == nameof(variable.m_B_IsSelected)) // если изменение - CheckBox на m_B_IsSelected
                {
                    string pathOfVariable = m_S_PathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths

                    bool isSerialized = m_L_Paths[pathOfVariable].Item2; // есть ли информация о этой переменной в файле pathOfVariable
                    bool isSelected = variable.m_B_IsSelected;

                    m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(isSelected, isSerialized);

                    if (isSerialized)
                        m_CH_Host.visibilityGeomView(m_S_GlobalPath + "\\" + pathOfVariable, isSelected); // если переменная выбрана для показа (isSelected)
                    else
                        draw();
                }
            }
        }

        private void Variables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null) // Добавлены новые элементы
            {
                foreach (Variable newItem in e.NewItems)
                    newItem.PropertyChanged += Variable_PropertyChanged;
            }

            if (e.OldItems != null) // Удалены элементы
            {
                foreach (Variable oldItem in e.OldItems)
                    oldItem.PropertyChanged -= Variable_PropertyChanged;
            }
        }


        private void btnOpenAddMenu_Click(object sender, RoutedEventArgs e) // срабатывает при нажатии на кнопку с отркыть окно AddVariable
        {
            if (m_DGV_Debugger.IsDebugMode()) // проверка на DebugMode
            {
                if (m_W_AddWindow == null || m_W_AddWindow.IsVisible == false) // если окно еще не инициализировано
                    InitAddWindowComponent();

                m_AM_AddMenu.BreakModDetected(); // обновляем переменные
                m_W_AddWindow.Show(); // показываем диалог

                //m_OBOV_Variables = m_AM_AddMenu.GetVariables();
                //dgObjects.ItemsSource = m_OBOV_Variables;
            }
            else
            {
                System.Windows.MessageBox.Show("ERROR: You need to start Debug mode.");
            }
        }
        private void OnAddWindowClosing(object sender, CancelEventArgs e) // когда закрыли окно
        {
            ObservableCollection<Variable> variablesFromAddMenu = m_AM_AddMenu.GetVariables(); // получаем список всех переменных, которые пришли из окна AddVariables (их отличительное
                                                                                               // свойство в том, что они все isAdded
            ObservableCollection<Variable> tempVariables = new ObservableCollection<Variable>(m_OBOV_Variables);
            this.m_OBOV_Variables = new ObservableCollection<Variable>();

            Dictionary<string, Tuple<bool, bool>> tempPaths = new Dictionary<string, Tuple<bool, bool>>(m_L_Paths); // сохраняем старый список переменных, которые уже были до этого
            m_L_Paths.Clear(); // очищаем исходные данные


            foreach (var tempVariable in tempVariables)
            {
                string pathOfVariable = m_S_PathForFile + tempVariable.m_S_Addres; // ключ в Dictionary m_L_Paths

                foreach (var variableFromAddMenu in variablesFromAddMenu)
                {
                    if (tempVariable.m_B_IsAdded == variableFromAddMenu.m_B_IsAdded &&
                           tempVariable.m_B_IsSelected == variableFromAddMenu.m_B_IsSelected &&
                           tempVariable.m_C_Color == variableFromAddMenu.m_C_Color &&
                           tempVariable.m_S_Addres == variableFromAddMenu.m_S_Addres &&
                           tempVariable.m_S_Name == variableFromAddMenu.m_S_Name &&
                           tempVariable.m_S_Source == variableFromAddMenu.m_S_Source &&
                           tempVariable.m_S_Type == variableFromAddMenu.m_S_Type)
                    {
                        m_OBOV_Variables.Add(variableFromAddMenu);
                        m_L_Paths.Add(pathOfVariable, tempPaths[pathOfVariable]);
                        break;
                    }
                    else if (tempVariable.m_B_IsAdded == variableFromAddMenu.m_B_IsAdded &&
                           tempVariable.m_B_IsSelected == variableFromAddMenu.m_B_IsSelected &&
                           tempVariable.m_C_Color != variableFromAddMenu.m_C_Color &&
                           tempVariable.m_S_Addres == variableFromAddMenu.m_S_Addres &&
                           tempVariable.m_S_Name == variableFromAddMenu.m_S_Name &&
                           tempVariable.m_S_Source == variableFromAddMenu.m_S_Source &&
                           tempVariable.m_S_Type == variableFromAddMenu.m_S_Type)
                    {
                        m_OBOV_Variables.Add(variableFromAddMenu);
                        m_L_Paths.Add(pathOfVariable, new Tuple<bool, bool>(tempPaths[pathOfVariable].Item1, false));
                        break;
                    }
                    else
                        continue;
                }
            }

            foreach (var variableFromAddMenu in variablesFromAddMenu)
            {
                string pathOfVariable = m_S_PathForFile + variableFromAddMenu.m_S_Addres; // ключ в Dictionary m_L_Paths

                bool isFind = false;

                foreach (var variable in m_OBOV_Variables)
                {
                    if (variable.m_B_IsAdded == variableFromAddMenu.m_B_IsAdded &&
                           variable.m_B_IsSelected == variableFromAddMenu.m_B_IsSelected &&
                           variable.m_C_Color == variableFromAddMenu.m_C_Color &&
                           variable.m_S_Addres == variableFromAddMenu.m_S_Addres &&
                           variable.m_S_Name == variableFromAddMenu.m_S_Name &&
                           variable.m_S_Source == variableFromAddMenu.m_S_Source &&
                           variable.m_S_Type == variableFromAddMenu.m_S_Type)
                    {
                        isFind = true;
                        break;
                    }
                }

                if (!isFind)
                {
                    m_L_Paths.Add(pathOfVariable, Tuple.Create(variableFromAddMenu.m_B_IsSelected, false)); // то создаем новую с флагами isSelected = false, isSerialized = false
                    m_OBOV_Variables.Add(variableFromAddMenu);
                }
            }

            dgObjects.ItemsSource = m_OBOV_Variables; // обновляем визуальную составляющую 

            draw();

        }

        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction action) // срабатывает при f5, f10, f11
        {
            m_AM_AddMenu.BreakModDetected(); // обновляем информацию о наших переменных, которые мы отслеживаем, валидны ли они
            ObservableCollection<Variable> variablesFromAddMenu = m_AM_AddMenu.GetVariables(); // получаем итоговые данные с валидными переменными
            ObservableCollection<Variable> tempVariables = new ObservableCollection<Variable>(m_OBOV_Variables);

            /*
             * Нам необходимо удалить все переменные из m_L_Paths, которые больше не валидны 
             * Обновить у них всех isSerialized на false
             * isSelected - оставить таким, каким было
            */

            foreach (var variable in tempVariables)
            {
                string pathOfVariable = m_S_PathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths

                bool isFind = false;

                foreach (var variableFromAddMenu in variablesFromAddMenu)
                {
                    if (variable.m_B_IsAdded == variableFromAddMenu.m_B_IsAdded &&
                               variable.m_B_IsSelected == variableFromAddMenu.m_B_IsSelected &&
                               variable.m_C_Color == variableFromAddMenu.m_C_Color &&
                               variable.m_S_Addres == variableFromAddMenu.m_S_Addres &&
                               variable.m_S_Name == variableFromAddMenu.m_S_Name &&
                               variable.m_S_Source == variableFromAddMenu.m_S_Source &&
                               variable.m_S_Type == variableFromAddMenu.m_S_Type)
                    {
                        isFind = true;
                        break;
                    }
                }


                if (isFind)
                {
                    m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(variable.m_B_IsSelected, false);
                }
                else
                {
                    m_L_Paths.Remove(pathOfVariable);
                    m_OBOV_Variables.Remove(variable);
                }
            }

            dgObjects.ItemsSource = m_OBOV_Variables; // обновляем визуальную составляющую

            draw();
        }

        private void ColorDisplay_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button; // получаем кнопку из DataGrid, на которую нажал пользователь

            if (button != null) // если кнопка не null
            {
                Variable variable = button.DataContext as Variable; // получаем из неё переменную

                if (variable != null) // если мы смогли получить переменную 
                {
                    // создаем окно colorPicker
                    ColorPicker colorPicker = new ColorPicker();

                    System.Windows.Window pickerWindow = new System.Windows.Window
                    {
                        Title = "Pick a Color",
                        Content = colorPicker,
                        SizeToContent = SizeToContent.WidthAndHeight,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    // Передаем нашему ColorPicker'у значение цветов, полученных из переменной (поле m_C_Color)
                    colorPicker.RedValue.Text = ((byte)variable.m_C_Color.m_i_R).ToString();
                    colorPicker.GreenValue.Text = ((byte)variable.m_C_Color.m_i_G).ToString();
                    colorPicker.BlueValue.Text = ((byte)variable.m_C_Color.m_i_B).ToString();

                    pickerWindow.ShowDialog(); // показываем ДИАЛОГ с нашим ColorPicker (так как это диалог, то другие элементы UI недоступны - аналогия выборки файла при его открытии в Word'e)

                    int R = (int)colorPicker.m_RGB.m_Byte_R;
                    int G = (int)colorPicker.m_RGB.m_Byte_G;
                    int B = (int)colorPicker.m_RGB.m_Byte_B;

                    if (variable.m_C_Color.m_i_R != R ||
                        variable.m_C_Color.m_i_G != G ||
                        variable.m_C_Color.m_i_B != B) // если цвет изменился (пользователь поменял значение слайдеров), кроме того, если цвет поменялся
                                                       // то мы должны изменить и отрисовку при условии, что элемент выбран или (?сериализован?)
                                                       // пока что пусть будет только что он выбран isSelected
                    {

                        // Передаем эти цвета в variable.m_C_Color
                        variable.PropertyChanged -= Variable_PropertyChanged;
                        variable.m_C_Color = new Utils.Color(R, G, B);
                        variable.PropertyChanged += Variable_PropertyChanged;

                        // Обновляем цвет кнопки
                        button.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)R, (byte)G, (byte)B));

                        string pathOfVariable = m_S_PathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths
                        bool isSelected = m_L_Paths[pathOfVariable].Item1; // isSelected (выбран ли он для отрисовки)

                        if (isSelected) // если он выбран, то его нужно пересериализировать
                        {
                            m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(true, false);
                            draw();
                        }
                        else // если он не выбран, то мы просто его помечаем, как неIsSelected и неIsSerialized
                        {
                            m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(false, false);
                        }
                    }
                    else // если цвет не изменился, то ничего не делаем, не меняем isSerialized (геометрия осталась такой же)
                        return;
                }
            }
        }
        private void MenuItemAddForDrawing_Click(object sender, RoutedEventArgs e) // если с помощью контекстного меню выбрали isSelected переменные (одну или несколько)
        {
            if (dgObjects.SelectedItems.Count == 0) // если кол-во выбранных элементов = 0, то есть пользователь ничего
                                                    // не выбрал для каких-либо действий через контекстное меню
                return;
            else // если же пользователь выбрал элементы для isSelected (в данном случае), т.е. dgObjects.SelectedItems.Count != 0
            {
                foreach (var item in dgObjects.SelectedItems) // проходимся по каждому элементу, который пользователь хочет сделать isSelected
                {
                    if (item is Variable)
                    {
                        Variable variable = (Variable)item;

                        if (!variable.m_B_IsSelected) // если переменная isSelected - false
                        {
                            /*
                             * Тут мы должны проверить два случая, является переменная isSerialized или нет 
                            */

                            string pathOfVariable = m_S_PathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths
                            bool isSerialized = m_L_Paths[pathOfVariable].Item2; // получаем информацию, сериализована ли переменная
                            variable.PropertyChanged -= Variable_PropertyChanged; // отписваемся от изменений, из-за них вызовется лишняя функция
                            variable.m_B_IsSelected = true;
                            variable.PropertyChanged += Variable_PropertyChanged; // подписываемся обратно
                            m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(true, isSerialized); // ставим, что переменная isSelected, isSerialized - сохраняем старый
                        }
                    }
                }

                dgObjects.CommitEdit();
                dgObjects.CommitEdit();

                dgObjects.Items.Refresh(); // обновляем визуальную составляющую

                draw();
            }
        }
        private void MenuItemAddForIsntDrawing_Click(object sender, RoutedEventArgs e) // если с помощью контекстного меню выбрали unSelected переменные (одну или несколько)
        {
            if (dgObjects.SelectedItems.Count == 0) // если кол-во выбранных элементов = 0, то есть пользователь ничего
                                                    // не выбрал для каких-либо действий через контекстное меню
                return;
            else // если же пользователь выбрал элементы для unSelected (в данном случае), т.е. dgObjects.SelectedItems.Count != 0
            {
                foreach (var item in dgObjects.SelectedItems) // проходимся по каждому элементу, который пользователь хочет сделать unSelected
                {
                    if (item is Variable)
                    {
                        Variable variable = (Variable)item;

                        if (variable.m_B_IsSelected)
                        {
                            /*
                             * Тут мы должны проверить два случая, является переменная isSerialized или нет 
                            */
                            string pathOfVariable = m_S_PathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths
                            bool isSerialized = m_L_Paths[pathOfVariable].Item2; // получаем информацию, сериализована ли переменная
                            variable.PropertyChanged -= Variable_PropertyChanged; // отписваемся от изменений, из-за них вызовется лишняя функция
                            variable.m_B_IsSelected = false;
                            variable.PropertyChanged += Variable_PropertyChanged; // подписываемся обратно
                            m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(false, isSerialized); // ставим, что переменная неIsSelected, isSerialized - сохраняем старый
                        }
                    }
                }

                dgObjects.CommitEdit();
                dgObjects.CommitEdit();

                dgObjects.Items.Refresh(); // обновляем визуальную составляющую

                draw();
            }
        }
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e) // если с помощью контекстного меню удалили переменные (одну или несколько)
        {
            if (dgObjects.SelectedItems.Count == 0) // если кол-во выбранных элементов = 0, то есть пользователь ничего
                                                    // не выбрал для каких-либо действий через контекстное меню
                return;
            else // если же пользователь выбрал элементы для удаления (в данном случае), т.е. dgObjects.SelectedItems.Count != 0
            {
                List<Variable> variablesOnDelete = dgObjects.SelectedItems.OfType<Variable>().ToList(); // создаем временный лист, хранящий переменные, которые удаляем

                foreach (var variable in variablesOnDelete)
                {
                    string pathOfVariable = m_S_PathForFile + variable.m_S_Addres;

                    m_L_Paths.Remove(pathOfVariable);
                    m_OBOV_Variables.Remove(variable);
                }

                dgObjects.ItemsSource = m_OBOV_Variables; // обновляем визуальную составляющую

                delete();
            }
        }
        private void MenuItemToDown_Click(object sender, RoutedEventArgs e)
        {
            if (dgObjects.SelectedItems.Count == 0) // Если нет выбранных элементов
                return;

            // Сохраняем список выделенных элементов
            var selectedVariables = dgObjects.SelectedItems.OfType<Variable>().ToList();

            if (selectedVariables.Count == 0) // Если нет выделенных элементов типа Variable
                return;

            // Перемещаем элементы вниз
            for (int i = selectedVariables.Count - 1; i >= 0; --i)
            {
                var variable = selectedVariables[i];
                int currentIndex = m_OBOV_Variables.IndexOf(variable);

                // Проверяем, можно ли переместить элемент вниз
                if (currentIndex < m_OBOV_Variables.Count - 1 && !selectedVariables.Contains(m_OBOV_Variables[currentIndex + 1]))
                {
                    m_OBOV_Variables.Move(currentIndex, currentIndex + 1); // Используем встроенный метод Move
                }
            }

            // Очищаем текущее выделение
            dgObjects.SelectedItems.Clear();

            // Восстанавливаем выделение
            foreach (var variable in selectedVariables)
                dgObjects.SelectedItems.Add(variable);

            reorder();
        }
        private void MenuItemToUp_Click(object sender, RoutedEventArgs e)
        {
            if (dgObjects.SelectedItems.Count == 0) // Если нет выбранных элементов
                return;

            // Сохраняем список выделенных элементов
            var selectedVariables = dgObjects.SelectedItems.OfType<Variable>().ToList();

            if (selectedVariables.Count == 0) // Если нет выделенных элементов типа Variable
                return;

            // Перемещаем элементы вверх
            foreach (var variable in selectedVariables)
            {
                int currentIndex = m_OBOV_Variables.IndexOf(variable);

                // Проверяем, можно ли переместить элемент вверх
                if (currentIndex > 0 && !selectedVariables.Contains(m_OBOV_Variables[currentIndex - 1]))
                {
                    m_OBOV_Variables.Move(currentIndex, currentIndex - 1); // Используем встроенный метод Move
                }
            }

            // Очищаем текущее выделение
            dgObjects.SelectedItems.Clear();

            // Восстанавливаем выделение
            foreach (var variable in selectedVariables)
                dgObjects.SelectedItems.Add(variable);

            reorder();
        }
        private void MenuItemToBottom_Click(object sender, RoutedEventArgs e)
        {
            if (dgObjects.SelectedItems.Count == 0) // Если нет выбранных элементов
                return;

            List<Variable> selectedVariables = dgObjects.SelectedItems.OfType<Variable>().ToList();

            if (selectedVariables.Count == 0) // Если нет выделенных элементов типа Variable
                return;

            // Удаляем выбранные элементы из коллекции
            foreach (var variable in selectedVariables)
                m_OBOV_Variables.Remove(variable);

            // Добавляем их в конец коллекции
            foreach (var variable in selectedVariables)
                m_OBOV_Variables.Add(variable);

            // Очищаем текущее выделение
            dgObjects.SelectedItems.Clear();

            // Восстанавливаем выделение
            foreach (var variable in selectedVariables)
                dgObjects.SelectedItems.Add(variable);

            reorder();
        }
        private void MenuItemToTop_Click(object sender, RoutedEventArgs e)
        {
            if (dgObjects.SelectedItems.Count == 0) // Если нет выбранных элементов
                return;

            List<Variable> selectedVariables = dgObjects.SelectedItems.OfType<Variable>().ToList();

            if (selectedVariables.Count == 0) // Если нет выделенных элементов типа Variable
                return; // значит ничего не нужно изменять, просто выходим из функции

            // Удаляем выбранные элементы из коллекции
            foreach (var variable in selectedVariables)
                m_OBOV_Variables.Remove(variable);

            // Добавляем их в начало коллекции
            for (int i = 0; i < selectedVariables.Count; i++)
                m_OBOV_Variables.Insert(i, selectedVariables[i]);

            // Очищаем текущее выделение
            dgObjects.SelectedItems.Clear();

            // Восстанавливаем выделение
            foreach (var variable in selectedVariables)
                dgObjects.SelectedItems.Add(variable);

            reorder(); // изменяем порядок отрисовки
        }

        private void draw()
        {
            List<Variable> variablesForSerializations = new List<Variable>(); // переменные, которые нужно сериализировать заново
            List<Tuple<string, bool>> files = new List<Tuple<string, bool>>(); // лист с путями и bool - isVisible

            foreach (var currentVariable in m_OBOV_Variables)
            {
                string pathOfVariable = m_S_PathForFile + currentVariable.m_S_Addres; // ключ в Dictionary m_L_Paths

                bool isSelected = m_L_Paths[pathOfVariable].Item1;
                bool isSerialized = m_L_Paths[pathOfVariable].Item2;

                if (isSerialized)
                {
                    if (isSelected) // в случае, если переменная уже сериализована (данные о ней есть в файле) и надо менять визибилити
                        files.Add(Tuple.Create(pathOfVariable, false)); // указываем, что эту переменную НЕ надо перезагружать 
                    else // в случае, если переменная уже сериализована (данные о ней есть в файле) и ей НЕ надо менять визибилити, то мы ничего с ней не делаем
                        continue;
                }
                else
                {
                    if (isSelected)
                    {
                        files.Add(Tuple.Create(pathOfVariable, true)); // указываем, что эту переменную надо перезагружать 
                        variablesForSerializations.Add(currentVariable);
                    }
                    else
                        continue;
                }
            }

            if (variablesForSerializations.Count != 0)
            {
                m_DE_DebuggerEvents.OnEnterBreakMode -= OnEnterBreakMode; // отписываемся от входа в дебаг мод, может отрицательно влиять на результат
                                                                          // будут пропадать элементы из таблицы
                SharedMemory sharedMemory = new SharedMemory(variablesForSerializations, m_DGV_Debugger.GetDTE()); // сюда мы отдаем только те переменные, которые выбраны и их надо пересериализировать
                sharedMemory.CreateMessages();
                sharedMemory.WriteToMemory();
                sharedMemory.DoSerialize();
                sharedMemory.CheckResulst();
                m_S_GlobalPath = sharedMemory.getPath();

                m_DE_DebuggerEvents.OnEnterBreakMode += OnEnterBreakMode; // подписываемся на вход в дебаг мод обратно

                foreach (var variable in variablesForSerializations)
                {
                    string pathOfVariable = m_S_PathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths
                    bool isSelected = m_L_Paths[pathOfVariable].Item1;

                    m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(isSelected, true);
                }

                m_CH_Host.reloadGeomView(files, m_S_GlobalPath, m_B_IsFirst);
                m_B_IsFirst = false;
                return;
            }

            // у нас есть готовая сериализация, то есть все переменные имеют данные в файлах

            m_CH_Host.reloadGeomView(files, m_S_GlobalPath);
        }
        private void reorder()
        {
            List<Tuple<string, bool>> files = new List<Tuple<string, bool>>(); // переменные, порядок которых изменился

            foreach (var variable in m_OBOV_Variables) // проходимся по всем переменным
            {
                string pathOfVariable = m_S_PathForFile + variable.m_S_Addres; // получаем ключ в m_L_Paths

                bool isSelected = m_L_Paths[pathOfVariable].Item1; // получаем, выбрана ли переменная для отрисовки в главной таблице
                bool isSerialized = m_L_Paths[pathOfVariable].Item2; // получаем, сериализована ли переменная (то есть данные о ней записаны в файл)

                if (isSerialized)
                    files.Add(Tuple.Create(pathOfVariable, isSelected)); // в зависимости от того, выбрана ли переменная, говорим, что её isSelected перегружать
                else // если переменная несериализована (данных о ней нет в файле или их необходимо обновить)
                    continue;
            }

            m_CH_Host.reloadGeomView(files, m_S_GlobalPath); // отправляем файл на перезагрузку уже в нужном порядке
        }
        private void delete()
        {
            List<Tuple<string, bool>> files = new List<Tuple<string, bool>>();

            foreach (var variable in m_OBOV_Variables)
            {
                string pathOfVariable = m_S_PathForFile + variable.m_S_Addres;

                bool isSelected = m_L_Paths[pathOfVariable].Item1;
                bool isSerialized = m_L_Paths[pathOfVariable].Item2;

                if (isSerialized) // если переменная уже сериализована (то есть данные о ней записаны в файл){
                    files.Add(Tuple.Create(pathOfVariable, isSelected));
                else
                    continue;
            }

            m_CH_Host.reloadGeomView(files, m_S_GlobalPath);
        }
    }
}
