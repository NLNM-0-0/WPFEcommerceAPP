using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFEcommerceApp.Models;
namespace WPFEcommerceApp
{
    public class ShopContactViewModel:BaseViewModel
    {
        private Models.MUser shop;
        public Models.MUser Shop
        {
            get => shop;
            set
            {
                shop = value;
                OnPropertyChanged();
            }
        }

        public ShopContactViewModel(Models.MUser shop)
        {
            Shop = shop;
        }
    }
}
