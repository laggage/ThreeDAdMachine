using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ThreeDAdMachine.Converters
{
    class RectToViewPortConverter:IValueConverter
    {
     

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(Rect))
                throw new ArgumentOutOfRangeException("转换器无法转换除了Rect类之外的其他类型");
            Size originSize = (Size) parameter;
            Rect selectedRegion = (Rect) value;
            Rect viewPort = new Rect(selectedRegion.X / originSize.Width,
                selectedRegion.Y / originSize.Height,
                selectedRegion.Width / originSize.Width,
                selectedRegion.Height / originSize.Height);
            return viewPort;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
