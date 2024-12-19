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
using System.ComponentModel;
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
        public RGB m_RGB = new RGB(0, 0, 0);
        private Color m_C_SelectedColor = Color.FromRgb(0, 0, 0);

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Point position = new Point(ColorCanvas.Width / 2, ColorCanvas.Height / 2);
            //UpdateHSLFromPosition(position);
            UpdateColorPreview();
            UpdateBrightnessRect();
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //UpdateHSLFromPosition(GetSelectorPosition());
            UpdateColorFields();
            UpdateColorPreview();
        }

        private void UpdateColorFromRGB(byte r, byte g, byte b)
        {
            m_C_SelectedColor = Color.FromRgb(r, g, b);
        }

        private Point GetSelectorPosition()
        {
            return new Point(
                Canvas.GetLeft(SelectorEllipse) + SelectorEllipse.Width / 2,
                Canvas.GetTop(SelectorEllipse) + SelectorEllipse.Height / 2);
        }

        private static Color ColorFromHue(double hue, double saturation, double lightness)
        {
            double h = hue * 360;
            double s = saturation;
            double l = lightness;

            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = l - c / 2;

            double r = 0, g = 0, b = 0;

            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return Color.FromRgb(
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255));
        }

        private void UpdateColorFields()
        {
            if (RedValue != null && GreenValue != null && BlueValue != null)
            {
                RedValue.Text = m_C_SelectedColor.R.ToString();
                GreenValue.Text = m_C_SelectedColor.G.ToString();
                BlueValue.Text = m_C_SelectedColor.B.ToString();
            }
        }

        private void UpdateSelectorPosition()
        {
            double hue = GetHueFromColor(m_C_SelectedColor);
            double saturation = GetSaturationFromColor(m_C_SelectedColor);

            Canvas.SetLeft(SelectorEllipse, hue * ColorCanvas.ActualWidth);
            Canvas.SetTop(SelectorEllipse, ColorCanvas.ActualHeight * (1 - saturation));
        }

        private void UpdateColorPreview()
        {
            if (ColorPreview != null)
            {
                ColorPreview.Fill = new SolidColorBrush(m_C_SelectedColor);
            }
        }

        private void UpdateBrightnessRect()
        {
            if (BrightnessRect == null) return;

            Point position = GetSelectorPosition();
            double hue = position.X / ColorCanvas.ActualWidth;
            double saturation = 1 - position.Y / ColorCanvas.ActualHeight;

            Color midColor = ColorFromHue(hue, saturation, 0.5);

            BrightnessRect.Fill = new LinearGradientBrush(
                new GradientStopCollection
                {
                    new GradientStop(Colors.Black, 0),
                    new GradientStop(midColor, 0.5),
                    new GradientStop(Colors.White, 1)
                });
        }

        private double GetHueFromColor(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            if (delta == 0) return 0;
            if (max == r) return ((g - b) / delta) % 6 / 6;
            if (max == g) return ((b - r) / delta + 2) / 6;
            return ((r - g) / delta + 4) / 6;
        }

        private double GetSaturationFromColor(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            if (max == 0) return 0;
            return (max - min) / max;
        }




        private void ColorCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(ColorCanvas);
                position.X = Clamp(position.X, 0, ColorCanvas.ActualWidth - 1);
                position.Y = Clamp(position.Y, 0, ColorCanvas.ActualHeight - 1);

                Canvas.SetLeft(SelectorEllipse, position.X - SelectorEllipse.Width / 2);
                Canvas.SetTop(SelectorEllipse, position.Y - SelectorEllipse.Height / 2);

                UpdateHSL();
            }
        }
        private void UpdateHSL()
        {
            double x = Canvas.GetLeft(SelectorEllipse) + SelectorEllipse.Width / 2;
            double y = Canvas.GetTop(SelectorEllipse) + SelectorEllipse.Height / 2;

            double hue = x / ColorCanvas.ActualWidth;
            double saturation = 1 - y / ColorCanvas.ActualHeight;
            double lightness = BrightnessSlider.Value;

            HueValue.Text = (hue * 360).ToString();
            SaturationValue.Text = (saturation * 100).ToString();
            LightnessValue.Text = (lightness * 100).ToString();
        }

        private void SyncHSLValueWithRectangles(double hue, double saturation, double lightness)
        {
            Canvas.SetLeft(SelectorEllipse, hue / 360 * ColorCanvas.ActualWidth);
            Canvas.SetTop(SelectorEllipse, ColorCanvas.ActualHeight - saturation / 100 * ColorCanvas.ActualHeight);

            BrightnessSlider.Value = lightness / 100;
        }

        private void HSL_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SaturationValue != null && LightnessValue != null && HueValue != null)
            {
                double hue = 0;
                double saturation = 0;
                double lightness = 0;

                if (double.TryParse(HueValue.Text, out hue) &&
                    double.TryParse(SaturationValue.Text, out saturation) &&
                    double.TryParse(LightnessValue.Text, out lightness))
                {
                    if (hue > 360 || hue < 0)
                    {
                        hue = (hue % 360 + 360) % 360;
                        HueValue.Text = hue.ToString();
                    }
                    else if (saturation < 0 || saturation > 100)
                    {
                        saturation = saturation < 0 ? 0 : 100;
                        SaturationValue.Text = saturation.ToString();
                    }
                    else if (lightness < 0 || lightness > 100)
                    {
                        lightness = lightness < 0 ? 0 : 100;
                        LightnessValue.Text = lightness.ToString();
                    }
                    else
                    {
                        SyncHSLValueWithRectangles(hue, saturation, lightness);
                    }
                }
            }
        }

        private void RGB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RedValue != null && GreenValue != null && BlueValue != null)
            {
                byte r = 0;
                byte g = 0;
                byte b = 0;

                if (byte.TryParse(RedValue.Text, out r) &&
                byte.TryParse(GreenValue.Text, out g) &&
                byte.TryParse(BlueValue.Text, out b))
                {
                    m_RGB.m_Byte_R = r;
                    m_RGB.m_Byte_G = g;
                    m_RGB.m_Byte_B = b;
                }
                else
                {
                    if (sender is TextBox)
                    {
                        TextBox textBox = (TextBox)sender;
                        if (textBox == RedValue)
                            textBox.Text = m_RGB.m_Byte_R.ToString();
                        else if (textBox == GreenValue)
                            textBox.Text = m_RGB.m_Byte_G.ToString();
                        else if (textBox == BlueValue)
                            textBox.Text = m_RGB.m_Byte_B.ToString();
                    }
                }
            }
        }
    }

    public class HSL
    {
        public int m_Hue { get; set; }
        public float m_S { get; set; }
        public float m_L { get; set; }


        public HSL(int hue = 0, float s = 0, float l = 0)
        {
            m_Hue = hue;
            m_S = s;
            m_L = l;
        }
    }

    public class RGB
    {
        public byte m_Byte_R { get; set; }
        public byte m_Byte_G { get; set; }
        public byte m_Byte_B { get; set; }


        public RGB(byte r = 0, byte g = 0, byte b = 0)
        {
            m_Byte_R = r;
            m_Byte_G = g;
            m_Byte_B = b;
        }
    }
}
