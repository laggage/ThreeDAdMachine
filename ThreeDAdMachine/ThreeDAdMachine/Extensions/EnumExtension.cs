using System;
using System.ComponentModel;
using System.Reflection;

namespace ThreeDAdMachine.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum enumValue)
        {
            string value = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(value);
            object[] obj = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (obj == null||obj.Length == 0)
                return value;
            return ((DescriptionAttribute)obj[0]).Description;
        }
    }
}
