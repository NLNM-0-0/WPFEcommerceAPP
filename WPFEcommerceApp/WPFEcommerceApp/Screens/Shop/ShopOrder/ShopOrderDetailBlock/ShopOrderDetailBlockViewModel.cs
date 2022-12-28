using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFEcommerceApp.Models;
namespace WPFEcommerceApp
{
    public class ShopOrderDetailBlockViewModel : BaseViewModel
    {
        private string productImage;
        public string ProductImage
        {
            get => productImage;
            set
            {
                productImage = value;
                OnPropertyChanged();
            }
        }
        private Models.OrderInfo orderInfo;
        public Models.OrderInfo OrderInfo
        {
            get => orderInfo;
            set
            {
                orderInfo = value;
                OnPropertyChanged();
            }
        }
        public int Rating
        {
            get
            {
                if(OrderInfo == null || OrderInfo.Rating == null)
                {
                    return 0;
                }    
                else
                {
                    return OrderInfo.Rating.Rating1??0;
                }    
            }
        }
        public ShopOrderDetailBlockViewModel(string productImage, OrderInfo orderInfo)
        {
            ProductImage = productImage;
            OrderInfo = orderInfo;
        }

    }
}
