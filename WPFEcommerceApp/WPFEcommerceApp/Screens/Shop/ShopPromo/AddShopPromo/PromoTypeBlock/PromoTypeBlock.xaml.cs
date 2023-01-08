using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for PromoTypeBlock.xaml
    /// </summary>
    public partial class PromoTypeBlock : UserControl
    {
        public static readonly DependencyProperty OverlayBackgroundProperty = DependencyProperty.Register(
            "OverlayBackground", typeof(System.Windows.Media.Brush), typeof(PromoTypeBlock), new FrameworkPropertyMetadata(default(System.Windows.Media.Brush)));
        public System.Windows.Media.Brush OverlayBackground
        {
            get => (System.Windows.Media.Brush)GetValue(OverlayBackgroundProperty);
            set => SetValue(OverlayBackgroundProperty, value);
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(PackIconKind), typeof(PromoTypeBlock), new FrameworkPropertyMetadata(default(PackIconKind)));
        public PackIconKind Icon
        {
            get => (PackIconKind)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty ContentPromoBlockProperty = DependencyProperty.Register(
            "ContentPromoBlock", typeof(string), typeof(PromoTypeBlock), new FrameworkPropertyMetadata(default(string)));
        public string ContentPromoBlock
        {
            get => (string)GetValue(ContentPromoBlockProperty);
            set => SetValue(ContentPromoBlockProperty, value);
        }
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
            "IsChecked", typeof(Boolean), typeof(PromoTypeBlock), new FrameworkPropertyMetadata(default(Boolean)));
        public Boolean IsChecked
        {
            get => (Boolean)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(
            "GroupName", typeof(string), typeof(PromoTypeBlock), new FrameworkPropertyMetadata(default(string)));
        public string GroupName
        {
            get => (string)GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }
        public PromoTypeBlock()
        {
            InitializeComponent();
        }
    }
}
