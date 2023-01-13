using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Newtonsoft.Json.Linq;

namespace WPFEcommerceApp {
    public class VoucherFilterConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            string id = (string)values[0];
            Dictionary<string, bool> promo = (values[1] as CheckoutScreenVM).ValidVoucherList as Dictionary<string, bool>;

            if(promo == null) return false;
            return promo.ContainsKey(id);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
