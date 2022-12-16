using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for ProductBlock.xaml
    /// </summary>
    public partial class ProductBlock : UserControl
    {
        public static readonly DependencyProperty ChangeMainPictuceCommandProperty = DependencyProperty.Register(
           "ChangeMainPictuceCommand", typeof(ICommand), typeof(ProductBlock), new FrameworkPropertyMetadata(default(ICommand)));
        public ICommand ChangeMainPictuceCommand
        {
            get => (ICommand)GetValue(ChangeMainPictuceCommandProperty);
            set => SetValue(ChangeMainPictuceCommandProperty, value);
        }
        public static readonly DependencyProperty ShowMiniPictureCommandProperty = DependencyProperty.Register(
           "ShowMiniPictureCommand", typeof(ICommand), typeof(ProductBlock), new FrameworkPropertyMetadata(default(ICommand)));
        public ICommand ShowMiniPictureCommand
        {
            get => (ICommand)GetValue(ShowMiniPictureCommandProperty);
            set => SetValue(ShowMiniPictureCommandProperty, value);
        }
        public static readonly DependencyProperty HideMiniPictureCommandProperty = DependencyProperty.Register(
            "HideMiniPictureCommand", typeof(ICommand), typeof(ProductBlock), new FrameworkPropertyMetadata(default(ICommand)));
        public ICommand HideMiniPictureCommand
        {
            get => (ICommand)GetValue(HideMiniPictureCommandProperty);
            set => SetValue(HideMiniPictureCommandProperty, value);
        }
        public static readonly DependencyProperty ShowProductMainInfoCommandProperty = DependencyProperty.Register(
            "ShowProductMainInfoCommand", typeof(ICommand), typeof(ProductBlock), new FrameworkPropertyMetadata(default(ICommand)));
        public ICommand ShowProductMainInfoCommand
        {
            get => (ICommand)GetValue(ShowProductMainInfoCommandProperty);
            set => SetValue(ShowProductMainInfoCommandProperty, value);
        }
        public static readonly DependencyProperty HideProductMainInfoCommandProperty = DependencyProperty.Register(
            "HideProductMainInfoCommand", typeof(ICommand), typeof(ProductBlock), new FrameworkPropertyMetadata(default(ICommand)));
        public ICommand HideProductMainInfoCommand
        {
            get => (ICommand)GetValue(HideProductMainInfoCommandProperty);
            set => SetValue(HideProductMainInfoCommandProperty, value);
        }
        public static readonly DependencyProperty OpenProductCommandProperty = DependencyProperty.Register(
            "OpenProductCommand", typeof(ICommand), typeof(ProductBlock), new FrameworkPropertyMetadata(default(ICommand)));
        public ICommand OpenProductCommand
        {
            get => (ICommand)GetValue(OpenProductCommandProperty);
            set => SetValue(OpenProductCommandProperty, value);
        }
        public static readonly DependencyProperty ListProductImageProperty = DependencyProperty.Register(
           "ListProductImage", typeof(ObservableCollection<string>), typeof(ProductBlock), new FrameworkPropertyMetadata(default(ObservableCollection<string>)));
        public ObservableCollection<string> ListProductImage
        {
            get => (ObservableCollection<string>)GetValue(ListProductImageProperty);
            set => SetValue(ListProductImageProperty, value);
        }
        public static readonly DependencyProperty MainImageProperty = DependencyProperty.Register(
           "MainImage", typeof(string), typeof(ProductBlock), new FrameworkPropertyMetadata(default(string)));
        public string MainImage
        {
            get => (string)GetValue(MainImageProperty);
            set => SetValue(MainImageProperty, value);
        }
        public static readonly DependencyProperty ProductNameProperty = DependencyProperty.Register(
            "ProductName", typeof(String), typeof(ProductBlock), new FrameworkPropertyMetadata(default(String)));
        public String ProductName
        {
            get => (String)GetValue(ProductNameProperty);
            set => SetValue(ProductNameProperty, value);
        }
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register(
            "Category", typeof(Models.Category), typeof(ProductBlock), new FrameworkPropertyMetadata(default(Models.Category)));
        public Models.Category Category
        {
            get => (Models.Category)GetValue(CategoryProperty);
            set => SetValue(CategoryProperty, value);
        }
        public static readonly DependencyProperty BrandProperty = DependencyProperty.Register(
            "Brand", typeof(Models.Brand), typeof(ProductBlock), new FrameworkPropertyMetadata(default(Models.Brand)));
        public Models.Brand Brand
        {
            get => (Models.Brand)GetValue(BrandProperty);
            set => SetValue(BrandProperty, value);
        }
        public static readonly DependencyProperty SaleProperty = DependencyProperty.Register(
            "Sale", typeof(int), typeof(ProductBlock), new FrameworkPropertyMetadata(default(int)));
        public int Sale { 
            get => (int)GetValue(SaleProperty);
            set => SetValue(SaleProperty, value);
        }
        public static readonly DependencyProperty PriceProperty = DependencyProperty.Register(
           "Price", typeof(long), typeof(ProductBlock), new FrameworkPropertyMetadata(default(long)));
        public long Price
        {
            get => (long)GetValue(PriceProperty);
            set => SetValue(PriceProperty, value);
        }
        public static readonly DependencyProperty ProductProperty = DependencyProperty.Register(
           "Product", typeof(Models.Product), typeof(ProductBlock), new FrameworkPropertyMetadata(default(Models.Product)));
        public Models.Product Product
        {
            get => (Models.Product)GetValue(ProductProperty);
            set => SetValue(ProductProperty, value);
        }
        public ProductBlock()
        {
            MainImage = Properties.Resources.DefaultProductImage;
            InitializeComponent();
            ListProductImage = new ObservableCollection<string>();
            if (ListProductImage == null || ListProductImage.Count == 0)
            {
                MainImage = Properties.Resources.DefaultProductImage;
            }
            else
            {
                MainImage = ListProductImage[0];
            }
            ShowMiniPictureCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                if (ListProductImage == null || ListProductImage.Count == 0 || ListProductImage.Count == 1)
                {
                    return;
                }
                Grid grid = (p as Grid);
                grid.Visibility = Visibility.Visible;
            });

            ShowProductMainInfoCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                Grid grid = (p as Grid);
                grid.Visibility = Visibility.Visible;
            });

            HideMiniPictureCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                Grid grid = (p as Grid);
                grid.Visibility = Visibility.Collapsed;
            });

            HideProductMainInfoCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                Grid grid = (p as Grid);
                grid.Visibility = Visibility.Collapsed;
            });

            ChangeMainPictuceCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                MainImage = (p as Image).Source.ToString();
            });
            OpenProductCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                ProductDetail productDetail = new ProductDetail();
                productDetail.DataContext = new ProductDetailViewModel(this.Product);
                DialogHost.Show(productDetail, "App");
            });
        }
    }
}
