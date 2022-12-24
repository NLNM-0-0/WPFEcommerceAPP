using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFEcommerceApp
{
    public class BooleanToGenderConverter : IValueConverter
    {
        public static BooleanToGenderConverter Instance => new BooleanToGenderConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
            {
                return (bool)value ? "Female" : "Male";
            }
            return "Female";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string)
            {
                return (string)value=="Female"?true:false;
            }
            return true;
        }
    }
}
