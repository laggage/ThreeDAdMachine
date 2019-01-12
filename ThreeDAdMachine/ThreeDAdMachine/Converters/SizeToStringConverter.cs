namespace ThreeDAdMachine.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(Size),typeof(string))]
    public class SizeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Size v = (Size) (value ?? default(Size));
            return v.Width + " x " + v.Height;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            if (string.IsNullOrEmpty(s))
                return null;
            string[] t = s.Split('x');
            double width = 0, height = 0;
            if (t.Length == 2)
            {
                double.TryParse(t[0], out width);
                double.TryParse(t[1], out height);
            }

            return new Size(width, height);
        }
    }
}
