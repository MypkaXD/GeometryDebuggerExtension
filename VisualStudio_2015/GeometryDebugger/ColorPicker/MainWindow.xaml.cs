using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Color m_C_SelectedColor = Colors.SkyBlue;

        public static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ColorCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(ColorCanvas);

                position.X = Clamp(position.X, 0, ColorCanvas.ActualWidth - 1);
                position.Y = Clamp(position.Y, 0, ColorCanvas.ActualHeight - 1);

                // Перемещаем кружок
                Canvas.SetLeft(SelectorEllipse, position.X - SelectorEllipse.Width / 2);
                Canvas.SetTop(SelectorEllipse, position.Y - SelectorEllipse.Height / 2);

                // Вычисляем цвет на основе позиции
                var hue = position.X / ColorCanvas.ActualWidth;
                var saturation = 1 - position.Y / ColorCanvas.ActualHeight;

                m_C_SelectedColor = ColorFromHue(hue, saturation, BrightnessSlider.Value);

                UpdateColorPreview();
                UpdateBrightnessRect();
                //MessageBox.Show(m_C_SelectedColor.R + " " + m_C_SelectedColor.G +" " + m_C_SelectedColor.B);
            }
        }

        private static Color ColorFromHue(double hue, double saturation, double lightness)
        {
            var h = hue * 360; // Преобразуем hue в градусы
            var s = saturation; // saturation в %
            var l = lightness; // lightness в %

            // Вычисляем промежуточные значения для RGB
            var c = (1 - Math.Abs(2 * l - 1)) * s;
            var x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            var m = l - c / 2;

            double r = 0, g = 0, b = 0;

            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            // Применяем яркость
            r = (r + m) * 255;
            g = (g + m) * 255;
            b = (b + m) * 255;

            // Преобразуем в формат RGB
            return Color.FromRgb(
                (byte)r,
                (byte)g,
                (byte)b);
        }


        private void UpdateColorPreview()
        {
            if (ColorPreview != null)
                ColorPreview.Fill = new SolidColorBrush(m_C_SelectedColor);
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var position = new Point(Canvas.GetLeft(SelectorEllipse) + SelectorEllipse.Width / 2,
                                     Canvas.GetTop(SelectorEllipse) + SelectorEllipse.Height / 2);

            var hue = position.X / ColorCanvas.ActualWidth;
            var saturation = 1 - position.Y / ColorCanvas.ActualHeight;

            m_C_SelectedColor = ColorFromHue(hue, saturation, BrightnessSlider.Value);

            UpdateColorPreview();
        }

        private void UpdateBrightnessRect()
        {
            Color tempColor = new Color();

            var position = new Point(Canvas.GetLeft(SelectorEllipse) + SelectorEllipse.Width / 2,
                                     Canvas.GetTop(SelectorEllipse) + SelectorEllipse.Height / 2);
            // Вычисляем цвет на основе позиции
            var hue = position.X / ColorCanvas.ActualWidth;
            var saturation = 1 - position.Y / ColorCanvas.ActualHeight;

            tempColor = ColorFromHue(hue, saturation, 0.5);

            GradientStop endStop = new GradientStop(Colors.White, 0); // Белый цвет на 0 (начало градиента)
            GradientStop middleStop = new GradientStop(tempColor, 0.5); // Средний цвет на 0.5
            GradientStop startStop = new GradientStop(Colors.Black, 1); // Черный цвет на 1 (конец градиента)

            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.GradientStops.Add(startStop); // Черный в конце градиента
            gradientBrush.GradientStops.Add(middleStop); // Средний цвет в середине
            gradientBrush.GradientStops.Add(endStop); // Белый в начале градиента

            if (BrightnessRect != null)
                BrightnessRect.Fill = gradientBrush;
        }
    }
}
