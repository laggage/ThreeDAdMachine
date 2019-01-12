using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MediaProcess.Model;

namespace ThreeDAdMachine.Converters
{
    class PathToImageSourceMultiConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var e in values)
            {
                if (e?.GetType() == typeof(string))
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri((string)e, UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    return bi;
                }
                else if (e?.GetType() == typeof(VideoModel))
                    return ((VideoModel)e).Thumbnail;
                else
                    continue;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
