using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ShopRatingBlockModel : BaseViewModel
    {
        private Models.MUser customer;
        public Models.MUser Customer
        {
            get => customer;
            set => customer = value;
        }
        private MOrder order;
        public MOrder Order
        {
            get => order;
            set => order = value;
        }
        private Models.OrderInfo orderInfo;
        public Models.OrderInfo OrderInfo
        {
            get => orderInfo;
            set => orderInfo = value;
        }
        public string SourceImageAva
        {
            get
            {
                if (Customer == null || String.IsNullOrEmpty(Customer.SourceImageAva))
                {
                    return Properties.Resources.DefaultShopAvaImage;
                }
                else
                {
                    return Customer.SourceImageAva;
                }
            }
        }
        private string imageProduct;
        public string ImageProduct
        {
            get => imageProduct;
            set => imageProduct = value;
        }
    }
}
