using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFEcommerceApp {
    internal class DescriptionConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string description = (string)value;
            description = description.Trim();
            if(description.Length > 100)
                description = description.Substring(0, 100);
            description += "...";
            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
