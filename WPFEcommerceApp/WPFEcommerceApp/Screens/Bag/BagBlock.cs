using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPFEcommerceApp
{
    public class BagBlock : BaseViewModel
    {
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
        private ImageSource productImage;
        public ImageSource ProductImage
        {
            get => productImage;
            set
            {
                productImage = value;
            }
        }
        private string productName;
        public string ProductName
        {
            get => productName;
            set
            {
                productName = value;
            }
        }
        private string shopName;
        public string ShopName
        {
            get => shopName;
            set
            {
                shopName = value;
            }
        }

        private int unitPrice;
        public int UnitPrice
        {
            get => unitPrice;
            set
            {
                unitPrice = value;
            }
        }
        private int amount;
        public int Amount
        {
            get => amount;
            set
            {
                amount = value;
            }
        }
        private int price;
        public int Price
        {
            get => price;
            set
            {
                price = value;
            }
        }
    }

}
