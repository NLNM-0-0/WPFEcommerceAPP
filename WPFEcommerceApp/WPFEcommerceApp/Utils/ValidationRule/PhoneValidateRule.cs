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
        public bool IsFirstTime { get; set; } = true;

        public static bool Validate(string value) {
            var str = value;
            if(str == null || 
                str.Length == 0 || 
                str.Length < 5 || 
                str.Length > 15 ||
                !ValidateRegex.Phone.IsMatch(str)) {
                return false;
            }
            return true;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            var str = value as string;
            if(str == null || str.Length == 0) {
                if(!IsFirstTime)
                    return new ValidationResult(false, "This field cannot be left blank.");
                else {
                    IsFirstTime = false;
                    return new ValidationResult(true, null);
                }
            }
            if(str.Length < 5 || str.Length > 15) {
                return new ValidationResult(false, "Phone number is in the wrong format!");
            }

            if(ValidateRegex.Phone.IsMatch(str)) {
                return new ValidationResult(true, null);
            }
            else return new ValidationResult(false, "Phone number is in the wrong format!");
        }
    }
}
