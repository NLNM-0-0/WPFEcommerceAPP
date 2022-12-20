using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace WPFEcommerceApp
{
    class ProductDetailViewModel : BaseViewModel
    {
        public ICommand NextImageCommand { get; set; }
        public ICommand PreviousImageCommand { get; set; }

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
        }
    }
}
