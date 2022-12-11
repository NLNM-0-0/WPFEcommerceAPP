using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for RatingBar.xaml
    /// </summary>
    /// 
    public partial class RatingBar : UserControl {


        public int Value {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(RatingBar), new PropertyMetadata(0));

        public RatingBar() {
            InitializeComponent();
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Value = 1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            Value = 2;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            Value = 3;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) {
            Value = 4;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e) {
            Value = 5;
        }
    }
}
