using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Forms;

namespace WPFEcommerceApp
{
    public class AdsDialogViewModel: BaseViewModel
    {
        public ICommand ChangeAdsCommand { get; set; }
        public ICommand SaveAdsCommand { get; set; }

        private string _sourceImageAds;
        public string SourceImageAds
        {
            get => _sourceImageAds;
            set
            {
                _sourceImageAds = value;
                ImageAds = new CroppedBitmap(new BitmapImage(new Uri(SourceImageAds)), new Int32Rect(0, 0, 0, 0));
                OnPropertyChanged();
            }
        }
        private CroppedBitmap _imageAds;
        public CroppedBitmap ImageAds
        {
            get => _imageAds;
            set
            {
                _imageAds = value;
                if (_imageAds.Height*20 >= _imageAds.Width*7)
                {
                    WidthImage = 700;
                    HeightImage = _imageAds.Height * 700 / _imageAds.Width;
                    CanvasLeft = 0;
                    CanvasTop = -(HeightImage - 210) / 2;
                }
                else
                {
                    HeightImage = 210;
                    WidthImage = _imageAds.Width * 210 / _imageAds.Height;
                    CanvasLeft = -(WidthImage - 700) / 2;
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
        public AdsDialogViewModel(CroppedBitmap croppedBitmap)
        {
            ImageAds = croppedBitmap;
            ChangeAdsCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if (op.FileName != "")
                {
                    SourceImageAds = op.FileName;
                }
            });

            SaveAdsCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                double ratio = ImageAds.PixelHeight / HeightImage;

                CroppedBitmap temp = new CroppedBitmap(ImageAds, new System.Windows.Int32Rect(
                    (int)Math.Round((Math.Abs(CanvasLeft)) * ratio),
                    (int)Math.Round((Math.Abs(canvasTop)) * ratio),
                    (int)Math.Round(700 * ratio),
                    (int)Math.Round(210 * ratio)));

                ImageAds = temp;
                croppedBitmap = temp;

                DialogHost.CloseDialogCommand.Execute(temp, null);
            });
        }
    }
}
