using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
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
        public ICommand DoubleClickCommand { get; set; }
        #endregion
        #region DataRepository
        private GenericDataRepository<Models.Product> ProductRepository = new GenericDataRepository<Models.Product>();
        private GenericDataRepository<Models.Brand> BrandRepository = new GenericDataRepository<Models.Brand>();
        private GenericDataRepository<Models.Category> CategoryRepository = new GenericDataRepository<Models.Category>();
        #endregion
        #region Field
        private ProductViewModel selectedProduct;
        public ProductViewModel SelectedProduct
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
        private ObservableCollection<ProductViewModel> products;
        public ObservableCollection<ProductViewModel> Products
        {
            get => products;
            set
            {
                products = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ProductViewModel> AllProducts { get; set; }
        private ObservableCollection<ProductViewModel> OnSaleProducts { get; set; }
        private ObservableCollection<ProductViewModel> OutOfStockProducts { get; set; }
        private ObservableCollection<ProductViewModel> BannedProducts { get; set; }
        private ObservableCollection<ProductViewModel> FilterProducts { get; set; }
        public MainItem StyleProductInfoPortrait
        {
            get
            {
                MainItem mainItem = null;
                if (SelectedProduct == null || !IsOpenProductInfoPortrait)
                {
                    return mainItem;
                }
                if (SelectedProduct.Product.BanLevel == 0)
                {
                    IsLoadingCheck.IsLoading = 3;
                    ProductInfoPortraitViewModel productInfoPortraitViewModel = new ProductInfoPortraitViewModel(SelectedProduct.Product);
                    productInfoPortraitViewModel.SelectedProductChanged += ProductInfoPortraitViewModel_SelectedProductChanged;
                    mainItem = new MainItem("NotBannedProductInfoPortrait", typeof(ProductInfoPortrait), productInfoPortraitViewModel);
                    IsLoadingCheck.IsLoading--;
                }
                else
                {
                    IsLoadingCheck.IsLoading = 2;
                    ProductInfoPortraitBannedViewModel productInfoPortraitBannedViewModel = new ProductInfoPortraitBannedViewModel(SelectedProduct.Product);
                    mainItem = new MainItem("BannedProductInfoPortrait", typeof(ProductInfoPortraitBanned), productInfoPortraitBannedViewModel);
                    IsLoadingCheck.IsLoading--;
                }
                return mainItem;
            }
        }

        private void ProductInfoPortraitViewModel_SelectedProductChanged()
        {
            if(SelectedProduct.Product.InStock == 0)
            {
                if(!OutOfStockProducts.Contains(SelectedProduct))
                {
                    OutOfStockProducts.Add(SelectedProduct);
                    OnSaleProducts.Remove(SelectedProduct);
                }
            }
            else
            {
                if (!OnSaleProducts.Contains(SelectedProduct))
                {
                    OnSaleProducts.Add(SelectedProduct);
                    OutOfStockProducts.Remove(SelectedProduct);
                }
            }
            string id = SelectedProduct.Product.Id;
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                Products = new ObservableCollection<ProductViewModel>(Products);
            }));
            SelectedProduct = Products.Where(p => p.Product.Id.Equals(id)).First();
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
                if (value == true)
                {
                    FilterProducts = AllProducts??new ObservableCollection<ProductViewModel>();
                    LoadProducts();
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
                    FilterProducts = OnSaleProducts ?? new ObservableCollection<ProductViewModel>();
                    LoadProducts();
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
                    FilterProducts = OutOfStockProducts ?? new ObservableCollection<ProductViewModel>();
                    LoadProducts();
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
                    FilterProducts = BannedProducts ?? new ObservableCollection<ProductViewModel>();
                    LoadProducts();
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
                    MainViewModel.SetLoading(true);
                    AddBrandDialog addBrandDialog = new AddBrandDialog();
                    addBrandDialog.DataContext = new AddBrandDialogViewModel(null);
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(addBrandDialog, "Main");
                });
                OpenAddCategoryDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
                {
                    MainViewModel.SetLoading(true);
                    AddCategoryDialog addCategoryDialog = new AddCategoryDialog();
                    addCategoryDialog.DataContext = new AddCategoryDialogViewModel(null);
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(addCategoryDialog, "Main");
                });
                OpenAddProductDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
                {
                    MainViewModel.SetLoading(true);
                    AddProductDialog addProductDialog = new AddProductDialog();
                    AddProductDialogViewModel addProductDialogViewModel = new AddProductDialogViewModel();
                    addProductDialogViewModel.ClosedDialog += AddProductDialogViewModel_ClosedDialog;
                    addProductDialog.DataContext = addProductDialogViewModel;
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(addProductDialog, "Main");
                });
                SearchProductCommand = new RelayCommand<bool?>((p) => 
                {
                    return p != null && p == true;
                }, async(p)=>
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
                        MainViewModel.SetLoading(true);
                        NotificationDialog notification = new NotificationDialog();
                        notification.Header = "Error";
                        notification.ContentDialog = "Min price is bigger than max price";
                        MainViewModel.SetLoading(false);
                        await DialogHost.Show(notification, "Main");
                        return;
                    }
                    else if (minInStock > maxInStock)
                    {
                        MainViewModel.SetLoading(true);
                        NotificationDialog notification = new NotificationDialog();
                        notification.Header = "Error";
                        notification.ContentDialog = "Min in stock is bigger than max in stock";
                        MainViewModel.SetLoading(false);
                        await DialogHost.Show(notification, "Main");
                        return;
                    }
                    else
                    {
                        MainViewModel.SetLoading(true);
                        LoadProducts();
                        MainViewModel.SetLoading(false);
                    }
                });
                ResetCommand = new RelayCommandWithNoParameter(() =>
                {
                    MainViewModel.SetLoading(true);
                    ProductNameSearch = "";
                    MinPriceSearch = "";
                    MaxPriceSearch = "";
                    MinInStockSearch = "";
                    MaxInStockSearch = "";
                    BrandSearch = null;
                    CategorySearch = null;
                    LoadProducts();
                    MainViewModel.SetLoading(false);
                });
                DoubleClickCommand = new RelayCommandWithNoParameter(() => { });
                ProductNameSearch = "";
                StatusAllSearch = true;
                StatusOnSaleSearch = false;
                StatusOutOfStockSearch = false;
                StatusBannedSearch = false;
                while (IsLoadingCheck.IsLoading >= 2) ;
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    Products = new ObservableCollection<ProductViewModel>(FilterProducts);
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

            Products = new ObservableCollection<ProductViewModel>(FilterProducts.Where(p => ((p.Product.Name.ToLower().Trim().Contains(ProductNameSearch == null ? "" : ProductNameSearch.ToLower().Trim()) &&
                                                                                                        ((CategorySearch == null) ? true : p.Product.Category.Id == CategorySearch.Id) &&
                                                                                                        ((BrandSearch == null) ? true : p.Product.Brand.Id == BrandSearch.Id) &&
                                                                                                        p.Product.Price <= maxPrice && p.Product.Price >= minPrice &&
                                                                                                        p.Product.InStock >= minInStock && p.Product.InStock <= maxInStock))));
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                Products = new ObservableCollection<ProductViewModel>(Products);
            }));
            if (Products.Count == 0)
            {
                IsOpenProductInfoPortrait = false;
            }
        }

        private async Task LoadListProducts()
        {
            ObservableCollection<Models.Product> listProducts = new ObservableCollection<Models.Product>((await ProductRepository.GetListAsync(p => p.IdShop == Shop.Id,
                                                                                                                                                p => p.Category,
                                                                                                                                                p => p.Brand,
                                                                                                                                                p => p.ImageProducts, 
                                                                                                                                                p => p.MUser)).OrderByDescending(p => p.BanLevel == 0).
                                                                                                                                                                ThenByDescending(p => (p.InStock > 0)).
                                                                                                                                                                ThenByDescending(p => p.DateOfSale));
            AllProducts = new ObservableCollection<ProductViewModel>();
            BannedProducts = new ObservableCollection<ProductViewModel>();
            OnSaleProducts = new ObservableCollection<ProductViewModel>();
            OutOfStockProducts = new ObservableCollection<ProductViewModel>();
            foreach(Models.Product product in listProducts)
            {
                ProductViewModel productViewModel = new ProductViewModel(product);
                AllProducts.Add(productViewModel);
                if(product.BanLevel == 0)
                {
                    if(product.InStock > 0)
                    {
                        OnSaleProducts.Add(productViewModel);
                    }
                    else
                    {
                        OutOfStockProducts.Add(productViewModel);
                    }
                }
                else
                {
                    BannedProducts.Add(productViewModel);
                }
            }
        }
    }
    public class MImageProuct
    {
        public BitmapImage BMImage { get; set; }
        public string Source { get; set; }
    }
    public class ProductViewModel : BaseViewModel
    {
        private Models.Product product;
        public Models.Product Product
        {
            get => product;
            set
            {
                product = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(StatusColor));
                OnPropertyChanged(nameof(StatusIndex));
            }
        }
        public string Status
        {
            get
            {
                if (product == null)
                {
                    return "Error";
                }
                if (product.BanLevel == 0)
                {
                    if (product.InStock > 0)
                    {
                        return "On Sale";
                    }
                    else
                    {
                        return "Out Of Stock";
                    }
                }
                else
                {
                    return "Banned";
                }
            }
        }
        public int StatusIndex
        {
            get
            {
                if (product == null)
                {
                    return 0;
                }
                if (product.BanLevel == 0)
                {
                    if (product.InStock > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    return 3;
                }
            }
        }
        public System.Windows.Media.Brush StatusColor
        {
            get
            {
                if (product == null)
                {
                    return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
                }
                else
                {
                    if (product.BanLevel == 0)
                    {
                        if (product.InStock > 0)
                        {
                            return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(42, 169, 82));
                        }
                        else
                        {
                            return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(253, 197, 0));
                        }
                    }
                    else
                    {
                        return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(219, 48, 34));
                    }
                }
            }
        }
        public ProductViewModel() { }
        public ProductViewModel(Models.Product product)
        {
            Product = product;
        }
    }
}
