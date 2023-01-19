using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class DrawerVM : BaseViewModel {
        private readonly AccountStore _accountStore;
        private readonly NavigationStore _navigationStore;

        public MUser CurrentUser => _accountStore?.CurrentAccount;

        public ObservableCollection<ButtonItem> ButtonItems =>
            CurrentUser == null
            ? NormalButtonCreate()
            : CurrentUser.Role != "Admin"
            ? NormalButtonCreate()
            : AdminButtonCreate();

        #region Props
        private int selectedIndex = 0;
        public int SelectedIndex {
            get { return selectedIndex; }
            set {
                if(selectedIndex != value) {
                    selectedIndex = value;
                    OnPropertyChanged();
                }
            }
        }
        private int prevSelected { get; set; } = 0;

        private bool canReload;
        public bool CanReload {
            get { return canReload; }
            set { canReload = value; OnPropertyChanged(); }
        }
        #endregion

        #region Command
        public ICommand OnChangeScreen { get; set; }
        public ICommand OnShopMouseOver { get; set; }
        public ICommand OnShopMouseLeave { get; set; }
        public ICommand OnButtonClick { get; }
        public ShopPopUpVM ShopPopUpDataContext { get; set; }
        #endregion

        public DrawerVM() {

            _accountStore = AccountStore.instance;
            _accountStore.AccountChanged += OnAccountChange;

            _navigationStore = NavigationStore.instance;
            _navigationStore.CurrentVMChanged += OnScreenChange;

            CanReload = true;
            ShopPopUpDataContext = new ShopPopUpVM(this);

            #region Command define

            OnChangeScreen = new RelayCommand<object>((p) => {
                if(CanReload) return true;
                if(!CanReload && SelectedIndex != 4) {
                    CanReload = true;
                    return true;
                }
                return false;
            },
            (p) => {
                if(DialogHost.IsDialogOpen("Main"))
                    DialogHost.Close("Main");
                if(CurrentUser == null || CurrentUser.Role != "Admin") {

                    if(CurrentUser != null)
                        ShopPopUpDataContext = new ShopPopUpVM(this);
                    else {
                        if(SelectedIndex != 0 && SelectedIndex != -1 && SelectedIndex != 6 && Internet.IsConnected) {
                            var dialog = new ConfirmDialog() {
                                Header = "Oops!",
                                Content = "You need to login to do this!",
                                CM = new ImmediateCommand<object>(pr => {
                                    Login login = App.serviceProvider.GetRequiredService<Login>();
                                    login.Show();
                                    App.Current.MainWindow.Hide();
                                }),
                            };
                            DialogHost.Show(dialog, "Main", ClosingEventHandler);
                            return;
                        }
                    }
                    if(SelectedIndex == 0) {
                        ChangeIndex(NavigateProvider.HomeScreen());
                    }
                    else if(SelectedIndex == 1) {
                        ChangeIndex(NavigateProvider.BagScreen());
                    }
                    else if(SelectedIndex == 2) {
                        ChangeIndex(NavigateProvider.OrderScreen());
                    }
                    else if(SelectedIndex == 3) {
                        ChangeIndex(NavigateProvider.FavouriteScreen());
                    }
                    else if(SelectedIndex == 4) {
                        ChangeIndex(NavigateProvider.ShopViewScreen(), CurrentUser);
                    }
                    else if(SelectedIndex == 5) {
                        ChangeIndex(NavigateProvider.ProfileScreen());
                    }
                    else {
                        //Unknown
                    }
                }
                else {
                    if(SelectedIndex == 0) {
                        ChangeIndex(NavigateProvider.AdminUserScreen());
                    }
                    else if(SelectedIndex == 1) {
                        ChangeIndex(NavigateProvider.ShopInformationScreen());
                    }
                    else if(SelectedIndex == 2) {
                        ChangeIndex(NavigateProvider.AdminProductScreen());
                    }
                    else if(SelectedIndex == 3) {
                        ChangeIndex(NavigateProvider.AdminAdsScreen());
                    }
                    else if(SelectedIndex == 4) {
                        ChangeIndex(NavigateProvider.AdminPromoScreen());
                    }
                    else if(SelectedIndex == 5) {
                        ChangeIndex(NavigateProvider.AdminCategoryScreen());
                    }
                    else if(SelectedIndex==6) {
                        ChangeIndex(NavigateProvider.AdminBrandScreen());
                    }
                    else {
                        ChangeIndex(NavigateProvider.ProfileScreen());
                    }
                }
                prevSelected = SelectedIndex;

            });

            OnButtonClick = new RelayCommand<object>(p => {
                if((int)p != SelectedIndex) {
                    SelectedIndex = (int)p;
                    return false;
                }
                if(AccountStore.instance.CurrentAccount == null && SelectedIndex != 0) return false;
                return true;
            }, p => {
                NavigationStore.instance.CurrentViewModel = null;
                SelectedIndex = -1;
                SelectedIndex = (int)p;
                var stackScreen = NavigationStore.instance.stackScreen;

                if(stackScreen.Count > 1 &&
                    stackScreen[stackScreen.Count - 1].Item1.GetType().Equals(
                    stackScreen[stackScreen.Count - 2].Item1.GetType()) ) {
                    stackScreen.RemoveAt(stackScreen.Count - 1);
                }
            });
            //Shop popup Handle
            var timer = new DispatcherTimer();
            OnShopMouseOver = new RelayCommand<object>(p => {
                if(CurrentUser == null || CurrentUser.StatusShop != "NotBanned" || !Internet.IsConnected) return false;
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
            #endregion
        }

        #region Outside change handle
        private void OnScreenChange() {
            if(NavigationStore.instance.CurrentViewModel == null) return;
            Type type = NavigationStore.instance.CurrentViewModel.GetType();
            switch(type.Name) {
                case "AdminUserManagerViewModel":
                    SelectedIndex = 0;
                    break;
                case "ShopInformationPageViewModel":
                    SelectedIndex = 1;
                    break;
                case "AdminProductManagerViewModel":
                    SelectedIndex = 2;
                    break;
                case "AdsManagerViewModel":
                    SelectedIndex = 3;
                    break;
                case "AdminPromoViewModel":
                    SelectedIndex = 4;
                    break;
                case "AdminCategoryViewModel":
                    SelectedIndex = 5;
                    break;
                case "AdminBrandViewModel":
                    SelectedIndex = 6;
                    break;
                case "MyProfileViewModel":
                    if(CurrentUser.Role == "Admin")
                        SelectedIndex= 7;
                    else SelectedIndex = 5;
                    break;
                case "MyHomeViewModel":
                    SelectedIndex = 0;
                    break;
                case "BagViewModel":
                    SelectedIndex = 1;
                    break;
                case "OrderScreenVM":
                    SelectedIndex = 2;
                    break;
                case "FavoriteViewModel":
                    SelectedIndex = 3;
                    break;
                case "ShopViewViewModel":
                    if(CurrentUser != null)
                        SelectedIndex = 4;
                    break;
            }
        }
        #endregion

        void ChangeIndex(INavigationService nav, object o = null) {
            var _navStore = NavigationStore.instance;
            if(!Internet.IsConnected) {
                if(AccountStore.instance.CurrentAccount == null)
                    SelectedIndex = 0;
                else
                    _navStore.stackScreen.Add(new Tuple<INavigationService, object>(nav, o));
                return;
            }
            if(_navStore.currentViewModel == null || nav.GetViewModel() != _navStore.CurrentViewModel.GetType()) {
                if(o != null)
                    nav.Navigate(o);
                else nav.Navigate();
            }
        }

        private void OnAccountChange() {
            OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(ButtonItems));
            SelectedIndex = prevSelected = 0;
        }
        public override void Dispose() {
            _accountStore.AccountChanged -= OnAccountChange;
            base.Dispose();
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs) {
            SelectedIndex = prevSelected;
        }

        private ObservableCollection<ButtonItem> AdminButtonCreate() {
            return new ObservableCollection<ButtonItem> {
                    new ButtonItem("AccountGroup", "Users", 0),
                    new ButtonItem("Store", "Shops", 1),
                    new ButtonItem("Dropbox", "Products", 2),
                    new ButtonItem("Advertisements", "Ads", 3),
                    new ButtonItem("TicketConfirmation", "Promo", 4),
                    new ButtonItem("TagMultiple", "Catergories", 5),
                    new ButtonItem("AlphaBBox", "Brands", 6),
                    new ButtonItem("AccountCircle", "My Profile", 7),
                };
        }

        private ObservableCollection<ButtonItem> NormalButtonCreate() {
            return new ObservableCollection<ButtonItem> {
                    new ButtonItem("Home", "Home", 0),
                    new ButtonItem("Shopping", "Bag", 1),
                    new ButtonItem("Cart", "Order", 2),
                    new ButtonItem("Heart", "Favourite", 3),
                    new ButtonItem("Store", "Shop", 4),
                    new ButtonItem("AccountCircle", "My profile", 5),
                    //new ButtonItem("Cog", "Settings"),
                };
        }
    }

}
public class ButtonItem {
    public string Icon { get; set; }
    public string Text { get; set; }
    public int Index { get; set; }
    public ButtonItem(string icon, string text, int index) {
        Icon=icon;
        Text=text;
        Index=index;
    }
}
