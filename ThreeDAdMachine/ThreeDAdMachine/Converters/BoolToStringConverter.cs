using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ThreeDAdMachine.Converters
{
    class BoolToStringConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (value.GetType() != typeof(bool)) throw new ArgumentOutOfRangeException(nameof(value));
            if (parameter.GetType() != typeof(string)) throw new ArgumentOutOfRangeException(nameof(parameter));
            string[] paramStrings = ((string) parameter).Split('|', '/');
            if (paramStrings.Length != 2) throw new ArgumentOutOfRangeException(nameof(parameter));
            return (bool) value ? paramStrings[0] : paramStrings[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
