using MaterialDesignThemes.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ChangeImageProductDialogViewModel : BaseViewModel
    {
        public ICommand ChangeSelectedImageCommand { get; set; }
        public ICommand AddImageCommand { get; set; }
        public ICommand DeleteImageCommand { get; set; }
        public ICommand CloseDialogCommand { get; set; }
        private ObservableCollection<MImageProuct> imageProducts;
        public ObservableCollection<MImageProuct> ImageProducts
        {
            get => imageProducts;
            set
            {
                imageProducts = value;
                OnPropertyChanged();
            }
        }
        private MImageProuct selectedImageSource;
        public MImageProuct SelectedImageSource
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
        public ChangeImageProductDialogViewModel(ObservableCollection<MImageProuct> imageProducts)
        {
            ImageProducts = imageProducts;
            if (ImageProducts.Count == 0)
            {
                SelectedImageSource = new MImageProuct() { BMImage = new BitmapImage(new Uri(Properties.Resources.DefaultProductImage)), Source = Properties.Resources.DefaultProductImage };
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
                MImageProuct image = p as MImageProuct;
                SelectedImageSource = image;
            });
            AddImageCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog() { Multiselect = true};
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if(op.FileNames!=null)
                {
                    int theNumberImageNeed = Math.Min(op.FileNames.Count(), 10 - ImageProducts.Count);
                    for (int i = 0; i < theNumberImageNeed; i++)
                    {
                        string fileName = op.FileNames[i];
                        var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        System.Drawing.Image img = new Bitmap(stream);
                        Bitmap copy = new Bitmap(img.Width, img.Height);
                        copy.SetResolution(img.HorizontalResolution, img.VerticalResolution);
                        using (var graphic = Graphics.FromImage(copy))
                        {
                            graphic.Clear(System.Drawing.Color.White);
                            graphic.DrawImageUnscaled(img, 0, 0);
                        }
                        using (var memory = new MemoryStream())
                        {
                            copy.Save(memory, ImageFormat.Jpeg);
                            memory.Position = 0;
                            var bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = memory;
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.EndInit();
                            bitmapImage.Freeze();
                            ImageProducts.Add(new MImageProuct() { BMImage = bitmapImage, Source = fileName });
                        }
                    }    
                }    
                if (ImageProducts.Count >= 1)
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
                foreach (MImageProuct imageSource in ImageProducts)
                {
                    if (imageSource == SelectedImageSource)
                    {
                        ImageProducts.Remove(imageSource);
                        if (ImageProducts.Count == 0)
                        {
                            IsCanDeleteImage = false;
                            SelectedImageSource = new MImageProuct() { BMImage = new BitmapImage(new Uri(Properties.Resources.DefaultProductImage)), Source = Properties.Resources.DefaultProductImage};
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
            if (CloseDialogCommand == null)
            {
                CloseDialogCommand = new RelayCommandWithNoParameter(() =>
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                });
            }
        }
    }
}
