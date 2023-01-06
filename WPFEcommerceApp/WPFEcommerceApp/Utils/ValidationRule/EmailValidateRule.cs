using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WPFEcommerceApp
{
    public class EmailValidateRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var str = value as string;

            if (string.IsNullOrEmpty(str))
            {
                return new ValidationResult(false, "Email cannot be empty");
            }
            string rgStr = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
              @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
              @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex regex = new Regex(rgStr);
            if (!regex.IsMatch(str))
            {
                return new ValidationResult(false, "Email is wrong type");
            }

            return new ValidationResult(true, null);
        }
    }
}
