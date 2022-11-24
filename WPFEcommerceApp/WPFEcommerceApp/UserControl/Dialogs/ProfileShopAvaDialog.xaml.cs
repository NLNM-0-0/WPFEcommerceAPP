using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for ProfileShopAvaDialog.xaml
    /// </summary>
    public partial class ProfileShopAvaDialog : System.Windows.Controls.UserControl
    {
        /*public static readonly DependencyProperty SourceImageAvaProperty = DependencyProperty.Register(
        "SourceImageAva", typeof(ImageSource), typeof(ProfileShopAvaDialog));*/
        public ProfileShopAvaDialog()
        {
            InitializeComponent();
            //this.DataContext = new ProfileShopAvaDialogViewModel();
        }
        /*public ImageSource SourceImageAva
        {
            get=>(ImageSource)GetValue(SourceImageAvaProperty);
            set => SetValue(SourceImageAvaProperty, value);
        }*/
    }
}
