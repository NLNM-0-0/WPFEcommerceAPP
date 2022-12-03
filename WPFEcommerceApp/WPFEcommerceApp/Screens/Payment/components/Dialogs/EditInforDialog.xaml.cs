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

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for EditInforDialog.xaml
    /// </summary>
    public partial class EditInforDialog : UserControl {
        public EditInforDialog() {
            InitializeComponent();
            DataContext = this;
        }


        public string Username {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Username.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(string), typeof(EditInforDialog), new PropertyMetadata("Name"));


        public string Phone {
            get { return (string)GetValue(PhoneProperty); }
            set { SetValue(PhoneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Phone.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PhoneProperty =
            DependencyProperty.Register("Phone", typeof(string), typeof(EditInforDialog), new PropertyMetadata("0123456789"));

        public string Address {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Address.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(string), typeof(EditInforDialog), new PropertyMetadata("Address"));



        public CheckoutScreenVM EditData {
            get { return (CheckoutScreenVM)GetValue(EditDataProperty); }
            set { SetValue(EditDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditDataProperty =
            DependencyProperty.Register("EditData", typeof(CheckoutScreenVM), typeof(EditInforDialog), new PropertyMetadata(null));

        private void Button_Click(object sender, RoutedEventArgs e) {
            EditData.Username = Username;
            EditData.UserPhone = Phone;
            EditData.UserAddress = Address;
        }
    }
}
