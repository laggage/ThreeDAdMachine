using System;
using System.Globalization;
using System.Windows.Data;
using MediaProcess.Model;

namespace ThreeDAdMachine.Converters
{
    class MediaTypeConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var e in values)
                if (e != null)
                    return e;
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new object[]
                {
                    null,
                    null
                };
            if (value.GetType() == typeof(ImageModel))
                return new[]
                {
                    value,
                    null
                };
            if (value.GetType() == typeof(VideoModel))
                return new[]
                {
                    null,
                    value
                };
            return new object[]
            {
                null,
                null
            };
        }
    }
}
