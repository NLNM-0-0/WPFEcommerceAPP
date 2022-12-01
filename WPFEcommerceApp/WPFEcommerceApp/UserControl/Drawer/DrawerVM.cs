using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFEcommerceApp{
    public class DrawerVM : BaseViewModel {
        private string role;

        public string Role {
            get { return role; }
            set { role = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ButtonItem> _buttonItems;
        public ObservableCollection<ButtonItem> ButtonItems {
            get => _buttonItems;
            set {
                _buttonItems = value;
                OnPropertyChanged();
            }
        }
        private int selectedIndex = 0;

        public int SelectedIndex {
            get { return selectedIndex; }
            set { selectedIndex = value; OnPropertyChanged(); }
        }
        public DrawerVM() {
            if(Role == "Admin") {
                ButtonItems = new ObservableCollection<ButtonItem> {
                    new ButtonItem("Users", "Users", "UserManagerScreen"),
                    new ButtonItem("Archive", "Shops", "ShopManagerScreen"),
                    new ButtonItem("Dropbox", "Products", "ProductManagerScreen"),
                    new ButtonItem("Ticket", "Ads", "AdsManagerScreen"),
                    new ButtonItem("Tags", "Catergories", "CategoriesManagerScreen"),
                    new ButtonItem("AddressCard", "Brands", "BrandManagerScreen"),
                    new ButtonItem("UserCircleOutline", "My Profile", "AdminProfileScreen"),
                };
            }
            else {
                ButtonItems = new ObservableCollection<ButtonItem> {
                    new ButtonItem(),
                    new ButtonItem("ShoppingBag", "Bag", "CheckoutScreen"),
                    new ButtonItem("ShoppingCart", "Order", "OrderScreen"),
                    new ButtonItem("Heart", "Favourite", "FavouriteScreen"),
                    new ButtonItem("Archive", "Shop", "ShopScreen"),
                    new ButtonItem("UserCircleOutline", "My profile", "UserScreen"),
                    new ButtonItem("Gear", "Settings"),
                };
            }
        }

    }
    public class ButtonItem {
        public string Icon { get; set; }
        public string Text { get; set; }
        public string NavLink { get; set; }
        public ButtonItem() {
            Icon = "Home";
            Text = "Home";
            NavLink="HomeScreen";
        }
        public ButtonItem(string icon, string text, string navLink="") {
            Icon=icon;
            Text=text;
            NavLink=navLink;
        }
    }
}
