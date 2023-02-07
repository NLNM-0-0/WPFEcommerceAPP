using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    class ProductDetailNormalViewModel : BaseViewModel
    {
        public GenericDataRepository<Models.Product> ProductRepository { get; set; }
        public GenericDataRepository<Models.Cart> CartRepo { get; set; }
        private readonly GenericDataRepository<Models.Product> productRepo = new GenericDataRepository<Models.Product>();
        private readonly GenericDataRepository<MUser> userRepo = new GenericDataRepository<MUser>();
        public ICommand NextImageCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand PreviousImageCommand { get; set; }

        public ICommand FavouriteCommand { get; set; }
        public ICommand AddToBagCommand { get; set; }
        public ICommand Plusamount { get; set; }

        public ICommand Tamount { get; set; }

        public ICommand BuyNowCommand { get; set; }
        public ICommand UnFavouriteCommand { get; set; }
        public ICommand RightCommand { get; set; }
        public ICommand LeftCommand { get; set; }
        public ICommand ViewShopCommand { get; set; }
        private double rating;
        public double Rating
        {
            get => rating;
            set
            {
                rating = value;
                OnPropertyChanged();
            }
        }
        private int ratingTimes;
        public int RatingTimes
        {
            get => ratingTimes;
            set
            {
                ratingTimes = value;
                OnPropertyChanged();
            }
        }
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
        private int selectedImageIndex = -1;
        public int SelectedImageIndex
        {
            get => selectedImageIndex;
            set
            {
                selectedImageIndex = value;
                OnPropertyChanged();
            }
        }
        private string size;
        public string Size
        {
            get
            {
                if (isHadOneSize == true)
                {
                    return "OneSize";
                }
                else if (isHadSizeS == true)
                {
                    return "S";
                }
                else if (isHadSizeM == true)
                {
                    return "M";
                }
                else if (isHadSizeL == true)
                {
                    return "L";
                }
                else if (isHadSizeXL == true)
                {
                    return "XL";
                }
                else
                {
                    return "XXL";
                }
            }
            set
            {
                size = value;
                OnPropertyChanged();
            }
        }
        private string amount;
        public string Amount
        {
            get => amount;
            set
            {
                amount = value;
                OnPropertyChanged();
            }
        }
        private string selectedImage;
        public string SelectedImage
        {
            get
            {
                if (SelectedImageIndex == -1)
                {
                    return Properties.Resources.DefaultProductImage; ;
                }
                return ImageProducts[SelectedImageIndex];
            }
            set
            {
                selectedImage = value;
                OnPropertyChanged();
            }
        }
        private IList<string> imageProducts;
        public IList<string> ImageProducts
        {
            get
            {
                return imageProducts;
            }
            set
            {
                imageProducts = value;
                OnPropertyChanged();
            }
        }
        private bool isHad1S;
        public bool IsHad1S
        {
            get
            {
                return isHad1S;
            }
            set
            {
                if (value)
                {
                    AmountStar = 1;
                }
                isHad1S = value;
                OnPropertyChanged();
            }
        }
        private bool isHad2S;
        public bool IsHad2S
        {
            get
            {
                return isHad2S;
            }
            set
            {
                if (value)
                {
                    AmountStar = 2;
                }
                isHad2S = value;
                OnPropertyChanged();
            }
        }
        private bool isHad3S;
        public bool IsHad3S
        {
            get
            {
                return isHad3S;
            }
            set
            {
                if (value)
                {
                    AmountStar = 3;
                }
                isHad3S = value;
                OnPropertyChanged();
            }
        }
        private bool isHad4S;
        public bool IsHad4S
        {
            get
            {
                return isHad4S;
            }
            set
            {
                if (value)
                {
                    AmountStar = 4;
                }
                isHad4S = value;
                OnPropertyChanged();
            }
        }
        private bool isHad5S;
        public bool IsHad5S
        {
            get
            {
                return isHad5S;
            }
            set
            {
                if (value)
                {
                    AmountStar = 5;
                }
                isHad5S = value;
                OnPropertyChanged();
            }
        }
        private bool isHadSizeS;
        public bool IsHadSizeS
        {
            get
            {
                return isHadSizeS;
            }
            set
            {
                isHadSizeS = value;
                OnPropertyChanged();
            }
        }
        private int amountStar;
        public int AmountStar
        {
            get
            {
                return amountStar;
            }
            set
            {
                amountStar = value;
                OnPropertyChanged();
            }
        }
        private IList<Models.Product> products;
        public IList<Models.Product> Products
        {
            get { return products; }
            set
            {
                products = value;
                OnPropertyChanged();
            }

        }

        private IList<Models.Product> productAlsos;
        public IList<Models.Product> ProductAlsos
        {
            get { return productAlsos; }
            set
            {
                productAlsos = value;
                OnPropertyChanged();
            }

        }
        private bool isHadSizeM;
        public bool IsHadSizeM
        {
            get
            {
                return isHadSizeM;
            }
            set
            {
                isHadSizeM = value;
                OnPropertyChanged();
            }
        }
        private bool isHadSizeL;
        public bool IsHadSizeL
        {
            get
            {
                return isHadSizeL;
            }
            set
            {
                isHadSizeL = value;
                OnPropertyChanged();
            }
        }
        private bool isHadSizeXL;
        public bool IsHadSizeXL
        {
            get
            {
                return isHadSizeXL;
            }
            set
            {
                isHadSizeXL = value;
                OnPropertyChanged();
            }
        }
        private bool isHadSizeXXL;
        public bool IsHadSizeXXL
        {
            get
            {
                return isHadSizeXXL;
            }
            set
            {
                isHadSizeXXL = value;
                OnPropertyChanged();
            }
        }
        private bool isHadOneSize;
        public bool IsHadOneSize
        {
            get
            {
                return isHadOneSize;
            }
            set
            {
                isHadOneSize = value;
                OnPropertyChanged();
            }
        }
        private IList<ProductBlockViewModel> productViewModels;
        public IList<ProductBlockViewModel> ProductViewModels
        {
            get { return productViewModels; }
            set
            {
                productViewModels = value;
                OnPropertyChanged();
            }

        }
        private IList<ProductBlockViewModel> productAlsoViewModels;
        public IList<ProductBlockViewModel> ProductAlsoViewModels
        {
            get { return productAlsoViewModels; }
            set
            {
                productAlsoViewModels = value;
                OnPropertyChanged();
            }

        }
        private IList<ShopRatingBlockModel> displayShopRatingBlockModels;
        public IList<ShopRatingBlockModel> DisplayShopRatingBlockModels
        {
            get => displayShopRatingBlockModels;
            set
            {
                displayShopRatingBlockModels = value;
                OnPropertyChanged();
            }
        }
        private IList<ShopRatingBlockModel> allShopRatingBlockModels;
        public IList<ShopRatingBlockModel> AllShopRatingBlockModels
        {
            get => allShopRatingBlockModels;
            set
            {
                allShopRatingBlockModels = value;
                OnPropertyChanged();
            }
        }
        private string sourceImageAva;
        public string SourceImageAva
        {
            get => sourceImageAva;
            set
            {
                sourceImageAva = value;
                OnPropertyChanged();
            }
        }
        private bool? isFavorite;
        public bool? IsFavorite
        {
            get => isFavorite;
            set
            {
                isFavorite = value;
            }
        }
        private bool? isFavoriteFirst;
        public bool? IsFavoriteFirst
        {
            get => isFavoriteFirst;
            set
            {
                isFavoriteFirst = value;
                OnPropertyChanged();
            }
        }
        public bool HeartVisibility
        {
            get
            {
                if (AccountStore.instance.CurrentAccount == null || AccountStore.instance.CurrentAccount.Id == SelectedProduct.IdShop)
                {
                    return false;
                }
                return true;
            }

        }
        public ProductDetailNormalViewModel(Models.Product product)
        {
            SelectedProduct = product;
            Rating = 0;
            Task.Run(async () =>
            {
                ImageProducts = new List<string>();
                ProductViewModels = new List<ProductBlockViewModel>();
                ProductAlsoViewModels = new List<ProductBlockViewModel>();
                ProductRepository = new GenericDataRepository<Models.Product>();
                DisplayShopRatingBlockModels = new List<ShopRatingBlockModel>();
                AllShopRatingBlockModels = new List<ShopRatingBlockModel>();
                await Load(); 
                await LoadComments(); 
                await LoadAlso();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    AllShopRatingBlockModels = new List<ShopRatingBlockModel>(AllShopRatingBlockModels);
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading += AllShopRatingBlockModels.Count - 1;
                    }
                }));
            }).ContinueWith((second) =>
            {
                Amount = "1";
                if (AccountStore.instance.CurrentAccount == null)
                {
                    IsFavorite = null;
                }
                else
                {
                    if (FavoriteStore.instance.FavoriteProductList.Any(p => p.Id == SelectedProduct.Id))
                    {
                        IsFavorite = true;
                    }
                    else
                    {
                        IsFavorite = false;
                    }
                }
                IsFavoriteFirst = IsFavorite;
                if (String.IsNullOrEmpty(SelectedProduct.MUser.SourceImageAva))
                {
                    SourceImageAva = Properties.Resources.DefaultShopAvaImage;
                }
                else
                {
                    SourceImageAva = SelectedProduct.MUser.SourceImageAva;
                }
                if (SelectedProduct.IsOneSize)
                {
                    IsHadOneSize = true;
                }
                else
                {
                    if (SelectedProduct.IsHadSizeS)
                    {
                        IsHadSizeS = true;
                    }
                    else if (SelectedProduct.IsHadSizeM)
                    {
                        IsHadSizeM = true;
                    }
                    else if (SelectedProduct.IsHadSizeL)
                    {
                        IsHadSizeL = true;
                    }
                    else if (SelectedProduct.IsHadSizeXL)
                    {
                        IsHadSizeXL = true;
                    }
                    else
                    {
                        IsHadSizeXXL = true;
                    }
                }
                DisplayShopRatingBlockModels = AllShopRatingBlockModels;
                LoadProductBLockViewModel();
                LoadProductBLockAlsoViewModel();
                foreach (Models.ImageProduct imageProduct in product?.ImageProducts)
                {
                    ImageProducts.Add(imageProduct.Source);
                }

                if (ImageProducts.Count == 0)
                {
                    SelectedImageIndex = -1;
                }
                else
                {
                    SelectedImageIndex = 0;
                }
                RightCommand = new RelayCommand<object>(p => true, Right);
                LeftCommand = new RelayCommand<object>(p => true, Left);
                Plusamount = new RelayCommand<object>((p) => 
                {
                    return int.Parse(Amount) < SelectedProduct.InStock; 
                }, (p) =>
                {
                    Amount = (int.Parse(Amount) + 1).ToString();
                    OnPropertyChanged();
                });
                Tamount = new RelayCommand<object>((p) =>
                {
                    return int.Parse(Amount) >= 2;
                }, (p) =>
                {
                    Amount = (int.Parse(Amount) - 1).ToString();
                    OnPropertyChanged();
                });
                NextImageCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
                {
                    SelectedImageIndex = ((selectedImageIndex + 1) % ImageProducts.Count);
                });
                PreviousImageCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
                {
                    if (selectedImageIndex == 0)
                    {
                        selectedImageIndex = ImageProducts.Count;
                    }
                    SelectedImageIndex = ((selectedImageIndex - 1) % ImageProducts.Count);
                });
                FavouriteCommand = new RelayCommand<object>((p) =>
                {
                    return (AccountStore.instance.CurrentAccount != null && SelectedProduct.IdShop != AccountStore.instance.CurrentAccount.Id);
                }, async (p) =>
                {
                    await FavoriteStore.instance.Add(SelectedProduct);
                    OnPropertyChanged(nameof(IsFavorite));
                });
                UnFavouriteCommand = new RelayCommand<object>((p) =>
                {
                    return (AccountStore.instance.CurrentAccount != null && 
                    SelectedProduct.IdShop != AccountStore.instance.CurrentAccount.Id) &&
                    SelectedProduct.InStock >= int.Parse(Amount);
                }, async (p) =>
                {
                    await FavoriteStore.instance.Delete(SelectedProduct);
                    OnPropertyChanged(nameof(IsFavorite));
                });
                BuyNowCommand = new RelayCommand<object>((p) => 
                {
                    return ((IsHadSizeS || IsHadSizeM || IsHadSizeL || IsHadSizeXL || IsHadSizeXXL || IsHadOneSize) &&
                    ((AccountStore.instance.CurrentAccount != null) ? (AccountStore.instance.CurrentAccount.Id != SelectedProduct.IdShop) : true) &&
                    !String.IsNullOrEmpty(Amount.Trim()) &&
                    SelectedProduct.InStock >= int.Parse(Amount));
                }, async (p) =>
                {
                    if (AccountStore.instance.CurrentAccount == null)
                    {
                        var dialog = new ConfirmDialog()
                        {
                            Header = "Oops!",
                            Content = "You need to login to do this!",
                            CM = new ImmediateCommand<object>(pr =>
                            {
                                Login login = App.serviceProvider.GetRequiredService<Login>();
                                login.Show();
                                App.Current.MainWindow.Hide();
                            }),
                        };
                        await DialogHost.Show(dialog, "Main");
                    }
                    else
                    {
                        var raw = p as Models.Product;
                        var raw2 = await productRepo.GetSingleAsync(d => d.Id == raw.Id, d => d.ImageProducts);

                        Product prd = new Product(raw2, Size, int.Parse(Amount));
                        List<Product> list = new List<Product>();
                        list.Add(prd);

                        var id = await GenerateID.Gen(typeof(MOrder));
                        var repo = new GenericDataRepository<MUser>();
                        var shop = await repo.GetSingleAsync(d => d.Id == raw.IdShop);

                        Order o = new Order(list)
                        {
                            ID = id,
                            IDCustomer = AccountStore.instance.CurrentAccount.Id,
                            IDShop = raw.IdShop,
                            Status = "Processing",
                            ShipTotal = 0,
                            DateBegin = DateTime.Now,
                            ShopName = shop.Name,
                            ShopImage = shop.SourceImageAva
                        };
                        var orderList = new List<Order>();
                        orderList.Add(o);
                        NavigateProvider.CheckoutScreen().Navigate(orderList);
                    }
                });

                AddToBagCommand = new RelayCommand<object>((p) => 
                { 
                    return ((IsHadSizeS || IsHadSizeM || IsHadSizeL || IsHadSizeXL || IsHadSizeXXL || IsHadOneSize) &&
                    ((AccountStore.instance.CurrentAccount != null)?(AccountStore.instance.CurrentAccount.Id != SelectedProduct.IdShop):true) &&
                    !String.IsNullOrEmpty(Amount.Trim()) &&
                    SelectedProduct.InStock >= int.Parse(Amount)); 
                }, async (p) =>
                {
                    if (AccountStore.instance.CurrentAccount == null)
                    {
                        var dialog = new ConfirmDialog()
                        {
                            Header = "Oops!",
                            Content = "You need to login to do this!",
                            CM = new ImmediateCommand<object>(pr =>
                            {
                                Login login = App.serviceProvider.GetRequiredService<Login>();
                                login.Show();
                                App.Current.MainWindow.Hide();
                            }),
                        };
                        await DialogHost.Show(dialog, "Main");
                    }
                    else
                    {
                        MainViewModel.SetLoading(true);
                        await LoadAddToBag();
                        var dl = new ConfirmDialog()
                        {
                            Header = "Success",
                            Content = "This product has been added to your Bag. \nCheck it out.",
                            CM = new RelayCommand<object>(pr => true, pr =>
                            {
                                NavigateProvider.BagScreen().Navigate();
                            }),
                            Param = ""
                        };
                        MainViewModel.SetLoading(false);
                        await DialogHost.Show(dl, "Main");
                    }
                });
                SearchCommand = new RelayCommandWithNoParameter(() =>
                {
                    MainViewModel.SetLoading(true);
                    Task.Run(() => { }).ContinueWith((third) =>
                    {
                        Search();
                        MainViewModel.SetLoading(false);
                    });
                });
                ViewShopCommand = new RelayCommandWithNoParameter(() =>
                {
                    NavigateProvider.ShopViewScreen().Navigate(SelectedProduct.MUser);
                });
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    ProductViewModels = new List<ProductBlockViewModel>(ProductViewModels);
                    ProductAlsoViewModels = new List<ProductBlockViewModel>(ProductAlsoViewModels);
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            });
        }
        private async Task LoadAddToBag()
        {
            MainViewModel.SetLoading(true);
            GenericDataRepository<Models.Cart> dataRepository = new GenericDataRepository<Models.Cart>();
            var product = await dataRepository.GetSingleAsync(
                d => d.IdProduct == SelectedProduct.Id &&
                    d.IdUser == AccountStore.instance.CurrentAccount.Id &&
                    d.Size == Size);
            if(product != null) {
                product.Amount += int.Parse(Amount);
                await dataRepository.Update(product);
                MainViewModel.SetLoading(false);
                return;
            }
            await dataRepository.Add(new Models.Cart
            {
                IdProduct = SelectedProduct.Id,
                IdUser = AccountStore.instance.CurrentAccount.Id,
                Amount = int.Parse(Amount),
                Size = Size
            });
            MainViewModel.SetLoading(false);
        }
        private async Task Load()
        {
            Products = new List<Models.Product>((await ProductRepository.GetListAsync(p => p.BanLevel == 0 && 
                                                                                        p.IdShop == SelectedProduct.IdShop && 
                                                                                        p.Id != SelectedProduct.Id && 
                                                                                        p.InStock!=0,
                                                                                        p => p.Category,
                                                                                        p => p.Brand,
                                                                                        p => p.ImageProducts, 
                                                                                        p => p.MUser)).Take(20));
        }

        private async Task LoadAlso()
        {
            ProductAlsos = new List<Models.Product>((await ProductRepository.GetListAsync(p => p.BanLevel == 0 &&
                                                                                            p.Id != SelectedProduct.Id && 
                                                                                            (p.IdBrand == SelectedProduct.IdBrand || p.IdCategory == SelectedProduct.IdCategory) && 
                                                                                            p.InStock != 0 &&
                                                                                            p.IdShop != SelectedProduct.IdShop,
                                                                                            p => p.Category,
                                                                                            p => p.Brand,
                                                                                            p => p.ImageProducts, 
                                                                                            p => p.MUser)).Take(20));
        }

        private void LoadProductBLockViewModel()
        {
            foreach (Models.Product product in Products)
            {
                ProductViewModels.Add(new ProductBlockViewModel(product));
            }
        }

        private void LoadProductBLockAlsoViewModel()
        {
            foreach (Models.Product product in ProductAlsos)
            {
                ProductAlsoViewModels.Add(new ProductBlockViewModel(product));
            }
        }

        public static void Right(object obj)
        {
            var listView = obj as ListView;
            //ScrollViewer scrollViewer = listView.GetVisualChild<ScrollViewer>();

            // Get the border of the listview (first child of a listview)
            Decorator border = VisualTreeHelper.GetChild(listView, 0) as Decorator;


            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;

            if (scrollViewer != null)
            {
                try
                {
                    var temp = scrollViewer.HorizontalOffset;
                    var temp2 = scrollViewer.ViewportWidth;
                    var temp3 = (int)((temp + temp2) / 280);
                    var items = listView.ItemsSource.Cast<object>();
                    listView.ScrollIntoView(items.ElementAt(temp3));
                }
                catch { }


            }
        }

        public static void Left(object obj)
        {
            var listView = obj as ListView;
            //ScrollViewer scrollViewer = listView.GetVisualChild<ScrollViewer>();

            // Get the border of the listview (first child of a listview)
            Decorator border = VisualTreeHelper.GetChild(listView, 0) as Decorator;

            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;

            if (scrollViewer != null)
            {
                try
                {
                    var temp = scrollViewer.HorizontalOffset;
                    var temp2 = scrollViewer.ViewportWidth;

                    int temp3 = (int)Math.Ceiling(temp / 295) - 1;

                    var items = listView.ItemsSource.Cast<object>();
                    listView.ScrollIntoView(items.ElementAt(temp3));
                }
                catch { }


            }
        }
        public async Task LoadComments()
        {
            GenericDataRepository<Models.MOrder> orderReposition = new GenericDataRepository<MOrder>();
            GenericDataRepository<Models.OrderInfo> orderInfoReposition = new GenericDataRepository<OrderInfo>();
            List<Models.MOrder> orders = new List<Models.MOrder>((await orderReposition.GetListAsync(r => (r != null && r.Status == "Completed" && r.IdShop == SelectedProduct.IdShop),
                                                                                                                                    r => r.MUser)));
            foreach (Models.MOrder order in orders)
            {
                List<Models.OrderInfo> orderInfos = new List<OrderInfo>(await orderInfoReposition.GetListAsync(oi => (oi.IdOrder == order.Id && oi.IdRating != null && oi.IdProduct == SelectedProduct.Id),
                                                                                                                                               oi => oi.Product,
                                                                                                                                               oi => oi.Product.ImageProducts,
                                                                                                                                               oi => oi.Rating,
                                                                                                                                               oi => oi.Rating.RatingInfoes,
                                                                                                                                               oi => oi.Rating.RatingInfoes.Select(ri => ri.MUser),
                                                                                                                                               oi => oi.MOrder,
                                                                                                                                               oi => oi.MOrder.MUser));
                for (int i = 0; i < orderInfos.Count; i++)
                {
                    Rating += (int)orderInfos[i].Rating.Rating1;
                    RatingTimes++;
                    ShopRatingBlockModel shopRatingBlockModel = new ShopRatingBlockModel(orderInfos[i]);
                    AllShopRatingBlockModels.Add(shopRatingBlockModel);
                }
            }
            Rating /= RatingTimes;
        }
        public void Search()
        {
            List<int> starNumber = new List<int>();
            if(IsHad1S)
            {
                starNumber.Add(1);
            }    
            if (IsHad2S)
            {
                starNumber.Add(2);
            }
            if (IsHad3S)
            {
                starNumber.Add(3);
            }
            if (IsHad4S)
            {
                starNumber.Add(4);
            }
            if (IsHad5S)
            {
                starNumber.Add(5);
            }
            if(starNumber.Count == 0)
            {
                DisplayShopRatingBlockModels = AllShopRatingBlockModels;
            }    
            else
            {
                DisplayShopRatingBlockModels = new List<ShopRatingBlockModel>(AllShopRatingBlockModels.Where(p => starNumber.Any(u=> (int)u == p.OrderInfo.Rating.Rating1)));
            }    
        }
        public async Task UpdateFavoriteProduct()
        {
            if (IsFavorite != null && IsFavorite == true)
            {
                await FavoriteStore.instance.Add(SelectedProduct);
            }
            else if (IsFavorite != null && IsFavorite == false)
            {
                await FavoriteStore.instance.Delete(SelectedProduct);
            }
        }
    }
}
