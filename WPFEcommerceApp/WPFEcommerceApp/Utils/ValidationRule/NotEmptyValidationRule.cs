using System.Globalization;
using System.Windows.Controls;

namespace WPFEcommerceApp
{
    public class NotEmptyValidationRule : ValidationRule
    {
        private bool isNotCheckFirstTime = true;
        public bool IsNotCheckFirstTime
        {
            get => isNotCheckFirstTime;
            set
            {
                isNotCheckFirstTime = value;
            }
        }
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
            if (IsNotCheckFirstTime)
            {
                return string.IsNullOrWhiteSpace((value ?? "").ToString())
                    ? new ValidationResult(false, ErrorMessage)
                    : ValidationResult.ValidResult;
            }
            else
            {
                IsNotCheckFirstTime = true;
                return ValidationResult.ValidResult;
            }
        }
    }
}