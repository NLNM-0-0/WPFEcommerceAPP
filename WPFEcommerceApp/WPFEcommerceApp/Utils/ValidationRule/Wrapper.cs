using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFEcommerceApp {
    public class Wrapper : DependencyObject {
        public bool AdminAccess {
            get { return (bool)GetValue(AdminAccessProperty); }
            set { SetValue(AdminAccessProperty, value); }
        }
        public static readonly DependencyProperty AdminAccessProperty = DependencyProperty.Register(
            "AdminAccess",
            typeof(bool),
            typeof(Wrapper),
            new PropertyMetadata(false));



        public string PrevPassword {
            get { return (string)GetValue(PrevPasswordProperty); }
            set { SetValue(PrevPasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrevPassword.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrevPasswordProperty =
            DependencyProperty.Register("PrevPassword", typeof(string), typeof(Wrapper), new PropertyMetadata(null));


    }

    public class BindingProxy : System.Windows.Freezable {
        protected override Freezable CreateInstanceCore() {
            return new BindingProxy();
        }

        public object Data {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
    }
}
