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
    /// Interaction logic for NotificationDialog.xaml
    /// </summary>
    public partial class NotificationDialog : UserControl
    {
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(NotificationDialog), new FrameworkPropertyMetadata(default(string)));

        public string ContentDialog
        {
            get { return (string)GetValue(ContentDialogProperty); }
            set { SetValue(ContentDialogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentDialogProperty =
            DependencyProperty.Register("ContentDialog", typeof(string), typeof(NotificationDialog), new PropertyMetadata(default(string)));
        public NotificationDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
