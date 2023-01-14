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
using System.Text.RegularExpressions;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for ProductDetail.xaml
    /// </summary>
    public partial class ProductDetail : UserControl
    {
        public ProductDetail()
        {
            InitializeComponent();
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}

