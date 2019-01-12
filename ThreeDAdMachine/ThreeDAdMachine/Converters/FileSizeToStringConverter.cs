namespace ThreeDAdMachine.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class FileSizeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long l = (long)value;
            string result = "";
            if (l < 1024)
                result = l.ToString("f2") + "B";
            else if (l >= 1024 && l < 1024 * 1024)
                result = (l / 1024.0).ToString("f2") + "KB";
            else if (l >= 1024 * 1024 && l < 1024 * 1024 * 1024)
                result = (l / (1024.0 * 1024.0)).ToString("f2") + "MB";
            else if (l >= 1024 * 1024 * 1024)
                result = (l / (1024 * 1024 * 1024)).ToString("f2") + "GB";
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
