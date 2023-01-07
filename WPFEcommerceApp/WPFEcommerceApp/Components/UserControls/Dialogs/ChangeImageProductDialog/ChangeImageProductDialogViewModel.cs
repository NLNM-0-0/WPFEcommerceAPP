using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFEcommerceApp
{
    public class ChangeImageProductDialogViewModel : BaseViewModel
    {
        public ICommand ChangeSelectedImageCommand { get; set; }
        public ICommand AddImageCommand { get; set; }
        public ICommand DeleteImageCommand { get; set; }

        private ObservableCollection<string> imageProducts;
        public ObservableCollection<string> ImageProducts
        {
            get => imageProducts;
            set
            {
                imageProducts = value;
                OnPropertyChanged();
            }
        }
        private string selectedImageSource;
        public string SelectedImageSource
        {
            get => selectedImageSource;
            set
            {
                selectedImageSource = value;
                OnPropertyChanged();
            }
        }
        private bool isCanAddImage;
        public bool IsCanAddImage
        {
            get => isCanAddImage;
            set
            {
                isCanAddImage = value;
                OnPropertyChanged();
            }
        }
        private bool isCanDeleteImage;
        public bool IsCanDeleteImage
        {
            get => isCanDeleteImage;
            set
            {
                isCanDeleteImage = value;
                OnPropertyChanged();
            }
        }
        public ChangeImageProductDialogViewModel(ObservableCollection<string> imageProducts)
        {
            ImageProducts = imageProducts;
            SelectedImageSource = "";

            if (ImageProducts.Count == 0)
            {
                SelectedImageSource = Properties.Resources.DefaultProductImage;
                isCanDeleteImage = false;
                IsCanAddImage = true;
            }
            else
            {
                selectedImageSource = ImageProducts.First();
                isCanDeleteImage = true;
                if (ImageProducts.Count < 10)
                {
                    isCanAddImage = true;
                }
                else
                {
                    isCanAddImage = false;
                }
            }
            ChangeSelectedImageCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                Image image = p as Image;
                //VHCMT => Null error
                if(image == null || image.Source == null) 
                {
                    return;
                }
                SelectedImageSource = image.Source.ToString();
            });
            AddImageCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if (op.FileName != "")
                {
                    ImageProducts.Add(op.FileName);
                }
                if (ImageProducts.Count == 1)
                {
                    SelectedImageSource = ImageProducts.First();
                }
                IsCanDeleteImage = true;
                if (ImageProducts.Count >= 10)
                {
                    IsCanAddImage = false;
                }
            });
            DeleteImageCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                foreach (string imageSource in ImageProducts)
                {
                    if (imageSource == SelectedImageSource)
                    {
                        ImageProducts.Remove(imageSource);
                        if (ImageProducts.Count == 0)
                        {
                            IsCanDeleteImage = false;
                            SelectedImageSource = Properties.Resources.DefaultProductImage;
                        }
                        else
                        {
                            if (ImageProducts.Count < 10)
                                IsCanAddImage = true;
                            SelectedImageSource = ImageProducts.First();
                        }
                        break;
                    }
                }
            });
        }
    }
}
