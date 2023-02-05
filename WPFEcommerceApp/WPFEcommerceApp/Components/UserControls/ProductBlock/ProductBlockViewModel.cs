using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ProductBlockViewModel : BaseViewModel
    {
        #region Command
        public ICommand ChangeMainPictuceCommand { get; set; }
        public ICommand ShowMiniPictureCommand { get; set; }
        public ICommand HideMiniPictureCommand { get; set; }
        public ICommand ShowProductMainInfoCommand { get; set; }
        public ICommand HideProductMainInfoCommand { get; set; }
        public ICommand OpenDialogCommand { get; set; }
        public ICommand OpenPageCommand { get; set; }
        #endregion

        #region Property
        private ObservableCollection<string> _Images = new ObservableCollection<string>();
        public ObservableCollection<string> Images
        {
            get => _Images;
            set
            {
                _Images = value;
                if (_Images.Count != 0)
                {
                    MainImage = Images[0];
                }
                for (int i = 0; i < 4; i++)
                {
                    if (i < _Images.Count)
                    {
                        MiniImagesProduct.Add(_Images[i]);
                    }
                }
                NumberProductRemainder = "+ " + (Images.Count - MiniImagesProduct.Count).ToString();
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> _MiniImagesProduct = new ObservableCollection<string>();
        public ObservableCollection<string> MiniImagesProduct
        {
            get => _MiniImagesProduct;
            private set
            {
                _MiniImagesProduct = value;
                OnPropertyChanged();
            }
        }
        private string _MainImage;
        public string MainImage
        {
            get => _MainImage;
            set
            {
                _MainImage = value;
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

        //Number of product remainder after displaying mini image
        public string NumberProductRemainder { get; private set; } = "";

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
        #endregion

        #region Contructor
        public ProductBlockViewModel(Models.Product product)
        {
            SelectedProduct = product;
            FavoriteStore.instance.FavoriteListChanged += Instance_FavoriteListChanged;

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
            IsShop = false;
            foreach (Models.ImageProduct image in SelectedProduct.ImageProducts)
            {
                Images.Add(image.Source);
            }
            if (Images.Count > 0)
            {
                MainImage = Images[0];
            }
            else
            {
                MainImage = Properties.Resources.DefaultProductImage;
            }

            for (int i = 0; i < 4; i++)
            {
                if (i < Images.Count)
                {
                    MiniImagesProduct.Add(Images[i]);
                }
            }
            NumberProductRemainder = "+ " + (Images.Count - MiniImagesProduct.Count).ToString();
            

            ShowMiniPictureCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                if (Images.Count <= 1)
                {
                    return;
                }
                Grid grid = (p as Grid);
                grid.Visibility = Visibility.Visible;
            });

            ShowProductMainInfoCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                Grid grid = (p as Grid);
                grid.Visibility = Visibility.Visible;
            });

            HideMiniPictureCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                Grid grid = (p as Grid);
                grid.Visibility = Visibility.Collapsed;
            });

            HideProductMainInfoCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                if (Images.Count <= 1)
                {
                    return;
                }
                Grid grid = (p as Grid);
                grid.Visibility = Visibility.Collapsed;
            });

            ChangeMainPictuceCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                MainImage = p.ToString();
                OnPropertyChanged(nameof(MainImage));
            });
            OpenDialogCommand = new RelayCommandWithNoParameter(async () =>
            {
                MainViewModel.SetLoading(true);
                ProductDetailMini productDetail = new ProductDetailMini();
                productDetail.DataContext = new ProductDetailNormalViewModel(SelectedProduct);
                MainViewModel.SetLoading(false);
                await DialogHost.Show(productDetail, "App");
            });
            OpenPageCommand = new RelayCommandWithNoParameter(() =>
            {
                NavigateProvider.ProductDetailScreen().Navigate(SelectedProduct);
            });
        }
        #endregion
        private void Instance_FavoriteListChanged()
        {
            OnPropertyChanged(nameof(HeartVisibility));
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
        }

        public async Task UpdateFavoriteProduct()
        {
            if(IsFavorite != null && IsFavorite == true)
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