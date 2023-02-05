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
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

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
                if (_imageAds.Height*10 >= _imageAds.Width*3)
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
                    var stream = File.Open(op.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
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

                        ImageAds = new CroppedBitmap(bitmapImage as BitmapSource, new Int32Rect(0, 0, 0, 0));
                    }
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
