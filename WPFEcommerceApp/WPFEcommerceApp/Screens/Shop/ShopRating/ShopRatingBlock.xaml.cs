using System;
using System.Collections.Generic;
using System.IO.Packaging;
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
    /// Interaction logic for ShopRatingBlock.xaml
    /// </summary>
    public partial class ShopRatingBlock : UserControl
    {
        public static readonly DependencyProperty CustomerProperty = DependencyProperty.Register(
            "Customer", typeof(Models.MUser), typeof(ShopRatingBlock), new FrameworkPropertyMetadata(default(Models.MUser)));
        public Models.MUser Customer
        {
            get => (Models.MUser)GetValue(CustomerProperty);
            set 
            {
                SetValue(CustomerProperty, value);
            }
        }
        public static readonly DependencyProperty ProductProperty = DependencyProperty.Register(
        "Product", typeof(Models.Product), typeof(ShopRatingBlock), new FrameworkPropertyMetadata(default(Models.Product)));
        public Models.Product Product
        {
            get => (Models.Product)GetValue(ProductProperty);
            set
            {
                SetValue(ProductProperty, value);
            }
        }
        public static readonly DependencyProperty OrderProperty = DependencyProperty.Register(
        "Order", typeof(Models.MOrder), typeof(ShopRatingBlock), new FrameworkPropertyMetadata(default(Models.MOrder)));
        public Models.MOrder Order
        {
            get => (Models.MOrder)GetValue(OrderProperty);
            set => SetValue(OrderProperty, value);
        }
        public static readonly DependencyProperty RatingProperty = DependencyProperty.Register(
            "Rating", typeof(int), typeof(ShopRatingBlock), new FrameworkPropertyMetadata(default(int)));
        public int Rating
        {
            get => (int)GetValue(RatingProperty);
            set => SetValue(RatingProperty, value);
        }
        public static readonly DependencyProperty DateRatingProperty = DependencyProperty.Register(
            "DateRating", typeof(DateTime), typeof(ShopRatingBlock), new FrameworkPropertyMetadata(default(DateTime)));
        public DateTime DateRating
        {
            get => (DateTime)GetValue(DateRatingProperty);
            set => SetValue(DateRatingProperty, value);
        }
        public static readonly DependencyProperty CustomerImageAvaProperty = DependencyProperty.Register(
            "CustomerImageAva", typeof(string), typeof(ShopRatingBlock), new FrameworkPropertyMetadata(Properties.Resources.DefaultShopAvaImage));
        public string CustomerImageAva
        {
            get
            {
                return (string)GetValue(CustomerImageAvaProperty);
            }
            set => SetValue(CustomerImageAvaProperty, value);
        }
        public static readonly DependencyProperty ProductImageProperty = DependencyProperty.Register(
            "ProductImage", typeof(string), typeof(ShopRatingBlock), new FrameworkPropertyMetadata(Properties.Resources.DefaultProductImage));
        public string ProductImage
        {
            get => (string)GetValue(ProductImageProperty);
            set => SetValue(ProductImageProperty, value);
        }
        public ShopRatingBlock()
        {
            InitializeComponent();
            IsLoadingCheck.IsLoading--;
        }
    }
}
