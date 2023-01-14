using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
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
        public ICommand CancelCommand { get; set; }
        private string sourceImageBackground;
        public string SourceImageBackground
        {
            get => sourceImageBackground;
            set
            {
                sourceImageBackground = value;
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
                if (imageBackground.Height * 5 >= imageBackground.Width)
                {
                    WidthImage = 850;
                    HeightImage = imageBackground.Height * 850 / imageBackground.Width;
                    CanvasLeft = 0;
                    CanvasTop = - (HeightImage - 170) / 2;
                }
                else
                {
                    HeightImage = 170;
                    WidthImage = imageBackground.Width * 170 / imageBackground.Height;
                    CanvasLeft = -(WidthImage - 850) / 2;
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
        System.Windows.Controls.UserControl PreviousItem;
        public ProfileShopBackgroundDialogViewModel(CroppedBitmap croppedBitmap, System.Windows.Controls.UserControl previous = null)
        {
            PreviousItem = previous;
            ImageBackground = croppedBitmap;
            ChangeBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if (op.FileName != "")
                {
                    SourceImageBackground = op.FileName;
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

                        ImageBackground = new CroppedBitmap(bitmapImage as BitmapSource, new Int32Rect(0, 0, 0, 0));
                    }
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
                    (int)Math.Round((Math.Abs(CanvasLeft)) * ratio),
                    (int)Math.Round((Math.Abs(canvasTop)) * ratio),
                    (int)Math.Round(850 * ratio),
                    (int)Math.Round(170 * ratio)));

                ImageBackground = temp;
                croppedBitmap = temp;

                if (PreviousItem != null)
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    ProfileShopDialogViewModel vm = (ProfileShopDialogViewModel)(PreviousItem as ProfileShopDialog).DataContext;
                    vm.LoadBackground(temp);
                    DialogHost.Show(PreviousItem, "Main");
                }
                else
                {
                    DialogHost.CloseDialogCommand.Execute(temp, null);
                }
            });
            CancelCommand = new RelayCommandWithNoParameter(() =>
            {
                DialogHost.CloseDialogCommand.Execute(null, null);
                if (PreviousItem != null)
                {
                    DialogHost.Show(PreviousItem, "Main");
                }
            });
        }
    }
}
