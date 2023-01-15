using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFEcommerceApp {
    public class DoubleToCurrencyConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            #region Get currency Symbol
            if(culture == null) {
                culture = CultureInfo.GetCultureInfo("en-US");
            }
            string symbol = culture.NumberFormat.CurrencySymbol;
            #endregion
            #region Format number
            var money = System.Convert.ToDecimal(value);
            var separate = CultureInfo.GetCultureInfo("ru-RU");
            var res = money.ToString("N2", separate);
            if(res[res.Length - 1] == '0') {
                if(res[res.Length - 2] == '0') {
                    res = res.Remove(res.Length - 3);
                }
                else res = res.Remove(res.Length - 1);
            }
            #endregion
            #region Get param
            string clone = System.Convert.ToString(parameter);
            List<string> param = new List<string>();
            if(!string.IsNullOrEmpty(clone)) {
                param = clone.Split('|').ToList();
            }
            else param.Add("");
            
            // set param
            string type = param[0];
            string prefix = null;
            if(param.Count > 1)
                prefix = param[1];
            if(param.Count > 2 && !string.IsNullOrEmpty(param[2]))
                symbol = param[2];
            #endregion
            #region Type of string
            switch(type) {
                case "BeforeWithSpace":
                    res = symbol + " " + res;
                    break;
                case "Before":
                    res = symbol + res;
                    break;
                case "After":
                    res = res + symbol;
                    break;
                case "AfterWithSpace":
                    res = res + " " + symbol;
                    break;
                case "None":
                    break;
                default:
                    res = res + " " + symbol;
                    break;
            }
            #endregion

            //leading
            if(!string.IsNullOrEmpty(prefix)) {
                res = prefix + res;
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

    }
}
