using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
using WPFEcommerceApp.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace WPFEcommerceApp
{
    public class ProfileShopAvaDialogViewModel :BaseViewModel
    {
        public ICommand ChangeAvaShopCommand { get; set; }
        public ICommand ChangeToDefaultAvaShopCommand { get; set; }
        public ICommand SaveAvaShopCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private string sourceImageAva = "";
        public string SourceImageAva
        {
            get => sourceImageAva;
            set
            {
                sourceImageAva = value; 
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
                    CanvasTop = -(HeightImage - 500) / 2;
                }
                else
                {
                    HeightImage = 500;
                    WidthImage = imageAva.Width * 500 / imageAva.Height;
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
        System.Windows.Controls.UserControl PreviousItem;
        public ProfileShopAvaDialogViewModel(CroppedBitmap croppedBitmap, System.Windows.Controls.UserControl previous = null)
        {
            PreviousItem = previous;
            ImageAva = croppedBitmap;
            ChangeAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) => 
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if(op.FileName != "")
                {
                    SourceImageAva = op.FileName;
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

                        ImageAva = new CroppedBitmap(bitmapImage as BitmapSource, new Int32Rect(0, 0, 0, 0));
                    }
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
                    (int)Math.Round((Math.Abs(CanvasLeft)) * ratio),
                    (int)Math.Round((Math.Abs(canvasTop)) * ratio),
                    (int)Math.Round(500 * ratio),
                    (int)Math.Round(500 * ratio)));

                ImageAva  = temp;
                croppedBitmap = temp;

                if (PreviousItem != null)
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    ProfileShopDialogViewModel vm = (ProfileShopDialogViewModel)(PreviousItem as ProfileShopDialog).DataContext;
                    vm.LoadAva(temp);
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
                if(PreviousItem!=null)
                {
                    DialogHost.Show(PreviousItem, "Main");
                }    
            });
        }
    }
}
