using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ThreeDAdMachine.Converters
{
    public class StringAppendConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value.ToString();
            string txtToAppend = (string) parameter;
            s += " "+txtToAppend;
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string) value;
            if (s == null)
                return null;
            string charToRemove = (string) parameter;
            int end = 0;
            if (charToRemove == null)
                end = s.Length;
            else
                end = s.IndexOf(charToRemove);
            string res = s.Substring(0, end);
            return res;
        }
    }
}
