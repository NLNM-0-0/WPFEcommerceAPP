using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFEcommerceApp {
    public class PhoneValidateRule : ValidationRule {
        public string MustEndWith { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            var str = value as string;
            if(str == null || str.Length == 0) {
                return new ValidationResult(true, null);
            }
                if(str.Length < 5) {
                return new ValidationResult(false, "Phone number is wrong type!");
            }
            string rgStr = @"^([+]?[\s0-9]+)?(\d{3}|[(]?[0-9]+[)])?([-]?[\s]?[0-9])+$";
            Regex regex = new Regex(rgStr);
            if(regex.IsMatch(str)) {
                return new ValidationResult(true, null);
            }
            else return new ValidationResult(false, "Phone number is wrong type!");
        }
    }
}
