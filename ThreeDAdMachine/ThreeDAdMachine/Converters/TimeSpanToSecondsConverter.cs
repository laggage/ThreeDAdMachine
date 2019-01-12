using System;
using System.Globalization;
using System.Windows.Data;

namespace ThreeDAdMachine.Converters
{
    public class TimeSpanToSecondsConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (value.GetType() != typeof(TimeSpan))
                throw new ArgumentException("value must be the typeof TimeSpan");
            TimeSpan t = (TimeSpan)value;
            return t.TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double totalSeconds = (double)value;
            return TimeSpan.FromSeconds(totalSeconds);
        }
    }
}
