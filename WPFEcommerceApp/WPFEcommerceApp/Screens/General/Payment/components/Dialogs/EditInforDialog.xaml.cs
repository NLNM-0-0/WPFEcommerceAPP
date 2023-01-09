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
using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for EditInforDialog.xaml
    /// </summary>
    public partial class EditInforDialog : UserControl {
        public EditInforDialog() {
            InitializeComponent();
            DataContext = this;
        }


        public string Header {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(EditInforDialog), new PropertyMetadata("Add address"));



        public Address address {
            get { return (Address)GetValue(addressProperty); }
            set { SetValue(addressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for address.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty addressProperty =
            DependencyProperty.Register("address", typeof(Address), typeof(EditInforDialog), new PropertyMetadata(new Address()));




        public ICommand OnOK {
            get { return (ICommand)GetValue(OnAddAddressProperty); }
            set { SetValue(OnAddAddressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnAddAddressProperty =
            DependencyProperty.Register("OnAddAddress", typeof(ICommand), typeof(EditInforDialog), new PropertyMetadata(null));


        private void Button_Click(object sender, RoutedEventArgs e) {
            if(address.Id != null) {
                OnOK.Execute(address);
                return;
            }
            var id = GenerateID.DateTimeID();
            Address add = new Address() {
                Id = id,
                IdUser = AccountStore.instance.CurrentAccount.Id,
                Name = address.Name,
                PhoneNumber = address.PhoneNumber,
                Address1 = address.Address1,
                Status = true
            };
            OnOK.Execute(add);
        }
    }
}
