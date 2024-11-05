using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace VSIXProjectHelloWorld
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Utils.Color color)
            {
                return new SolidColorBrush(System.Windows.Media.Color.FromRgb(
                    (byte)color.m_i_R,
                    (byte)color.m_i_G,
                    (byte)color.m_i_B));
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                var color = brush.Color;
                return new Utils.Color(color.R, color.G, color.B);
            }
            return new Utils.Color(0, 0, 0);
        }
    }
}
