using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFEcommerceApp
{
    public class ShopProductStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {                     
            if (values[0].ToString() == "0")
            {
                if ((int)values[1] > 0)
                {
                    return "On Sale";
                }
                else
                {
                    return "Out Of Stock";
                }
            }
            else
            {
                return "Banned";
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
