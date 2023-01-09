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
    /// Interaction logic for ProdfileShopAvaDialog.xaml
    /// </summary>
    public partial class ProfileShopAvaDialog : UserControl
    {
        public ProfileShopAvaDialog()
        {
            InitializeComponent();
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DataContext != null)
            {
                double ratio = (double)e.NewValue / (double)e.OldValue;
                if ((double)e.OldValue == 0)
                {
                    ratio = 1;
                }
                (DataContext as ProfileShopAvaDialogViewModel).HeightImage *= ratio;
                (DataContext as ProfileShopAvaDialogViewModel).WidthImage *= ratio;
                if (Canvas.GetLeft(content) * ratio - 250 * (ratio - 1) > 0)
                {
                    Canvas.SetLeft(content, 0);
                }
                else
                {
                    if (Canvas.GetLeft(content) * ratio - 250 * (ratio - 1) < 500 - (DataContext as ProfileShopAvaDialogViewModel).WidthImage)
                    {
                        Canvas.SetLeft(content, 500 - (DataContext as ProfileShopAvaDialogViewModel).WidthImage);
                    }
                    else
                    {
                        Canvas.SetLeft(content, Canvas.GetLeft(content) * ratio - 250 * (ratio - 1));
                    }
                }
                if (Canvas.GetTop(content) * ratio - 250 * (ratio - 1) > 0)
                {
                    Canvas.SetTop(content, 0);
                }
                else
                {
                    if (Canvas.GetTop(content) * ratio - 250 * (ratio - 1) < 500 - (DataContext as ProfileShopAvaDialogViewModel).HeightImage)
                    {
                        Canvas.SetTop(content, 500 - (DataContext as ProfileShopAvaDialogViewModel).HeightImage);
                    }
                    else
                    {
                        Canvas.SetTop(content, Canvas.GetTop(content) * ratio - 250 * (ratio - 1));
                    }
                }
            } 
        }
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                slider.Value -= 10;
            }
            else if (e.Delta > 0)
            {
                slider.Value += 10;
            }
        }
    }
}
