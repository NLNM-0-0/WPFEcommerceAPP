using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPFEcommerceApp
{
    internal class ShopViewViewModel:BaseViewModel
    { 
        private MainItem selectedItem;
        public MainItem SelectedItem 
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }
        public ShopViewViewModel(Models.MUser user)
        {
            Task.Run(() =>
            {
                if (user.StatusShop == "Banned")
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        IsLoadingCheck.IsLoading = 2;
                    }));
                    SelectedItem = new MainItem("UnShopMainBanned", typeof(UnShopMain), new BannedShopViewModel());
                }
                else if (user.StatusShop == "NotExist" && AccountStore.instance.CurrentAccount.Id == user.Id)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        IsLoadingCheck.IsLoading = 3;
                    }));
                    SelectedItem = new MainItem("UnShopMainNotRegister", typeof(UnShopMain), new NotRegisterShopViewModel());
                }
                else if (user.StatusShop == "NotBanned")
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        IsLoadingCheck.IsLoading = 3;
                    }));
                    ShopMainViewModel shopMainViewModel = new ShopMainViewModel(user);
                    SelectedItem = new MainItem("ShopMain", typeof(ShopMain), shopMainViewModel);
                }
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock(IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            });
            
        }
    }
}
