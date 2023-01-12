using DataAccessLayer;
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
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for ShopOrder.xaml
    /// </summary>
    public partial class ShopOrder : UserControl
    {
        public ShopOrder()
        {
            InitializeComponent();
        }
        private void Scroll_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void scroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (-e.VerticalChange > 0)
            {
                if (scroll.VerticalOffset < 320)
                {
                    Canvas.SetTop(buttonScroll, 320 - scroll.VerticalOffset);
                    buttonScroll.Visibility = Visibility.Collapsed;
                }
                if (scroll.VerticalOffset < 250)
                {
                    Canvas.SetTop(status, 250 - scroll.VerticalOffset);
                }
            }
            else
            {
                if (Canvas.GetTop(buttonScroll) - e.VerticalChange >= 0)
                {
                    Canvas.SetTop(buttonScroll, Canvas.GetTop(buttonScroll) - e.VerticalChange);
                }
                else
                {
                    Canvas.SetTop(buttonScroll, 70);
                    buttonScroll.Visibility = Visibility.Visible;
                }
                if (Canvas.GetTop(status) - e.VerticalChange >= 0)
                {
                    Canvas.SetTop(status, Canvas.GetTop(status) - e.VerticalChange);
                }
                else
                {
                    Canvas.SetTop(status, 0);
                }
            }
        }
    }
}
