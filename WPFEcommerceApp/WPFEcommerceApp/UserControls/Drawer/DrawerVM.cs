using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DataAccessLayer;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class DrawerVM : BaseViewModel {
        private readonly AccountStore _accountStore;
        public MUser CurrentUser => _accountStore?.CurrentAccount;

        public ObservableCollection<ButtonItem> ButtonItems =>
            CurrentUser == null
            ? NormalButtonCreate()
            : CurrentUser.Role != "Admin"
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

        private bool canReload;

        public bool CanReload {
            get { return canReload; }
            set { canReload = value; OnPropertyChanged(); }
        }


        public ICommand OnChangeScreen { get; set; }
        public ICommand OnShopMouseOver { get; set; }
        public ICommand OnShopMouseLeave { get; set; }
        public ShopPopUpVM ShopPopUpDataContext { get; set; }

        public DrawerVM(
            AccountStore accountStore,
            INavigationService CheckoutNavigateService,
            INavigationService OrderNavigateService,
            INavigationService ShopInformationPageNavigateService,
            INavigationService ShopMainNavigateService,
            INavigationService ShopOrderNavigateService,
            INavigationService ShopProductNavigateService,
            INavigationService ShopRatingNavigateService) {

            _accountStore = accountStore;
            _accountStore.AccountChanged += OnAccountChange;

            CanReload = true;
            ShopPopUpDataContext = new ShopPopUpVM(
                this,
                ShopMainNavigateService,
                ShopOrderNavigateService,
                ShopProductNavigateService,
                ShopRatingNavigateService);

            OnChangeScreen = new RelayCommand<object>((p) => {
                if(CanReload) return true;
                if(!CanReload && SelectedIndex != 4) {
                    CanReload = true;
                    return true;
                }
                return false;
            }, (p) => {
                if(CurrentUser == null || CurrentUser.Role != "Admin") {
                    ShopPopUpDataContext = new ShopPopUpVM(
                        this,
                        ShopMainNavigateService,
                        ShopOrderNavigateService,
                        ShopProductNavigateService,
                        ShopRatingNavigateService);

                    if(SelectedIndex == 0) {
                        CheckoutNavigateService.Navigate();
                    }
                    else if(SelectedIndex == 2) {
                        //var t = new GenericDataRepository<MUser>();
                        //_accountStore.CurrentAccount = await t.GetSingleAsync(d => d.Id.Equals("user01"));
                        OrderNavigateService.Navigate();
                    }
                    else if(SelectedIndex == 4) {
                        ShopMainNavigateService.Navigate();
                    }
                }
                else {
                    if(SelectedIndex == 1) {
                        ShopInformationPageNavigateService.Navigate();
                    }
                }
            });

            //Shop popup Handle
            var timer = new DispatcherTimer();
            OnShopMouseOver = new RelayCommand<object>(p => {
                var values = (object[])p;
                if((values[0] as ButtonItem).Text == "Shop") return true;
                return false;
            }, p => {
                var values = (object[])p;
                timer.Stop();
                timer.Interval = TimeSpan.FromMilliseconds(200);
                timer.Tick += delegate {
                    (values[1] as ShopPopUp).Visibility = Visibility.Visible;
                };
                timer.Start();
            });
            OnShopMouseLeave = new RelayCommand<object>(p => {
                var values = (object[])p;
                if((values[0] as ButtonItem).Text == "Shop") return true;
                return false;
            }, p => {
                var values = (object[])p;
                timer.Stop();
                timer.Interval = TimeSpan.FromMilliseconds(300);
                timer.Tick += delegate {
                    if((values[1] as ShopPopUp).IsMouseOver == false)
                        (values[1] as ShopPopUp).Visibility = Visibility.Collapsed;
                };
                timer.Start();
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
