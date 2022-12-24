using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        #endregion

        #region Contructor
        public ProductBlockViewModel(Models.Product product)
        {
            SelectedProduct = product;
            foreach (Models.ImageProduct image in SelectedProduct.ImageProducts)
            {
                Images.Add(image.Source);
            }  
            if(Images.Count > 0) 
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
            #endregion

            ShowMiniPictureCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                if (Images.Count == 0)
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
                Grid grid = (p as Grid);
                grid.Visibility = Visibility.Collapsed;
            });

            ChangeMainPictuceCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                MainImage = p.ToString();
                OnPropertyChanged(nameof(MainImage));
            });
            OpenDialogCommand = new RelayCommandWithNoParameter(() =>
            {
                ProductDetail productDetail = new ProductDetail();
                productDetail.DataContext = new ProductDetailViewModel(SelectedProduct);
                DialogHost.Show(productDetail, "App");
            });
            OpenPageCommand = new RelayCommandWithNoParameter(() =>
            {
                MessageBox.Show("Navigate to page product");
            });
        }
    }
}