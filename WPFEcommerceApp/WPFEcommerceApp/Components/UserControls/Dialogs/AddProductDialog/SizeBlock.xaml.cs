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

namespace WPFEcommerceApp.UserControls.Dialogs.AddProductDialog
{
    /// <summary>
    /// Interaction logic for SizeBlock.xaml
    /// </summary>
    public partial class SizeBlock : UserControl
    {
        public SizeBlock()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            "Size", typeof(object), typeof(SizeBlock), new FrameworkPropertyMetadata(default(object)));
        public object Size
        {
            get => GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(SizeBlock), new FrameworkPropertyMetadata(default(object)));
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(SizeBlock), new FrameworkPropertyMetadata(default(object)));
        public object CommandParameter
        {
            get => (ICommand)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        public static readonly DependencyProperty IsCanDeleteProperty = DependencyProperty.Register(
            "IsCanDelete", typeof(Boolean), typeof(SizeBlock), new FrameworkPropertyMetadata(default(Boolean)));
        public Boolean IsCanDelete
        {
            get => (Boolean)GetValue(IsCanDeleteProperty);
            set => SetValue(CommandParameterProperty, value);
        }
    }
}
