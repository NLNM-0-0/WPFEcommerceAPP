using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFEcommerceApp
{
    public class ShopProductStatusColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values[0] == null || values[1] == null)
            {
                return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            }
            else
            {
                if (values[0].ToString() == "NotBanned")
                {
                    if ((int)values[1] > 0)
                    {
                        return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(42, 169, 82));
                    }
                    else
                    {
                        return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(253, 197, 0));
                    }
                }
                else if (values[0].ToString() == "Banned")
                {
                    return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(219, 48, 34));
                }
            }
            return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        /*public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (String.IsNullOrEmpty(value.ToString()))
            {
                return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            }
            else
            {
                if (value.ToString() == "On Sale")
                {
                    return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(42, 169, 82));
                }
                else if(value.ToString() == "Out Of Stock")
                {
                    return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(253, 197, 0));
                }
                else if (value.ToString() == "Banned")
                {
                    return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(219, 48, 34));
                }
            }
            return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }*/
    }
}
