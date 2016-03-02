using System;
using System.Globalization;
using System.Windows.Data;

namespace MapperWPF.Utilities
{
    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            var propertyInfo = value.GetType().GetProperty("Count");
            if (propertyInfo != null)
            {
                var count = (int)propertyInfo.GetValue(value, null);
                return count > 0;
            }
            if (!(value is bool)) return true;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? "" : null;
        }
    }
}
