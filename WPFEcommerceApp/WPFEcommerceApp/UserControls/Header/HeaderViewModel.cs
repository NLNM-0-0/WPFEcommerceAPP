using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DataAccessLayer;
using LiveCharts;
using MaterialDesignThemes.Wpf;
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
        public ICommand HomeCommand { get; set; }
        #endregion

        #region Properties

        private GenericDataRepository<MUser> userRepo;
        private GenericDataRepository<Models.Product> productRepo;
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

        private ObservableCollection<SearchItemViewModel> _allItems;
        public ObservableCollection<SearchItemViewModel> AllItems
        {
            get { return _allItems; }
            set { _allItems = value; OnPropertyChanged(); }
        }

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
        #endregion

        public HeaderViewModel()
        {
            if(AccountStore.instance!=null)
                AccountStore.instance.AccountChanged += OnAccountChange;

            userRepo = new GenericDataRepository<MUser>();
            productRepo = new GenericDataRepository<Models.Product>();

            OpenSearchCommand = new RelayCommandWithNoParameter(OpenSearch);
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearchText);
            ClosePopupCommand = new RelayCommandWithNoParameter(ClosePopup);
            SearchCommand = new RelayCommandWithNoParameter(Search);
            SignInOutCommand = new RelayCommandWithNoParameter(SignInOut);
            HomeCommand = new RelayCommandWithNoParameter(ToHome);

            Task.Run(async()=>await Load());

            SearchText = string.Empty;
        }

        private void OnAccountChange()
        {
            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(IconTooltip));
        }

        public async Task Load()
        {
            var users = new ObservableCollection<MUser>(
                await userRepo.GetListAsync(user => user.StatusShop == Status.NotBanned.ToString() || user.StatusUser == Status.NotBanned.ToString()));

            var products = new ObservableCollection<Models.Product>(
                await productRepo.GetListAsync(prd => prd.Status == Status.NotBanned.ToString(), prd => prd.ImageProducts));

                var userSearchItems = new ObservableCollection<SearchItemViewModel>(
                users.Select(user => new SearchItemViewModel
                {
                    Name = user.Name,
                    SourceImage = ReturnDefault(user.SourceImageAva),
                    IsProduct = false,
                    Model = user,
                }));

            
                var productsSearchItems = new ObservableCollection<SearchItemViewModel>(
                products.Select(prd => new SearchItemViewModel
                {

                    Name = prd.Name,
                    SourceImage = ReturnDefault(prd.ImageProducts.Count()!=0?prd.ImageProducts.FirstOrDefault().Source:null),
                    IsProduct = true,
                    Model = prd
                }));

                AllItems = userSearchItems;
                foreach (var product in productsSearchItems)
                    AllItems.Add(product);
            
                ItemsSource=AllItems;

        }

        private string ReturnDefault(string s)
        {
            return string.IsNullOrEmpty(s)? "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQHZyOpzMZDaV-Cs1E-hjOJ-Dr2m4UIqc6j7w&usqp=CAU" : s;
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
                ItemsSource = new ObservableCollection<SearchItemViewModel>();

            else
                ItemsSource = new ObservableCollection<SearchItemViewModel>(AllItems.Where(item => (item.Name.ToLower()).Contains(SearchText.ToLower())));
        }

        public void SignInOut()
        {
            if(AccountStore.instance.CurrentAccount != null) {
                var dialog = new ConfirmDialog() {
                    Param = "",
                    CM = new RelayCommand<object>(p => true, p => {
                        //need to be HomeScreen here
                        NavigateProvider.HomeScreen().Navigate();
                        AccountStore.instance.CurrentAccount = null;
                    })
                };
                DialogHost.Show(dialog, "App");
                return;
            }
            //Sign In handle here
        }

        public void ToHome()
        {

        }
        #endregion

        public override void Dispose()
        {
            AccountStore.instance.AccountChanged -= OnAccountChange;
            base.Dispose();
        }
    }
}
