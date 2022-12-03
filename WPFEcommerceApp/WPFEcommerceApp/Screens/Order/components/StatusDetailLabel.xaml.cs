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
    /// Interaction logic for StatusDetailLabel.xaml
    /// </summary>
    public partial class StatusDetailLabel : UserControl {


        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(StatusDetailLabel), new PropertyMetadata("Text here"));



        public string Icon {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(StatusDetailLabel), new PropertyMetadata("Home"));


        public bool Status {
            get { return (bool)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(bool), typeof(StatusDetailLabel), new PropertyMetadata(false));


        public bool CanProcess {
            get { return (bool)GetValue(CanProcessProperty); }
            set { SetValue(CanProcessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanProcess.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanProcessProperty =
            DependencyProperty.Register("CanProcess", typeof(bool), typeof(StatusDetailLabel), new PropertyMetadata(true));

        public StatusDetailLabel() {
            InitializeComponent();
            DataContext = this;
        }
    }
}
