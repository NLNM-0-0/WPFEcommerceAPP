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

namespace WPFEcommerceApp{
    /// <summary>
    /// Interaction logic for ConfirmDialog.xaml
    /// </summary>
    public partial class ConfirmDialogCustom : UserControl {



        public string Header {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("HeaderCustom", typeof(string), typeof(ConfirmDialog), new PropertyMetadata("Confirm"));

        public string Content {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("ContentCustom", typeof(string), typeof(ConfirmDialog), new PropertyMetadata("Are you sure?"));

        public ICommand CM {
            get { return (ICommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty commandProperty =
            DependencyProperty.Register("commandCustom", typeof(ICommand), typeof(ConfirmDialog), new PropertyMetadata(null));



        public object Param {
            get { return (object)GetValue(ParamProperty); }
            set { SetValue(ParamProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Param.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParamProperty =
            DependencyProperty.Register("ParamCustom", typeof(object), typeof(ConfirmDialog), new PropertyMetadata(null));

        public ConfirmDialogCustom() {
            InitializeComponent();
            DataContext = this;
        }
    }
}
