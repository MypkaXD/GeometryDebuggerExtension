using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyExtension
{
    public partial class MyToolWindowControl : UserControl
    {
        public MyToolWindowControl()
        {
            InitializeComponent();

            ObservableCollection<MyObject> objects = new ObservableCollection<MyObject>();
            objects.Add(new MyObject() { m_C_Color = Color.FromRgb(1, 10, 1), m_B_IsSelected = true, m_S_Name = "Name1" });
            objects.Add(new MyObject() { m_C_Color = Color.FromRgb(100, 10, 10), m_B_IsSelected = false, m_S_Name = "Name2" });
            objects.Add(new MyObject() { m_C_Color = Color.FromRgb(10, 100, 10), m_B_IsSelected = false, m_S_Name = "Name3" });
            objects.Add(new MyObject() { m_C_Color = Color.FromRgb(10, 10, 100), m_B_IsSelected = true, m_S_Name = "Name4" });

            dgObjects.ItemsSource = objects;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("MyExtension", "Button clicked");
        }
    }

    public class MyObject : INotifyPropertyChanged
    {
        private string name;
        private bool isSelected;
        private Color color;

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