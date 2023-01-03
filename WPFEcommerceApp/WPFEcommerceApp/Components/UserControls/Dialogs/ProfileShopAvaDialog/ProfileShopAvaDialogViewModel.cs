using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace WPFEcommerceApp
{
    public class ProfileShopAvaDialogViewModel :BaseViewModel
    {
        public ICommand ChangeAvaShopCommand { get; set; }
        public ICommand ChangeToDefaultAvaShopCommand { get; set; }
        public ICommand SaveAvaShopCommand { get; set; }

        private string sourceImageAva = "";
        public string SourceImageAva
        {
            get => sourceImageAva;
            set
            {
                sourceImageAva = value; 
                ImageAva = new CroppedBitmap(new BitmapImage(new Uri(SourceImageAva)), new Int32Rect(0,0,0,0));
                OnPropertyChanged();
            }
        }
        private CroppedBitmap imageAva;
        public CroppedBitmap ImageAva
        {
            get => imageAva;
            set
            {
                imageAva = value;
                if (imageAva.Height >= imageAva.Width)
                {
                    WidthImage = 500;
                    HeightImage = imageAva.Height * 500 / imageAva.Width;
                    CanvasLeft = 0;
                    CanvasTop = -(HeightImage - 400) / 2;
                }
                else
                {
                    HeightImage = 400;
                    WidthImage = imageAva.Width * 400 / imageAva.Height;
                    CanvasLeft = -(WidthImage - 500) / 2;
                    CanvasTop = 0;
                }
                OnPropertyChanged();
            }
        }
        private double heightImage;
        public double HeightImage
        {
            get => heightImage;
            set
            {
                heightImage = value;
                OnPropertyChanged();
            }
        }
        private double widthImage;
        public double WidthImage
        {
            get => widthImage;
            set
            {
                widthImage = value;
                OnPropertyChanged();
            }
        }
        private double canvasLeft;
        public double CanvasLeft
        {
            get => canvasLeft;
            set
            {
                canvasLeft = value;
                OnPropertyChanged();
            }
        }
        private double canvasTop;
        public double CanvasTop
        {
            get => canvasTop;
            set
            {
                canvasTop = value;
                OnPropertyChanged();
            }
        }
        public ProfileShopAvaDialogViewModel(CroppedBitmap croppedBitmap)
        {
            ImageAva = croppedBitmap;
            ChangeAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) => 
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if(op.FileName != "")
                {
                    SourceImageAva = op.FileName;
                }    
            });
            ChangeToDefaultAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                SourceImageAva = Properties.Resources.DefaultShopAvaImage;
            });
            SaveAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                double ratio = ImageAva.PixelHeight / HeightImage;

                CroppedBitmap temp = new CroppedBitmap(ImageAva, new System.Windows.Int32Rect(
                    (int)Math.Round((Math.Abs(CanvasLeft) + 75) * ratio),
                    (int)Math.Round((Math.Abs(canvasTop) + 25) * ratio),
                    (int)Math.Round(350 * ratio),
                    (int)Math.Round(350 * ratio)));

                ImageAva  = temp;
                croppedBitmap = temp;

                DialogHost.CloseDialogCommand.Execute(temp, null);
            });
        }
    }
}
