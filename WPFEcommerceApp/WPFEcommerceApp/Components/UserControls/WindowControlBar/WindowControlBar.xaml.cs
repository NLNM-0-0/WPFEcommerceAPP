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
    /// Interaction logic for WindowControlBar.xaml
    /// </summary>
    public partial class WindowControlBar : UserControl {


        public bool Minimize {
            get { return (bool)GetValue(MinimizeProperty); }
            set { SetValue(MinimizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimizeProperty =
            DependencyProperty.Register("Minimize", typeof(bool), typeof(WindowControlBar), new PropertyMetadata(true));



        public bool Maximize {
            get { return (bool)GetValue(MaximizeProperty); }
            set { SetValue(MaximizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximizeProperty =
            DependencyProperty.Register("Maximize", typeof(bool), typeof(WindowControlBar), new PropertyMetadata(true));



        public bool Close {
            get { return (bool)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Close.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register("Close", typeof(bool), typeof(WindowControlBar), new PropertyMetadata(true));



        public ICommand CloseCM {
            get { return (ICommand)GetValue(CloseCMProperty); }
            set { SetValue(CloseCMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseCM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCMProperty =
            DependencyProperty.Register("CloseCM", typeof(ICommand), typeof(WindowControlBar), new PropertyMetadata(null));

        public WindowControlBar() {
            InitializeComponent();
            DataContext = new WindowControlBarVM();
        }
    }
}
