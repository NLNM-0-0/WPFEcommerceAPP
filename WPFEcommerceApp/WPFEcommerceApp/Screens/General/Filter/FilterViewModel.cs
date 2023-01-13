using DataAccessLayer;
using MaterialDesignThemes.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using WPFEcommerceApp.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WPFEcommerceApp
{
    internal class FilterViewModel : BaseViewModel
    {

        public GenericDataRepository<Models.Product> ProductRepository { get; set; }
        private ObservableCollection<Models.Product> products;
        public ICommand SearchCommand { get; set; }
        public ObservableCollection<Models.Product> Products
        {
            get { return products; }
            set
            {
                products = value;
                OnPropertyChanged();
            }

        }
        private ObservableCollection<ProductBlockViewModel> allProducts;
        public ObservableCollection<ProductBlockViewModel> AllProducts
        {
            get { return allProducts; }
            set
            {
                allProducts = value;
                OnPropertyChanged();
            }

        }
        private ObservableCollection<ProductBlockViewModel> bigDiscountProducts;
        public ObservableCollection<ProductBlockViewModel> BigDiscountProducts
        {
            get { return bigDiscountProducts; }
            set
            {
                bigDiscountProducts = value;
                OnPropertyChanged();
            }

        }
        private ObservableCollection<ProductBlockViewModel> bestSellerProducts;
        public ObservableCollection<ProductBlockViewModel> BestSellerProducts
        {
            get { return bestSellerProducts; }
            set
            {
                bestSellerProducts = value;
                OnPropertyChanged();
            }

        }
        private ObservableCollection<ProductBlockViewModel> newProducts;
        public ObservableCollection<ProductBlockViewModel> NewProducts
        {
            get { return newProducts; }
            set
            {
                newProducts = value;
                OnPropertyChanged();
            }

        }
        private ObservableCollection<ProductBlockViewModel> filterProducts;
        public ObservableCollection<ProductBlockViewModel> FilterProducts
        {
            get { return filterProducts; }
            set
            {
                filterProducts = value;
                OnPropertyChanged();
            }

        }
        private ObservableCollection<ProductBlockViewModel> displayedProducts;
        public ObservableCollection<ProductBlockViewModel> DisplayedProducts
        {
            get { return displayedProducts; }
            set
            {
                displayedProducts = value;
                OnPropertyChanged();
            }

        }
        private bool newProduct;
        public bool NewProdcut
        {
            get => newProduct;
            set
            {
                newProduct = value;
                OnPropertyChanged();
            }
        }

        private bool isCheckCategory;
        public bool IsCheckCategory
        {
            get => isCheckCategory;
            set
            {
                isCheckCategory = value;
                if (value)
                {
                    Task task = Task.Run(async () => await LoadCategory());
                    while (!task.IsCompleted) ;
                    Search();
                }
            }
        }

        private bool isCheckBrand;
        public bool IsCheckBrand
        {
            get => isCheckBrand;
            set
            {
                isCheckBrand = value;
                if (value)
                {
                    Task task = Task.Run(async () => await LoadBrand());
                    while (!task.IsCompleted) ;
                    Search();
                }
            }
        }
        private bool sortPrice0To200k;
        public bool SortPrice0To200k
        {
            get => sortPrice0To200k;
            set
            {
                if (value)
                {
                    MinPrice = 0;
                    MaxPrice = 200000;
                    Search();
                }
                sortPrice0To200k = value;
                OnPropertyChanged();
            }
        }
        private bool sortPrice200kTo500k;
        public bool SortPrice200kTo500k
        {
            get => sortPrice200kTo500k;
            set
            {
                if (value)
                {
                    MinPrice = 200000;
                    MaxPrice = 500000;
                    Search();
                }
                sortPrice200kTo500k = value;
                OnPropertyChanged();
            }
        }
        private bool sortPrice500kTo1000k;
        public bool SortPrice500kTo1000k
        {
            get => sortPrice500kTo1000k;
            set
            {
                if (value)
                {
                    MinPrice = 500000;
                    MaxPrice = 1000000;
                    Search();
                }
                sortPrice500kTo1000k = value;
                OnPropertyChanged();
            }
        }
        private bool sortPriceP1000k;
        public bool SortPriceP1000k
        {
            get => sortPriceP1000k;
            set
            {
                if (value)
                {
                    MinPrice = 1000000;
                    MaxPrice = long.MaxValue;
                    Search();
                }
                sortPriceP1000k = value;
                OnPropertyChanged();
            }
        }
        private bool isBigDiscount;
        public bool IsBigDiscount
        {
            get => isBigDiscount;
            set
            {
                isBigDiscount = value;
                if (value)
                {
                    FilterProducts = BigDiscountProducts;
                    Search();
                }
            }
        }
        private bool isNew;
        public bool IsNew
        {
            get => isNew;
            set
            {
                isNew = value;
                if (value)
                {
                    FilterProducts = NewProducts;
                    Search();
                }
            }
        }
        private bool isBestSeller;
        public bool IsBestSeller
        {
            get => isBestSeller;
            set
            {
                isBestSeller = value;
                if (value)
                {
                    FilterProducts = BestSellerProducts;
                    Search();
                }
            }
        }
        private bool isAllFilter;
        public bool IsAllFilter
        {
            get => isAllFilter;
            set
            {
                isAllFilter = value;
                if (value)
                {
                    FilterProducts = AllProducts;
                    Search();
                }
            }
        }
        private bool isHadOneSize;
        public bool IsHadOneSize
        {
            get => isHadOneSize;
            set
            {
                isHadOneSize = value;
                CheckSize[0] = value;
                Search();
                OnPropertyChanged();
            }
        }
        private bool isHadSizeS;
        public bool IsHadSizeS
        {
            get => isHadSizeS;
            set
            {
                isHadSizeS = value;
                CheckSize[1] = value;
                Search();
                OnPropertyChanged();
            }
        }
        private bool isHadSizeM;
        public bool IsHadSizeM
        {
            get => isHadSizeM;
            set
            {
                    
                isHadSizeM = value;
                CheckSize [2] = value;
                Search();
                OnPropertyChanged();
            }
        }
        private bool isHadSizeL;
        public bool IsHadSizeL
        {
            get => isHadSizeL;
            set
            {
                isHadSizeL = value;
                CheckSize[3] = value;
                Search();
                OnPropertyChanged();
            }
        }
        private bool isHadSizeXL;
        public bool IsHadSizeXL
        {
            get => isHadSizeXL;
            set
            {
                isHadSizeXL = value;
                CheckSize[4] = value;
                Search();
                OnPropertyChanged();
            }
        }
        private bool isHadSizeXXL;
        public bool IsHadSizeXXL
        {
            get => isHadSizeXXL;
            set
            {
                isHadSizeXXL = value;
                CheckSize[5] = value;
                Search();
                OnPropertyChanged();
            }
        }
        private long minPrice = long.MinValue;
        public long MinPrice
        {
            get => minPrice;
            set
            {
                minPrice = value;
                OnPropertyChanged();
            }
        }
        private long maxPrice = long.MaxValue;
        public long MaxPrice
        {
            get => maxPrice;
            set
            {
                maxPrice = value;
                OnPropertyChanged();
            }
        }
        public bool[] CheckSize = { false, false, false, false, false, false };
        private FilterObject Condition;
        public FilterViewModel(FilterObject filterObject)
        {
            Condition = filterObject;
            if (Condition.Categories == null)
            {
                Condition.Categories = new List<string>();
            }
            if (Condition.Brands == null)
            {
                Condition.Brands = new List<string>();
            }
            if (Condition.ShopText == null)
            {
                Condition.ShopText = "";
            }
            IsLoadingCheck.IsLoading = 2;
            Task.Run(async () =>
            {
                AllProducts = new ObservableCollection<ProductBlockViewModel>();
                BestSellerProducts = new ObservableCollection<ProductBlockViewModel>();
                BigDiscountProducts = new ObservableCollection<ProductBlockViewModel>();
                NewProducts = new ObservableCollection<ProductBlockViewModel>();
                DisplayedProducts = new ObservableCollection<ProductBlockViewModel>();
                FilterProducts = new ObservableCollection<ProductBlockViewModel>();
                ProductRepository = new GenericDataRepository<Models.Product>();
                CategoryCheckBoxViewModels = new ObservableCollection<CategoryCheckBoxViewModel>();
                BrandCheckBoxViewModels = new ObservableCollection<BrandCheckViewModel>();
                await Load();
                await LoadCategoryCheckBox();
                await LoadBrandCheckBox();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            }).ContinueWith((first) =>
            {
                if (Condition.Status == FilterStatus.All)
                {
                    IsAllFilter = true;
                }
                else if (Condition.Status == FilterStatus.BigDiscount)
                {
                    IsBigDiscount = true;
                }
                else if (Condition.Status == FilterStatus.BestSeller)
                {
                    IsBestSeller = true;
                }
                else if (Condition.Status == FilterStatus.New)
                {
                    IsNew = true;
                }
                else
                {
                    IsAllFilter = true;
                }
                SearchCommand = new RelayCommandWithNoParameter(() =>
                {
                    MainViewModel.IsLoading = true;
                    Task.Run(() => { }).ContinueWith((second) =>
                    {
                        Search();
                        MainViewModel.IsLoading = false;
                    });
                });
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            });
        }
        private void Search()
        {
            List<string> listCategoryId = new List<string>();
            List<string> allCategoryId = new List<string>();
            foreach (CategoryCheckBoxViewModel categoryCheckBoxViewModel in CategoryCheckBoxViewModels)
            {
                if (categoryCheckBoxViewModel.IsChecked)
                {
                    listCategoryId.Add(categoryCheckBoxViewModel.Category.Id);
                }
                allCategoryId.Add(categoryCheckBoxViewModel.Category.Id);
            }
            if (listCategoryId.Count == 0)
            {
                listCategoryId = allCategoryId;
            }
            List<string> listBrandId = new List<string>();
            List<string> allBrandId = new List<string>();
            foreach (BrandCheckViewModel brandCheckViewModel in BrandCheckBoxViewModels)
            {
                if (brandCheckViewModel.IsChecked)
                {
                    listBrandId.Add(brandCheckViewModel.Brand.Id);
                }
                allBrandId.Add(brandCheckViewModel.Brand.Id);
            }
            if (listBrandId.Count == 0)
            {
                listBrandId = allBrandId;
            }
            bool isHasSize = false; 
            foreach(bool size in CheckSize)
            {
                if(size)
                {
                    isHasSize = true;
                    break;
                }    
            }
            if (isHasSize)
            {
                DisplayedProducts = new ObservableCollection<ProductBlockViewModel>( FilterProducts.Where(p => (p.SelectedProduct.BanLevel == 0 &&
                                                                                                        (p.SelectedProduct.Price * (100 - p.SelectedProduct.Sale) / 100 <= MaxPrice && p.SelectedProduct.Price * (100 - p.SelectedProduct.Sale) / 100 >= MinPrice) &&
                                                                                                        (listCategoryId.Contains(p.SelectedProduct.IdCategory)) && (listBrandId.Contains(p.SelectedProduct.IdBrand)) && ((CheckSize[0] && (p.SelectedProduct.IsOneSize == CheckSize[0])) ||
                                                                                                        ((CheckSize[1] && p.SelectedProduct.IsHadSizeS == CheckSize[1])) || (CheckSize[2] && (p.SelectedProduct.IsHadSizeM == CheckSize[2])) || (CheckSize[3] && (p.SelectedProduct.IsHadSizeL == CheckSize[3])) ||
                                                                                                        (CheckSize[4] && (p.SelectedProduct.IsHadSizeXL == CheckSize[4])) || (CheckSize[5] && (p.SelectedProduct.IsHadSizeXXL == CheckSize[5]))))));
            }
            else
            {
                DisplayedProducts = new ObservableCollection<ProductBlockViewModel>(FilterProducts.Where(p => (p.SelectedProduct.BanLevel == 0 &&
                                                                                                        (p.SelectedProduct.Price * (100 - p.SelectedProduct.Sale) / 100 <= MaxPrice && p.SelectedProduct.Price * (100 - p.SelectedProduct.Sale) / 100 >= MinPrice) &&
                                                                                                        (listCategoryId.Contains(p.SelectedProduct.IdCategory)) && (listBrandId.Contains(p.SelectedProduct.IdBrand)))));
            }
        }
        private async Task Load()
        {
            if (String.IsNullOrEmpty(Condition.ShopText))
            {
                Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => p.BanLevel == 0 && p.Name.Contains(Condition.SearchText ?? ""),
                                                                                                            p => p.Category,
                                                                                                            p => p.Brand,
                                                                                                            p => p.ImageProducts,
                                                                                                            p => p.MUser));
            }
            else
            {
                Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => p.BanLevel == 0 && p.Name.Contains(Condition.SearchText ?? "") && p.IdShop == Condition.ShopText,
                                                                                                            p => p.Category,
                                                                                                            p => p.Brand,
                                                                                                            p => p.ImageProducts,
                                                                                                            p => p.MUser));
            }    
            DateTime lastDateHasNewProduct = DateTime.MinValue;
            foreach (Models.Product product in Products)
            {
                ProductBlockViewModel productBlockViewModel = new ProductBlockViewModel(product);
                AllProducts.Add(productBlockViewModel);
                if (product.DateOfSale > lastDateHasNewProduct)
                {
                    lastDateHasNewProduct = product.DateOfSale ?? DateTime.MinValue;
                }
            }
            if (lastDateHasNewProduct != DateTime.MinValue)
            {
                NewProducts = new ObservableCollection<ProductBlockViewModel>(AllProducts.Where(p => (lastDateHasNewProduct - p.SelectedProduct.DateOfSale) < new TimeSpan(7, 0, 0, 0)).Take(50));
            }
            else
            {
                NewProducts = new ObservableCollection<ProductBlockViewModel>();
            }
            BestSellerProducts = new ObservableCollection<ProductBlockViewModel>(AllProducts.OrderByDescending(p => p.SelectedProduct.Sold).Take(50));
            BigDiscountProducts = new ObservableCollection<ProductBlockViewModel>(AllProducts.OrderByDescending(p => p.SelectedProduct.Sale).Take(50));
        }
        private async Task LoadCategory()
        { 
            List<string> hellos = new List<string>();
            foreach (CategoryCheckBoxViewModel categoryCheckBoxViewModel in CategoryCheckBoxViewModels)
            {
                if (categoryCheckBoxViewModel.IsChecked)
                {
                    hellos.Add(categoryCheckBoxViewModel.Category.Name);
                }
            }
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => hellos.Contains(p.Category.Name),
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts,
                                                                                                        p => p.MUser));
        }

        private async Task LoadBrand()
        {
            MainViewModel.IsLoading = true;
            List<string> hellos = new List<string>();
            foreach (BrandCheckViewModel brandCheckViewModel in BrandCheckBoxViewModels)
            {
                if (brandCheckViewModel.IsChecked)
                {
                    hellos.Add(brandCheckViewModel.Brand.Name);
                }
            }
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => hellos.Contains(p.Brand.Name),
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts,
                                                                                                        p => p.MUser));
            MainViewModel.IsLoading = false;
        }
        private ObservableCollection<CategoryCheckBoxViewModel> categoryCheckBoxViewModels;
        public ObservableCollection<CategoryCheckBoxViewModel> CategoryCheckBoxViewModels
        {
            get => categoryCheckBoxViewModels;
            set
            {
                categoryCheckBoxViewModels = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<BrandCheckViewModel> brandCheckBoxViewModels;
        public ObservableCollection<BrandCheckViewModel> BrandCheckBoxViewModels
        {
            get => brandCheckBoxViewModels;
            set
            {
                brandCheckBoxViewModels = value;
                OnPropertyChanged();
            }
        }
        private async Task LoadCategoryCheckBox()
        { 
            GenericDataRepository<Models.Category> genericDataRepository = new GenericDataRepository<Models.Category>();
            ObservableCollection<Models.Category> categories = new ObservableCollection<Models.Category>(await genericDataRepository.GetListAsync(p => p.Status != "Banned"));
            foreach (Models.Category category in categories)
            {
                CategoryCheckBoxViewModel categoryCheckBoxViewModel = new CategoryCheckBoxViewModel();
                categoryCheckBoxViewModel.Category = category;
                categoryCheckBoxViewModel.IsChecked = Condition.Categories.Any(c => c == category.Id);
                CategoryCheckBoxViewModels.Add(categoryCheckBoxViewModel);
            }
        }
        private async Task LoadBrandCheckBox()
        {
            GenericDataRepository<Models.Brand> genericDataRepository = new GenericDataRepository<Models.Brand>();
            ObservableCollection<Models.Brand> brands = new ObservableCollection<Models.Brand>(await genericDataRepository.GetListAsync(p => p.Status != "Banned"));
            foreach (Models.Brand brand in brands)
            {
                BrandCheckViewModel brandCheckViewModel = new BrandCheckViewModel();
                brandCheckViewModel.Brand = brand;
                brandCheckViewModel.IsChecked = Condition.Brands.Any(b => b == brand.Id);
                BrandCheckBoxViewModels.Add(brandCheckViewModel);
            }
        }
    }
    public enum FilterStatus
    {
        BigDiscount,
        New,
        BestSeller,
        All
    }
    public class FilterObject
    {
        public string SearchText { get; set; } 
        public string ShopText { get; set; } 
        public List<string> Categories { get; set; } 
        public List<string> Brands { get; set; }
        public FilterStatus Status  { get; set; }
        public FilterObject(string searchText, string shopText, List<string> categories, List<string> brands, FilterStatus status)
        {
            SearchText = searchText;
            ShopText = shopText;
            Categories = categories;
            Brands = brands;
            Status = status;
        }
    }
}
