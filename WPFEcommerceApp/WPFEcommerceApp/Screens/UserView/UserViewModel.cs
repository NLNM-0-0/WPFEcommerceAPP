using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

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
        public ICommand OnChangeScreen { get; set; }
        public ICommand OnSuccessPayment { get; set; }
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
            MainItems.Add(new MainItem(
                "CheckoutScreen",
                typeof(CheckoutScreen)));
            MainItems.Add(new MainItem(
                "OrderScreen",
                typeof(OrderScreen)));
            MainItems.Add(new MainItem(
                "SuccessScreen",
                typeof(SuccessScreen)));
            MainItems.Add(new MainItem(
                "ProductDetails",
                typeof(ProductDetails)));
            SelectedItem = MainItems[6];

            OnChangeScreen = new RelayCommand<object>((p) => true, (p) => {
            });
            OnSuccessPayment = new RelayCommand<object>((p) => true, (p) => {
                DialogHost.CloseDialogCommand.Execute(p, (p as IInputElement));
                SelectedItem = MainItems[5];
            });
        }
    }
}
