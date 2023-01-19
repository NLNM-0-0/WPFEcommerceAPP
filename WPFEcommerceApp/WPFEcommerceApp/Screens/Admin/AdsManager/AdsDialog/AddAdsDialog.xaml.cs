
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for AddBranDialog.xaml
    /// </summary>
    public partial class AddAdsDialog : UserControl
    {
        public AddAdsDialog()
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
                (DataContext as AdsDialogViewModel).HeightImage *= ratio;
                (DataContext as AdsDialogViewModel).WidthImage *= ratio;
                if (Canvas.GetLeft(content) * ratio - 350 * (ratio - 1) > 0)
                {
                    Canvas.SetLeft(content, 0);
                }
                else
                {
                    if (Canvas.GetLeft(content) * ratio - 350 * (ratio - 1) < 700 - (DataContext as AdsDialogViewModel).WidthImage)
                    {
                        Canvas.SetLeft(content, 700 - (DataContext as AdsDialogViewModel).WidthImage);
                    }
                    else
                    {
                        Canvas.SetLeft(content, Canvas.GetLeft(content) * ratio - 350 * (ratio - 1));
                    }
                }
                if (Canvas.GetTop(content) * ratio - 105 * (ratio - 1) > 0)
                {
                    Canvas.SetTop(content, 0);
                }
                else
                {
                    if (Canvas.GetTop(content) * ratio - 105 * (ratio - 1) < 210 - (DataContext as AdsDialogViewModel).HeightImage)
                    {
                        Canvas.SetTop(content, 210 - (DataContext as AdsDialogViewModel).HeightImage);
                    }
                    else
                    {
                        Canvas.SetTop(content, Canvas.GetTop(content) * ratio - 105 * (ratio - 1));
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
