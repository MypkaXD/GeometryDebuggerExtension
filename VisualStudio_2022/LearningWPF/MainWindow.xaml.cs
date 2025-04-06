using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LearningWPF
{

    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color)
            {
                return new SolidColorBrush(System.Windows.Media.Color.FromRgb(
                    (byte)(((Color)value).m_i_R),
                    (byte)(((Color)value).m_i_G),
                    (byte)(((Color)value).m_i_B)
                ));
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush)
            {
                var color = ((SolidColorBrush)value).Color;
                return new Color(color.R, color.G, color.B);
            }
            return new Color(0, 0, 0);
        }
    }

    public struct Color
    {
        public int m_i_R { get; set; }
        public int m_i_G { get; set; }
        public int m_i_B { get; set; }

        public Color(int r, int g, int b)
        {
            m_i_R = r;
            m_i_G = g;
            m_i_B = b;
        }

        public static bool operator !=(Color colorFirst, Color colorSecond)
        {
            return !(colorFirst == colorSecond);
        }

        public static bool operator ==(Color colorFirst, Color colorSecond)
        {
            return (colorFirst.m_i_R == colorSecond.m_i_R &&
                colorFirst.m_i_G == colorSecond.m_i_G &&
                colorFirst.m_i_B == colorSecond.m_i_B);
        }
    }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Color _color;

        public Color m_C_Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged(nameof(m_C_Color));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // Required for binding to work

            m_C_Color = new Color(20, 244, 244);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ColorDisplay_Click(object sender, RoutedEventArgs e)
        {
            // Example: Change color on click
            m_C_Color = new Color(
                (m_C_Color.m_i_R + 50) % 255,
                (m_C_Color.m_i_G + 50) % 255,
                (m_C_Color.m_i_B + 50) % 255
            );
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_C_Color = new Color(1, 1, 1);
        }
    }
}