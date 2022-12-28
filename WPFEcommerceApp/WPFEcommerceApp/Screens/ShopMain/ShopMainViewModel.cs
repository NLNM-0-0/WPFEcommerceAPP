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
        public ObservableCollection<Models.Product> BestSelledProducts
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
        private ProductBlockByCategoryViewModel bestSelledProductBlock;
        public ProductBlockByCategoryViewModel BestSelledProductBlock
        {
            get => bestSelledProductBlock;
            set
            {
                bestSelledProductBlock = value;
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
            if (AccountStore.instance.CurrentAccount?.Id == user.Id)
            {
                Shop = AccountStore.instance.CurrentAccount;
                AccountStore.instance.AccountChanged += _accountStore_AccountChanged;
                IsShop = true;
            }
            else
            {
                Shop = user;
                IsShop = false;
            }
            
            Task task = Task.Run(async () => await LoadAllProduct());
            while(!task.IsCompleted) { }
            
            AllProductBlock = new ProductBlockByCategoryViewModel("All", AllProducts, AllProducts.Count == 0, true, IsShop);
            NewProductBlock = new ProductBlockByCategoryViewModel("New Products", NewProducts, NewProducts.Count == 0, false, IsShop);
            BestSelledProductBlock = new ProductBlockByCategoryViewModel("Best Seller Products", BestSelledProducts, BestSelledProducts.Count == 0, false, IsShop);
            FavoriteProductBlock = new ProductBlockByCategoryViewModel("Favorite Products", FavoriteProducts, FavoriteProducts.Count == 0, true, IsShop);
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
                if (offset < heightNewProducts)
                {
                    IsNewProductsCheck = true;
                    if (NewProductBlock.FullProducts.Count == 0)
                    {
                        IsNewProductsCheck = false;
                    }
                }
                else if (offset < heightNewProducts + heightBestSellerProducts)
                {
                    IsBestSellerProductsCheck = true;
                    if (BestSelledProductBlock.FullProducts.Count == 0)
                    {
                        IsBestSellerProductsCheck = false;
                    }
                }
                else if (offset < heightNewProducts + heightBestSellerProducts + heightFavoriteProducts)
                {
                    IsFavoriteProductsCheck = true;
                    if (FavoriteProductBlock.FullProducts.Count == 0)
                    {
                        IsFavoriteProductsCheck = false;
                    }
                }
                else
                {
                    IsAllProductsCheck = true;
                    if (AllProductBlock.FullProducts.Count == 0)
                    {
                        IsAllProductsCheck = false;
                    }
                }
            });
            MoveToNewProducts = new RelayCommand<object>((p) => { return p != null; }, p =>
            {
                ScrollViewer scrollViewer = p as ScrollViewer;
                scrollViewer.ScrollToTop();
            });
            MoveToBestSellerProducts = new RelayCommand<object>((p) => { return p != null; }, p =>
            {
                ScrollViewer scrollViewer = p as ScrollViewer;
                scrollViewer.ScrollToVerticalOffset(heightNewProducts);
            });
            MoveToFavoriteProducts = new RelayCommand<object>((p) => { return p != null; }, p =>
            {
                ScrollViewer scrollViewer = p as ScrollViewer;
                scrollViewer.ScrollToVerticalOffset(heightNewProducts + heightBestSellerProducts);
            });
            MoveToAllProducts = new RelayCommand<object>((p) => { return p != null; }, p =>
            {
                ScrollViewer scrollViewer = p as ScrollViewer;
                scrollViewer.ScrollToVerticalOffset(heightNewProducts + heightBestSellerProducts + heightFavoriteProducts);
            });
        }

        private void _accountStore_AccountChanged()
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
            FavoriteProducts = new ObservableCollection<Models.Product>(AllProducts.Where(p => p.MUsers.Count != 0).ToList().OrderByDescending(p => p.MUsers.Count));
            BestSelledProducts = new ObservableCollection<Models.Product>(AllProducts.OrderByDescending(p => p.OrderInfoes.Count));
            NewProducts = new ObservableCollection<Models.Product>(AllProducts.Where(p => (DateTime.Now - p.DateOfSale) <= new TimeSpan(7, 0, 0, 0)).ToList().OrderByDescending(p => p.DateOfSale));
        }
    }
}
