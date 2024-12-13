using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GeometryDebugger.Utils
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
                return new Utils.Color(color.R, color.G, color.B);
            }
            return new Utils.Color(0, 0, 0);
        }
    }
}
