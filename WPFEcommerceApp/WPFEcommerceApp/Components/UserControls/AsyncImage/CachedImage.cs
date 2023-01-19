using System.Windows.Media.Imaging;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Net.Cache;
using Newtonsoft.Json.Linq;

namespace WPFEcommerceApp {
    public class CachedImage : Image {
        static CachedImage() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CachedImage), new FrameworkPropertyMetadata(typeof(CachedImage)));
        }

        public ImageSource CachedSource {
            get { return (ImageSource)GetValue(CachedSourceProperty); }
            set { SetValue(CachedSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CachedSourceProperty =
            DependencyProperty.Register("CachedSource", typeof(ImageSource), typeof(CachedImage), new PropertyMetadata(default(ImageSource), OnSourceChange));

        public FileCache.CacheMode ImageCacheMode {
            get { return (FileCache.CacheMode)GetValue(ImageCacheModeProperty); }
            set { SetValue(ImageCacheModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageCacheMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageCacheModeProperty =
            DependencyProperty.Register("ImageCacheMode", typeof(FileCache.CacheMode), typeof(CachedImage), new PropertyMetadata(FileCache.CacheMode.FromIE));



        private static async void OnSourceChange(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            var cachedImage = (CachedImage)obj;
            var bitmapImage = new BitmapImage();

            var url = (obj.GetValue(CachedSourceProperty) as ImageSource).ToString();
            var cacheMode = (FileCache.CacheMode)(obj.GetValue(ImageCacheModeProperty));

            if(cacheMode == FileCache.CacheMode.Local) 
                cachedImage.Source = new BitmapImage(new Uri("..\\..\\..\\Assets\\Images\\NoImage.jpg", UriKind.RelativeOrAbsolute));
            Uri outUri;
            if(!Uri.TryCreate(url, UriKind.Absolute, out outUri)
               || (outUri.Scheme != Uri.UriSchemeHttp && outUri.Scheme != Uri.UriSchemeHttps)) {
                cachedImage.Source = (obj.GetValue(CachedSourceProperty) as ImageSource);
                return;
            }

            switch(cacheMode) {
                case FileCache.CacheMode.FromIE:
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(url);
                    // Enable IE-like cache policy.
                    bitmapImage.UriCachePolicy = FileCache.CachePolicy;
                    bitmapImage.EndInit();
                    cachedImage.Source = bitmapImage;
                    break;

                case FileCache.CacheMode.Local:
                    try {
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = await FileCache.SaveCache(url);
                        bitmapImage.EndInit();
                        cachedImage.Source = bitmapImage;
                    } catch(Exception) {
                        // ignored, in case the downloaded file is a broken or not an image.
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


}