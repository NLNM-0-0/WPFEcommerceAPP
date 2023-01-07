using Microsoft.JScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp
{
    public class BrandCheckViewModel : BaseViewModel
    {
        private Models.Brand brand;
        public Models.Brand Brand
        {
            get => brand;
            set
            {
                brand = value;
                OnPropertyChanged();
            }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged();
            }
        }
    }
}
