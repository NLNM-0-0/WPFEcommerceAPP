using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ShopProductViewModel : BaseViewModel
    {
        #region Command
        public ICommand OpenAddBrandDialogCommand { get; set; }
        public ICommand OpenAddCategoryDialogCommand { get; set; }
        public ICommand OpenAddProductDialogCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand SearchProductCommand { get; set; }
        #endregion
        #region DataRepository
        private GenericDataRepository<Models.Product> ProductRepository = new GenericDataRepository<Models.Product>();
        private GenericDataRepository<Models.Brand> BrandRepository = new GenericDataRepository<Models.Brand>();
        private GenericDataRepository<Models.Category> CategoryRepository = new GenericDataRepository<Models.Category>();
        #endregion
        #region Field
        private Models.Product selectedProduct;
        public Models.Product SelectedProduct
        {
            get => selectedProduct;
            set
            {
                selectedProduct = value;
                OnPropertyChanged(nameof(StyleProductInfoPortrait));
                OnPropertyChanged();
            }
        }

        private Models.MUser shop;
        public Models.MUser Shop
        {
            get => shop;
            set
            {
                shop = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Brand> brands;
        public ObservableCollection<Models.Brand> Brands
        {
            get => brands;
            set
            {
                brands = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Category> categories;
        public ObservableCollection<Models.Category> Categories
        {
            get => categories;
            set
            {
                categories = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Product> products;
        public ObservableCollection<Models.Product> Products
        {
            get => products;
            set
            {
                products = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Product> AllProducts { get; set; }
        private ObservableCollection<Models.Product> OnSaleProducts { get; set; }
        private ObservableCollection<Models.Product> OutOfStockProducts { get; set; }
        private ObservableCollection<Models.Product> BannedProducts { get; set; }
        private ObservableCollection<Models.Product> FilterProducts { get; set; }
        public MainItem StyleProductInfoPortrait
        {
            get
            {
                MainItem mainItem = null;
                if (SelectedProduct == null || !IsOpenProductInfoPortrait)
                {
                    return mainItem;
                }
                if (SelectedProduct.BanLevel == 0)
                {
                    IsLoadingCheck.IsLoading = 3;
                    ProductInfoPortraitViewModel productInfoPortraitViewModel = new ProductInfoPortraitViewModel(SelectedProduct);
                    productInfoPortraitViewModel.SelectedProductChanged += LoadDataFromModel;
                    mainItem = new MainItem("NotBannedProductInfoPortrait", typeof(ProductInfoPortrait), productInfoPortraitViewModel);
                    IsLoadingCheck.IsLoading--;
                }
                else
                {
                    IsLoadingCheck.IsLoading = 2;
                    ProductInfoPortraitBannedViewModel productInfoPortraitBannedViewModel = new ProductInfoPortraitBannedViewModel(SelectedProduct);
                    mainItem = new MainItem("BannedProductInfoPortrait", typeof(ProductInfoPortraitBanned), productInfoPortraitBannedViewModel);
                    IsLoadingCheck.IsLoading--;
                }
                return mainItem;
            }
        }

        private string productNameSearch;
        public string ProductNameSearch
        {
            get => productNameSearch;
            set
            {
                productNameSearch = value;
                OnPropertyChanged();
            }
        }
        private string minPriceSearch;
        public string MinPriceSearch
        {
            get => minPriceSearch;
            set
            {
                minPriceSearch = value;
                OnPropertyChanged();
            }
        }
        private string maxpriceSearch;
        public string MaxPriceSearch
        {
            get => maxpriceSearch;
            set
            {
                maxpriceSearch = value;
                OnPropertyChanged();
            }
        }
        private string minInStockSearch;
        public string MinInStockSearch
        {
            get => minInStockSearch;
            set
            {
                minInStockSearch = value;
                OnPropertyChanged();
            }
        }
        private string maxInStockSearch;
        public string MaxInStockSearch
        {
            get => maxInStockSearch;
            set
            {
                maxInStockSearch = value;
                OnPropertyChanged();
            }
        }
        private Models.Category categorySearch;
        public Models.Category CategorySearch
        {
            get => categorySearch;
            set
            {
                categorySearch = value;
                OnPropertyChanged();
            }
        }
        private Models.Brand brandSearch;
        public Models.Brand BrandSearch
        {
            get => brandSearch;
            set
            {
                brandSearch = value;
                OnPropertyChanged();
            }
        }
        private bool statusAllSearch;
        public bool StatusAllSearch
        {
            get => statusAllSearch;
            set
            {
                statusAllSearch = value;
                if (value)
                {
                    FilterProducts = AllProducts;
                    LoadAllProduct();
                }
                OnPropertyChanged();
            }
        }
        private bool statusOnSaleSearch;
        public bool StatusOnSaleSearch
        {
            get => statusOnSaleSearch;
            set
            {
                statusOnSaleSearch = value;
                if (value)
                {
                    FilterProducts = OnSaleProducts;
                    LoadOnSaleProduct();
                }
                OnPropertyChanged();
            }
        }
        private bool statusOutOfStockSearch;
        public bool StatusOutOfStockSearch
        {
            get => statusOutOfStockSearch;
            set
            {
                statusOutOfStockSearch = value;
                if (value)
                {
                    FilterProducts = OutOfStockProducts;
                    LoadOutOfStockProduct();
                }
                OnPropertyChanged();
            }
        }
        private bool statusBannedSearch;
        public bool StatusBannedSearch
        {
            get => statusBannedSearch;
            set
            {
                statusBannedSearch = value;
                if (value)
                {
                    FilterProducts = BannedProducts;
                    LoadBannedProduct();
                }
                OnPropertyChanged();
            }
        }
        private bool isOpenProductInfoPortrait;
        public bool IsOpenProductInfoPortrait
        {
            get => isOpenProductInfoPortrait;
            set
            {
                isOpenProductInfoPortrait = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region Constructor
        public ShopProductViewModel()
        {
            IsLoadingCheck.IsLoading = 2;
            Task.Run(async () =>
            {
                Shop = AccountStore.instance.CurrentAccount;
                await LoadListProducts();
                await LoadBrands();
                await LoadCategories();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            }).ContinueWith((first) =>
            {
                OpenAddBrandDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
                {
                    MainViewModel.IsLoading = true;
                    AddBrandDialog addBrandDialog = new AddBrandDialog();
                    addBrandDialog.DataContext = new AddBrandDialogViewModel(null);
                    MainViewModel.IsLoading = false;
                    await DialogHost.Show(addBrandDialog, "Main");
                });
                OpenAddCategoryDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
                {
                    MainViewModel.IsLoading = true;
                    AddCategoryDialog addCategoryDialog = new AddCategoryDialog();
                    addCategoryDialog.DataContext = new AddCategoryDialogViewModel(null);
                    MainViewModel.IsLoading = false;
                    await DialogHost.Show(addCategoryDialog, "Main");
                });
                OpenAddProductDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
                {
                    MainViewModel.IsLoading = true;
                    AddProductDialog addProductDialog = new AddProductDialog();
                    AddProductDialogViewModel addProductDialogViewModel = new AddProductDialogViewModel();
                    addProductDialogViewModel.ClosedDialog += AddProductDialogViewModel_ClosedDialog;
                    addProductDialog.DataContext = addProductDialogViewModel;
                    MainViewModel.IsLoading = false;
                    await DialogHost.Show(addProductDialog, "Main");
                });
                SearchProductCommand = new RelayCommandWithNoParameter(async () =>
                {
                    double maxPrice = double.MaxValue;
                    double minPrice = 0;
                    int maxInStock = int.MaxValue;
                    int minInStock = 0;
                    if (!String.IsNullOrEmpty(MaxPriceSearch))
                    {
                        double.TryParse(MaxPriceSearch, out maxPrice);
                    }
                    if (!String.IsNullOrEmpty(MinPriceSearch))
                    {
                        double.TryParse(MinPriceSearch, out minPrice);
                    }
                    if (!String.IsNullOrEmpty(MaxInStockSearch))
                    {
                        int.TryParse(MaxInStockSearch, out maxInStock);
                    }
                    if (!String.IsNullOrEmpty(MinInStockSearch))
                    {
                        int.TryParse(MinInStockSearch, out minInStock);
                    }
                    if (minPrice > maxPrice)
                    {
                        MainViewModel.IsLoading = true;
                        NotificationDialog notification = new NotificationDialog();
                        notification.Header = "Error";
                        notification.ContentDialog = "Min price is bigger than max price";
                        await DialogHost.Show(notification, "Main");
                        MainViewModel.IsLoading = false;
                        return;
                    }
                    else if (minInStock > maxInStock)
                    {
                        MainViewModel.IsLoading = true;
                        NotificationDialog notification = new NotificationDialog();
                        notification.Header = "Error";
                        notification.ContentDialog = "Min in stock is bigger than max in stock";
                        await DialogHost.Show(notification, "Main");
                        MainViewModel.IsLoading = false;
                        return;
                    }
                    else
                    {
                        MainViewModel.IsLoading = true;
                        LoadProducts();
                        MainViewModel.IsLoading = false;
                    }
                });
                ResetCommand = new RelayCommandWithNoParameter(() =>
                {
                    MainViewModel.IsLoading = true;
                    ProductNameSearch = "";
                    MinPriceSearch = "";
                    MaxPriceSearch = "";
                    MinInStockSearch = "";
                    MaxInStockSearch = "";
                    BrandSearch = null;
                    CategorySearch = null;
                    LoadProducts();
                    MainViewModel.IsLoading = false;
                }); 
                ProductNameSearch = "";
                StatusAllSearch = true;
                StatusOnSaleSearch = false;
                StatusOutOfStockSearch = false;
                StatusBannedSearch = false;
                while (IsLoadingCheck.IsLoading >= 2) ;
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    Products = new ObservableCollection<Models.Product>(FilterProducts);
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion  
        private void AddProductDialogViewModel_ClosedDialog()
        {
            LoadDataFromModel();
        }

        private void LoadAllProducts()
        {
            AllProducts = new ObservableCollection<Models.Product>();
            foreach (var product in OnSaleProducts)
            {
                AllProducts.Add(product);
            }
            foreach (var product in OutOfStockProducts)
            {
                AllProducts.Add(product);
            }
            foreach (var product in BannedProducts)
            {
                AllProducts.Add(product);
            }
        }
        private void LoadAgainAfterAdding(object sender, DialogClosingEventArgs eventArgs)
        {
            LoadDataFromModel();
        }
        private async void LoadDataFromModel()
        {
            await LoadListProducts();
            LoadFilterProducts();
            LoadProducts();
            OnPropertyChanged(nameof(Products));
        }
        private void LoadFilterProducts()
        {
            if (statusAllSearch)
            {
                FilterProducts = AllProducts;
            }
            else if (statusOnSaleSearch)
            {
                FilterProducts = OnSaleProducts;
            }
            else if (statusOutOfStockSearch)
            {
                FilterProducts = OutOfStockProducts;
            }
            else if (statusBannedSearch)
            {
                FilterProducts = BannedProducts;
            }
        }
        private async Task LoadBrands()
        {
            Brands = new ObservableCollection<Models.Brand>(await BrandRepository.GetListAsync(b => b.Status == "NotBanned"));
        }
        private async Task LoadCategories()
        {
            Categories = new ObservableCollection<Models.Category>(await CategoryRepository.GetListAsync(c => c.Status == "NotBanned"));
        }
        private void LoadProducts()
        {
            if (StatusAllSearch)
            {
                LoadAllProduct();
            }
            else if (StatusOnSaleSearch)
            {
                LoadOnSaleProduct();
            }
            else if (StatusOutOfStockSearch)
            {
                LoadOutOfStockProduct();
            }
            else if (StatusBannedSearch)
            {
                LoadBannedProduct();
            }
        }
        private void LoadOutOfStockProduct()
        {
            double maxPrice = double.MaxValue;
            double minPrice = 0;
            int maxInStock = int.MaxValue;
            int minInStock = 0;
            if (!String.IsNullOrEmpty(MaxPriceSearch))
            {
                double.TryParse(MaxPriceSearch, out maxPrice);
            }
            if (!String.IsNullOrEmpty(MinPriceSearch))
            {
                double.TryParse(MinPriceSearch, out minPrice);
            }
            if (!String.IsNullOrEmpty(MaxInStockSearch))
            {
                int.TryParse(MaxInStockSearch, out maxInStock);
            }
            if (!String.IsNullOrEmpty(MinInStockSearch))
            {
                int.TryParse(MinInStockSearch, out minInStock);
            }
            
            Products = new ObservableCollection<Models.Product>(FilterProducts.Where(p => ((p.Name.ToLower().Trim().Contains(ProductNameSearch == null ? "" : ProductNameSearch.ToLower().Trim()) &&
                                                                                                        ((CategorySearch == null) ? true : p.Category.Id == CategorySearch.Id) &&
                                                                                                        ((BrandSearch == null) ? true : p.Brand.Id == BrandSearch.Id) &&
                                                                                                        p.Price <= maxPrice && p.Price >= minPrice &&
                                                                                                        p.InStock == 0))));
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                Products = new ObservableCollection<Models.Product>(Products);
            }));
            if (Products.Count == 0)
            {
                IsOpenProductInfoPortrait = false;
            }
        }
        private void LoadOnSaleProduct()
        {
            double maxPrice = double.MaxValue;
            double minPrice = 0;
            int maxInStock = int.MaxValue;
            int minInStock = 0;
            if (!String.IsNullOrEmpty(MaxPriceSearch))
            {
                double.TryParse(MaxPriceSearch, out maxPrice);
            }
            if (!String.IsNullOrEmpty(MinPriceSearch))
            {
                double.TryParse(MinPriceSearch, out minPrice);
            }
            if (!String.IsNullOrEmpty(MaxInStockSearch))
            {
                int.TryParse(MaxInStockSearch, out maxInStock);
            }
            if (!String.IsNullOrEmpty(MinInStockSearch))
            {
                int.TryParse(MinInStockSearch, out minInStock);
            }   
            Products = new ObservableCollection<Models.Product>(FilterProducts.Where(p => ((p.Name.ToLower().Trim().Contains(ProductNameSearch == null ? "" : ProductNameSearch.ToLower().Trim()) &&
                                                                                                        ((CategorySearch == null) ? true : p.Category.Id == CategorySearch.Id) &&
                                                                                                        ((BrandSearch == null) ? true : p.Brand.Id == BrandSearch.Id) &&
                                                                                                        p.Price <= maxPrice && p.Price >= minPrice &&
                                                                                                        p.InStock > 0))));
            if (Products.Count == 0)
            {
                IsOpenProductInfoPortrait = false;
            }
        }
        private void LoadAllProduct()
        {
            double maxPrice = double.MaxValue;
            double minPrice = 0;
            int maxInStock = int.MaxValue;
            int minInStock = 0;
            if (!String.IsNullOrEmpty(MaxPriceSearch))
            {
                double.TryParse(MaxPriceSearch, out maxPrice);
            }
            if (!String.IsNullOrEmpty(MinPriceSearch))
            {
                double.TryParse(MinPriceSearch, out minPrice);
            }
            if (!String.IsNullOrEmpty(MaxInStockSearch))
            {
                int.TryParse(MaxInStockSearch, out maxInStock);
            }
            if (!String.IsNullOrEmpty(MinInStockSearch))
            {
                int.TryParse(MinInStockSearch, out minInStock);
            }
            Products = new ObservableCollection<Models.Product>(FilterProducts.Where(p => (p.Name.ToLower().Trim().Contains(ProductNameSearch == null ? "" : ProductNameSearch.ToLower().Trim()) &&
                                                                                                    ((CategorySearch == null) ? true : p.Category.Id == CategorySearch.Id) &&
                                                                                                    ((BrandSearch == null) ? true : p.Brand.Id == BrandSearch.Id) &&
                                                                                                    p.Price <= maxPrice && p.Price >= minPrice &&
                                                                                                    p.InStock <= maxInStock && p.InStock >= minInStock)));
            if (Products.Count == 0)
            {
                IsOpenProductInfoPortrait = false;
            }
        }
        private void LoadBannedProduct()
        {
            double maxPrice = double.MaxValue;
            double minPrice = 0;
            int maxInStock = int.MaxValue;
            int minInStock = 0;
            if (!String.IsNullOrEmpty(MaxPriceSearch))
            {
                double.TryParse(MaxPriceSearch, out maxPrice);
            }
            if (!String.IsNullOrEmpty(MinPriceSearch))
            {
                double.TryParse(MinPriceSearch, out minPrice);
            }
            if (!String.IsNullOrEmpty(MaxInStockSearch))
            {
                int.TryParse(MaxInStockSearch, out maxInStock);
            }
            if (!String.IsNullOrEmpty(MinInStockSearch))
            {
                int.TryParse(MinInStockSearch, out minInStock);
            }
            Products = new ObservableCollection<Models.Product>(FilterProducts.Where(p => ((p.Name.ToLower().Trim().Contains(ProductNameSearch == null ? "" : ProductNameSearch.ToLower().Trim())) &&
                                                                                                        ((CategorySearch == null) ? true : p.Category.Id == CategorySearch.Id) &&
                                                                                                        ((BrandSearch == null) ? true : p.Brand.Id == BrandSearch.Id) &&
                                                                                                        p.Price <= maxPrice && p.Price >= minPrice &&
                                                                                                        p.InStock <= maxPrice && p.InStock >= minPrice &&
                                                                                                        p.BanLevel != 0)));
            if (Products.Count == 0)
            {
                IsOpenProductInfoPortrait = false;
            }
        }
        private async Task LoadListProducts()
        {
            AllProducts = new ObservableCollection<Models.Product>((await ProductRepository.GetListAsync(p => p.IdShop == Shop.Id,
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts)).OrderByDescending(p=>p.BanLevel == 0).
                                                                                                                                ThenByDescending(p=>(p.InStock > 0)).
                                                                                                                                ThenByDescending(p=>p.DateOfSale));
            BannedProducts = new ObservableCollection<Models.Product>(AllProducts.Where(p => p.BanLevel != 0));
            OnSaleProducts = new ObservableCollection<Models.Product>(AllProducts.Where(p => p.BanLevel == 0 && p.InStock > 0));
            OutOfStockProducts = new ObservableCollection<Models.Product>(AllProducts.Where(p => p.BanLevel == 0 && p.InStock == 0));
        }
    }
}
