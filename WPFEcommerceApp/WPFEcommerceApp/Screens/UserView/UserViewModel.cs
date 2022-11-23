using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp
{
    internal class UserViewModel:BaseViewModel
    {
        private MainItem _SelectedItem;
        public MainItem SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
            }

        }


        public ObservableCollection<MainItem> MainItems { get; private set; }


        public UserViewModel()
        {
            MainItems = new ObservableCollection<MainItem>();
            MainItems.Add(new MainItem
            (
                "ShopRating",
                typeof(ShopRating)
            ));
            MainItems.Add(new MainItem
            (
                "ShopProduct",
                typeof(ShopProduct)
            ));
            MainItems.Add(new MainItem
            (
                "ShopMain",
                typeof(ShopMain)
            ));
            SelectedItem = MainItems[0];
        }
    }
}
