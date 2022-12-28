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
    /// Interaction logic for NotificationBlock.xaml
    /// </summary>
    public partial class NotificationBlockUC : UserControl
    {
        public static readonly DependencyProperty AvaImageProperty = DependencyProperty.Register(
            "AvaImage", typeof(ImageSource), typeof(NotificationBlockUC), new FrameworkPropertyMetadata(default(ImageSource)));
        public ImageSource AvaImage
        {
            get => (ImageSource)GetValue(AvaImageProperty);
            set => SetValue(AvaImageProperty, value);
        }
        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(
            "UserName", typeof(String), typeof(NotificationBlockUC), new FrameworkPropertyMetadata(default(String)));
        public String UserName
        {
            get => (String)GetValue(UserNameProperty);
            set => SetValue(NameProperty, value);
        }
        public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
           "Date", typeof(string), typeof(NotificationBlockUC), new FrameworkPropertyMetadata(default(string)));
        public string Date
        {
            get => (string)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }
        public static readonly DependencyProperty NotificationContentProperty = DependencyProperty.Register(
           "NotificationContent", typeof(string), typeof(NotificationBlockUC), new FrameworkPropertyMetadata(default(string)));
        public string NotificationContent
        {
            get => (string)GetValue(NotificationContentProperty);
            set => SetValue(NotificationContentProperty, value);
        }
        public NotificationBlockUC()
        {
            InitializeComponent();
        }
    }
}

