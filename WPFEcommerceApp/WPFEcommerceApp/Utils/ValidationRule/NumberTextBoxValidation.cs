using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFEcommerceApp
{
    public class NumberTextBoxValidation : ValidationRule
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
        public enum NumberType
        {
            IntergerType,
            DoubleType
        }
        private string errorNumberMessage = "Please enter a number.";
        public string ErrorNumerMessage
        {
            set => errorNumberMessage = value;
            get => errorNumberMessage;
        }
        private string errorMinMaxMessage;
        public string ErrorMinMaxMessage
        {
            get
            {
                if (String.IsNullOrEmpty(errorMinMaxMessage))
                {
                    errorMinMaxMessage = $"Enter number between {Min} and {Max}";
                }
                return errorMinMaxMessage;
            }
            set => errorMinMaxMessage = value;
        }
        private NumberType type = NumberType.IntergerType;
        public NumberType Type
        {
            get
            {
                return type;
            }
            set => type = value;
        }
        private bool isCanEmpty = false;
        public bool IsCanEmpty
        {
            get => isCanEmpty;
            set => isCanEmpty = value;
        }
        private double min = double.MinValue;
        public double Min
        {
            get => min;
            set => min = value;
        }

        private double max = double.MaxValue;
        public double Max
        {
            get => max;
            set => max = value;
        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (IsNotCheckFirstTime)
            {
                if (string.IsNullOrEmpty((value ?? "").ToString()))
                {
                    if (!IsCanEmpty)
                    {
                        return new ValidationResult(false, ErrorNumerMessage);
                    }
                    else
                    {
                        return ValidationResult.ValidResult;
                    }
                }
                else
                {
                    if (IsCanEmpty)
                    {
                        if ((value ?? "").ToString() == "")
                        {
                            return ValidationResult.ValidResult;
                        }
                    }
                    string resultString = (value ?? "").ToString();
                    double resultNumber;
                    if (double.TryParse(resultString, out resultNumber))
                    {
                        if (type == NumberType.IntergerType)
                        {
                            if(resultString.Any(c=>c < '0' || c > '9'))
                            {
                                return new ValidationResult(false, ErrorMinMaxMessage);
                            }
                            if (Min == double.MinValue)
                            {
                                min = int.MinValue;
                            }
                            if (Max == double.MaxValue)
                            {
                                max = int.MaxValue;
                            }
                            if (Min < int.MinValue || Max > int.MaxValue)
                            {
                                min = int.MinValue;
                                max = int.MaxValue;
                                return new ValidationResult(false, ErrorMinMaxMessage);
                            }
                        }
                        if (resultNumber >= Min && resultNumber <= Max)
                        {
                            return ValidationResult.ValidResult;
                        }
                        else
                        {
                            return new ValidationResult(false, ErrorMinMaxMessage);
                        }

                    }
                    return new ValidationResult(false, ErrorNumerMessage);
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