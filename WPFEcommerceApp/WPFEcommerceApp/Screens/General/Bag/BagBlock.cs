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
        private string productImage;
        public string ProductImage
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

        private long unitPrice;
        public long UnitPrice
        {
            get => unitPrice;
            set
            {
                unitPrice = value;
            }
        }
        private long amount;
        public long Amount
        {
            get => amount;
            set
            {
                amount = value;
            }
        }
        private long price;
        public long Price
        {
            get => price;
            set
            {
                price = value;
            }
        }
        private string iD;
        public string ID
        {
            get => iD;
            set
            {
                iD = value;
            }
        }
    }

}
