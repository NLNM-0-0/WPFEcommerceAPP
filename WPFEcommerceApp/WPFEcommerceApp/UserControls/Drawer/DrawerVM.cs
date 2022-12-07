using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFEcommerceApp {
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
            set { 
                selectedIndex = value; 
                OnPropertyChanged();
            }
        }

        public ICommand OnChangeScreen { get; set; }
        public DrawerVM(
            INavigationService CheckoutNavigateService, 
            INavigationService OrderNavigateService) {
            if(Role == "Admin") {
                ButtonItems = AdminButtonCreate();
            }
            else {
                ButtonItems = NormalButtonCreate();
            }

            OnChangeScreen = new RelayCommand<object>((p) => true, (p) => {
                if(SelectedIndex == 0) {
                    CheckoutNavigateService.Navigate();
                }
                else if(SelectedIndex == 1) {
                    OrderNavigateService.Navigate();
                }
            });
        }

        private ObservableCollection<ButtonItem> AdminButtonCreate() {
            return new ObservableCollection<ButtonItem> {
                    new ButtonItem("Users", "Users"),
                    new ButtonItem("Archive", "Shops"),
                    new ButtonItem("Dropbox", "Products"),
                    new ButtonItem("Ticket", "Ads"),
                    new ButtonItem("Tags", "Catergories"),
                    new ButtonItem("AddressCard", "Brands"),
                    new ButtonItem("UserCircleOutline", "My Profile"),
                };
        }

        private ObservableCollection<ButtonItem> NormalButtonCreate() {
            return new ObservableCollection<ButtonItem> {
                    new ButtonItem(),
                    new ButtonItem("ShoppingBag", "Bag"),
                    new ButtonItem("ShoppingCart", "Order"),
                    new ButtonItem("Heart", "Favourite"),
                    new ButtonItem("Archive", "Shop"),
                    new ButtonItem("UserCircleOutline", "My profile"),
                    new ButtonItem("Gear", "Settings"),
                };
        }
    }

}
public class ButtonItem {
    public string Icon { get; set; }
    public string Text { get; set; }
    public ButtonItem() {
        Icon = "Home";
        Text = "Home";
    }
    public ButtonItem(string icon, string text) {
        Icon=icon;
        Text=text;
    }
}
