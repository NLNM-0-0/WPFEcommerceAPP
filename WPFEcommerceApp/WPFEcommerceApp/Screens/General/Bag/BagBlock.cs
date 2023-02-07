using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WPFEcommerceApp.Models;

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
        public Models.Product Product { get; set; }
        public MUser Shop { get; set; }
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
        private string productName;
        public string ProductName
        {
            get => productName;
            set
            {
                productName = value;
                OnPropertyChanged();
            }
        }
        private string shopName;
        public string ShopName
        {
            get => shopName;
            set
            {
                shopName = value;
                OnPropertyChanged();
            }
        }
        private string productSize;
        public string ProductSize
        {
            get => productSize;
            set
            {
                productSize = value;
                OnPropertyChanged();
            }
        }

        private double unitPrice;
        public double UnitPrice
        {
            get => unitPrice;
            set
            {
                unitPrice = value;
                OnPropertyChanged();
            }
        }
        private int amount;
        public int Amount
        {
            get => amount;
            set
            {
                amount = value;
                OnPropertyChanged();
            }
        }
        private double price;
        public double Price
        {
            get => price;
            set
            {
                price = value;
                OnPropertyChanged();
            }
        }
        private bool isBanned;
        public bool IsBanned
        {
            get => isBanned;
            set
            {
                isBanned = value;
                OnPropertyChanged();
            }
        }
        private bool isOutOfSold;
        public bool IsOutOfSold
        {
            get => isOutOfSold;
            set
            {
                isOutOfSold = value;
                OnPropertyChanged();
            }
        }
        private string iD;
        public string ID
        {
            get => iD;
            set
            {
                iD = value;
                OnPropertyChanged();
            }
        }
        private ICommand plusamount;
        public ICommand Plusamount
        {
            get => plusamount;
            set
            {
                plusamount = value;
                OnPropertyChanged();
            }
        }
        private ICommand tamount;
        public ICommand Tamount
        {
            get => tamount;
            set
            {
                tamount = value;
                OnPropertyChanged();
            }
        }
        public BagBlock()
        {
            Plusamount = new RelayCommandWithNoParameter(() => { });
            Tamount = new RelayCommandWithNoParameter(() => { });
        }
    }

}
