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
using WPFEcommerceApp.UserControls.Dialogs.AddProductDialog;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for ProductBlockByCategory.xaml
    /// </summary>
    public partial class ProductBlockByCategory : UserControl
    {
        public ProductBlockByCategory()
        {
            InitializeComponent();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            double scaleX = 1, scaleY = 1;
            if (source != null)
            {
                scaleX = source.CompositionTarget.TransformToDevice.M11;
                scaleY = source.CompositionTarget.TransformToDevice.M22;
            }
            this.Measure(new Size(this.ActualWidth * scaleX, this.ActualHeight * scaleY));
        }
    }
}
