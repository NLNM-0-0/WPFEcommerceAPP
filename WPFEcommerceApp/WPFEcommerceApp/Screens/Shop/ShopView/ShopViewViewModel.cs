using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (user.StatusShop == "Banned")
            {
                SelectedItem = new MainItem("UnShopMainBanned", typeof(UnShopMain), new BannedShopViewModel());
            }
            else if (user.StatusShop == "NotExist" && AccountStore.instance.CurrentAccount.Id == user.Id)
            {
                SelectedItem = new MainItem("UnShopMainNotRegister", typeof(UnShopMain), new NotRegisterShopViewModel());
            }
            else if (user.StatusShop == "NotBanned")
            {
                SelectedItem = new MainItem("ShopMain", typeof(ShopMain), new ShopMainViewModel(user));
            }
        }
    }
}
