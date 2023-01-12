using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AdminProductManagerViewModel: BaseViewModel
    {
        #region Public Properties
        private GenericDataRepository<Models.Product> productRepo;

        private ObservableCollection<Models.Product> _productsToSearch;

        private ObservableCollection<Models.Product> bannedProducts;
        private ObservableCollection<Models.Product> notBannedProducts;

        private ObservableCollection<Models.Product> _filteredProducts;
        public ObservableCollection<Models.Product> FilteredProducts
        {
            get
            {
                return _filteredProducts;
            }

            set
            {
                _filteredProducts = value;
                OnPropertyChanged();
            }
        }

        private string _searchBy;

        public string SearchBy
        {
            get { return _searchBy; }
            set { _searchBy = value; OnPropertyChanged(); }
        }
        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged(); }
        }

        private string _lastSearchText;
        private string _lastSearchOption;
        public List<string> SearchByOptions { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value)
                {
                    StatusText = "Not Banned";
                    FilteredProducts = _productsToSearch = notBannedProducts;
                    RemoveOrUnBanned = "Remove";
                }
                else
                {
                    StatusText = "Banned";
                    FilteredProducts = _productsToSearch = bannedProducts;
                    RemoveOrUnBanned = "Restore";
                }

                _isChecked = value;
            }
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        private string _removeOrUnBanned;
        public string RemoveOrUnBanned
        {
            get => _removeOrUnBanned;
            set { _removeOrUnBanned = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands

        public ICommand RemoveProductCommand { get; set; }

        public ICommand SearchCommand { get; set; }

        public ICommand CloseSearchCommand { get; set; }

        #endregion

        #region Command Methods

        public async Task Search()
        {
            await Task.Run(() =>
            {
                MainViewModel.IsLoading = true;
                if (string.IsNullOrEmpty(SearchBy))
                    FilteredProducts = _productsToSearch;

                if (string.IsNullOrEmpty(_lastSearchText) && string.IsNullOrEmpty(SearchText) ||
                    (string.Equals(_lastSearchText, SearchText) && _lastSearchOption == SearchBy))
                {
                    FilteredProducts = _productsToSearch;
                }

                if (string.IsNullOrEmpty(SearchText) || _productsToSearch.Count <= 0 || _productsToSearch == null)
                {
                    FilteredProducts = _productsToSearch;
                    return;
                }

                if (SearchBy == "Name")
                {
                    _lastSearchOption = "Name";
                    FilteredProducts = new ObservableCollection<Models.Product>(_productsToSearch.Where(br => br.Name.ToLower().Contains(SearchText.ToLower())));
                }
                else if (SearchBy == "ID")
                {
                    _lastSearchOption = "ID";
                    FilteredProducts = new ObservableCollection<Models.Product>(_productsToSearch.Where(br => br.Id.ToLower().Contains(SearchText.ToLower())));
                }
                else if (SearchBy == "ShopID")
                {
                    _lastSearchOption = "ID";
                    FilteredProducts = new ObservableCollection<Models.Product>(_productsToSearch.Where(br => br.IdShop.ToLower().Contains(SearchText.ToLower())));
                }
                else if (SearchBy == "Category")
                {
                    _lastSearchOption = "Categroy";
                    FilteredProducts = new ObservableCollection<Models.Product>(_productsToSearch.Where(br => br.Category.Name.ToLower().Contains(SearchText.ToLower())));
                }
                else if (SearchBy == "Brand")
                {
                    _lastSearchOption = "Brand";
                    FilteredProducts = new ObservableCollection<Models.Product>(_productsToSearch.Where(br => br.Brand.Name.ToLower().Contains(SearchText.ToLower())));
                }
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    FilteredProducts = new ObservableCollection<Models.Product>(FilteredProducts);
                }));
            });

            MainViewModel.IsLoading = false;


        }

        public void CloseSearch()
        {
            SearchText = string.Empty;
        }

        public async Task RemoveProduct(object obj)
        {
            MainViewModel.IsLoading = true;

            var removeProduct = obj as Models.Product;
            if (removeProduct == null)
                return;

            if (removeProduct.Status == Status.NotBanned.ToString())
                removeProduct.Status = Status.Banned.ToString();
            else
                removeProduct.Status = Status.NotBanned.ToString();

            await productRepo.Update(removeProduct);
            await Load();
            MainViewModel.IsLoading = false;

        }

        #endregion

        #region Constructor
        public AdminProductManagerViewModel()
        {
            MainViewModel.IsLoading = true;
            productRepo=new GenericDataRepository<Models.Product>();
            SearchByOptions = new List<string> { "ID", "ShopID", "Name", "Category", "Brand" };

            RemoveProductCommand = new RelayCommand<object>(p => p != null,async(p)=>await RemoveProduct(p));
            SearchCommand = new RelayCommandWithNoParameter(async()=>await Search());
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearch);

            Task.Run(async () =>
            {
                MainViewModel.IsLoading = true;
                await Load();
            }).ContinueWith((first) =>
            {
                MainViewModel.IsLoading = false;

            });

        }

        private async Task Load()
        {
            IsChecked = true;
            RemoveOrUnBanned = "Remove";
            SearchBy = SearchByOptions[0];

            notBannedProducts = new ObservableCollection<Models.Product>( 
                await productRepo.GetListAsync(
                    item=>item.Status.Equals(Status.NotBanned.ToString()), item=>item.Brand, item=>item.Category));

            bannedProducts = new ObservableCollection<Models.Product>(
                await productRepo.GetListAsync(
                    item => item.Status.Equals(Status.Banned.ToString()), item => item.Brand, item => item.Category));

            _productsToSearch = FilteredProducts = notBannedProducts;

            _lastSearchOption = null;
            _lastSearchText = string.Empty;
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                FilteredProducts = new ObservableCollection<Models.Product>(FilteredProducts);
            }));
        }

        #endregion
    }
}
