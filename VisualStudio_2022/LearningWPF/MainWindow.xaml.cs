using System.Collections.ObjectModel;
using System.Windows;

namespace LearningWPF
{
    public partial class MainWindow : Window
    {
        public class VariableItem
        {
            public bool IsVisible { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Source { get; set; }
            public string Address { get; set; }
            public ObservableCollection<VariableItem> Children { get; set; }

            public VariableItem(bool isVisible, string name, string type, string source, string address)
            {
                IsVisible = isVisible;
                Name = name;
                Type = type;
                Source = source;
                Address = address;
                Children = new ObservableCollection<VariableItem>();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            var variables = new ObservableCollection<VariableItem>();

            // Создаем родительские элементы
            var var1 = new VariableItem(true, "MainVariable1", "struct", "Internal", "0x001");
            var var2 = new VariableItem(true, "MainVariable2", "class", "External", "0x002");

            // Добавляем дочерние элементы
            var1.Children.Add(new VariableItem(true, "Child1", "int", "Internal", "0x001A"));
            var1.Children.Add(new VariableItem(false, "Child2", "float", "Internal", "0x001B"));

            var2.Children.Add(new VariableItem(true, "SubItem1", "string", "External", "0x002A"));
            var2.Children[0].Children.Add(new VariableItem(true, "NestedItem", "char", "External", "0x002AA"));

            variables.Add(var1);
            variables.Add(var2);

            dg.ItemsSource = variables;
        }
    }
}