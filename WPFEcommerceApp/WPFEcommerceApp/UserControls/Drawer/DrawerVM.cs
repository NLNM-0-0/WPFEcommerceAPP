﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataAccessLayer;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class DrawerVM : BaseViewModel {
        private readonly AccountStore _accountStore;
        public MUser CurrentUser => _accountStore?.CurrentAccount;

        public ObservableCollection<ButtonItem> ButtonItems =>
            CurrentUser == null
            ? NormalButtonCreate()
            : CurrentUser.IsAdmin == false
            ? NormalButtonCreate()
            : AdminButtonCreate();

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
            AccountStore accountStore,
            INavigationService CheckoutNavigateService, 
            INavigationService OrderNavigateService) {

            _accountStore = accountStore;
            _accountStore.AccountChanged += OnAccountChange;

            OnChangeScreen = new RelayCommand<object>((p) => true, (p) => {
                if(SelectedIndex == 0) {
                    CheckoutNavigateService.Navigate();
                }
                else if(SelectedIndex == 1) {
                    //var t = new GenericDataRepository<MUser>();
                    //_accountStore.CurrentAccount = await t.GetSingleAsync(d => d.Id.Equals("000000"));
                    OrderNavigateService.Navigate();
                }
            });
        }

        private void OnAccountChange() {
            OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(ButtonItems));
        }
        public override void Dispose() {
            _accountStore.AccountChanged -= OnAccountChange;
            base.Dispose();
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