using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for ReviewProductDialog.xaml
    /// </summary>
    public partial class ReviewProductDialog : UserControl, INotifyPropertyChanged {


        private int _value;

        public int Value {
            get { return _value; }
            set { _value = value; OnPropertyChanged(); }
        }

        private ICommand onOK;

        public ICommand OnOK {
            get { return onOK; }
            set { onOK = value; OnPropertyChanged(); }
        }



        public Product Param {
            get { return (Product)GetValue(ParamProperty); }
            set { SetValue(ParamProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Param.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParamProperty =
            DependencyProperty.Register("Param", typeof(Product), typeof(ReviewProductDialog), new PropertyMetadata(null));



        public ReviewProductDialog() {
            InitializeComponent();
            DataContext = this;
            OnOK = new RelayCommand<object>(p => true, p => {
                //do something with Product
            });
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
