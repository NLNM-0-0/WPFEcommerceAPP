using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace WPFEcommerceApp
{
    public class ProfileShopBackgroundDialogViewModel : BaseViewModel
    {
        public ICommand ChangeToDefaultBackgroundShopCommand { get; set; }
        public ICommand ChangeBackgroundShopCommand { get; set; }
        public ICommand SaveBackgroundShopCommand { get; set; }
        private string sourceImageBackground;
        public string SourceImageBackground
        {
            get => sourceImageBackground;
            set
            {
                sourceImageBackground = value;
                ImageBackground = new CroppedBitmap(new BitmapImage(new Uri(SourceImageBackground)), new Int32Rect(0, 0, 0, 0));
                OnPropertyChanged();
            }
        }
        private CroppedBitmap imageBackground;
        public CroppedBitmap ImageBackground
        {
            get => imageBackground;
            set
            {
                imageBackground = value;
                if (imageBackground.Height >= imageBackground.Width)
                {
                    WidthImage = 500;
                    HeightImage = imageBackground.Height * 500 / imageBackground.Width;
                    CanvasLeft = 0;
                    CanvasTop = - (HeightImage - 400) / 2;
                }
                else
                {
                    HeightImage = 400;
                    WidthImage = imageBackground.Width * 400 / imageBackground.Height;
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
        public ProfileShopBackgroundDialogViewModel(CroppedBitmap croppedBitmap)
        {
            ImageBackground = croppedBitmap;
            ChangeBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if (op.FileName != "")
                {
                    SourceImageBackground = op.FileName;
                }
            });
            ChangeToDefaultBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                SourceImageBackground = Properties.Resources.DefaultShopBackgroundImage;
            });
            SaveBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                double ratio = ImageBackground.PixelHeight / HeightImage;

                CroppedBitmap temp = new CroppedBitmap(ImageBackground, new System.Windows.Int32Rect(
                    (int)Math.Round((Math.Abs(CanvasLeft) + 75) * ratio),
                    (int)Math.Round((Math.Abs(canvasTop) + 25) * ratio),
                    (int)Math.Round(350 * ratio),
                    (int)Math.Round(350 * ratio)));

                ImageBackground = temp;
                croppedBitmap = temp;

                DialogHost.CloseDialogCommand.Execute(temp, null);
            });
        }
    }
}
