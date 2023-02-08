using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using ThreadTimer = System.Threading.Timer;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DataAccessLayer;
using LiveCharts;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class HeaderViewModel : BaseViewModel
    {
        #region Commands
        public ICommand OpenSearchCommand { get; set; }
        public ICommand CloseSearchCommand { get; set; }
        public ICommand ClosePopupCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand SignInOutCommand { get; set; }
        public ICommand OnBack { get; set; }
        public ICommand ToNoteCommand { get; set; }
        public ICommand FilterSearchCommand { get; }
        #endregion

        #region Properties

        private GenericDataRepository<MUser> userRepo;
        private GenericDataRepository<Models.Product> productRepo;
        private readonly GenericDataRepository<Models.Notification> notiRepo = new GenericDataRepository<Models.Notification>();
        private bool _isSearchOpen = false;
        public bool IsSearchOpen
        {
            get { return _isSearchOpen; }
            set { _isSearchOpen = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SearchItemViewModel> _itemsSource;
        public ObservableCollection<SearchItemViewModel> ItemsSource
        {
            get { return _itemsSource; }
            set { _itemsSource = value; OnPropertyChanged(); }
        }

        static private ObservableCollection<SearchItemViewModel> _allItems;
        static public ObservableCollection<SearchItemViewModel> AllItems
        {
            get { return _allItems; }
            set { _allItems = value;}
        }
        public ObservableCollection<SearchItemViewModel> DefaultItems { get; set; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText == value)
                    return;
                _searchText = value;

                OnPropertyChanged();
                Search();
            }
        }
        public string Icon
        {
            get => AccountStore.instance.CurrentAccount != null ? "SignOut" : "SignIn";
            set { OnPropertyChanged(); }
        }
        public string IconTooltip { get => AccountStore.instance.CurrentAccount != null ? "Sign Out" : "Sign In"; set { } }
        public bool IsAdmin
        {
            get => AccountStore.instance.CurrentAccount != null
                && AccountStore.instance.CurrentAccount.Role == "Admin";
        }


        #endregion

        #region NotiField
        private Dictionary<bool?, List<Models.Notification>> ListNoti { get; set; } = new Dictionary<bool?, List<Models.Notification>>();
        public bool HaveNewNoti { get; set; } = false;
        private ThreadTimer notiTimer;

        public async void NotiListener(object o) {
            //Noti
            var notilist = await notiRepo.GetAllAsync();
            if(!ListNoti.ContainsKey(false))
                ListNoti[false] = new List<Models.Notification>();
            if(!ListNoti.ContainsKey(true))
                ListNoti[true] = new List<Models.Notification>();
            foreach(var item in notilist) {
                if(!ListNoti[item.HaveSeen].Contains(item))
                    ListNoti[item.HaveSeen].Add(item);
            }
            if(ListNoti[false].Count > 0) HaveNewNoti = true;
        }

        public void NotiStart() {
            notiTimer = new ThreadTimer(NotiListener, null, new TimeSpan(0, 0, 0, 0, 1000), new TimeSpan(0, 0, 0, 0, 1000));
        }

        public async Task NotiSeen() {
            for(int i = 0; i < ListNoti[false].Count;) {
                var temp = ListNoti[false][i];
                temp.HaveSeen = true;
                await notiRepo.Update(temp);
                ListNoti[true].Add(temp);
                ListNoti[false].Remove(temp);
            }
            HaveNewNoti = false;
        }

        #endregion
        public HeaderViewModel()
        {
            MainViewModel.SetLoading(true);
            if (AccountStore.instance != null)
                AccountStore.instance.AccountChanged += OnAccountChange;

            userRepo = new GenericDataRepository<MUser>();
            productRepo = new GenericDataRepository<Models.Product>();

            OpenSearchCommand = new RelayCommandWithNoParameter(OpenSearch);
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearchText);
            ClosePopupCommand = new RelayCommandWithNoParameter(ClosePopup);
            SearchCommand = new RelayCommandWithNoParameter(Search);

            SignInOutCommand = new ImmediateCommand<object>(async o => {
                if(AccountStore.instance.CurrentAccount != null) {
                    var dialog = new ConfirmDialog() {
                        Param = "",
                        CM = new RelayCommand<object>(p => true, p =>
                        {
                            //need to be HomeScreen here
                            AccountStore.instance.CurrentAccount = null;
                            NavigateProvider.HomeScreen().Navigate();
                            WPFEcommerceApp.Properties.Settings.Default.Cookie = "";
                            WPFEcommerceApp.Properties.Settings.Default.Save();
                        })
                    };
                    await DialogHost.Show(dialog, "App");
                    
                    return;
                }
                NavigateProvider.LoginScreenHandle(true);
            });

            OnBack = new RelayCommand<object>(p => NavigationStore.instance.stackScreen.Count > 1, p =>
            {
                NavigateProvider.Back();
            });
            ToNoteCommand = new RelayCommand<object>(p => {
                var temp = AccountStore.instance.CurrentAccount;
                return temp != null && temp.Role != "Admin";
            }, async p => {
                await NotiSeen();
                NavigateProvider.NotificationScreen().Navigate();
            });

            FilterSearchCommand = new ImmediateCommand<object>(p => {
                IsSearchOpen = false;
                FilterObject filter = new FilterObject(SearchText, null, null, null, FilterStatus.All);
                NavigateProvider.FilterScreen().Navigate(filter);
                SearchText = "";
            });

            var task=Task.Run(async () =>
            {
                MainViewModel.SetLoading(true);
                await Load();
                MainViewModel.SetLoading(false);
            });
            while (!task.IsCompleted) ;

            SearchText = string.Empty;
            NotiStart();

            MainViewModel.SetLoading(false);
        }

        private async void OnAccountChange()
        {
            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(IconTooltip));
            OnPropertyChanged(nameof(IsAdmin));
            await Load();
        }

        public async Task Load()
        {
            var users = new ObservableCollection<MUser>(
                await userRepo.GetListAsync(user => user.StatusShop == Status.NotBanned.ToString()));

            var products = new ObservableCollection<Models.Product>(
                await productRepo.GetListAsync(
                    prd => prd.BanLevel == 0, 
                    prd => prd.ImageProducts, 
                    prd => prd.MUser,
                    prd => prd.Category,
                    prd => prd.Brand
                    ));

            var userSearchItems = new ObservableCollection<SearchItemViewModel>(
                users.Select(user => new SearchItemViewModel
                {
                    Name = user.Name,
                    SourceImage = user.SourceImageAva,
                    IsProduct = false,
                    Model = user,
                }));


            var productsSearchItems = new ObservableCollection<SearchItemViewModel>(
                products.Select(prd => new SearchItemViewModel
                {
                    Name = prd.Name,
                    SourceImage = prd.ImageProducts.Count() != 0 ? prd.ImageProducts.FirstOrDefault().Source : null,
                    IsProduct = true,
                    Model = prd
                }));

            AllItems = userSearchItems;
            foreach (var product in productsSearchItems)
                AllItems.Add(product);

            Random rnd = new Random();
            if (AllItems.Count() > 0)
            {
                DefaultItems = new ObservableCollection<SearchItemViewModel>
                {
                    AllItems.ElementAtOrDefault(rnd.Next(0, AllItems.Count/2)),
                    AllItems.ElementAtOrDefault(rnd.Next(AllItems.Count/2, AllItems.Count)),
                };
            }
            else
                DefaultItems = null;

            ItemsSource = DefaultItems;
        }

        

        #region Command Methods
        public void OpenSearch()
        {
            IsSearchOpen = true;
        }

        public void CloseSearchText()
        {
            SearchText = string.Empty;
        }

        public void ClosePopup()
        {
            IsSearchOpen = false;
            SearchText = string.Empty;
        }

        public void Search()
        {
            if (SearchText == string.Empty)
                ItemsSource = DefaultItems;

            else
                ItemsSource = new ObservableCollection<SearchItemViewModel>(AllItems.Where(item => (item.Name.ToLower()).Contains(SearchText.ToLower())));
        }
        #endregion

        public override void Dispose()
        {
            AccountStore.instance.AccountChanged -= OnAccountChange;
            base.Dispose();
        }
    }
}
