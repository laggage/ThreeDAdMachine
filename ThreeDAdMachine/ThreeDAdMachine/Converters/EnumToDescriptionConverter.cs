using System;
using System.Globalization;
using System.Windows.Data;
using ThreeDAdMachine.Extensions;

namespace ThreeDAdMachine.Converters
{
    class EnumToDescriptionConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Enum) value)?.GetDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
