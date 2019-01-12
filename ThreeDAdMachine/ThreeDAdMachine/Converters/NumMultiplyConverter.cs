using System;
using System.Globalization;
using System.Windows.Data;

namespace ThreeDAdMachine.Converters
{
    /********************************************************************************

    ** 类名称： NumMultiplyConverter

    ** 描述：值转换器,将数值类型转换为自身的n倍

    ** 作者： Laggage

    ** 创建时间：2018 12/24

    ** 最后修改人：Laggage

    ** 最后修改时间：Laggage

    ** 版权所有 (C) :Laggage

    *********************************************************************************/
    class NumMultiplyConverter :IValueConverter
    {
        /// <inheritdoc />
        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">表示倍数,如果不是double或int类型的或者为null,则默认倍数为1</param>
        /// <param name="culture"></param>
        /// <returns>返回转换后double类型的结果</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //value只能为double或者int类型,并且不能为null,否则返回null
            if (value == null ||
                (value.GetType() != typeof(int) && value.GetType() != typeof(double))) return null;

            double times;
            times = double.TryParse((string) parameter, out times)?times:1;
            if(value is double d) return d * times;
            return (int)value * times;
        }

        /// <inheritdoc />
        /// <summary>
        /// 执行逆转换
        /// </summary>
        /// <param name="value">被Converter转换过的值</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">表示倍数,如果不是double或int类型的或者为null,则默认倍数为1</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            double times;
            times = double.TryParse((string)parameter, out times) ? times : 1;
            //value要么为double类型,要么为int类型
            if (value is double d)
                return d / times;
            return (int) ((int)value / times);
        }
    }
}
