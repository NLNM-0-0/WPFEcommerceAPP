using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    internal class BagViewModel : BaseViewModel
    {
        public ICommand CheckAllProductCommand { get; set; }


        private ObservableCollection<BagBlock> bags;
        public ObservableCollection<BagBlock> Bags
        {
            get { return bags; }
            set
            {
                bags = value;
                OnPropertyChanged();
            }
        }
        private bool isCheckedAll = false;
        public bool IsCheckedAll
        {
            get { return isCheckedAll; }
            set
            {
                isCheckedAll = value;
                OnPropertyChanged();
                foreach (BagBlock b in bags)
                {
                    b.IsChecked = isCheckedAll;
                }
            }
        }
        public BagViewModel()
        {
            Bags = new ObservableCollection<BagBlock>();

            /*Bags.Add(new BagBlock
            {
                IsChecked = false,
                ProductImage = Product.DefaultImageSource,
                ProductName = "abc",
                UnitPrice = 20000,
                Amount = 3,
                Price = 60000,
                ShopName = "fff"
            });
            Bags.Add(new BagBlock
            {
                IsChecked = false,
                ProductImage = Product.DefaultImageSource,
                ProductName = "xyz",
                UnitPrice = 2000,
                Amount = 5,
                Price = 10000,
                ShopName = "eee"
            });*/
            CheckAllProductCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                bool isCheckedAll = true;
                foreach (BagBlock bag in Bags)
                {
                    if (bag.IsChecked == false)
                    {
                        isCheckedAll = false;
                        break;
                    }
                }
                IsCheckedAll = isCheckedAll;
            });
        }
    }
}

