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
        private bool m_B_IsFirstTemp = true; // костыль

        private DebuggerGetterVariables m_DGV_Debugger;
        private AddMenu m_AM_AddMenu;
        private System.Windows.Window m_W_AddWindow;
        private DebuggerEvents m_DE_DebuggerEvents;
        private ControlHost m_CH_Host;

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
            m_OBOV_Variables = new ObservableCollection<Variable>(); // хранилище для переменных, которые будут в DataGrid

            InitAddWindowComponent(); // инициализация второго окна (для добавления переменных)

            ResetTheme();
        }


        private void ClearGeomViewWindow()
        {
            m_OBOV_Variables.Clear();
            dgObjects.ItemsSource = m_OBOV_Variables;
            m_AM_AddMenu.m_OBOV_Variables.Clear();
            m_AM_AddMenu.dgAddVariables.ItemsSource = m_AM_AddMenu.m_OBOV_Variables;

            m_CH_Host.reloadGeomView(new List<Tuple<string, bool>> { }, m_S_GlobalPath);
            m_CH_Host.destroyOpenGLWindow();

            //ControlHostElement.Child = null;
            //m_CH_Host = null;
        }

        private void Variable_PropertyChanged(object sender, PropertyChangedEventArgs e) // срабатывает, если какой-то элемент в таблице изменил своё свойство (пример, CheckBox на m_B_IsSelected)
        {
            var variable = sender as Variable;

            if (variable != null)
            {
                // Здесь можно обработать изменения конкретных свойств
                if (e.PropertyName == nameof(variable.m_B_IsSelected)) // если изменение - CheckBox на m_B_IsSelected
                {
                    if (variable.m_B_IsSerialized)
                    {
                        string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable);
                        m_CH_Host.visibilityGeomView(pathOfVariable, m_S_GlobalPath, variable.m_B_IsSelected);
                    }
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
                        variable.m_B_IsSerialized = false;
                        variable.PropertyChanged += Variable_PropertyChanged;

                        // Обновляем цвет кнопки
                        button.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)R, (byte)G, (byte)B));

                        if (variable.m_B_IsSelected) // если он выбран, то его нужно пересериализировать
                            draw();
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

            m_DE_DebuggerEvents.OnEnterBreakMode -= OnEnterBreakMode; // отписываемся от входа в дебаг мод, может отрицательно влиять на результат
                                                                      // будут пропадать элементы из таблицы

            SharedMemory sharedMemory = new SharedMemory(m_DGV_Debugger.GetDTE());
            List<Tuple<string, bool>> files = new List<Tuple<string, bool>>(); // лист с путями и bool - надо ли перезагружать

            foreach (var variable in m_OBOV_Variables) // проходимся по текущим переменным из DataGrid
            {
                if (variable.m_B_IsSerialized) // елси переменная сериализована
                {
                    string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable);
                    files.Add(Tuple.Create(pathOfVariable, false));
                    continue;
                }
                else
                {
                    if (variable.m_B_IsSelected)
                    {
                        string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable);

                        variable.PropertyChanged -= Variable_PropertyChanged;

                        sharedMemory.CreateMessages(variable); // создаем сообщение
                        sharedMemory.WriteToMemory(); // записываем сообщение в MMF
                        sharedMemory.DoSerialize(); // вызываем функцию Serialize()

                        string response = sharedMemory.getResponse(); // получаем сообщение с сериализацией (представляем собой "1/0|Path") 1 - сериализирована, 0 - нет

                        if (!response.Contains("\\")) // в случае если не получил ответ, что-то плохое произошло (все переменные неиницализированы)
                        {
                            variable.m_B_IsSerialized = false;
                            variable.m_B_IsSelected = false;
                            variable.PropertyChanged += Variable_PropertyChanged;

                            MessageBox.Show("Error: Type \"" + variable.m_S_Type + "\" of variable \"" + variable.m_S_Name + "\" wasn't serialized", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            if (response[1] == '0')
                            {
                                variable.m_B_IsSerialized = false;
                                variable.m_B_IsSelected = false;

                                MessageBox.Show("Error: Type \"" + variable.m_S_Type + "\" of variable \"" + variable.m_S_Name + "\" wasn't serialized", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                variable.m_B_IsSerialized = true;
                                m_B_IsFirstTemp = false;

                                m_S_GlobalPath = sharedMemory.getPath(); // получаем путь, куда сохранились файлы с данными
                                files.Add(Tuple.Create(pathOfVariable, true));
                            }
                        }

                        variable.PropertyChanged += Variable_PropertyChanged;
                    }
                    else
                        continue;
                }
            }

            m_DE_DebuggerEvents.OnEnterBreakMode += OnEnterBreakMode; // подписываемся на вход в дебаг мод обратно

            m_CH_Host.reloadGeomView(files, m_S_GlobalPath, m_B_IsFirst);

            if (!m_B_IsFirstTemp)
                m_B_IsFirst = false;
        }
        private void delete()
        {
            List<Tuple<string, bool>> files = new List<Tuple<string, bool>>();

            foreach (var variable in m_OBOV_Variables)
            {
                string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable);

                if (variable.m_B_IsSerialized)
                {
                    if (variable.m_B_IsSelected) // в случае, если переменная уже сериализована (данные о ней есть в файле) и надо менять визибилити
                        files.Add(Tuple.Create(pathOfVariable, false)); // указываем, что эту переменную НЕ надо перезагружать 
                    else // в случае, если переменная уже сериализована (данные о ней есть в файле) и ей НЕ надо менять визибилити, то мы ничего с ней не делаем
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

                if (variable.m_B_IsSerialized)
                {
                    if (variable.m_B_IsSelected) // в случае, если переменная уже сериализована (данные о ней есть в файле) и надо менять визибилити
                        files.Add(Tuple.Create(pathOfVariable, false)); // указываем, что эту переменную НЕ надо перезагружать 
                    else // в случае, если переменная уже сериализована (данные о ней есть в файле) и ей НЕ надо менять визибилити, то мы ничего с ней не делаем
                        continue;
                }
                else
                {
                    if (variable.m_B_IsSelected)
                        files.Add(Tuple.Create(pathOfVariable, true)); // указываем, что эту переменную надо перезагружать 
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

            foreach (var tempVariable in tempVariables) // проходимся по сохраненным переменным
            {
                string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, tempVariable);

                foreach (var variableFromAddMenu in variablesFromAddMenu) // проходимся по тем переменным, которые получили из m_AddMenu
                {
                    if (tempVariable.m_B_IsAdded == variableFromAddMenu.m_B_IsAdded &&
                        tempVariable.m_S_Addres == variableFromAddMenu.m_S_Addres &&
                        tempVariable.m_S_Name == variableFromAddMenu.m_S_Name &&
                        tempVariable.m_S_Type == variableFromAddMenu.m_S_Type &&
                        tempVariable.m_S_Source == variableFromAddMenu.m_S_Source &&
                        tempVariable.m_C_Color == variableFromAddMenu.m_C_Color) // в случае, если переменные никак не поменялись
                    {
                        variableFromAddMenu.m_B_IsSelected = tempVariable.m_B_IsSelected;
                        variableFromAddMenu.m_B_IsSerialized = tempVariable.m_B_IsSerialized;
                        m_OBOV_Variables.Add(variableFromAddMenu);
                        break;
                    }
                    else if (tempVariable.m_B_IsAdded == variableFromAddMenu.m_B_IsAdded &&
                           tempVariable.m_S_Addres == variableFromAddMenu.m_S_Addres &&
                           tempVariable.m_S_Name == variableFromAddMenu.m_S_Name &&
                           tempVariable.m_S_Source == variableFromAddMenu.m_S_Source &&
                           tempVariable.m_S_Type == variableFromAddMenu.m_S_Type &&
                           tempVariable.m_C_Color != variableFromAddMenu.m_C_Color) // если переменная поменяла только цвет, то мы добавляем её сразу (чтобы сохранить порядок отрисовки)
                    {
                        variableFromAddMenu.m_B_IsSelected = tempVariable.m_B_IsSelected;
                        variableFromAddMenu.m_B_IsSelected = false;
                        m_OBOV_Variables.Add(variableFromAddMenu);
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


            if (!m_B_IsSubscribeOnBreakMod) // в случае если мы не подписаны на срабатывание BreakMod'a
            {
                // создаем GeomView Context
                m_CH_Host = new ControlHost();
                if (ControlHostElement.Child == null)
                    ControlHostElement.Child = m_CH_Host;

                SubscribeOnDebugEvents(); // подписываемся на дебаг ивенты
                m_B_IsSubscribeOnBreakMod = true; // boolева переменная m_B_IsSubscribeOnBreakMod в true (подписаны на дебаг ивенты)
            }
        }
        private void GeometryDebuggerToolWindowUnloaded(object sender, EventArgs e) // срабатывает, когда основное окно выгружено (срабатывает, когда мы вытаскиваем окно из MVS или наоборот)
        {
            VSColorTheme.ThemeChanged -= OnThemeChanged; // отписываемся от событий по смене темы

            // ??????????????????????????????????
            // удаляем сцену с GeomView и очищаем таблицу. 

            if (m_B_IsSubscribeOnBreakMod) // в случае, если мы не подписаны на дебаг ивенты
            {
                //ClearGeomViewWindow();
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
            m_DE_DebuggerEvents.OnEnterDesignMode += OnEnterDesignMode;
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
            m_OBOV_Variables = new ObservableCollection<Variable>(m_AM_AddMenu.UpdateVariableAfterBreakMod(m_OBOV_Variables)); // получаем итоговые данные с валидными переменными и которые isAdded = true

            dgObjects.ItemsSource = m_OBOV_Variables; // обновляем визуальную составляющую

            draw();
        }
        private void OnEnterDesignMode(dbgEventReason reason)
        {
            if (reason == dbgEventReason.dbgEventReasonStopDebugging || reason == dbgEventReason.dbgEventReasonEndProgram)
                ClearGeomViewWindow();
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
                int countOnSerialization = 0;

                foreach (var item in dgObjects.SelectedItems) // проходимся по каждому элементу, который пользователь хочет сделать isSelected
                {
                    if (item is Variable)
                    {
                        Variable variable = (Variable)item;

                        if (!variable.m_B_IsSelected) // если переменная isSelected - false
                        {
                            variable.PropertyChanged -= Variable_PropertyChanged; // отписваемся от изменений, из-за них вызовется лишняя функция
                            variable.m_B_IsSelected = true;
                            variable.PropertyChanged += Variable_PropertyChanged; // подписываемся обратно
                        }

                        if (variable.m_B_IsSerialized)
                        {
                            string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable);
                            m_CH_Host.visibilityGeomView(pathOfVariable, m_S_GlobalPath, variable.m_B_IsSelected);
                        }
                        else
                            ++countOnSerialization;
                    }
                }

                dgObjects.CommitEdit();
                dgObjects.CommitEdit();

                //dgObjects.Items.Refresh(); // обновляем визуальную составляющую

                if (countOnSerialization > 0)
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
                int countOnSerialization = 0;

                foreach (var item in dgObjects.SelectedItems) // проходимся по каждому элементу, который пользователь хочет сделать unSelected
                {
                    if (item is Variable)
                    {
                        Variable variable = (Variable)item;

                        if (variable.m_B_IsSelected)
                        {
                            variable.PropertyChanged -= Variable_PropertyChanged; // отписваемся от изменений, из-за них вызовется лишняя функция
                            variable.m_B_IsSelected = false;
                            variable.PropertyChanged += Variable_PropertyChanged; // подписываемся обратно
                        }

                        if (variable.m_B_IsSerialized)
                        {
                            string pathOfVariable = Util.getPathOfVariable(m_S_PathForFile, variable);
                            m_CH_Host.visibilityGeomView(pathOfVariable, m_S_GlobalPath, variable.m_B_IsSelected);
                        }
                        else
                            ++countOnSerialization;
                    }
                }

                dgObjects.CommitEdit();
                dgObjects.CommitEdit();

                //dgObjects.Items.Refresh(); // обновляем визуальную составляющую

                if (countOnSerialization > 0)
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
        private void MenuItemReload_Click(object sender, RoutedEventArgs e)
        {
            if (dgObjects.SelectedItems.Count == 0) // если кол-во выбранных элементов = 0, то есть пользователь ничего
                                                    // не выбрал для каких-либо действий через контекстное меню
                return;
            else // если же пользователь выбрал элементы
            {
                int countOnSerialization = 0;

                foreach (var item in dgObjects.SelectedItems) // проходимся по каждому элементу, который пользователь хочет сделать unSelected
                {
                    if (item is Variable)
                    {
                        Variable variable = (Variable)item;

                        if (variable.m_B_IsSerialized)
                        {
                            variable.PropertyChanged -= Variable_PropertyChanged; // отписваемся от изменений, из-за них вызовется лишняя функция
                            variable.m_B_IsSerialized = false;
                            variable.PropertyChanged += Variable_PropertyChanged; // подписываемся обратно
                        }

                        ++countOnSerialization;
                    }
                }

                dgObjects.CommitEdit();
                dgObjects.CommitEdit();

                if (countOnSerialization > 0)
                    draw();
            }
        }

        private List<bool> isDownSorting = new List<bool>(){ true, true, true, true, true, true };

        private void dgObjects_Sorting(object sender, DataGridSortingEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            int indexOfSort = getIndexOfColumn(e.Column.SortMemberPath);

            if (m_OBOV_Variables != null && indexOfSort != -1)
            {
                // Определяем направление сортировки
                ListSortDirection sortDirection = (isDownSorting[indexOfSort] == true ? ListSortDirection.Ascending
                    : ListSortDirection.Descending);

                // Устанавливаем направление сортировки для столбца
                e.Column.SortDirection = sortDirection;

                // Получаем имя свойства для сортировки
                string sortPropertyName = e.Column.SortMemberPath;

                // Сортируем коллекцию
                var sortedCollection = new ObservableCollection<Variable>(
                    sortDirection == ListSortDirection.Ascending
                        ? m_OBOV_Variables.OrderBy(x => GetPropertyValue(x, sortPropertyName))
                        : m_OBOV_Variables.OrderByDescending(x => GetPropertyValue(x, sortPropertyName)));

                // Заменяем исходную коллекцию на отсортированную
                dataGrid.ItemsSource = sortedCollection;
                m_OBOV_Variables = sortedCollection;

                if (isDownSorting[indexOfSort])
                    isDownSorting[indexOfSort] = false;
                else
                    isDownSorting[indexOfSort] = true;

                reorder();
            }

            // Отменяем стандартную сортировку
            e.Handled = true;
        }

        // Вспомогательный метод для получения значения свойства по имени
        private object GetPropertyValue(Variable obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
        }
        private int getIndexOfColumn(string name)
        {
            if (name == "m_B_IsSelected")
                return 0;
            else if (name == "m_C_Color")
                return 1;
            else if (name == "m_S_Name")
                return 2;
            if (name == "m_S_Type")
                return 3;
            else if (name == "m_S_Addres")
                return 4;
            else if (name == "m_S_Source")
                return 5;
            else
                return -1;
        }
    }
}
