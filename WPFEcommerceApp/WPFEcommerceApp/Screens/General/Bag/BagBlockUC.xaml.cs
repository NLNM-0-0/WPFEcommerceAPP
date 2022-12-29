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
    /// Interaction logic for BagBlockUC.xaml
    /// </summary>
    public partial class BagBlockUC : UserControl
    {
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
           "IsChecked", typeof(Boolean), typeof(BagBlockUC), new FrameworkPropertyMetadata(default(Boolean)));
        public Boolean IsChecked
        {
            get => (Boolean)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
        public static readonly DependencyProperty ProductImageProperty = DependencyProperty.Register(
           "ProductImage", typeof(ImageSource), typeof(BagBlockUC), new FrameworkPropertyMetadata(default(ImageSource)));
        public ImageSource ProductImage
        {
            get => (ImageSource)GetValue(ProductImageProperty);
            set => SetValue(ProductImageProperty, value);
        }
        public static readonly DependencyProperty ProductNameProperty = DependencyProperty.Register(
           "ProductName", typeof(string), typeof(BagBlockUC), new FrameworkPropertyMetadata(default(string)));
        public string ProductName
        {
            get => (string)GetValue(ProductNameProperty);
            set => SetValue(ProductNameProperty, value);
        }
        public static readonly DependencyProperty ShopNameProperty = DependencyProperty.Register(
           "ShopName", typeof(string), typeof(BagBlockUC), new FrameworkPropertyMetadata(default(string)));
        public string ShopName
        {
            get => (string)GetValue(ShopNameProperty);
            set => SetValue(ShopNameProperty, value);
        }
        public static readonly DependencyProperty UnitPriceProperty = DependencyProperty.Register(
           "UnitPrice", typeof(long), typeof(BagBlockUC), new FrameworkPropertyMetadata(default(long)));
        public long UnitPrice
        {
            get => (long)GetValue(UnitPriceProperty);
            set => SetValue(UnitPriceProperty, value);
        }
        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(
           "Amount", typeof(long), typeof(BagBlockUC), new FrameworkPropertyMetadata(default(long)));
        public long Amount
        {
            get => (long)GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }
        public static readonly DependencyProperty PriceProperty = DependencyProperty.Register(
           "Price", typeof(int), typeof(BagBlockUC), new FrameworkPropertyMetadata(default(int)));
        public int Price
        {
            get => (int)GetValue(PriceProperty);
            set => SetValue(PriceProperty, value);
        }
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
           "Command", typeof(ICommand), typeof(BagBlockUC), new FrameworkPropertyMetadata(default(ICommand)));
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
           "CommandParameter", typeof(object), typeof(BagBlockUC), new FrameworkPropertyMetadata(default(object)));
        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        public BagBlockUC()
        {
            InitializeComponent();
        }
    }
}
