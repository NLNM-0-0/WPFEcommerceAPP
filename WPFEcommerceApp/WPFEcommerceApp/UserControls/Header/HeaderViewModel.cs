using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DataAccessLayer;
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
        #endregion

        #region Properties
        private AccountStore AccountStore { get => _accountStore; 
            set { 
                _accountStore = value; 
                OnPropertyChanged(nameof(Icon));
                OnPropertyChanged(nameof(IconTooltip));
            } 
        }
        private AccountStore _accountStore;

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
            get => _accountStore != null ? "SignOut" : "SignIn";
            set { OnPropertyChanged(); }
        }
        public string IconTooltip { get => _accountStore != null ? "Sign Out" : "Sign In"; set { } }
        #endregion

        public HeaderViewModel(AccountStore accountStore)
        {
            _accountStore = accountStore;

            userRepo = new GenericDataRepository<MUser>();
            productRepo = new GenericDataRepository<Models.Product>();

            OpenSearchCommand = new RelayCommandWithNoParameter(OpenSearch);
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearchText);
            ClosePopupCommand = new RelayCommandWithNoParameter(ClosePopup);
            SearchCommand = new RelayCommandWithNoParameter(Search);

            //VHCMT
            //Load();
            SearchText = string.Empty;

        }


        public async void Load()
        {
            var users = new ObservableCollection<MUser>(
                await userRepo.GetListAsync(user => user.StatusShop == Status.NotBanned.ToString() || user.StatusUser == Status.NotBanned.ToString()));

            var products = new ObservableCollection<Models.Product>(
                await productRepo.GetListAsync(prd => prd.Status == Status.NotBanned.ToString(), prd => prd.ImageProducts));

                var userSearchItems = new ObservableCollection<SearchItemViewModel>(
                users.Select(user => new SearchItemViewModel
                {
                    Name = user.Name,
                    SourceImage = new BitmapImage(new Uri(ReturnDefault(user.SourceImageAva))),
                    IsProduct = false,
                    Model = user,
                }));

            
                var productsSearchItems = new ObservableCollection<SearchItemViewModel>(
                products.Select(prd => new SearchItemViewModel
                {

                    Name = prd.Name,
                    SourceImage = new BitmapImage(new Uri(ReturnDefault(prd.ImageProducts.FirstOrDefault().Source))),
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
        #endregion
    }
}
