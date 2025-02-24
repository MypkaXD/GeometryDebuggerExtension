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
        private string m_S_PathForFile = "vis_dbg_"; // приписка к файлу
        private string m_S_GlobalPath = ""; // глобальный путь до файла
        private string m_S_Message { get; set; } // сообщение, которое будет записано в MMF

        private bool m_B_IsSubscribeOnBreakMod = false; // подписаны на дебаг ивенты
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
        private ResourceDictionary darkTheme = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/GeometryDebugger;component/Style/Dark_Theme.xaml", UriKind.RelativeOrAbsolute)
        };

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
            InitializeComponent(); // инициализация компонент

            m_DGV_Debugger = new DebuggerGetterVariables(); // создаем объект класса DebuggerGetterVariables для получения переменных в будущем (иниц. DTE)

            Loaded += GeometryDebuggerToolWindowLoaded; // срабатывает при собитии загрузки основного окна
            Unloaded += GeometryDebuggerToolWindowUnloaded; // срабатывает при событии выгрузки основного окна

            m_AM_AddMenu = new AddMenu(m_DGV_Debugger); // инициализируем второе окно для добавления переменных из CF (CurrentStackFrame), WL (WatchList), MyS (MySelfAdded)
            m_L_Paths = new Dictionary<string, Tuple<bool, bool>>(); // создаем словарь с ключем string - уникальное название переменной и знчением Tuple<bool, bool> (isSerialized, isVisible - записана ли информация о переменной в файл, видна ли переменная на сцене)
            m_OBOV_Variables = new ObservableCollection<Variable>(); // хранилище для переменных, которые будут в DataGrid

            InitAddWindowComponent(); // инициализация второго окна (для добавления переменных)

            ResetTheme();
        }


        private void ClearGeomViewWindow()
        {
            m_OBOV_Variables.Clear();
            dgObjects.ItemsSource = m_OBOV_Variables;

            m_CH_Host.reloadGeomView(new List<Tuple<string, bool>> { }, m_S_GlobalPath);
        }

        private void Variable_PropertyChanged(object sender, PropertyChangedEventArgs e) // срабатывает, если какой-то элемент в таблице изменил своё свойство (пример, CheckBox на m_B_IsSelected)
        {
            var variable = sender as Variable;

            if (variable != null)
            {
                // Здесь можно обработать изменения конкретных свойств
                if (e.PropertyName == nameof(variable.m_B_IsSelected)) // если изменение - CheckBox на m_B_IsSelected
                {
                    string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable); // ключ в Dictionary m_L_Paths

                    bool isSerialized = m_L_Paths[pathOfVariable].Item2; // есть ли информация о этой переменной в файле pathOfVariable
                    bool isSelected = variable.m_B_IsSelected;

                    m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(isSelected, isSerialized);

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
                m_W_AddWindow.ShowDialog(); // показываем диалог

                //m_OBOV_Variables = m_AM_AddMenu.GetVariables();
                //dgObjects.ItemsSource = m_OBOV_Variables;
            }
            else
            {
                MessageBox.Show("Error: You need to start Debug mode.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

                        string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable); // ключ в Dictionary m_L_Paths
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


        //////////
        ////////////////////////////////////////////////////////////
        //////////
        // methods for GeomView
        private void draw()
        {
            // reload в GeomView работает по принципу все переменные, которые он принял - остаются, не принял - удаляются. Флаг true, false (надо ли рисовать заново)
            // visibility true - видно, false - не видно

            List<Variable> variablesForSerializations = new List<Variable>(); // переменные, которые нужно сериализировать заново
            List<Tuple<string, bool>> files = new List<Tuple<string, bool>>(); // лист с путями и bool - isVisible

            foreach (var currentVariable in m_OBOV_Variables) // проходимся по текущим переменным из DataGrid
            {
                string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, currentVariable); // ключ в Dictionary m_L_Paths

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

            if (variablesForSerializations.Count != 0) // если число сериализирумых переменных больше 0
            {
                m_DE_DebuggerEvents.OnEnterBreakMode -= OnEnterBreakMode; // отписываемся от входа в дебаг мод, может отрицательно влиять на результат
                                                                          // будут пропадать элементы из таблицы

                SharedMemory sharedMemory = new SharedMemory(variablesForSerializations, m_DGV_Debugger.GetDTE()); // сюда мы отдаем только те переменные, которые выбраны и их надо пересериализировать
                sharedMemory.CreateMessages(); // создаем сообщение
                sharedMemory.WriteToMemory(); // записываем сообщение в MMF
                sharedMemory.DoSerialize(); // вызываем функцию Serialize()

                string response = sharedMemory.getResponse(); // получаем сообщение с сериализацией (представляем собой "10|Path") 1 - сериализирована, 0 - нет

                // проверям результат
                if (!response.Contains("\\")) // в случае если не получил ответ, что-то плохое произошло (все переменные неиницализированы)
                {
                    MessageBox.Show("Error: Can't serialize variables", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    for (int i = 0; i < variablesForSerializations.Count; ++i)
                    {
                        Variable variable = variablesForSerializations[i];

                        m_L_Paths[Util.getPathOfVariable(m_S_PathForFile, variable)] = new Tuple<bool, bool>(false, false); // указываем, что переменная isSerialized = false, isVisible = false

                        variable.PropertyChanged -= Variable_PropertyChanged;
                        m_OBOV_Variables[m_OBOV_Variables.IndexOf(variable)].m_B_IsSelected = false;
                        variable.PropertyChanged += Variable_PropertyChanged;

                    }

                    return;
                }

                for (int i = 0; i < variablesForSerializations.Count; ++i)
                {
                    if (response[i + 1] == '0')
                    {
                        Variable variable = variablesForSerializations[i];

                        m_L_Paths[Util.getPathOfVariable(m_S_PathForFile, variable)] = new Tuple<bool, bool>(false, false); // указываем, что переменная isSerialized = false, isVisible = false

                        variable.PropertyChanged -= Variable_PropertyChanged;
                        m_OBOV_Variables[m_OBOV_Variables.IndexOf(variable)].m_B_IsSelected = false;
                        variable.PropertyChanged += Variable_PropertyChanged;

                        MessageBox.Show("Error: Type \"" + variable.m_S_Type + "\" of variable \"" + variable.m_S_Name + "\" wasn't serialized", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                    else
                    {
                        string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variablesForSerializations[i]); // ключ в Dictionary m_L_Paths
                        bool isSelected = m_L_Paths[pathOfVariable].Item1;

                        m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(isSelected, true);
                    }
                }

                m_S_GlobalPath = sharedMemory.getPath(); // получаем путь, куда сохранились файлы с данными

                m_DE_DebuggerEvents.OnEnterBreakMode += OnEnterBreakMode; // подписываемся на вход в дебаг мод обратно

                m_CH_Host.reloadGeomView(files, m_S_GlobalPath, m_B_IsFirst);
                m_B_IsFirst = false;
                return;
            }

            // у нас есть готовая сериализация, то есть все переменные имеют данные в файлах

            m_CH_Host.reloadGeomView(files, m_S_GlobalPath); // в случае, если мы должны удалить все объекты со сцены
        }
        private void delete()
        {
            List<Tuple<string, bool>> files = new List<Tuple<string, bool>>();

            foreach (var variable in m_OBOV_Variables)
            {
                string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable);

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
                    }
                    else
                        continue;
                }
            }

            m_CH_Host.reloadGeomView(files, m_S_GlobalPath);
        }
        private void reorder()
        {
            List<Tuple<string, bool>> files = new List<Tuple<string, bool>>(); // переменные, порядок которых изменился

            foreach (var variable in m_OBOV_Variables) // проходимся по всем переменным
            {
                string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable); // получаем ключ в m_L_Paths

                bool isSelected = m_L_Paths[pathOfVariable].Item1; // получаем, выбрана ли переменная для отрисовки в главной таблице
                bool isSerialized = m_L_Paths[pathOfVariable].Item2; // получаем, сериализована ли переменная (то есть данные о ней записаны в файл)

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
                    }
                    else
                        continue;
                }
            }

            m_CH_Host.reloadGeomView(files, m_S_GlobalPath); // отправляем файл на перезагрузку уже в нужном порядке
        }
        //////////
        ////////////////////////////////////////////////////////////
        //////////
        // methods for (Un)Loaded second window
        private void InitAddWindowComponent()
        {
            // создаем окно
            m_W_AddWindow = new System.Windows.Window
            {
                Title = "Add Variable",
                Content = m_AM_AddMenu,
                Height = 800,
                Width = 600,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            m_W_AddWindow.Loaded += OnAddWindowLoaded; // подписываемся на событие загрузки второго окна
            m_W_AddWindow.Closing += OnAddWindowClosing; // подписываемся на события выгрузки второго окна
        }
        private void OnAddWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (isLightTheme()) // в лучае если тема светлая
            {
                m_AM_AddMenu.WL.Style = lightTheme["CheckBoxStyleWL_Light"] as Style;
                m_AM_AddMenu.CF.Style = lightTheme["CheckBoxStyleCF_Light"] as Style;
            }
            else
            {
                m_AM_AddMenu.WL.Style = darkTheme["CheckBoxStyleWL_Dark"] as Style;
                m_AM_AddMenu.CF.Style = darkTheme["CheckBoxStyleCF_Dark"] as Style;

            }

            var imageSource = new BitmapImage(new Uri("../Images/Export_" + (isLightTheme() ? "Light" : "Dark") + ".png", UriKind.Relative)); // иконка экспорта

            var image = new Image
            {
                Source = imageSource,
                Width = 16,
                Height = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            m_AM_AddMenu.ButtonImport.Content = new Image()
            {
                Source = imageSource
            }; // устанавливаем картинку на кнопку
        }
        private void OnAddWindowClosing(object sender, CancelEventArgs e) // когда закрыли окно
        {
            ObservableCollection<Variable> variablesFromAddMenu = m_AM_AddMenu.GetVariables(); // получаем список всех переменных, которые пришли из окна AddVariables (их отличительное
                                                                                               // свойство в том, что они все isAdded

            ObservableCollection<Variable> tempVariables = new ObservableCollection<Variable>(m_OBOV_Variables); // делаем временный список переменных, которые были до этого
            m_OBOV_Variables = new ObservableCollection<Variable>(); // очищаем список текущих переменных

            Dictionary<string, Tuple<bool, bool>> tempPaths = new Dictionary<string, Tuple<bool, bool>>(m_L_Paths); // сохраняем старый список переменных, которые уже были до этого
            m_L_Paths.Clear(); // очищаем исходные данные

            foreach (var tempVariable in tempVariables) // проходимся по сохраненным переменным
            {
                string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, tempVariable); // ключ в Dictionary m_L_Paths

                foreach (var variableFromAddMenu in variablesFromAddMenu) // проходимся по тем переменным, которые получили из m_AddMenu
                {
                    if (Util.isEqualVariables(tempVariable, variableFromAddMenu)) // в случае, если переменные никак не поменялись
                    {
                        m_OBOV_Variables.Add(variableFromAddMenu);
                        m_L_Paths.Add(pathOfVariable, tempPaths[pathOfVariable]);
                        break;
                    }
                    else if (tempVariable.m_B_IsAdded == variableFromAddMenu.m_B_IsAdded &&
                           tempVariable.m_B_IsSelected == variableFromAddMenu.m_B_IsSelected &&
                           tempVariable.m_S_Addres == variableFromAddMenu.m_S_Addres &&
                           tempVariable.m_S_Name == variableFromAddMenu.m_S_Name &&
                           tempVariable.m_S_Source == variableFromAddMenu.m_S_Source &&
                           tempVariable.m_S_Type == variableFromAddMenu.m_S_Type &&
                           tempVariable.m_C_Color != variableFromAddMenu.m_C_Color) // если переменная поменяла только цвет, то мы добавляем её сразу (чтобы сохранить порядок отрисовки)
                    {
                        m_OBOV_Variables.Add(variableFromAddMenu);
                        m_L_Paths.Add(pathOfVariable, new Tuple<bool, bool>(tempPaths[pathOfVariable].Item1, false));
                        break;
                    }
                    else // иначе переменная новая (но мы её не добавляем, потому что иначе порядок отрисовки изменится)
                    {
                        continue;
                    }
                }
            }

            foreach (var variableFromAddMenu in variablesFromAddMenu) // теперь проходимся по всем переменным, которые получиил из AddMenu (нам нужно сохранить только те переменные, которых еще нет)
            {
                string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variableFromAddMenu); // ключ в Dictionary m_L_Paths

                bool isFind = false; // переменная была найдена в DataGrid?

                foreach (var variable in m_OBOV_Variables) // проходимся по всем сохраненным в пред. цикле переменным
                {
                    if (Util.isEqualVariables(variable, variableFromAddMenu)) // если найдена
                    {
                        isFind = true; // стафим флаг
                        break;
                    }
                }

                if (!isFind) // если не найдена
                {
                    m_L_Paths.Add(pathOfVariable, Tuple.Create(variableFromAddMenu.m_B_IsSelected, false)); // то создаем новую с флагами isSelected = false, isSerialized = false
                    m_OBOV_Variables.Add(variableFromAddMenu); // добавляем в коллекцию DataGrid
                }
            }

            dgObjects.ItemsSource = m_OBOV_Variables; // обновляем визуальную составляющую 

            draw();

        }
        //////////
        ////////////////////////////////////////////////////////////
        //////////
        // methods for (Un)LoadedMainWindow
        public void GeometryDebuggerToolWindowLoaded(object sender, RoutedEventArgs e) // срабатывает, когда основное окно загружено (доступны элементы управления - кнопки, dataGrid и т.д)
        {
            VSColorTheme.ThemeChanged += OnThemeChanged; // подписываемся на событие смены темы VisualStudio

            // создаем GeomView Context
            m_CH_Host = new ControlHost();
            if (ControlHostElement.Child == null)
                ControlHostElement.Child = m_CH_Host;

            if (!m_B_IsSubscribeOnBreakMod) // в случае если мы не подписаны на срабатывание BreakMod'a
            {
                SubscribeOnDebugEvents(); // подписываемся на дебаг ивенты
                m_B_IsSubscribeOnBreakMod = true; // boolева переменная m_B_IsSubscribeOnBreakMod в true (подписаны на дебаг ивенты)
            }
        }
        private void GeometryDebuggerToolWindowUnloaded(object sender, EventArgs e) // срабатывает, когда основное окно выгружено (срабатывает, когда мы вытаскиваем окно из MVS или наоборот)
        {
            VSColorTheme.ThemeChanged -= OnThemeChanged; // отписываемся от событий по смене темы

            // ??????????????????????????????????
            // удаляем сцену с GeomView и очищаем таблицу. 
            ClearGeomViewWindow();

            if (m_B_IsSubscribeOnBreakMod) // в случае, если мы не подписаны на дебаг ивенты
            {
                UnsubscribeFromDebugEvents(); // отписываемся
                m_B_IsSubscribeOnBreakMod = false; // усатанвливаем флаг, что не подписаны
            }
        }
        //
        //////////
        ////////////////////////////////////////////////////////////
        //////////
        // methods for DebugEvets
        public void SubscribeOnDebugEvents() // подписываемся на "дебаг" ивенты
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (m_DGV_Debugger.GetDTE() == null) // в случае, если наш DTE не инициализировался из класса DebuggerGetterVariables
                return;

            m_DE_DebuggerEvents = m_DGV_Debugger.GetDTE().Events.DebuggerEvents;
            m_DE_DebuggerEvents.OnEnterBreakMode += OnEnterBreakMode; // подписываемся на срабатаываение f10, f11, f5.
        }
        public void UnsubscribeFromDebugEvents()
        {
            if (m_DE_DebuggerEvents != null)
            {
                m_DE_DebuggerEvents.OnEnterBreakMode -= OnEnterBreakMode;
            }
        }
        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction action) // срабатывает при f5, f10, f11
        {
            m_AM_AddMenu.BreakModDetected(); // обновляем информацию о наших переменных, которые мы отслеживаем, валидны ли они

            m_OBOV_Variables = new ObservableCollection<Variable>(m_AM_AddMenu.GetVariables()); // получаем итоговые данные с валидными переменными и которые isAdded = true
            Dictionary<string, Tuple<bool, bool>> oldPaths = new Dictionary<string, Tuple<bool, bool>>(m_L_Paths);
            m_L_Paths.Clear();

            // Кол-во новых переменных меньши или равно кол-ву старых переменных

            /*
             * Нам необходимо удалить все переменные из m_L_Paths, которые больше не валидны 
             * Обновить у них всех isSerialized на false
             * isSelected - оставить таким, каким было
            */

            foreach (var variable in m_OBOV_Variables)
            {
                string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable); // ключ в Dictionary m_L_Paths
                m_L_Paths.Add(pathOfVariable, new Tuple<bool, bool>(oldPaths[pathOfVariable].Item1, false));
            }

            dgObjects.ItemsSource = m_OBOV_Variables; // обновляем визуальную составляющую

            draw();
        }
        //
        //////////
        ////////////////////////////////////////////////////////////
        ////////// 
        //methods for Themes:
        private void OnThemeChanged(ThemeChangedEventArgs e)
        {
            ResetTheme(); // перезагружаем цветовую палитру элементов
        }
        private bool isLightTheme()
        {
            System.Drawing.Color newBackgroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundBrushKey);

            if (newBackgroundColor.R >= 155 || newBackgroundColor.G >= 155 || newBackgroundColor.G >= 155)
                return true;
            else
                return false;
        }
        public void ResetTheme()
        {
            var menuItems = this.dgObjects.ContextMenu.Items.OfType<MenuItem>(); // получаем MenuItems-элементы из dataGrid - dgObjects

            foreach (var menuItem in menuItems) // проходимся по каждому элементу
            {
                string fileName = menuItem.Header.ToString(); // получаем заголовок

                if (isLightTheme())
                    fileName += "_Light";
                else
                    fileName += "_Dark";

                var imageSource = new BitmapImage(new Uri("../Images/" + fileName + ".png", UriKind.Relative)); // получаем путь до картинки

                var image = new Image
                {
                    Source = imageSource,
                    Width = 16,
                    Height = 16,
                    HorizontalAlignment = HorizontalAlignment.Left
                }; // создаем изображение с заданными характеристиками

                menuItem.Icon = image;
            }

            var menuItemsAddMenu = this.m_AM_AddMenu.dgAddVariables.ContextMenu.Items.OfType<MenuItem>(); // получаем MenuItems-элементы из dataGrid AddMenuWindow

            foreach (var menuItem in menuItemsAddMenu)
            {
                string fileName = menuItem.Header.ToString();

                if (isLightTheme())
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
        //
        //////////
        ////////////////////////////////////////////////////////////
        ////////// 
        // methods for MenuItems events
        private void MenuItemAddForDrawing_Click(object sender, RoutedEventArgs e) // если с помощью контекстного меню выбрали isSelected переменные (одну или несколько)
        {
            if (dgObjects.SelectedItems.Count == 0) // если кол-во выбранных элементов = 0, то есть пользователь ничего
                                                    // не выбрал для каких-либо действий через контекстное меню
                return;
            else // если же пользователь выбрал элементы для isSelected, т.е. dgObjects.SelectedItems.Count != 0
            {
                foreach (var item in dgObjects.SelectedItems) // проходимся по каждому элементу, который пользователь хочет сделать isSelected
                {
                    if (item is Variable)
                    {
                        Variable variable = (Variable)item;

                        if (!variable.m_B_IsSelected) // если переменная isSelected - false
                        {
                            // Тут мы должны проверить два случая, является переменная isSerialized или нет 

                            string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable); // ключ в Dictionary m_L_Paths
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

                //dgObjects.Items.Refresh(); // обновляем визуальную составляющую

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
                            string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable); // ключ в Dictionary m_L_Paths
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

                //dgObjects.Items.Refresh(); // обновляем визуальную составляющую

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
                    string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable);

                    m_L_Paths.Remove(pathOfVariable);
                    m_OBOV_Variables.Remove(variable);
                    m_AM_AddMenu.m_OBOV_Variables.Remove(variable);
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
    }
}
