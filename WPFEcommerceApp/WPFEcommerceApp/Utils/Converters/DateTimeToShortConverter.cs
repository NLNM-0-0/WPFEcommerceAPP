using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFEcommerceApp
{
    public class DateTimeToShortConverter : IValueConverter
    {
        public static DateTimeToShortConverter Instance=>new DateTimeToShortConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (DateTime)value;
            if (item != null)
            {
                return item.ToString("dd/MM/yyyy");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
