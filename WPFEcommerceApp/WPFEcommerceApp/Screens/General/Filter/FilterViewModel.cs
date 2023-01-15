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
        public ICommand SearchCommand { get; set; }
        public ICommand OpenAllBrands { get; set; }
        public ICommand OpenAllCategories { get; set; }

        private IList<ProductBlockViewModel> allProducts;
        public IList<ProductBlockViewModel> AllProducts
        {
            get { return allProducts; }
            set
            {
                allProducts = value;
                OnPropertyChanged();
            }

        }
        private IList<ProductBlockViewModel> bigDiscountProducts;
        public IList<ProductBlockViewModel> BigDiscountProducts
        {
            get { return bigDiscountProducts; }
            set
            {
                bigDiscountProducts = value;
                OnPropertyChanged();
            }

        }
        private IList<ProductBlockViewModel> bestSellerProducts;
        public IList<ProductBlockViewModel> BestSellerProducts
        {
            get { return bestSellerProducts; }
            set
            {
                bestSellerProducts = value;
                OnPropertyChanged();
            }

        }
        private IList<ProductBlockViewModel> newProducts;
        public IList<ProductBlockViewModel> NewProducts
        {
            get { return newProducts; }
            set
            {
                newProducts = value;
                OnPropertyChanged();
            }

        }
        private IList<ProductBlockViewModel> filterProducts;
        public IList<ProductBlockViewModel> FilterProducts
        {
            get { return filterProducts; }
            set
            {
                filterProducts = value;
                OnPropertyChanged();
            }

        }
        private IList<ProductBlockViewModel> displayedProducts;
        public IList<ProductBlockViewModel> DisplayedProducts
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
                    MainViewModel.IsLoading = true;
                    Task.Run(() => { }).ContinueWith((second) =>
                    {
                        Search();
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                        }));
                        MainViewModel.IsLoading = false;
                    });
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
                    MainViewModel.IsLoading = true;
                    Task.Run(() => { }).ContinueWith((second) =>
                    {
                        Search();
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                        }));
                        MainViewModel.IsLoading = false;
                    });
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
                    MainViewModel.IsLoading = true;
                    Task.Run(() => { }).ContinueWith((second) =>
                    {
                        Search();
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                        }));
                        MainViewModel.IsLoading = false;
                    });
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
                    MainViewModel.IsLoading = true;
                    Task.Run(() => { }).ContinueWith((second) =>
                    {
                        Search();
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                        }));
                        MainViewModel.IsLoading = false;
                    });
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
                    MainViewModel.IsLoading = true;
                    Task.Run(() => { }).ContinueWith((second) =>
                    {
                        Search();
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                        }));
                        MainViewModel.IsLoading = false;
                    });
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
                    MainViewModel.IsLoading = true;
                    Task.Run(() => { }).ContinueWith((second) =>
                    {
                        Search();
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                        }));
                        MainViewModel.IsLoading = false;
                    });
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
                    MainViewModel.IsLoading = true;
                    Task.Run(() => { }).ContinueWith((second) =>
                    {
                        Search();
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                        }));
                        MainViewModel.IsLoading = false;
                    });
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
                    MainViewModel.IsLoading = true;
                    Task.Run(() => { }).ContinueWith((second) =>
                    {
                        Search();
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                        }));
                        MainViewModel.IsLoading = false;
                    });
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
                MainViewModel.IsLoading = true;
                Task.Run(() => { }).ContinueWith((second) =>
                {
                    Search();
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                    }));
                    MainViewModel.IsLoading = false;
                });
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
                MainViewModel.IsLoading = true;
                Task.Run(() => { }).ContinueWith((second) =>
                {
                    Search();
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                    }));
                    MainViewModel.IsLoading = false;
                });
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
                CheckSize[2] = value;
                MainViewModel.IsLoading = true;
                Task.Run(() => { }).ContinueWith((second) =>
                {
                    Search();
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                    }));
                    MainViewModel.IsLoading = false;
                });
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
                MainViewModel.IsLoading = true;
                Task.Run(() => { }).ContinueWith((second) =>
                {
                    Search();
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                    }));
                    MainViewModel.IsLoading = false;
                });
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
                MainViewModel.IsLoading = true;
                Task.Run(() => { }).ContinueWith((second) =>
                {
                    Search();
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                    }));
                    MainViewModel.IsLoading = false;
                });
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
                MainViewModel.IsLoading = true;
                Task.Run(() => { }).ContinueWith((second) =>
                {
                    Search();
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                    }));
                    MainViewModel.IsLoading = false;
                });
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
        private IList<CategoryCheckBoxViewModel> categoryCheckBoxViewModels;
        public IList<CategoryCheckBoxViewModel> CategoryCheckBoxViewModels
        {
            get => categoryCheckBoxViewModels;
            set
            {
                categoryCheckBoxViewModels = value;
                OnPropertyChanged();
            }
        }
        private IList<CategoryCheckBoxViewModel> dislayedCategoryCheckBoxViewModels;
        public IList<CategoryCheckBoxViewModel> DislayedCategoryCheckBoxViewModels
        {
            get => dislayedCategoryCheckBoxViewModels;
            set
            {
                dislayedCategoryCheckBoxViewModels = value;
                OnPropertyChanged();
            }
        }
        private IList<BrandCheckViewModel> brandCheckBoxViewModels;
        public IList<BrandCheckViewModel> BrandCheckBoxViewModels
        {
            get => brandCheckBoxViewModels;
            set
            {
                brandCheckBoxViewModels = value;
                OnPropertyChanged();
            }
        }
        private IList<BrandCheckViewModel> dislayedBrandCheckBoxViewModels;
        public IList<BrandCheckViewModel> DislayedBrandCheckBoxViewModels
        {
            get => dislayedBrandCheckBoxViewModels;
            set
            {
                dislayedBrandCheckBoxViewModels = value;
                OnPropertyChanged();
            }
        }
        public bool[] CheckSize = { false, false, false, false, false, false };
        private string searchBy;
        public string SearchBy
        {
            get => searchBy;
            set
            {
                searchBy = value;
                OnPropertyChanged();
            }
        }
        private string searchByValue;
        public string SearchByValue
        {
            get => searchByValue;
            set
            {
                searchByValue = value;
                OnPropertyChanged();
            }
        }
        private bool isNeedSearchBy;
        public bool IsNeedSearchBy
        {
            get => isNeedSearchBy;
            set
            {
                isNeedSearchBy = value;
                OnPropertyChanged();
            }
        }
        private bool isNeedShowAllCategory;
        public bool IsNeedShowAllCategory
        {
            get => isNeedShowAllCategory;
            set
            {
                isNeedShowAllCategory = value;
                OnPropertyChanged();
            }
        }
        private bool isNeedShowAllBrand;
        public bool IsNeedShowAllBrand
        {
            get => isNeedShowAllBrand;
            set
            {
                isNeedShowAllBrand = value;
                OnPropertyChanged();
            }
        }
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
                SearchBy = "In WANO";
                IsNeedSearchBy = false;
            }
            else
            {
                SearchBy = "Only in this shop";
                IsNeedSearchBy = true;
            }
            SearchByValue = Condition.SearchText;
            IsLoadingCheck.IsLoading = 2;
            Task.Run(async () =>
            {
                AllProducts = new List<ProductBlockViewModel>();
                BestSellerProducts = new List<ProductBlockViewModel>();
                BigDiscountProducts = new List<ProductBlockViewModel>();
                NewProducts = new List<ProductBlockViewModel>();
                DisplayedProducts = new List<ProductBlockViewModel>();
                FilterProducts = new List<ProductBlockViewModel>();
                CategoryCheckBoxViewModels = new List<CategoryCheckBoxViewModel>();
                BrandCheckBoxViewModels = new List<BrandCheckViewModel>();
                ProductRepository = new GenericDataRepository<Models.Product>();
                await Load();
                await LoadCategoryCheckBox();
                await LoadBrandCheckBox();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        CategoryCheckBoxViewModels = new List<CategoryCheckBoxViewModel>(CategoryCheckBoxViewModels);
                        BrandCheckBoxViewModels = new List<BrandCheckViewModel>(BrandCheckBoxViewModels);
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
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                        }));
                        MainViewModel.IsLoading = false;
                    });
                });
                OpenAllBrands = new RelayCommandWithNoParameter(() =>
                {
                    DislayedBrandCheckBoxViewModels = new List<BrandCheckViewModel>(BrandCheckBoxViewModels);
                    IsNeedShowAllBrand = true;
                });
                OpenAllCategories = new RelayCommandWithNoParameter(() =>
                {
                    DislayedCategoryCheckBoxViewModels = new List<CategoryCheckBoxViewModel>(CategoryCheckBoxViewModels);
                    IsNeedShowAllCategory = true;
                });
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        DisplayedProducts = new List<ProductBlockViewModel>(DisplayedProducts);
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            });
        }
        private void Search()
        {
            List<string> listCategoryId = new List<string>();
            List<string> allCategoryId = new List<string>();
            foreach (CategoryCheckBoxViewModel categoryCheckBoxViewModel in DislayedCategoryCheckBoxViewModels)
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
            foreach (BrandCheckViewModel brandCheckViewModel in DislayedBrandCheckBoxViewModels)
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
            foreach (bool size in CheckSize)
            {
                if (size)
                {
                    isHasSize = true;
                    break;
                }
            }
            if (isHasSize)
            {
                if (SearchBy == "In WANO")
                {
                    DisplayedProducts = new List<ProductBlockViewModel>(FilterProducts.Where(p => (p.SelectedProduct.BanLevel == 0 && p.SelectedProduct.Name.ToLower().Contains(SearchByValue.ToLower()) &&
                                                                                                            (p.SelectedProduct.Price * (100 - p.SelectedProduct.Sale) / 100 <= MaxPrice && p.SelectedProduct.Price * (100 - p.SelectedProduct.Sale) / 100 >= MinPrice) &&
                                                                                                            listBrandId.Contains(p.SelectedProduct.IdBrand) && listCategoryId.Contains(p.SelectedProduct.IdCategory) &&
                                                                                                            (listCategoryId.Contains(p.SelectedProduct.IdCategory)) && (listBrandId.Contains(p.SelectedProduct.IdBrand)) && ((CheckSize[0] && (p.SelectedProduct.IsOneSize == CheckSize[0])) ||
                                                                                                            ((CheckSize[1] && p.SelectedProduct.IsHadSizeS == CheckSize[1])) || (CheckSize[2] && (p.SelectedProduct.IsHadSizeM == CheckSize[2])) || (CheckSize[3] && (p.SelectedProduct.IsHadSizeL == CheckSize[3])) ||
                                                                                                            (CheckSize[4] && (p.SelectedProduct.IsHadSizeXL == CheckSize[4])) || (CheckSize[5] && (p.SelectedProduct.IsHadSizeXXL == CheckSize[5]))))));
                    IsNeedSearchBy = false;
                }
                else
                {
                    DisplayedProducts = new List<ProductBlockViewModel>(FilterProducts.Where(p => (p.SelectedProduct.BanLevel == 0 && p.SelectedProduct.Name.ToLower().Contains(SearchByValue.ToLower()) && p.SelectedProduct.IdShop == Condition.ShopText &&
                                                                                                            (p.SelectedProduct.Price * (100 - p.SelectedProduct.Sale) / 100 <= MaxPrice && p.SelectedProduct.Price * (100 - p.SelectedProduct.Sale) / 100 >= MinPrice) &&
                                                                                                            listBrandId.Contains(p.SelectedProduct.IdBrand) && listCategoryId.Contains(p.SelectedProduct.IdCategory) &&
                                                                                                            (listCategoryId.Contains(p.SelectedProduct.IdCategory)) && (listBrandId.Contains(p.SelectedProduct.IdBrand)) && ((CheckSize[0] && (p.SelectedProduct.IsOneSize == CheckSize[0])) ||
                                                                                                            ((CheckSize[1] && p.SelectedProduct.IsHadSizeS == CheckSize[1])) || (CheckSize[2] && (p.SelectedProduct.IsHadSizeM == CheckSize[2])) || (CheckSize[3] && (p.SelectedProduct.IsHadSizeL == CheckSize[3])) ||
                                                                                                            (CheckSize[4] && (p.SelectedProduct.IsHadSizeXL == CheckSize[4])) || (CheckSize[5] && (p.SelectedProduct.IsHadSizeXXL == CheckSize[5]))))));
                }
            }
            else
            {
                if (SearchBy == "In WANO")
                {
                    DisplayedProducts = new List<ProductBlockViewModel>(FilterProducts.Where(p => (p.SelectedProduct.BanLevel == 0 && p.SelectedProduct.Name.ToLower().Contains(SearchByValue.ToLower()) &&
                                                                                                        (p.SelectedProduct.Price * (100 - p.SelectedProduct.Sale) / 100 <= MaxPrice && p.SelectedProduct.Price * (100 - p.SelectedProduct.Sale) / 100 >= MinPrice) &&
                                                                                                        listBrandId.Contains(p.SelectedProduct.IdBrand) && listCategoryId.Contains(p.SelectedProduct.IdCategory) &&
                                                                                                        (listCategoryId.Contains(p.SelectedProduct.IdCategory)) && (listBrandId.Contains(p.SelectedProduct.IdBrand)))));
                    IsNeedSearchBy = false;
                }
                else
                {
                    DisplayedProducts = new List<ProductBlockViewModel>(FilterProducts.Where(p => (p.SelectedProduct.BanLevel == 0 && p.SelectedProduct.Name.ToLower().Contains(SearchByValue.ToLower()) && p.SelectedProduct.IdShop == Condition.ShopText &&
                                                                                                        (p.SelectedProduct.Price * (100 - p.SelectedProduct.Sale) / 100 <= MaxPrice && p.SelectedProduct.Price * (100 - p.SelectedProduct.Sale) / 100 >= MinPrice) &&
                                                                                                        listBrandId.Contains(p.SelectedProduct.IdBrand) && listCategoryId.Contains(p.SelectedProduct.IdCategory) &&
                                                                                                        (listCategoryId.Contains(p.SelectedProduct.IdCategory)) && (listBrandId.Contains(p.SelectedProduct.IdBrand)))));
                }
            }
        }
        private async Task Load()
        {
            IList<Models.Product> products = new List<Models.Product>();
            products = new List<Models.Product>(await ProductRepository.GetListAsync(p => p.BanLevel == 0 
                                                                                        && p.InStock!=0,
                                                                                        p => p.Category,
                                                                                        p => p.Brand,
                                                                                        p => p.ImageProducts,
                                                                                        p => p.MUser));
            DateTime lastDateHasNewProduct = DateTime.MinValue;
            foreach (Models.Product product in products)
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
                NewProducts = new List<ProductBlockViewModel>(AllProducts.Where(p => (lastDateHasNewProduct - p.SelectedProduct.DateOfSale) < new TimeSpan(7, 0, 0, 0)).Take(50));
            }
            else
            {
                NewProducts = new List<ProductBlockViewModel>();
            }
            BestSellerProducts = new List<ProductBlockViewModel>(AllProducts.OrderByDescending(p => p.SelectedProduct.Sold).Take(50));
            BigDiscountProducts = new List<ProductBlockViewModel>(AllProducts.OrderByDescending(p => p.SelectedProduct.Sale).Take(50));
        }
        private async Task LoadCategoryCheckBox()
        {
            GenericDataRepository<Models.Category> genericDataRepository = new GenericDataRepository<Models.Category>();
            IList<Models.Category> categories = new List<Models.Category>(await genericDataRepository.GetListAsync(p => p.Status != "Banned"));
            foreach (Models.Category category in categories)
            {
                CategoryCheckBoxViewModel categoryCheckBoxViewModel = new CategoryCheckBoxViewModel();
                categoryCheckBoxViewModel.Category = category;
                categoryCheckBoxViewModel.IsChecked = Condition.Categories.Any(c => c == category.Id);
                CategoryCheckBoxViewModels.Add(categoryCheckBoxViewModel);
            }
            if(CategoryCheckBoxViewModels.Count() > 5)
            {
                IsNeedShowAllCategory = false;
            }
            else
            {
                IsNeedShowAllCategory = true;    
            }
            DislayedCategoryCheckBoxViewModels = new List<CategoryCheckBoxViewModel>(CategoryCheckBoxViewModels.Take(5));
        }
        private async Task LoadBrandCheckBox()
        {
            GenericDataRepository<Models.Brand> genericDataRepository = new GenericDataRepository<Models.Brand>();
            IList<Models.Brand> brands = new List<Models.Brand>(await genericDataRepository.GetListAsync(p => p.Status != "Banned"));
            foreach (Models.Brand brand in brands)
            {
                BrandCheckViewModel brandCheckViewModel = new BrandCheckViewModel();
                brandCheckViewModel.Brand = brand;
                brandCheckViewModel.IsChecked = Condition.Brands.Any(b => b == brand.Id);
                BrandCheckBoxViewModels.Add(brandCheckViewModel);
            }
            if(BrandCheckBoxViewModels.Count() > 5)
            {
                IsNeedShowAllBrand = false;
            }
            else
            {
                IsNeedShowAllBrand = true;    
            }
            DislayedBrandCheckBoxViewModels = new List<BrandCheckViewModel>(BrandCheckBoxViewModels.Take(5));
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
        public FilterStatus Status { get; set; }
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
