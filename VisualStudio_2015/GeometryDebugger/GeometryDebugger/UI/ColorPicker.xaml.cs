using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeometryDebugger.UI
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public ColorPicker()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (RedSlider != null)
                RedSlider.ValueChanged += RGBSlider_ValueChanged;
            if (GreenSlider != null)
                GreenSlider.ValueChanged += RGBSlider_ValueChanged;
            if (BlueSlider != null)
                BlueSlider.ValueChanged += RGBSlider_ValueChanged;

            if (RedValue != null)
                RedValue.TextChanged += RGBTextBox_TextChanged;
            if (GreenValue != null)
                GreenValue.TextChanged += RGBTextBox_TextChanged;
            if (BlueValue != null)
                BlueValue.TextChanged += RGBTextBox_TextChanged;

            UpdateColorDisplay();
        }

        private void RGBSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (RedValue != null)
                RedValue.Text = ((int)RedSlider.Value).ToString();
            if (GreenValue != null)
                GreenValue.Text = ((int)GreenSlider.Value).ToString();
            if (BlueValue != null)
                BlueValue.Text = ((int)BlueSlider.Value).ToString();

            UpdateColorDisplay();
        }

        private void RGBTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int red = 0;
            if (RedValue != null && int.TryParse(RedValue.Text, out red) && red >= 0 && red <= 255)
                RedSlider.Value = red;
            else if (RedSlider != null)
                RedSlider.Value = 0;

            int green = 0;
            if (GreenValue != null && int.TryParse(GreenValue.Text, out green) && green >= 0 && green <= 255)
                GreenSlider.Value = green;
            else if (GreenSlider != null)
                GreenSlider.Value = 0;

            int blue = 0;
            if (BlueValue != null && int.TryParse(BlueValue.Text, out blue) && blue >= 0 && blue <= 255)
                BlueSlider.Value = blue;
            else if (BlueSlider != null)
                BlueSlider.Value = 0;

            UpdateColorDisplay();
        }

        private void UpdateColorDisplay()
        {
            // Проверяем, что слайдеры и ColorDisplay инициализированы
            if (RedSlider != null && GreenSlider != null && BlueSlider != null && ColorDisplay != null)
            {
                // Получаем значения RGB слайдеров
                byte red = (byte)RedSlider.Value;
                byte green = (byte)GreenSlider.Value;
                byte blue = (byte)BlueSlider.Value;

                var color = Color.FromRgb(red, green, blue);
                ColorDisplay.Background = new SolidColorBrush(color);
            }
        }
    }
}
