using System;
using System.Globalization;
using System.Windows.Data;

namespace ThreeDAdMachine.Converters
{
    class IntToTextConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return default(int);
            if (value.GetType() != typeof(string)) throw new ArgumentException("无法转换除string外的其他类型");
            int.TryParse((string) value, out int d);
            return d;
        }
    }
}
