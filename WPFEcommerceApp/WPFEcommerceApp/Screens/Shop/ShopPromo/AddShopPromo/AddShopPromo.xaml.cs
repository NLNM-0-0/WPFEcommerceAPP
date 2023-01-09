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

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for AddShopPromo.xaml
    /// </summary>
    public partial class AddShopPromo : UserControl
    {
        public AddShopPromo()
        {
            InitializeComponent();
            this.DataContext = new AddShopPromoViewModel();
            this.Loaded += AddShopPromo_Loaded;
        }

        private void AddShopPromo_Loaded(object sender, RoutedEventArgs e)
        {
            //Declare a scroll viewer object.
            Decorator border = VisualTreeHelper.GetChild(listView, 0) as Decorator;

            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;
            scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if((sender as ScrollViewer).VerticalOffset == 0 && e.Delta > 0)
            {
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
            }
            else if ((sender as ScrollViewer).VerticalOffset + (sender as ScrollViewer).ViewportHeight == (sender as ScrollViewer).ExtentHeight && e.Delta < 0)
            {
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
            }
            else
            {
                (sender as ScrollViewer).ScrollToVerticalOffset((sender as ScrollViewer).VerticalOffset - e.Delta);
            }
            e.Handled = true;
        }
    }
}
