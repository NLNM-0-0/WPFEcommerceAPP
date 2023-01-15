using System.Globalization;
using System.Windows.Controls;

namespace WPFEcommerceApp {
    public class PasswordValidateRule : ValidationRule {
        public bool CanRefresh { get; set; } = true;
        public bool IsFirstTime { get; set; } = true;
        public bool CheckPrevPass { get; set; } = false;
        public Wrapper Wrapper { get; set; }

        public static bool Validate(string value) {
            var str = value;
            if(str == null ||
                str.Length < 6) {
                return false;
            }
            return true;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            var str = value as string;
            if(string.IsNullOrEmpty(str)) {
                if(!IsFirstTime)
                    return new ValidationResult(false, "This field cannot be left blank.");
                else {
                    if(CanRefresh)
                        IsFirstTime = false;
                    return new ValidationResult(true, null);
                }
            }
            if(str.Length < 6) {
                return new ValidationResult(false, "*Password length needs to be more than 6 characters.");
            }
            if(CheckPrevPass) {
                if(str == Wrapper.PrevPassword) return new ValidationResult(true, null);
                else return new ValidationResult(false, "Not the same as Password");
            }

            return new ValidationResult(true, "Fuck");
        }
    }
}
