using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for AsyncImage.xaml
    /// </summary>
    public partial class AsyncImage : UserControl {

        /// <summary>
        /// get or set Stretch Property
        /// </summary>
        public Stretch Stretch {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(AsyncImage), new PropertyMetadata(Stretch.Uniform));


        /// <summary>
        /// get or set Default Image when Image not load from Http
        /// </summary>
        public string Default {
            get { return (string)GetValue(DefaultProperty); }
            set { SetValue(DefaultProperty, value); }
        }

        public static readonly DependencyProperty DefaultProperty =
            DependencyProperty.Register("Default", typeof(string), typeof(AsyncImage), new PropertyMetadata("..\\..\\..\\Assets\\Images\\NoImage.jpg"));


        /// <summary>
        /// get, set CacheMode
        /// </summary>
        public FileCache.CacheMode ImageCacheMode {
            get { return (FileCache.CacheMode)GetValue(ImageCacheModeProperty); }
            set { SetValue(ImageCacheModeProperty, value); }
        }

        public static readonly DependencyProperty ImageCacheModeProperty =
            DependencyProperty.Register("ImageCacheMode", typeof(FileCache.CacheMode), typeof(AsyncImage), new PropertyMetadata(FileCache.CacheMode.FromIE));

        /// <summary>
        /// get or set Source of image
        /// </summary>
        public ImageSource Source {
            get { return (ImageSource)GetValue(ImgSourceProperty); }
            set { SetValue(ImgSourceProperty, value); }
        }

        public static readonly DependencyProperty ImgSourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(AsyncImage), new PropertyMetadata(default(ImageSource)));

        public AsyncImage() {
            InitializeComponent();
        }
    }
}