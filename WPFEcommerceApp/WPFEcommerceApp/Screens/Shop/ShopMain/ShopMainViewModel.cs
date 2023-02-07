using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ShopMainViewModel : BaseViewModel
    {
        private GenericDataRepository<Models.Product> productRepository = new GenericDataRepository<Models.Product>();
        private double heightNewProducts;
        private double heightBestSellerProducts;
        private double heightBigDiscountProducts;
        private bool isNewProductsCheck;
        public ICommand OpenEditProfileCommand { get; set; }
        public ICommand MoveToNewProducts { get; set; }
        public ICommand MoveToBigDiscountProducts { get; set; }
        public ICommand MoveToBestSellerProducts { get; set; }
        public ICommand MoveToAllProducts { get; set; }
        public ICommand ScrollChangedCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand OpenContactInfoCommand { get; set; }
        public ICommand DoubleClickCommand { get; set; }
        public string SourceImageBackground
        {
            get
            {
                if (Shop == null || string.IsNullOrEmpty(Shop.SourceImageBackground))
                {
                    return Properties.Resources.DefaultShopBackgroundImage;
                }
                else
                {
                    return Shop.SourceImageBackground;
                }
            }
        }
        public string SourceImageAva
        {
            get
            {
                if (Shop == null || string.IsNullOrEmpty(Shop.SourceImageAva))
                {
                    return Properties.Resources.DefaultShopAvaImage;
                }
                else
                {
                    return Shop.SourceImageAva;
                }
            }
        }
        private MUser shop;
        public MUser Shop
        {
            get => shop;
            set
            {
                shop = value;
                OnPropertyChanged();
            }
        }
        private double averageRating;
        public double AverageRating
        {
            get => averageRating;
            set
            {
                averageRating = Math.Round(value, 2);
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Product> allProducts;
        public ObservableCollection<Models.Product> AllProducts
        {
            get => allProducts;
            set
            {
                allProducts = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Product> newProducts;
        public ObservableCollection<Models.Product> NewProducts
        {
            get => newProducts;
            set
            {
                newProducts = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Product> bigDiscountProducts;
        public ObservableCollection<Models.Product> BigDiscountProducts
        {
            get => bigDiscountProducts;
            set
            {
                bigDiscountProducts = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Product> bestSelledProducts;
        public ObservableCollection<Models.Product> BestSellerProducts
        {
            get => bestSelledProducts;
            set
            {
                bestSelledProducts = value;
                OnPropertyChanged();
            }
        }
        private ProductBlockByCategoryViewModel allProductBlock;
        public ProductBlockByCategoryViewModel AllProductBlock
        {
            get => allProductBlock;
            set
            {
                allProductBlock = value;
                OnPropertyChanged();
            }
        }
        private ProductBlockByCategoryViewModel bigDiscountProductBlock;
        public ProductBlockByCategoryViewModel BigDiscountProductBlock
        {
            get => bigDiscountProductBlock;
            set
            {
                bigDiscountProductBlock = value;
                OnPropertyChanged();
            }
        }
        private ProductBlockByCategoryViewModel bestSellerProductBlock;
        public ProductBlockByCategoryViewModel BestSellerProductBlock
        {
            get => bestSellerProductBlock;
            set
            {
                bestSellerProductBlock = value;
                OnPropertyChanged();
            }
        }
        private ProductBlockByCategoryViewModel newProductBlock;
        public ProductBlockByCategoryViewModel NewProductBlock
        {
            get => newProductBlock;
            set
            {
                newProductBlock = value;
                OnPropertyChanged();
            }
        }
        private bool isShop;
        public bool IsShop
        {
            get => isShop;
            set
            {
                isShop = value;
                OnPropertyChanged();
            }
        }

        public bool IsNewProductsCheck
        {
            get => isNewProductsCheck;
            set
            {
                isNewProductsCheck = value;
                OnPropertyChanged();
            }
        }
        private bool isBestSellerProductsCheck;
        public bool IsBestSellerProductsCheck
        {
            get => isBestSellerProductsCheck;
            set
            {
                isBestSellerProductsCheck = value;
                OnPropertyChanged();
            }
        }
        private bool isBigDiscountProductsCheck;
        public bool IsBigDiscountProductsCheck
        {
            get => isBigDiscountProductsCheck;
            set
            {
                isBigDiscountProductsCheck = value;
                OnPropertyChanged();
            }
        }
        private bool isAllProductsCheck;
        public bool IsAllProductsCheck
        {
            get => isAllProductsCheck;
            set
            {
                isAllProductsCheck = value;
                OnPropertyChanged();
            }
        }

        public ShopMainViewModel(Models.MUser user)
        {
            Task.Run(async () =>
            {
                if (AccountStore.instance.CurrentAccount?.Id == user.Id)
                {
                    Shop = AccountStore.instance.CurrentAccount;
                    AccountStore.instance.AccountUpdated += _accountStore_AccountUpdated;
                    IsShop = true;
                }
                else
                {
                    Shop = user;
                    IsShop = false;
                }
                BestSellerProducts = new ObservableCollection<Models.Product>();
                BigDiscountProducts = new ObservableCollection<Models.Product>();
                NewProducts = new ObservableCollection<Models.Product>();
                await LoadAllProduct();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            }).ContinueWith((first) =>
            {
                OpenEditProfileCommand = new RelayCommandWithNoParameter(async () =>
                {
                    MainViewModel.SetLoading(true);
                    ProfileShopDialog profileShopDialog = new ProfileShopDialog();
                    profileShopDialog.DataContext = new ProfileShopDialogViewModel();
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(profileShopDialog, "Main");
                });
                LoadedCommand = new RelayCommand<Tuple<double, double, double>>((p) => { return p != null; }, p =>
                {
                    Tuple<double, double, double> tuple = p as Tuple<double, double, double>;
                    heightNewProducts = (double)tuple.Item1;
                    heightBestSellerProducts = (double)tuple.Item2;
                    heightBigDiscountProducts = (double)tuple.Item3;
                    IsNewProductsCheck = true;
                });
                ScrollChangedCommand = new RelayCommand<object>((p) => { return p != null; }, p =>
                {
                    ScrollViewer scrollViewer = p as ScrollViewer;
                    double offset = (double)scrollViewer.VerticalOffset;
                    if(offset + scrollViewer.ViewportHeight == scrollViewer.ExtentHeight)
                    {
                        IsAllProductsCheck = true;
                        if (AllProductBlock == null || AllProductBlock.FullProducts.Count == 0)
                        {
                            IsAllProductsCheck = false;
                        }
                    }    
                    else if (offset < heightNewProducts + 230)
                    {
                        IsNewProductsCheck = true;
                        if (NewProductBlock == null || NewProductBlock.FullProducts.Count == 0)
                        {
                            IsNewProductsCheck = false;
                        }
                    }
                    else if (offset < heightNewProducts + heightBestSellerProducts + 230)
                    {
                        IsBestSellerProductsCheck = true;
                        if (BestSellerProductBlock == null || BestSellerProductBlock.FullProducts.Count == 0)
                        {
                            IsBestSellerProductsCheck = false;
                        }
                    }
                    else if (offset < heightNewProducts + heightBestSellerProducts + heightBigDiscountProducts + 230)
                    {
                        IsBigDiscountProductsCheck = true;
                        if (BigDiscountProductBlock == null || BigDiscountProductBlock.FullProducts.Count == 0)
                        {
                            IsBigDiscountProductsCheck = false;
                        }
                    }
                    else
                    {
                        IsAllProductsCheck = true;
                        if (AllProductBlock == null || AllProductBlock.FullProducts.Count == 0)
                        {
                            IsAllProductsCheck = false;
                        }
                    }
                });
                MoveToNewProducts = new RelayCommand<object>((p) => { return p != null; }, p =>
                {
                    ScrollViewer scrollViewer = p as ScrollViewer;
                    scrollViewer.ScrollToVerticalOffset(230);
                });
                MoveToBestSellerProducts = new RelayCommand<object>((p) => { return true; }, p =>
                {
                    ScrollViewer scrollViewer = p as ScrollViewer;
                    scrollViewer.ScrollToVerticalOffset(heightNewProducts + 230);
                });
                MoveToBigDiscountProducts = new RelayCommand<object>((p) => { return p != null; }, p =>
                {
                    ScrollViewer scrollViewer = p as ScrollViewer;
                    scrollViewer.ScrollToVerticalOffset(heightNewProducts + heightBestSellerProducts + 230);
                });
                MoveToAllProducts = new RelayCommand<object>((p) => { return p != null; }, p =>
                {
                    ScrollViewer scrollViewer = p as ScrollViewer;
                    scrollViewer.ScrollToVerticalOffset(heightNewProducts + heightBestSellerProducts + heightBigDiscountProducts + 230);
                });
                OpenContactInfoCommand = new RelayCommandWithNoParameter(async () =>
                {
                    IsLoadingCheck.IsLoading++;
                    ShopContact shopContact = new ShopContact();
                    shopContact.DataContext = new ShopContactViewModel(Shop);
                    IsLoadingCheck.IsLoading--;
                    await DialogHost.Show(shopContact, "Main");
                });
                DoubleClickCommand = new RelayCommandWithNoParameter(() =>
                {

                });
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    FilterObject all = new FilterObject("", Shop.Id, null, null, FilterStatus.All);
                    FilterObject new_ = new FilterObject("", Shop.Id, null, null, FilterStatus.New);
                    FilterObject bestSeller = new FilterObject("", Shop.Id, null, null, FilterStatus.BestSeller);
                    FilterObject bigDiscount = new FilterObject("", Shop.Id, null, null, FilterStatus.BigDiscount);
                    AllProductBlock = new ProductBlockByCategoryViewModel("All", AllProducts, AllProducts.Count == 0, true, IsShop)
                    { ChangeToFilterCommand = new RelayCommandWithNoParameter(() => NavigateProvider.FilterScreen().Navigate(all))};
                    NewProductBlock = new ProductBlockByCategoryViewModel("New Products", NewProducts, NewProducts.Count == 0, false, IsShop)
                    { ChangeToFilterCommand = new RelayCommandWithNoParameter(() => NavigateProvider.FilterScreen().Navigate(new_))};
                    BestSellerProductBlock = new ProductBlockByCategoryViewModel("Best Seller Products", BestSellerProducts, BestSellerProducts.Count == 0, false, IsShop)
                    { ChangeToFilterCommand = new RelayCommandWithNoParameter(() => NavigateProvider.FilterScreen().Navigate(bestSeller))};
                    BigDiscountProductBlock = new ProductBlockByCategoryViewModel("Big Discount Products", BigDiscountProducts, BigDiscountProducts.Count == 0, false, IsShop)
                    { ChangeToFilterCommand = new RelayCommandWithNoParameter(() => NavigateProvider.FilterScreen().Navigate(bigDiscount))};
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            });
        }
        private void _accountStore_AccountUpdated()
        {
            Shop = AccountStore.instance.CurrentAccount;
            OnPropertyChanged(nameof(Shop));
            OnPropertyChanged(nameof(SourceImageAva));
            OnPropertyChanged(nameof(SourceImageBackground));
        }

        private async Task LoadAllProduct()
        {
            AllProducts = new ObservableCollection<Models.Product>(await productRepository.GetListAsync(p => p.IdShop == Shop.Id && p.InStock!=0 && p.BanLevel == 0,
                                                                                                        p => p.Brand,
                                                                                                        p => p.Category,
                                                                                                        p => p.OrderInfoes,
                                                                                                        p => p.OrderInfoes.Select(oi => oi.Rating),
                                                                                                        p => p.ImageProducts,
                                                                                                        p => p.MUser));
            int number = 0;
            long sumRating = 0;
            DateTime lastDateHasNewProduct = DateTime.MinValue;
            foreach (Models.Product product in AllProducts)
            {
                foreach (Models.OrderInfo orderInfo in product.OrderInfoes)
                {
                    if (orderInfo.Rating != null)
                    {
                        sumRating += orderInfo.Rating.Rating1 ?? 0;
                        number += 1;
                    }
                }
                if(product.DateOfSale > lastDateHasNewProduct)
                {
                    lastDateHasNewProduct = product.DateOfSale??DateTime.MinValue;
                }    
            }
            AverageRating = sumRating * 1.0 / number;
            BigDiscountProducts = new ObservableCollection<Models.Product>(AllProducts.Where(p=>p.Sale > 0).OrderByDescending(p => p.Sale).Take(16));
            BestSellerProducts = new ObservableCollection<Models.Product>(AllProducts.Where(p => p.Sold > 0).OrderByDescending(p => p.Sold).Take(16));
            if (lastDateHasNewProduct != DateTime.MinValue)
            {
                NewProducts = new ObservableCollection<Models.Product>(AllProducts.Where(p => (lastDateHasNewProduct - p.DateOfSale) <= new TimeSpan(7, 0, 0, 0)).ToList().OrderByDescending(p => p.DateOfSale).Take(16));
            }
        }
    }
}
