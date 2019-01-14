using System;
using System.Globalization;
using System.Windows.Data;
using Communication.Models;

namespace ThreeDAdMachine.Converters
{
    /********************************************************************************

    ** 类名称： DeviceStateToStringConverter

    ** 描述：用来将DeviceState枚举类型相对应的转换成需要的string类型

    ** 作者： Laggage

    ** 创建时间：2019 1/14

    ** 最后修改人：Laggage

    ** 最后修改时间：Laggage

    ** 版权所有 (C) :Laggage

    *********************************************************************************/

    class DeviceStateToStringConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) throw new ArgumentNullException();
            if (value.GetType() != typeof(DeviceState))
                throw new ArgumentException("DeviceStateToButtonStringConverter 只能将DeviceState转换为string");
            if (parameter == null)
                throw new ArgumentNullException(nameof(value));
            if (parameter.GetType() != typeof(string))
                throw new ArgumentException("DeviceStateToButtonStringConverter.Converter的parameter参数必须为string类型!");
            string[] paramString = ((string) parameter).Split('/',' ','\\');
            if (paramString.Length > 2)
                throw new ArgumentException("parameter中只能包含一个 '/' '\\' ' ' 中的一个");
            return ((DeviceState) value) == DeviceState.OffLine ? paramString[0] : paramString[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
