using DataAccessLayer;
using MaterialDesignThemes.Wpf;
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
    class ProductDetailViewModel : BaseViewModel
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
        private int amount;
        public int Amount
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
        private ObservableCollection<string> imageProducts;
        public ObservableCollection<string> ImageProducts
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
        private ObservableCollection<Models.Product> products;
        public ObservableCollection<Models.Product> Products
        {
            get { return products; }
            set
            {
                products = value;
                OnPropertyChanged();
            }

        }

        private ObservableCollection<Models.Product> productAlsos;
        public ObservableCollection<Models.Product> ProductAlsos
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
        public bool IsFavorite
        {
            get
            {
                return FavoriteStore.instance != null && FavoriteStore.instance.FavoriteProductList != null && FavoriteStore.instance.FavoriteProductList.Any(p => p.Id == SelectedProduct.Id);
            }
        }
        private ObservableCollection<ProductBlockViewModel> productViewModels;
        public ObservableCollection<ProductBlockViewModel> ProductViewModels
        {
            get { return productViewModels; }
            set
            {
                productViewModels = value;
                OnPropertyChanged();
            }

        }
        private ObservableCollection<ProductBlockViewModel> productAlsoViewModels;
        public ObservableCollection<ProductBlockViewModel> ProductAlsoViewModels
        {
            get { return productAlsoViewModels; }
            set
            {
                productAlsoViewModels = value;
                OnPropertyChanged();
            }

        }
        private ObservableCollection<ShopRatingBlockModel> displayShopRatingBlockModels;
        public ObservableCollection<ShopRatingBlockModel> DisplayShopRatingBlockModels
        {
            get => displayShopRatingBlockModels;
            set
            {
                displayShopRatingBlockModels = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopRatingBlockModel> allShopRatingBlockModels;
        public ObservableCollection<ShopRatingBlockModel> AllShopRatingBlockModels
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
        public ProductDetailViewModel(Models.Product product)
        {
            SelectedProduct = product;
            ImageProducts = new ObservableCollection<string>();
            ProductViewModels = new ObservableCollection<ProductBlockViewModel>();
            ProductAlsoViewModels = new ObservableCollection<ProductBlockViewModel>();
            ProductRepository = new GenericDataRepository<Models.Product>();
            DisplayShopRatingBlockModels = new ObservableCollection<ShopRatingBlockModel>();
            AllShopRatingBlockModels = new ObservableCollection<ShopRatingBlockModel>();
            RightCommand = new RelayCommand<object>(p => true, Right);
            LeftCommand = new RelayCommand<object>(p => true, Left);
            Amount = 1;
            //if(String.IsNullOrEmpty(SelectedProduct.MUser.SourceImageAva))
            //{
            //    SourceImageAva = Properties.Resources.DefaultShopAvaImage;
            //}    
            //else
            //{
            //    SourceImageAva = SelectedProduct.MUser.SourceImageAva;
            //}
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
            Task task = Task.Run(async () => { await Load(); await LoadComments(); await LoadAlso(); });
            while (!task.IsCompleted) ;
            DisplayShopRatingBlockModels = AllShopRatingBlockModels;
            LoadProductBLockViewModel();
            LoadProductBLockAlsoViewModel();
            Plusamount = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                Amount += 1;
                OnPropertyChanged();
            });
            Tamount = new RelayCommand<object>((p) =>
            {
                return Amount >= 2;
            }, (p) =>
            {
                Amount -= 1;
                OnPropertyChanged();
            });
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
                /*var t = await FavouriteApi.Add(AccountStore.instance.CurrentAccount.Id, (p as Models.Product).Id);

                var dl = new ConfirmDialog() {
                    Header = t ? "Success" : "Oops",
                    Content = "This product has been added to your Favourite. \nCheck it out.",
                    CM = new RelayCommand<object>(pr => true, pr => {
                        NavigateProvider.FavouriteScreen().Navigate();
                    }),
                    Param = ""
                };
                await DialogHost.Show(dl, "Main");*/
                await FavoriteStore.instance.Add(SelectedProduct);
                OnPropertyChanged(nameof(IsFavorite));
            });
            UnFavouriteCommand = new RelayCommand<object>((p) =>
            {
                return (AccountStore.instance.CurrentAccount != null && SelectedProduct.IdShop != AccountStore.instance.CurrentAccount.Id);
            }, async (p) =>
            {
                /*var t = await FavouriteApi.Add(AccountStore.instance.CurrentAccount.Id, (p as Models.Product).Id);

                var dl = new ConfirmDialog() {
                    Header = t ? "Success" : "Oops",
                    Content = "This product has been added to your Favourite. \nCheck it out.",
                    CM = new RelayCommand<object>(pr => true, pr => {
                        NavigateProvider.FavouriteScreen().Navigate();
                    }),
                    Param = ""
                };
                await DialogHost.Show(dl, "Main");*/
                await FavoriteStore.instance.Delete(SelectedProduct);
                OnPropertyChanged(nameof(IsFavorite));
            });
            BuyNowCommand = new RelayCommand<object>((p) => { return ((IsHadSizeS || IsHadSizeM || IsHadSizeL || IsHadSizeXL || IsHadSizeXXL || IsHadOneSize) && AccountStore.instance.CurrentAccount != null && SelectedProduct.IdShop != AccountStore.instance.CurrentAccount.Id && Amount > 0); }, async (p) =>
            {
                var raw = p as Models.Product;
                var raw2 = await productRepo.GetSingleAsync(d => d.Id == raw.Id, d => d.ImageProducts);

                Product prd = new Product(raw2, Size, 1);
                List<Product> list = new List<Product>();
                list.Add(prd);


                var id = await GenerateID.Gen(typeof(MOrder));
                var repo = new GenericDataRepository<MUser>();
                var shop = await repo.GetSingleAsync(d => d.Id == raw.IdShop);

                Order o = new Order(id, AccountStore.instance.CurrentAccount.Id, raw.IdShop, "Processing", 20000, list, DateTime.Now, shop.Name, shop.SourceImageAva);

                NavigateProvider.CheckoutScreen().Navigate(o);
            });

            AddToBagCommand = new RelayCommand<object>((p) => { return ((IsHadSizeS || IsHadSizeM || IsHadSizeL || IsHadSizeXL || IsHadSizeXXL || IsHadOneSize) && AccountStore.instance.CurrentAccount != null && SelectedProduct.IdShop != AccountStore.instance.CurrentAccount.Id && Amount > 0); }, async (p) =>
            {
                await LoadAddToBag();
                var dl = new ConfirmDialog()
                {
                    Header = "Success",
                    Content = "This product has been added to your Bag. \nCheck it out.",
                    CM = new RelayCommand<object>(pr => true, pr => {
                        NavigateProvider.BagScreen().Navigate();
                    }),
                    Param = ""
                };
                await DialogHost.Show(dl, "Main");
            });
            SearchCommand = new RelayCommandWithNoParameter(() =>
            {
                Search();
            });
        }
        private async Task LoadAddToBag()
        {
            MainViewModel.IsLoading = true;
            GenericDataRepository<Models.Cart> dataRepository = new GenericDataRepository<Models.Cart>();
            await dataRepository.Add(new Models.Cart
            {
                IdProduct = SelectedProduct.Id,
                IdUser = AccountStore.instance.CurrentAccount.Id,
                Amount = Amount,
                Size = Size
            });
            MainViewModel.IsLoading = false;
        }
        private async Task Load()
        {
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => p.IdShop == SelectedProduct.IdShop && p.Id != SelectedProduct.Id,
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts, 
                                                                                                        p => p.MUser));
        }

        private async Task LoadAlso()
        {
            ProductAlsos = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p =>p.Id != SelectedProduct.Id && (p.IdBrand == SelectedProduct.IdBrand || p.IdCategory == SelectedProduct.IdCategory),
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts, 
                                                                                                        p => p.MUser));
        }

        private void LoadProductBLockViewModel()
        {
            ProductViewModels.Clear();
            foreach (Models.Product product in Products)
            {
                ProductViewModels.Add(new ProductBlockViewModel(product));
            }
        }

        private void LoadProductBLockAlsoViewModel()
        {
            ProductAlsoViewModels.Clear();
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
                    var temp3 = (int)((temp + temp2) / 350);
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

                    int temp3 = (int)Math.Ceiling(temp / 365) - 1;

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
            ObservableCollection<Models.MOrder> orders = new ObservableCollection<Models.MOrder>((await orderReposition.GetListAsync(r => (r != null && r.Status == "Completed"),
                                                                                                                                    r => r.MUser)));
            foreach (Models.MOrder order in orders)
            {
                ObservableCollection<Models.OrderInfo> orderInfos = new ObservableCollection<OrderInfo>(await orderInfoReposition.GetListAsync(oi => (oi.IdOrder == order.Id && oi.IdRating != null && oi.IdProduct == SelectedProduct.Id),
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
                Rating /= RatingTimes;
            }
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
                DisplayShopRatingBlockModels = new ObservableCollection<ShopRatingBlockModel>(AllShopRatingBlockModels.Where(p => starNumber.Any(u=> (int)u == p.OrderInfo.Rating.Rating1)));
            }    
        }
    }
}
