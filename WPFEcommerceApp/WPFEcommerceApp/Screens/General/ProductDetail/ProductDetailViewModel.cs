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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    class ProductDetailViewModel : BaseViewModel
    {
        public GenericDataRepository<Models.Cart> CartRepo { get; set; }
        private readonly GenericDataRepository<Models.Product> productRepo = new GenericDataRepository<Models.Product>();
        private readonly GenericDataRepository<MUser> userRepo = new GenericDataRepository<MUser>();
        public ICommand NextImageCommand { get; set; }
        public ICommand PreviousImageCommand { get; set; }

        public ICommand FavouriteCommand { get; set; }
        public ICommand AddToBagCommand { get; set; }
        public ICommand Plusamount { get; set; }

        public ICommand Tamount { get; set; }

        public ICommand BuyNowCommand { get; set; }
        public ICommand UnFavouriteCommand { get; set; }
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


        public ProductDetailViewModel(Models.Product product)
        {
            SelectedProduct = product;
            ImageProducts = new ObservableCollection<string>();
            Plusamount = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                Amount += 1;
                OnPropertyChanged();
            });
            Tamount = new RelayCommand<object>((p) =>
            {
                return Amount >= 1;
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



    }
}
