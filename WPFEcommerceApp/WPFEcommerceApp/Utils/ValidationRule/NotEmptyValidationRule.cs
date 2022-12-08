using System.Globalization;
using System.Windows.Controls;

namespace WPFEcommerceApp
{
    public class NotEmptyValidationRule : ValidationRule
    {
        private string errorMessage = "Field is required.";
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
            }
        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, ErrorMessage)
                : ValidationResult.ValidResult;
        }
    }
}