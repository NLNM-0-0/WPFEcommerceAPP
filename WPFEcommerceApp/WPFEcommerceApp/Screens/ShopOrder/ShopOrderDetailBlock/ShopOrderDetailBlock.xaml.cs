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
    /// Interaction logic for ShopOrderDetailBlock.xaml
    /// </summary>
    public partial class ShopOrderDetailBlock : UserControl
    {
        /*public static readonly DependencyProperty ProductImageProperty = DependencyProperty.Register(
            "ProductImage", typeof(string), typeof(ShopRatingBlock), new FrameworkPropertyMetadata(Properties.Resources.DefaultProductImage));
        public string ProductImage
        {
            get => (string)GetValue(ProductImageProperty);
            set => SetValue(ProductImageProperty, value);
        }
        public static readonly DependencyProperty OrderInfoProperty = DependencyProperty.Register(
           "OrderInfo", typeof(Models.OrderInfo), typeof(ShopRatingBlock), new FrameworkPropertyMetadata(default(Models.OrderInfo)));
        public Models.OrderInfo OrderInfo
        {
            get => (Models.OrderInfo)GetValue(OrderInfoProperty);
            set => SetValue(OrderInfoProperty, value);
        }*/
        public ShopOrderDetailBlock()
        {
            InitializeComponent();
        }
    }
}
