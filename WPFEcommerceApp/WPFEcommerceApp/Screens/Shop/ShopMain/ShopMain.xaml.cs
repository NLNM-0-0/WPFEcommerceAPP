using WPFEcommerceApp.Models;
using System.Windows.Controls;
using System.Threading.Tasks;
using DataAccessLayer;
using System.Linq;
using System.Windows.Threading;
using System;
using System.Windows;
using WPFEcommerceApp.UserControls.Dialogs.AddProductDialog;
using System.Runtime.CompilerServices;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for ShopMain.xaml
    /// </summary>
    public partial class ShopMain : UserControl
    {
        public ShopMain()
        {
            InitializeComponent();
        }

        private void ProductBlockByCategory_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                if(this.DataContext.GetType() != typeof(ShopMainViewModel) || (this.DataContext as ShopMainViewModel).LoadedCommand == null)
                {
                    return;
                }
                (this.DataContext as ShopMainViewModel).LoadedCommand.Execute(new Tuple<double, double, double>(newProductBlock.ActualHeight, bestSellerProductBlock.ActualHeight, BigDiscountProductBlock.ActualHeight));
            }
        }

        private void Scroll_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void scroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (- e.VerticalChange > 0)
            {
                if (scroll.VerticalOffset < 230)
                {
                     Canvas.SetTop(categoryGrid, 230 - scroll.VerticalOffset);
                }
            }
            else
            {
                if (Canvas.GetTop(categoryGrid) - e.VerticalChange >= 0)
                {
                    Canvas.SetTop(categoryGrid, Canvas.GetTop(categoryGrid) - e.VerticalChange);
                }
                else
                {
                    Canvas.SetTop(categoryGrid, 0);
                }
            }
        }
    }
}
