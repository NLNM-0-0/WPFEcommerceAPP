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
    /// Interaction logic for ProductInfoPortrait.xaml
    /// </summary>
    public partial class ProductInfoPortrait : UserControl
    {
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(string), typeof(ProductInfoPortrait), new FrameworkPropertyMetadata(default(string)));
        public string Status
        {
            get => (string)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }
        public ProductInfoPortrait()
        {
            InitializeComponent();
            this.Status = "NotBanned";
        }
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.GetBindingExpression(TextBox.TextProperty).ValidateWithoutUpdate();
        }
    }
}
