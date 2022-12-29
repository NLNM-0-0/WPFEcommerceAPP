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
    /// Interaction logic for ProductInfoLandscape.xaml
    /// </summary>
    public partial class ProductInfoLandscape : UserControl
    {
        public static readonly DependencyProperty IsCanEditProperty = DependencyProperty.Register(
        "IsCanEdit", typeof(Boolean), typeof(ProductInfoLandscape), new FrameworkPropertyMetadata(default(Boolean)));
        public Boolean IsCanEdit
        {
            get => (Boolean)GetValue(IsCanEditProperty);
            set => SetValue(IsCanEditProperty, value);
        }
        public ProductInfoLandscape()
        {
            this.IsCanEdit = true;
            InitializeComponent();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.GetBindingExpression(TextBox.TextProperty).ValidateWithoutUpdate();
        }
    }
}
