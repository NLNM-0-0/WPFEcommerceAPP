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
    internal class ProductBlockViewModel : BaseViewModel
    {
        #region Command
        public ICommand ChangeMainPictuceCommand { get; set; }
        public ICommand ShowMiniPictureCommand { get; set; }
        public ICommand HideMiniPictureCommand { get; set; }
        public ICommand ShowProductMainInfoCommand { get; set; }
        public ICommand HideProductMainInfoCommand { get; set; }
        #endregion

        #region Property
       /* private ObservableCollection<ImageProduct> _Images;
        public ObservableCollection<ImageProduct> Images
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
        private ObservableCollection<ImageProduct> _MiniImagesProduct = new ObservableCollection<ImageProduct>();
        public ObservableCollection<ImageProduct> MiniImagesProduct
        {
            get => _MiniImagesProduct;
            private set
            {
                _MiniImagesProduct = value;
                OnPropertyChanged();
            }
        }
        private ImageProduct _MainImage;
        public ImageProduct MainImage
        {
            get => _MainImage;
            set
            {
                _MainImage = value;
                OnPropertyChanged();
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        private string _Category;
        public string Category
        {
            get
            {
                return _Category;
            }
            set
            {
                _Category = value;
                OnPropertyChanged();
            }
        }

        private string _Brand;
        public string Brand
        {
            get
            {
                return _Brand;
            }
            set
            {
                _Brand = value;
                OnPropertyChanged();
            }
        }*/

        //Number of product remainder after displaying mini image
        public string NumberProductRemainder { get; private set; } = "";
        #endregion

        #region Contructor

        //đáng lẽ ở đây truyền 1 thể hiện của model mới đúng
        //mà chưa có nên tui ghi v tượng trưng thui
        public ProductBlockViewModel(ProductBlock product)
        {
            #region Will change after having database
            /*Images = new ObservableCollection<ImageProduct>();

            //Set a default image
            Images.Add(new ImageProduct());
            Images.Add(new ImageProduct("https://thuviendohoa.vn/upload/images/items/hinh-anh-ao-phong-nam-mau-trang-png-514.webp"));
            Images.Add(new ImageProduct());
            Images.Add(new ImageProduct());
            Images.Add(new ImageProduct());
            Images.Add(new ImageProduct());

            MainImage = Images[0];

            for (int i = 0; i < 4; i++)
            {
                if (i < _Images.Count)
                {
                    MiniImagesProduct.Add(_Images[i]);
                }
            }
            NumberProductRemainder = "+ " + (Images.Count - MiniImagesProduct.Count).ToString();

            Name = "Product Name";
            Category = "Category";
            Brand = "Brand";*/
            #endregion

            ShowMiniPictureCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                if(product.ListProductImage == null || product.ListProductImage.Count == 0)
                {
                    product.MainImage = Properties.Resources.DefaultProductImage;
                    return;
                }
                if(product.ListProductImage.Count==1)
                {
                    return;
                }    
                Grid grid = (p as Grid);
                grid.Visibility = Visibility.Visible;
            });

            ShowProductMainInfoCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                if (product.ListProductImage == null || product.ListProductImage.Count == 0)
                {
                    product.MainImage = Properties.Resources.DefaultProductImage;
                    return;
                }
                if (product.ListProductImage.Count == 1)
                {
                    return;
                }
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
                product.MainImage = (p as Image).Source.ToString();
                OnPropertyChanged();
            });
        }
        #endregion
    }
}
