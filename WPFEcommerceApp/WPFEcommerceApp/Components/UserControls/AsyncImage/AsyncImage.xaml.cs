using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for AsyncImage.xaml
    /// </summary>
    public partial class AsyncImage : UserControl {


        public Stretch Stretch {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(AsyncImage), new PropertyMetadata(Stretch.None));


        public ImageSource Source {
            get { return (ImageSource)GetValue(ImgSourceProperty); }
            set { SetValue(ImgSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImgSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImgSourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(AsyncImage), new PropertyMetadata(default(ImageSource)));

        public AsyncImage() {
            InitializeComponent();
            //never assign DataContext in here;
        }
    }
}
