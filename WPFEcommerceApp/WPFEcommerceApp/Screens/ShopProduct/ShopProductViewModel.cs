using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace WPFEcommerceApp
{
    public class ShopProductViewModel : BaseViewModel
    {
        public ICommand OpenAddBrandDialogCommand { get; set; }
        public ICommand OpenAddCategoryDialogCommand { get; set; }
        public ICommand OpenAddProductDialogCommand { get; set; }

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
        private IList<Models.Brand> brands;
        public IList<Models.Brand> Brands
        {
            get => brands;
            set
            {
                brands = value;
                OnPropertyChanged();
            }
        }
        private IList<Models.Category> categories;
        public IList<Models.Category> Categories
        {
            get => categories;
            set
            {
                categories = value;
                OnPropertyChanged();
            }
        }
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
        private string categorySearch;
        public string CategorySearch
        {
            get => categorySearch;
            set
            {
                categorySearch = value;
                OnPropertyChanged();
            }
        }
        private string brandSearch;
        public string BrandSearch
        {
            get => brandSearch;
            set
            {
                brandSearch = value;
                OnPropertyChanged();
            }
        }
        public ShopProductViewModel(Models.MUser shop)
        {
            Shop = shop;
            Task task = Task.Run(async () => { await LoadBrands(); await LoadCategories(); });
            while(!task.IsCompleted);
            OpenAddBrandDialogCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                AddBrandDialog addBrandDialog = new AddBrandDialog();
                addBrandDialog.DataContext = new AddBrandDialogViewModel();
                DialogHost.Show(addBrandDialog);
            });
            OpenAddCategoryDialogCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                AddCategoryDialog addCategoryDialog = new AddCategoryDialog();
                addCategoryDialog.DataContext = new AddCategoryDialogViewModel();
                DialogHost.Show(addCategoryDialog);
            });
            OpenAddProductDialogCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                AddProductDialog addProductDialog = new AddProductDialog();
                addProductDialog.DataContext = new AddProductDialogViewModel();
                DialogHost.Show(addProductDialog);
            });
        }
        private async Task LoadBrands()
        {
            var repository = new GenericDataRepository<Models.Brand>();
            Brands = await repository.GetAllAsync();
        }
        private async Task LoadCategories()
        {
            var repository = new GenericDataRepository<Models.Category>();
            Categories = await repository.GetAllAsync();
        }
    }
}
