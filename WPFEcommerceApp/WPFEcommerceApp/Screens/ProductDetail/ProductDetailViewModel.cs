using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace WPFEcommerceApp
{
    class ProductDetailViewModel : BaseViewModel
    {
        public GenericDataRepository<Models.Cart> CartRepo { get; set; }
        public GenericDataRepository<Models.Product> FavouriteRepo { get; set; }
        public ICommand NextImageCommand { get; set; }
        public ICommand PreviousImageCommand { get; set; }

        public ICommand FavouriteCommand { get; set; }
        public ICommand AddToBagCommand { get; set; }

        public ICommand BuyNowCommand { get; set; }
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
                if(isHadOneSize == true)
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
        public ProductDetailViewModel(Models.Product product)
        {
            SelectedProduct = product;
            ImageProducts = new ObservableCollection<string>();
            foreach (Models.ImageProduct imageProduct in product.ImageProducts)
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
            FavouriteCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                MessageBox.Show("Dang them vao trang Yeu thich cua ban");
            });
            BuyNowCommand = new RelayCommand<object>((p) => { return (IsHadSizeS || IsHadSizeM || IsHadSizeL || IsHadSizeXL || IsHadSizeXXL || IsHadOneSize); }, (p) =>
            {
                
                MessageBox.Show("Dang chuyen den trang mua hang");
            });
            AddToBagCommand = new RelayCommand<object>((p) => { return (IsHadSizeS || IsHadSizeM || IsHadSizeL || IsHadSizeXL || IsHadSizeXXL || IsHadOneSize); }, (p) =>
            {
                Task task = Task.Run(async () => await LoadAddToBag());
                while (!task.IsCompleted) ;
                MessageBox.Show("Da them vao gio hang cua ban");
            });
        }
        private async Task LoadAddToBag()
        {
            GenericDataRepository<Models.Cart> dataRepository = new GenericDataRepository<Models.Cart>();
            await dataRepository.Add(new Models.Cart
            {
                IdProduct = SelectedProduct.Id,
                IdUser = AccountStore.instance.CurrentAccount.Id,
                Amount = 1,
                Size = Size
            });
        }

       

    }
}
