using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using SharpGL;
using System.Windows.Interop;

namespace LearningWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RedSlider.ValueChanged += RGBSlider_ValueChanged;
            GreenSlider.ValueChanged += RGBSlider_ValueChanged;
            BlueSlider.ValueChanged += RGBSlider_ValueChanged;

            RedValue.TextChanged += RGBTextBox_TextChanged;
            GreenValue.TextChanged += RGBTextBox_TextChanged;
            BlueValue.TextChanged += RGBTextBox_TextChanged;

            HexValue.TextChanged += HexValue_TextChanged;

            UpdateColorDisplay();
        }

        private void RGBSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (RedSlider == null || GreenSlider == null || BlueSlider == null ||
                RedValue == null || GreenValue == null || BlueValue == null)
                return; // Ждём полной инициализации элементов

            // Обновляем значения TextBox'ов при изменении слайдеров
            RedValue.Text = ((int)RedSlider.Value).ToString();
            GreenValue.Text = ((int)GreenSlider.Value).ToString();
            BlueValue.Text = ((int)BlueSlider.Value).ToString();

            UpdateColorDisplay();
        }

        private void RGBTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RedSlider == null || GreenSlider == null || BlueSlider == null ||
                RedValue == null || GreenValue == null || BlueValue == null)
                return; // Ждём полной инициализации элементов
            // Проверяем, что ввод является числом, и обновляем слайдеры
            if (int.TryParse(RedValue.Text, out int red) && red >= 0 && red <= 255)
                RedSlider.Value = red;
            if (int.TryParse(GreenValue.Text, out int green) && green >= 0 && green <= 255)
                GreenSlider.Value = green;
            if (int.TryParse(BlueValue.Text, out int blue) && blue >= 0 && blue <= 255)
                BlueSlider.Value = blue;

            UpdateColorDisplay();
        }

        private void HexValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Обновляем цвет по HEX-коду
            try
            {
                if (HexValue.Text.Length == 7 && HexValue.Text[0] == '#')
                {
                    var color = (Color)ColorConverter.ConvertFromString(HexValue.Text);
                    RedSlider.Value = color.R;
                    GreenSlider.Value = color.G;
                    BlueSlider.Value = color.B;
                    UpdateColorDisplay();
                }
            }
            catch
            {
                // Игнорируем неправильные значения
            }
        }

        private void UpdateColorDisplay()
        {
            if (RedSlider == null || GreenSlider == null || BlueSlider == null ||
                RedValue == null || GreenValue == null || BlueValue == null)
                return; // Ждём полной инициализации элементов

            // Получаем значения RGB слайдеров
            byte red = (byte)RedSlider.Value;
            byte green = (byte)GreenSlider.Value;
            byte blue = (byte)BlueSlider.Value;

            // Обновляем цвет отображения и HEX-код
            var color = Color.FromRgb(red, green, blue);
            ColorDisplay.Background = new SolidColorBrush(color);
            //HexValue.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

    }


}