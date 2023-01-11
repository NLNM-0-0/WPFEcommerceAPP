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
        private double heightFavoriteProducts;
        private bool isNewProductsCheck;
        public ICommand OpenEditProfileCommand { get; set; }
        public ICommand MoveToNewProducts { get; set; }
        public ICommand MoveToFavoriteProducts { get; set; }
        public ICommand MoveToBestSellerProducts { get; set; }
        public ICommand MoveToAllProducts { get; set; }
        public ICommand ScrollChangedCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
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
                averageRating = value;
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
        private ObservableCollection<Models.Product> favoriteProducts;
        public ObservableCollection<Models.Product> FavoriteProducts
        {
            get => favoriteProducts;
            set
            {
                favoriteProducts = value;
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
        private ProductBlockByCategoryViewModel favoriteProductBlock;
        public ProductBlockByCategoryViewModel FavoriteProductBlock
        {
            get => favoriteProductBlock;
            set
            {
                favoriteProductBlock = value;
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
        private bool isFavoriteProductsCheck;
        public bool IsFavoriteProductsCheck
        {
            get => isFavoriteProductsCheck;
            set
            {
                isFavoriteProductsCheck = value;
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
                    MainViewModel.IsLoading = true;
                    ProfileShopDialog profileShopDialog = new ProfileShopDialog();
                    profileShopDialog.DataContext = new ProfileShopDialogViewModel();
                    MainViewModel.IsLoading = false;
                    await DialogHost.Show(profileShopDialog, "Main");
                });
                LoadedCommand = new RelayCommand<Tuple<double, double, double>>((p) => { return p != null; }, p =>
                {
                    Tuple<double, double, double> tuple = p as Tuple<double, double, double>;
                    heightNewProducts = (double)tuple.Item1;
                    heightBestSellerProducts = (double)tuple.Item2;
                    heightFavoriteProducts = (double)tuple.Item3;
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
                    else if (offset < heightNewProducts + heightBestSellerProducts + heightFavoriteProducts + 230)
                    {
                        IsFavoriteProductsCheck = true;
                        if (FavoriteProductBlock == null || FavoriteProductBlock.FullProducts.Count == 0)
                        {
                            IsFavoriteProductsCheck = false;
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
                MoveToFavoriteProducts = new RelayCommand<object>((p) => { return p != null; }, p =>
                {
                    ScrollViewer scrollViewer = p as ScrollViewer;
                    scrollViewer.ScrollToVerticalOffset(heightNewProducts + heightBestSellerProducts + 230);
                });
                MoveToAllProducts = new RelayCommand<object>((p) => { return p != null; }, p =>
                {
                    ScrollViewer scrollViewer = p as ScrollViewer;
                    scrollViewer.ScrollToVerticalOffset(heightNewProducts + heightBestSellerProducts + heightFavoriteProducts + 230);
                });
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    AllProductBlock = new ProductBlockByCategoryViewModel("All", AllProducts, AllProducts.Count == 0, true, IsShop);
                    NewProductBlock = new ProductBlockByCategoryViewModel("New Products", NewProducts, NewProducts.Count == 0, false, IsShop);
                    BestSellerProductBlock = new ProductBlockByCategoryViewModel("Best Seller Products", BestSellerProducts, BestSellerProducts.Count == 0, false, IsShop);
                    FavoriteProductBlock = new ProductBlockByCategoryViewModel("Favorite Products", FavoriteProducts, FavoriteProducts.Count == 0, true, IsShop);
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
            AllProducts = new ObservableCollection<Models.Product>(await productRepository.GetListAsync(p => p.IdShop == Shop.Id,
                                                                                                        p => p.Brand,
                                                                                                        p => p.Category,
                                                                                                        p => p.ImageProducts,
                                                                                                        p => p.OrderInfoes,
                                                                                                        p => p.OrderInfoes.Select(oi => oi.Rating),
                                                                                                        p => p.MUsers,
                                                                                                        p => p.ImageProducts));
            int number = 0;
            long sumRating = 0;
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
            }
            AverageRating = sumRating * 1.0 / number;
            FavoriteProducts = new ObservableCollection<Models.Product>(AllProducts.Where(p => p.MUsers.Count != 0).ToList().OrderByDescending(p => p.MUsers.Count).Take(9));
            BestSellerProducts = new ObservableCollection<Models.Product>(AllProducts.Where(p => p.OrderInfoes.Count > 0).OrderByDescending(p => p.OrderInfoes.Count).Take(9));
            NewProducts = new ObservableCollection<Models.Product>(AllProducts.Where(p => (DateTime.Now - p.DateOfSale) <= new TimeSpan(7, 0, 0, 0)).ToList().OrderByDescending(p => p.DateOfSale).Take(9));
        }
    }
}
