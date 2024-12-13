﻿using EnvDTE;
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

namespace GeometryDebugger.UI
{
    public partial class GeometryDebuggerToolWindow : UserControl
    {
        private string pathForFile = "vis_dbg_";
        private string globalPath = "";
        private string message { get; set; }
        private bool isSet = false;

        private DebuggerGetterVariables m_DGV_debugger;
        private AddMenu addMenu;
        private System.Windows.Window addWindow;
        private DebuggerEvents m_DE_debuggerEvents;
        private ControlHost host;
        private Dictionary<string, Tuple<bool, bool>> m_L_Paths;

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

                if (_m_OBOV_Variables != null)
                {
                    // Подписываемся на события новой коллекции
                    _m_OBOV_Variables.CollectionChanged += Variables_CollectionChanged;
                    foreach (var variable in _m_OBOV_Variables)
                        variable.PropertyChanged += Variable_PropertyChanged;
                }
            }
        }

        public GeometryDebuggerToolWindow()
        {
            InitializeComponent();

            Loaded += GeometryDebuggerToolWindowLoaded;
            Unloaded += GeometryDebuggerToolWindowUnloaded;

            m_DGV_debugger = new DebuggerGetterVariables();

            addMenu = new AddMenu(m_DGV_debugger);
            m_L_Paths = new Dictionary<string, Tuple<bool, bool>>();
            m_OBOV_Variables = new ObservableCollection<Variable>();

            InitAddWindowComponent();
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

            addWindow.Closing += OnAddWindowClosing;
        }

        public void GeometryDebuggerToolWindowLoaded(object sender, RoutedEventArgs e)
        {
            host = new ControlHost();
            if (ControlHostElement.Child == null)
                ControlHostElement.Child = host;

            SubscribeOnDebugEvents();
        }

        public void SubscribeOnDebugEvents()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte == null)
            {
                return;
            }

            m_DE_debuggerEvents = dte.Events.DebuggerEvents;
            m_DE_debuggerEvents.OnEnterBreakMode += OnEnterBreakMode;
        }

        public void UnsubscribeFromDebugEvents()
        {
            if (m_DE_debuggerEvents != null)
            {
                m_DE_debuggerEvents.OnEnterBreakMode -= OnEnterBreakMode;
            }
        }

        private void GeometryDebuggerToolWindowUnloaded(object sender, EventArgs e)
        {
        }

        private void Variables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SADASDAS");
        }
        private void Variable_PropertyChanged(object sender, PropertyChangedEventArgs e) // срабатывает, если какой-то элемент в таблице изменил своё свойство (пример, CheckBox на m_B_IsSelected)
        {
            var variable = sender as Variable;

            if (variable != null)
            {
                // Здесь можно обработать изменения конкретных свойств
                if (e.PropertyName == nameof(variable.m_B_IsSelected)) // если изменение - CheckBox на m_B_IsSelected
                {

                    string pathOfVariable = pathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths
                    bool isSerialized = m_L_Paths[pathOfVariable].Item2; // есть ли информация о этой переменной в файле pathOfVariable
                    bool isSelected = variable.m_B_IsSelected;

                    m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(isSelected, isSerialized);

                    draw();
                }
            }
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
        private void OnAddWindowClosing(object sender, CancelEventArgs e)
        {
            m_OBOV_Variables = addMenu.GetVariables(); // получаем список всех переменных, которые пришли из окна AddVariables (их отличительное
                                                       // свойство в том, что они все isAdded

            Dictionary<string, Tuple<bool, bool>> tempPaths = new Dictionary<string, Tuple<bool, bool>>(m_L_Paths); // сохраняем старый список переменных, которые уже были до этого
            m_L_Paths.Clear(); // очищаем исходные данные

            foreach (var variable in m_OBOV_Variables) // проходимся по каждой переменной в m_OBOV_Variables
            {
                string pathOfVariable = pathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths

                if (!tempPaths.ContainsKey(pathOfVariable)) // если переменной нет в старой коллекции
                    m_L_Paths.Add(pathOfVariable, Tuple.Create(false, false)); // то создаем новую с флагами isSelected = false, isSerialized = false
                else // иначе, то есть перменная была найдена в коллекции tempPaths
                {
                    Tuple<bool, bool> properties = new Tuple<bool, bool>(tempPaths[pathOfVariable].Item1, tempPaths[pathOfVariable].Item2); // сохраняем свойство isSelected и isSerialized
                    m_L_Paths.Add(pathOfVariable, properties); // добавляем обратно в m_L_Paths
                }
            }

            dgObjects.ItemsSource = m_OBOV_Variables; // обновляем визуальную составляющую

            /*
             * Тут не надо вызывать draw, так как переменная, которая уже была добавлена (isAdded) и выбрана (isSelected), 
             * она либо уже отрисована или не отрисована (зависит от флага isSelected)
             */
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
                    colorPicker.RedSlider.Value = variable.m_C_Color.m_i_R; // красный
                    colorPicker.GreenSlider.Value = variable.m_C_Color.m_i_G; // зеленый
                    colorPicker.BlueSlider.Value = variable.m_C_Color.m_i_B; // синий

                    pickerWindow.ShowDialog(); // показываем ДИАЛОГ с нашим ColorPicker (так как это диалог, то другие элементы UI недоступны - аналогия выборки файла при его открытии в Word'e)

                    int R = (int)colorPicker.RedSlider.Value;
                    int G = (int)colorPicker.GreenSlider.Value;
                    int B = (int)colorPicker.BlueSlider.Value;

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

                        string pathOfVariable = pathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths
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
        private void draw()
        {
            /*
             * есть функция reload в geomView, она принимает вектор пар (строка - путь до файла, bool - надо ли перерисовать)
             * если у меня поменялся цвет только у одной переменной, то происходит сериализация
             * меняется файл и потом вызватется релоад со всеми false, кроме этой (у нее остается
             * true).
             * есть функция visibilities в geomView, она отвечает только за визуализацию
             * если пришел false - то не отображаем
             * если пришел true - то отображаем
            */

            List<Tuple<string, bool>> files = new List<Tuple<string, bool>>(); // лист с путями и bool - isVisible
            List<Variable> variables = new List<Variable>(); // переменные, которые нужно будет заново сериализировать

            foreach (var variable in m_OBOV_Variables)
            {
                string pathOfVariable = pathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths

                bool isSelected = m_L_Paths[pathOfVariable].Item1;
                bool isSerialized = m_L_Paths[pathOfVariable].Item2;

                if (isSerialized) // если переменная уже сериализована (то есть данные о ней записаны в файл){
                {
                    files.Add(Tuple.Create(pathOfVariable, false)); // указываем, что эту переменную НЕ надо перезагружать 
                    System.Threading.Thread.Sleep(1);
                    host.visibilityGeomView(globalPath + "\\" + pathOfVariable, isSelected); // если переменная выбрана для показа (isSelected)
                }
                else // если переменная несериализована (данных о ней нет в файле или их необходимо обновить)
                {
                    if (isSelected)
                    {
                        files.Add(Tuple.Create(pathOfVariable, true)); // указываем, что эту переменную надо перезагружать 
                        variables.Add(variable);
                    }
                    else // в данном случае переменная неIsSelected и неIsSerialized => эту переменную не надо отрисовывать и что-то вообще с ней делать
                        continue;
                }
            }

            if (variables.Count != 0)
            {
                m_DE_debuggerEvents.OnEnterBreakMode -= OnEnterBreakMode; // отписываемся от входа в дебаг мод, может отрицательно влиять на результат
                                                                  // будут пропадать элементы из таблицы
                SharedMemory sharedMemory = new SharedMemory(variables, m_DGV_debugger.GetDTE()); // сюда мы отдаем только те переменные, которые выбраны и их надо пересериализировать
                sharedMemory.CreateMessages();
                sharedMemory.WriteToMemory();
                sharedMemory.DoSerialize();
                globalPath = sharedMemory.getResult();

                m_DE_debuggerEvents.OnEnterBreakMode += OnEnterBreakMode; // подписываемся на вход в дебаг мод обратно

                foreach (var variable in variables)
                {
                    string pathOfVariable = pathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths
                    bool isSelected = m_L_Paths[pathOfVariable].Item1;

                    m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(isSelected, true);
                }

                host.reloadGeomView(files, globalPath);
            }
        }


        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction action) // срабатывает при f5, f10, f11
        {
            addMenu.BreakModDetected(); // обновляем информацию о наших переменных, которые мы отслеживаем, валидны ли они
            m_OBOV_Variables = addMenu.GetVariables(); // получаем итоговые данные с валидными переменными

            /*
             * Нам необходимо удалить все переменные из m_L_Paths, которые больше не валидны 
             * Обновить у них всех isSerialized на false
             * isSelected - оставить таким, каким было
            */

            Dictionary<string, Tuple<bool, bool>> tempPaths = new Dictionary<string, Tuple<bool, bool>>(m_L_Paths);
            m_L_Paths.Clear();

            foreach (var variable in m_OBOV_Variables)
            {
                string pathOfVariable = pathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths

                bool isSelected = tempPaths[pathOfVariable].Item1;
                Tuple<bool, bool> tuple = new Tuple<bool, bool>(isSelected, false);
                m_L_Paths.Add(pathOfVariable, tuple);
            }

            dgObjects.ItemsSource = m_OBOV_Variables; // обновляем визуальную составляющую

            draw();
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
                            string pathOfVariable = pathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths
                            bool isSerialized = m_L_Paths[pathOfVariable].Item2; // получаем информацию, сериализована ли переменная
                            variable.PropertyChanged -= Variable_PropertyChanged; // отписваемся от изменений, из-за них вызовется лишняя функция
                            variable.m_B_IsSelected = false;
                            variable.PropertyChanged += Variable_PropertyChanged; // подписываемся обратно
                            m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(false, isSerialized); // ставим, что переменная неIsSelected, isSerialized - сохраняем старый
                        }
                    }
                }

                dgObjects.Items.Refresh(); // обновляем визуальную составляющую

                draw();
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

                            string pathOfVariable = pathForFile + variable.m_S_Addres; // ключ в Dictionary m_L_Paths
                            bool isSerialized = m_L_Paths[pathOfVariable].Item2; // получаем информацию, сериализована ли переменная
                            variable.PropertyChanged -= Variable_PropertyChanged; // отписваемся от изменений, из-за них вызовется лишняя функция
                            variable.m_B_IsSelected = true;
                            variable.PropertyChanged += Variable_PropertyChanged; // подписываемся обратно
                            m_L_Paths[pathOfVariable] = new Tuple<bool, bool>(true, isSerialized); // ставим, что переменная isSelected, isSerialized - сохраняем старый
                        }
                    }
                }

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
                List<Variable> variablesOnDelete = new List<Variable>(); // создаем временный лист, хранящий переменные, которые isAdded

                foreach (var item in dgObjects.SelectedItems) // проходимся по каждому элементу, который пользователь хочет удалить
                {
                    if (item is Variable) // если этот элемент - переменная
                    {

                        Variable variable = (Variable)item;

                        variable.m_B_IsAdded = false; // то мы меняем его isAdded - false
                        variable.m_B_IsSelected = false; // isSelected - false

                        variablesOnDelete.Add(variable);
                    }
                }


                foreach (var variable in variablesOnDelete) // проходимся по всем переменным в m_OBOV_Variables (наша цель найти все переменные, которые НЕ isAdded)
                {
                    if (m_OBOV_Variables.Contains(variable))
                    {
                        string pathOfVariable = pathForFile + variable.m_S_Addres;

                        m_L_Paths.Remove(pathForFile);
                        m_OBOV_Variables.Remove(variable);
                    }
                }

                dgObjects.ItemsSource = m_OBOV_Variables; // обновляем визуальную составляющую

                delete();
            }
        }
        
        private void delete()
        {
            List<Tuple<string, bool>> files = new List<Tuple<string, bool>>();

            foreach (var variable in m_OBOV_Variables)
            {
                string pathOfVariable = pathForFile + variable.m_S_Addres;

                bool isSelected = m_L_Paths[pathOfVariable].Item1;
                bool isSerialized = m_L_Paths[pathOfVariable].Item2;

                if (isSerialized) // если переменная уже сериализована (то есть данные о ней записаны в файл){
                    files.Add(Tuple.Create(pathOfVariable, isSelected)); // указываем, что эту переменную НЕ надо перезагружать 
                else // если переменная несериализована (данных о ней нет в файле или их необходимо обновить)
                    continue;
            }

            host.reloadGeomView(files, globalPath);
        }

        private void GeometryDebuggerUnLoaded(object sender, RoutedEventArgs e)
        {
            //host.destroyOpenGLWindow(IntPtr.Zero);
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
            {
                dgObjects.SelectedItems.Add(variable);
            }

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
            {
                dgObjects.SelectedItems.Add(variable);
            }

            reorder();
        }
        private void MenuItemToBottom_Click(object sender, RoutedEventArgs e)
        {
            if (dgObjects.SelectedItems.Count == 0) // Если нет выбранных элементов
                return;

            // Сохраняем список выбранных элементов в порядке их появления в коллекции
            var selectedVariables = dgObjects.SelectedItems.OfType<Variable>()
                                        .OrderBy(v => m_OBOV_Variables.IndexOf(v))
                                        .ToList();

            if (selectedVariables.Count == 0) // Если нет выделенных элементов типа Variable
                return;

            // Удаляем выбранные элементы из коллекции
            foreach (var variable in selectedVariables)
            {
                m_OBOV_Variables.Remove(variable);
            }

            // Добавляем их в конец коллекции
            foreach (var variable in selectedVariables)
            {
                m_OBOV_Variables.Add(variable);
            }

            // Очищаем текущее выделение
            dgObjects.SelectedItems.Clear();

            // Восстанавливаем выделение
            foreach (var variable in selectedVariables)
            {
                dgObjects.SelectedItems.Add(variable);
            }

            reorder();
        }
        private void MenuItemToTop_Click(object sender, RoutedEventArgs e)
        {
            if (dgObjects.SelectedItems.Count == 0) // Если нет выбранных элементов
                return;

            // Сохраняем список выбранных элементов в порядке их появления в коллекции
            var selectedVariables = dgObjects.SelectedItems.OfType<Variable>()
                                        .OrderBy(v => m_OBOV_Variables.IndexOf(v))
                                        .ToList();

            if (selectedVariables.Count == 0) // Если нет выделенных элементов типа Variable
                return; // asd

            // Удаляем выбранные элементы из коллекции
            foreach (var variable in selectedVariables)
            {
                m_OBOV_Variables.Remove(variable);
            }

            // Добавляем их в начало коллекции
            for (int i = 0; i < selectedVariables.Count; i++)
            {
                m_OBOV_Variables.Insert(i, selectedVariables[i]);
            }

            // Очищаем текущее выделение
            dgObjects.SelectedItems.Clear();

            // Восстанавливаем выделение
            foreach (var variable in selectedVariables)
            {
                dgObjects.SelectedItems.Add(variable);
            }

            reorder(); // изменяем порядок отрисовки
        }

        private void reorder()
        {
            List<Tuple<string, bool>> files = new List<Tuple<string, bool>>();

            foreach (var variable in m_OBOV_Variables)
            {
                string pathOfVariable = pathForFile + variable.m_S_Addres;

                bool isSelected = m_L_Paths[pathOfVariable].Item1;
                bool isSerialized = m_L_Paths[pathOfVariable].Item2;

                if (isSerialized) // если переменная уже сериализована (то есть данные о ней записаны в файл){
                    files.Add(Tuple.Create(pathOfVariable, isSelected)); // указываем, что эту переменную НЕ надо перезагружать 
                else // если переменная несериализована (данных о ней нет в файле или их необходимо обновить)
                    continue;
            }

            host.reloadGeomView(files, globalPath);
        }
    }
}
