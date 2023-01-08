using Microsoft.Xaml.Behaviors.Media;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace WPFEcommerceApp
{
    public class PromoCodeValidation : ValidationRule
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
        private string errorMessage = "Please enter only alphabetic characters (A-Z) and numbers (0-9).";
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
                string stringCheck = (value ?? "").ToString();
                if(string.IsNullOrWhiteSpace(stringCheck))
                {
                    return new ValidationResult(false, ErrorMessage);
                }  
                else
                {
                    if(stringCheck.Any(p => (p < 'A' || p > 'Z') && (p < '0' || p > '9')))
                    {
                        return new ValidationResult(false, ErrorMessage);
                    }  
                    return ValidationResult.ValidResult;
                }    
            }
            else
            {
                IsNotCheckFirstTime = true;
                return ValidationResult.ValidResult;
            }
        }
    }
}