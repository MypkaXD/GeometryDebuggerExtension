using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GeometryDebugger.UI
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public RGB m_RGB = new RGB(0, 0, 0);
        public HSL m_HSL = new HSL(0, 0, 0);

        public static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public ColorPicker()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (isInit())
            {
                UnSubscribeFromEvents();
                SyncHSLValueWithRectangles(m_HSL.m_Hue, m_HSL.m_S, m_HSL.m_L);
                updateColorPreview();
                updateLightnessRect();
                updateColorRect();
                SubscribeOnEvents();
            }
        }

        private void LightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isInit())
            {
                UnSubscribeFromEvents();
                UpdateHSLFromCoords();
                ConvertHSLToRGB();
                updateColorPreview();
                updateColorRect();
                updateLightnessRect();
                SubscribeOnEvents();
            }
        }


        private void updateColorPreview()
        {
            if (isInit())
            {
                ColorPreview.Fill = new SolidColorBrush(Color.FromRgb(m_RGB.m_Byte_R, m_RGB.m_Byte_G, m_RGB.m_Byte_B));
            }
        }

        private void updateLightnessRect()
        {
            if (isInit())
            {
                var gradientBrush = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1)
                };

                HSL HSLMiddleColor = GetHSLFromRGB(m_RGB.m_Byte_R, m_RGB.m_Byte_G, m_RGB.m_Byte_B);
                HSLMiddleColor.m_L = 50;
                HSLMiddleColor.m_Hue = (HSLMiddleColor.m_Hue % 360 + 360) % 360;
                RGB RGBMiddleColor = GetRGBFromHSL(HSLMiddleColor);

                gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 255, 255), 0));
                gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(RGBMiddleColor.m_Byte_R, RGBMiddleColor.m_Byte_G, RGBMiddleColor.m_Byte_B), 0.5));
                gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 0, 0), 1));

                LightnessRect.Fill = gradientBrush;
            }
        }

        private void UnSubscribeFromEvents()
        {
            ColorCanvas.MouseMove -= ColorCanvas_MouseMove;
            ColorCanvas.MouseDown -= ColorCanvas_MouseMove;

            RedValue.TextChanged -= RGB_TextChanged;
            GreenValue.TextChanged -= RGB_TextChanged;
            BlueValue.TextChanged -= RGB_TextChanged;

            HueValue.TextChanged -= HSL_TextChanged;
            SaturationValue.TextChanged -= HSL_TextChanged;
            LightnessValue.TextChanged -= HSL_TextChanged;

            LightnessSlider.ValueChanged -= LightnessSlider_ValueChanged;
        }

        private void SubscribeOnEvents()
        {
            ColorCanvas.MouseMove += ColorCanvas_MouseMove;
            ColorCanvas.MouseDown += ColorCanvas_MouseMove;

            RedValue.TextChanged += RGB_TextChanged;
            GreenValue.TextChanged += RGB_TextChanged;
            BlueValue.TextChanged += RGB_TextChanged;

            HueValue.TextChanged += HSL_TextChanged;
            SaturationValue.TextChanged += HSL_TextChanged;
            LightnessValue.TextChanged += HSL_TextChanged;

            LightnessSlider.ValueChanged += LightnessSlider_ValueChanged;
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

                UnSubscribeFromEvents();

                updateLightnessRect();
                UpdateHSLFromCoords();
                ConvertHSLToRGB();
                updateColorPreview();

                SubscribeOnEvents();
            }
        }

        private RGB GetRGBFromHSL(HSL hsl)
        {
            double hue = hsl.m_Hue;
            double saturation = hsl.m_S / 100;
            double lightness = hsl.m_L / 100;

            double c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            double x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            double m = lightness - c / 2;

            double r = 0, g = 0, b = 0;

            if (hue < 60) { r = c; g = x; b = 0; }
            else if (hue < 120) { r = x; g = c; b = 0; }
            else if (hue < 180) { r = 0; g = c; b = x; }
            else if (hue < 240) { r = 0; g = x; b = c; }
            else if (hue < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return new RGB((byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b + m) * 255));
        }

        private void ConvertHSLToRGB()
        {
            double hue = Convert.ToDouble(HueValue.Text);
            double saturation = Convert.ToDouble(SaturationValue.Text) / 100;
            double lightness = Convert.ToDouble(LightnessValue.Text) / 100;

            double c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            double x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            double m = lightness - c / 2;

            double r = 0, g = 0, b = 0;

            if (hue < 60) { r = c; g = x; b = 0; }
            else if (hue < 120) { r = x; g = c; b = 0; }
            else if (hue < 180) { r = 0; g = c; b = x; }
            else if (hue < 240) { r = 0; g = x; b = c; }
            else if (hue < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            m_RGB.m_Byte_R = (byte)((r + m) * 255);
            m_RGB.m_Byte_G = (byte)((g + m) * 255);
            m_RGB.m_Byte_B = (byte)((b + m) * 255);

            RedValue.Text = (m_RGB.m_Byte_R).ToString();
            GreenValue.Text = (m_RGB.m_Byte_G).ToString();
            BlueValue.Text = (m_RGB.m_Byte_B).ToString();
        }
        private void UpdateHSLFromCoords()
        {
            double x = Canvas.GetLeft(SelectorEllipse) + SelectorEllipse.Width / 2;
            double y = Canvas.GetTop(SelectorEllipse) + SelectorEllipse.Height / 2;

            double hue = x / ColorCanvas.ActualWidth;
            double saturation = 1 - y / ColorCanvas.ActualHeight;
            double lightness = LightnessSlider.Value;

            m_HSL.m_Hue = hue * 360;
            m_HSL.m_S = saturation * 100;
            m_HSL.m_L = lightness * 100;

            HueValue.Text = Math.Round(hue * 360, 2).ToString();
            SaturationValue.Text = Math.Round(saturation * 100, 2).ToString();
            LightnessValue.Text = Math.Round(lightness * 100, 2).ToString();
        }

        private void SyncHSLValueWithRectangles(double hue, double saturation, double lightness)
        {
            Canvas.SetLeft(SelectorEllipse, hue / 360 * ColorCanvas.ActualWidth);
            Canvas.SetTop(SelectorEllipse, ColorCanvas.ActualHeight - saturation / 100 * ColorCanvas.ActualHeight);

            LightnessSlider.Value = lightness / 100;
        }

        private void HSL_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isInit())
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
                        HueValue.Text = Math.Round(hue, 2).ToString();
                    }
                    else if (saturation < 0 || saturation > 100)
                    {
                        saturation = saturation < 0 ? 0 : 100;
                        SaturationValue.Text = Math.Round(saturation, 2).ToString();
                    }
                    else if (lightness < 0 || lightness > 100)
                    {
                        lightness = lightness < 0 ? 0 : 100;
                        LightnessValue.Text = Math.Round(lightness, 2).ToString();
                    }
                    else
                    {
                        UnSubscribeFromEvents();

                        m_HSL.m_Hue = hue;
                        m_HSL.m_S = saturation;
                        m_HSL.m_L = lightness;

                        SyncHSLValueWithRectangles(hue, saturation, lightness);
                        ConvertHSLToRGB();
                        updateColorPreview();

                        updateLightnessRect();
                        updateColorRect();

                        SubscribeOnEvents();
                    }
                }
            }
        }

        private void updateColorRect()
        {
            var gradientBrush = new LinearGradientBrush
            {
                EndPoint = new Point(1, 0)
            };

            var gradientBrushForGray = new LinearGradientBrush
            {
                EndPoint = new Point(0, 1)
            };

            HSL gray = GetHSLFromRGB(Colors.Gray.R, Colors.Gray.G, Colors.Gray.B);
            HSL red = GetHSLFromRGB(Colors.Red.R, Colors.Red.G, Colors.Red.B);
            HSL yellow = GetHSLFromRGB(Colors.Yellow.R, Colors.Yellow.G, Colors.Yellow.B);
            HSL lime = GetHSLFromRGB(Colors.Lime.R, Colors.Lime.G, Colors.Lime.B);
            HSL cyan = GetHSLFromRGB(Colors.Cyan.R, Colors.Cyan.G, Colors.Cyan.B);
            HSL blue = GetHSLFromRGB(Colors.Blue.R, Colors.Blue.G, Colors.Blue.B);
            HSL magenta = GetHSLFromRGB(Colors.Magenta.R, Colors.Magenta.G, Colors.Magenta.B);

            RGB newGray = GetRGBFromHSL(new HSL((gray.m_Hue % 360 + 360) % 360, gray.m_S, m_HSL.m_L));
            RGB newRed = GetRGBFromHSL(new HSL((red.m_Hue % 360 + 360) % 360, red.m_S, m_HSL.m_L));
            RGB newYellow = GetRGBFromHSL(new HSL((yellow.m_Hue % 360 + 360) % 360, yellow.m_S, m_HSL.m_L));
            RGB newLime = GetRGBFromHSL(new HSL((lime.m_Hue % 360 + 360) % 360, lime.m_S, m_HSL.m_L));
            RGB newCyan = GetRGBFromHSL(new HSL((cyan.m_Hue % 360 + 360) % 360, cyan.m_S, m_HSL.m_L));
            RGB newBlue = GetRGBFromHSL(new HSL((blue.m_Hue % 360 + 360) % 360, blue.m_S, m_HSL.m_L));
            RGB newMagneta = GetRGBFromHSL(new HSL((magenta.m_Hue % 360 + 360) % 360, magenta.m_S, m_HSL.m_L));

            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(newRed.m_Byte_R, newRed.m_Byte_G, newRed.m_Byte_B), 0));
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(newYellow.m_Byte_R, newYellow.m_Byte_G, newYellow.m_Byte_B), 0.17));
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(newLime.m_Byte_R, newLime.m_Byte_G, newLime.m_Byte_B), 0.33));
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(newCyan.m_Byte_R, newCyan.m_Byte_G, newCyan.m_Byte_B), 0.5));
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(newBlue.m_Byte_R, newBlue.m_Byte_G, newBlue.m_Byte_B), 0.67));
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(newMagneta.m_Byte_R, newMagneta.m_Byte_G, newMagneta.m_Byte_B), 0.83));
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(newRed.m_Byte_R, newRed.m_Byte_G, newRed.m_Byte_B), 1));

            gradientBrushForGray.GradientStops.Add(new GradientStop(Color.FromArgb(0, 255, 255, 255), 0));
            gradientBrushForGray.GradientStops.Add(new GradientStop(Color.FromRgb(newGray.m_Byte_R, newGray.m_Byte_G, newGray.m_Byte_B), 1));

            if (ColorRect != null)
                ColorRect.Fill = gradientBrush;
            if (RectangleGray != null)
                RectangleGray.Fill = gradientBrushForGray;
        }

        private void RGB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isInit())
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

                    UnSubscribeFromEvents();

                    ConverRGBToHSL();
                    SyncHSLValueWithRectangles(m_HSL.m_Hue, m_HSL.m_S, m_HSL.m_L);
                    updateColorPreview();
                    updateLightnessRect();
                    updateColorRect();

                    SubscribeOnEvents();
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

        private HSL GetHSLFromRGB(byte red, byte green, byte blue)
        {
            double r = red / 255.0;
            double g = green / 255.0;
            double b = blue / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double hue = 0;
            double saturation = 0;
            double lightness = (max + min) / 2;

            if (delta == 0)
                saturation = 0;
            else
                saturation = delta / (1 - Math.Abs(2 * lightness - 1));

            if (delta == 0)
                hue = 0;
            else if (max == r)
                hue = 60 * (((g - b) / delta) % 6);
            else if (max == g)
                hue = 60 * ((b - r) / delta + 2);
            else if (max == b)
                hue = 60 * ((r - g) / delta + 4);

            return new HSL(hue, saturation * 100, lightness * 100);
        }

        private void ConverRGBToHSL()
        {
            double r = m_RGB.m_Byte_R / 255.0;
            double g = m_RGB.m_Byte_G / 255.0;
            double b = m_RGB.m_Byte_B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double hue = 0;
            double saturation = 0;
            double lightness = (max + min) / 2;

            if (delta == 0)
                saturation = 0;
            else
                saturation = delta / (1 - Math.Abs(2 * lightness - 1));

            if (delta == 0)
                hue = 0;
            else if (max == r)
                hue = 60 * (((g - b) / delta) % 6);
            else if (max == g)
                hue = 60 * ((b - r) / delta + 2);
            else if (max == b)
                hue = 60 * ((r - g) / delta + 4);
            else
                hue = 0;

            m_HSL.m_Hue = (hue % 360 + 360) % 360;
            m_HSL.m_S = Math.Round(saturation * 100, 2);
            m_HSL.m_L = Math.Round(lightness * 100, 2);

            HueValue.Text = Math.Round(m_HSL.m_Hue, 2).ToString();
            SaturationValue.Text = Math.Round(m_HSL.m_S, 2).ToString();
            LightnessValue.Text = Math.Round(m_HSL.m_L, 2).ToString();
        }

        private bool isInit()
        {
            return ColorCanvas != null && SelectorEllipse != null &&
                   RedValue != null && GreenValue != null && BlueValue != null &&
                   HueValue != null && SaturationValue != null && LightnessValue != null &&
                   ColorPreview != null && LightnessRect != null;
        }
    }

    public class HSL
    {
        public double m_Hue { get; set; }
        public double m_S { get; set; }
        public double m_L { get; set; }


        public HSL(double hue = 0, double s = 0, double l = 0)
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
