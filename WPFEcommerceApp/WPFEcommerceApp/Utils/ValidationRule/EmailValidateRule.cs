using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Markup;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WPFEcommerceApp {
    public class EmailValidateRule : ValidationRule {

        public bool CanRefresh { get; set; } = true;
        public bool IsFirstTime { get; set; } = true;
        public Wrapper Wrapper { get; set; }

        public static bool Validate(string value, bool IsAdmin = false) {
            var str = value;

            if(string.IsNullOrEmpty(str)) {
                return false;
            }
            if(IsAdmin) return true;
            if(!ValidateRegex.Email.IsMatch(str)) {
                return false;
            }

            return true;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            var str = value as string;

            if(string.IsNullOrEmpty(str)) {
                if(!IsFirstTime)
                    return new ValidationResult(false, "Email cannot be empty");
                else {
                    if(CanRefresh)
                        IsFirstTime = false;
                    return ValidationResult.ValidResult;
                }
            }

            if(Wrapper != null && Wrapper.AdminAccess) return new ValidationResult(true, null);

            if(!ValidateRegex.Email.IsMatch(str)) {
                return new ValidationResult(false, "Email is wrong type");
            }

            return new ValidationResult(true, null);
        }
    }
}
