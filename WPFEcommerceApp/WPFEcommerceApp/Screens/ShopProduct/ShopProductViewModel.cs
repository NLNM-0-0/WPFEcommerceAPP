using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public ICommand OpenAddBrandDialogCommand { get; set; }
        public ICommand OpenAddCategoryDialogCommand { get; set; }
        public ICommand OpenAddProductDialogCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand SearchProductCommand { get; set; }
        private GenericDataRepository<Models.Product> ProductRepository = new GenericDataRepository<Models.Product>();
        private GenericDataRepository<Models.Brand> BrandRepository = new GenericDataRepository<Models.Brand>();
        private GenericDataRepository<Models.Category> CategoryRepository = new GenericDataRepository<Models.Category>();

        private Models.Product selectedProduct;
        public Models.Product SelectedProduct
        {
            get => selectedProduct;
            set
            {
                selectedProduct = value;
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
                if (SelectedProduct == null)
                {
                    return null;
                }
                if (SelectedProduct.Status == "NotBanned")
                {
                    ProductInfoPortraitViewModel productInfoPortraitViewModel = new ProductInfoPortraitViewModel(SelectedProduct);
                    productInfoPortraitViewModel.SelectedProductChanged += LoadDataFromModel;
                    return new MainItem("NotBannedProductInfoPortrait", typeof(ProductInfoPortrait), productInfoPortraitViewModel);
                }
                else
                {
                    ProductInfoPortraitBannedViewModel productInfoPortraitBannedViewModel = new ProductInfoPortraitBannedViewModel(SelectedProduct);
                    return new MainItem("BannedProductInfoPortrait", typeof(ProductInfoPortraitBanned), productInfoPortraitBannedViewModel);
                }
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
        public ShopProductViewModel()
        {
            Shop = AccountStore.instance.CurrentAccount;
            LoadData();
            OpenAddBrandDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
            {
                MainViewModel.IsLoading = true;
                AddBrandDialog addBrandDialog = new AddBrandDialog();
                addBrandDialog.DataContext = new AddBrandDialogViewModel();
                MainViewModel.IsLoading = false;
                await DialogHost.Show(addBrandDialog, "Main");
            });
            OpenAddCategoryDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
            {
                MainViewModel.IsLoading = true;
                AddCategoryDialog addCategoryDialog = new AddCategoryDialog();
                addCategoryDialog.DataContext = new AddCategoryDialogViewModel();
                MainViewModel.IsLoading = false;
                await DialogHost.Show(addCategoryDialog, "Main");
            });
            OpenAddProductDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
            {
                MainViewModel.IsLoading = true;
                AddProductDialog addProductDialog = new AddProductDialog();
                addProductDialog.DataContext = new AddProductDialogViewModel();
                MainViewModel.IsLoading = false;
                await DialogHost.Show(addProductDialog, "Main", LoadAgainAfterAdding); 
            });
            SearchProductCommand = new RelayCommandWithNoParameter(async() =>
            {
                long maxPrice = long.MaxValue;
                long minPrice = 0;
                int maxInStock = int.MaxValue;
                int minInStock = 0;
                if (!String.IsNullOrEmpty(MaxPriceSearch))
                {
                    long.TryParse(MaxPriceSearch, out maxPrice);
                }
                if (!String.IsNullOrEmpty(MinPriceSearch))
                {
                    long.TryParse(MinPriceSearch, out minPrice);
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
            await LoadListBannedProducts();
            await LoadListOutOfStockProducts();
            await LoadListOnSaleProducts();
            LoadAllProducts();
            LoadFilterProducts();
            LoadProducts();
            OnPropertyChanged(nameof(Products));
        }
        private async void LoadData()
        {
            await LoadListBannedProducts();
            await LoadListOutOfStockProducts();
            await LoadListOnSaleProducts();
            await LoadBrands();
            await LoadCategories();
            LoadAllProducts();
            ProductNameSearch = "";
            StatusAllSearch = true;
            StatusOnSaleSearch = false;
            StatusOutOfStockSearch = false;
            StatusBannedSearch = false;
            Products = new ObservableCollection<Models.Product>(FilterProducts);
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
            long maxPrice = long.MaxValue;
            long minPrice = 0;
            int maxInStock = int.MaxValue;
            int minInStock = 0;
            if (!String.IsNullOrEmpty(MaxPriceSearch))
            {
                long.TryParse(MaxPriceSearch, out maxPrice);
            }
            if (!String.IsNullOrEmpty(MinPriceSearch))
            {
                long.TryParse(MinPriceSearch, out minPrice);
            }
            if (!String.IsNullOrEmpty(MaxInStockSearch))
            {
                int.TryParse(MaxInStockSearch, out maxInStock);
            }
            if (!String.IsNullOrEmpty(MinInStockSearch))
            {
                int.TryParse(MinInStockSearch, out minInStock);
            }
            Products = new ObservableCollection<Models.Product>(FilterProducts.Where(p => ((p.Name.Contains(ProductNameSearch ?? "") &&
                                                                                                        ((CategorySearch == null) ? true : p.Category.Id == CategorySearch.Id) &&
                                                                                                        ((BrandSearch == null) ? true : p.Brand.Id == BrandSearch.Id) &&
                                                                                                        p.Price <= maxPrice && p.Price >= minPrice &&
                                                                                                        p.InStock == 0))));
            if (Products.Count == 0)
            {
                IsOpenProductInfoPortrait = false;
            }
        }
        private void LoadOnSaleProduct()
        {
            long maxPrice = long.MaxValue;
            long minPrice = 0;
            int maxInStock = int.MaxValue;
            int minInStock = 0;
            if (!String.IsNullOrEmpty(MaxPriceSearch))
            {
                long.TryParse(MaxPriceSearch, out maxPrice);
            }
            if (!String.IsNullOrEmpty(MinPriceSearch))
            {
                long.TryParse(MinPriceSearch, out minPrice);
            }
            if (!String.IsNullOrEmpty(MaxInStockSearch))
            {
                int.TryParse(MaxInStockSearch, out maxInStock);
            }
            if (!String.IsNullOrEmpty(MinInStockSearch))
            {
                int.TryParse(MinInStockSearch, out minInStock);
            }
            Products = new ObservableCollection<Models.Product>(FilterProducts.Where(p => ((p.Name.Contains(ProductNameSearch ?? "") &&
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
            long maxPrice = long.MaxValue;
            long minPrice = 0;
            int maxInStock = int.MaxValue;
            int minInStock = 0;
            if (!String.IsNullOrEmpty(MaxPriceSearch))
            {
                long.TryParse(MaxPriceSearch, out maxPrice);
            }
            if (!String.IsNullOrEmpty(MinPriceSearch))
            {
                long.TryParse(MinPriceSearch, out minPrice);
            }
            if (!String.IsNullOrEmpty(MaxInStockSearch))
            {
                int.TryParse(MaxInStockSearch, out maxInStock);
            }
            if (!String.IsNullOrEmpty(MinInStockSearch))
            {
                int.TryParse(MinInStockSearch, out minInStock);
            }
            Products = new ObservableCollection<Models.Product>(FilterProducts.Where(p => (p.Name.Contains(ProductNameSearch ?? "") &&
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
            long maxPrice = long.MaxValue;
            long minPrice = 0;
            int maxInStock = int.MaxValue;
            int minInStock = 0;
            if (!String.IsNullOrEmpty(MaxPriceSearch))
            {
                long.TryParse(MaxPriceSearch, out maxPrice);
            }
            if (!String.IsNullOrEmpty(MinPriceSearch))
            {
                long.TryParse(MinPriceSearch, out minPrice);
            }
            if (!String.IsNullOrEmpty(MaxInStockSearch))
            {
                int.TryParse(MaxInStockSearch, out maxInStock);
            }
            if (!String.IsNullOrEmpty(MinInStockSearch))
            {
                int.TryParse(MinInStockSearch, out minInStock);
            }
            Products = new ObservableCollection<Models.Product>(FilterProducts.Where(p => ((p.Name.Contains(ProductNameSearch ?? "")) &&
                                                                                                        ((CategorySearch == null) ? true : p.Category.Id == CategorySearch.Id) &&
                                                                                                        ((BrandSearch == null) ? true : p.Brand.Id == BrandSearch.Id) &&
                                                                                                        p.Price <= maxPrice && p.Price >= minPrice &&
                                                                                                        p.InStock <= maxPrice && p.InStock >= minPrice &&
                                                                                                        p.Status == "Banned")));
            if (Products.Count == 0)
            {
                IsOpenProductInfoPortrait = false;
            }
        }
        private async Task LoadListBannedProducts()
        {
            if(BannedProducts != null ) 
            {
                BannedProducts.Clear();
            }
            BannedProducts = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => p.Status == "Banned" && p.IdShop == Shop.Id,
                                                                                                p => p.Category,
                                                                                                p => p.Brand,
                                                                                                p => p.ImageProducts));
            BannedProducts = new ObservableCollection<Models.Product>(BannedProducts.OrderByDescending(p => p.DateOfSale));
        }
        private async Task LoadListOnSaleProducts()
        {
            OnSaleProducts = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => p.Status != "Banned" && p.InStock > 0 && p.IdShop == Shop.Id,
                                                                                                p => p.Category,
                                                                                                p => p.Brand,
                                                                                                p => p.ImageProducts));
            OnSaleProducts = new ObservableCollection<Models.Product>(OnSaleProducts.OrderByDescending(p => p.DateOfSale));
        }
        private async Task LoadListOutOfStockProducts()
        {
            OutOfStockProducts = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => p.Status != "Banned" && p.InStock == 0 && p.IdShop == Shop.Id,
                                                                                                p => p.Category,
                                                                                                p => p.Brand,
                                                                                                p => p.ImageProducts));
            OutOfStockProducts = new ObservableCollection<Models.Product>(OutOfStockProducts.OrderByDescending(p => p.DateOfSale));
        }
    }
}
