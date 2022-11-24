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

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for ShopMain.xaml
    /// </summary>
    public partial class ShopMain : UserControl
    {
        public ShopMain()
        {
            InitializeComponent();
            this.DataContext = new ShopMainViewModel(new Shop()
            {
                Name = "Shop Name",
                PhoneNumber = "12345678910",
                Email = "123@gmail.com",
                Address = "duong cay mit thanh pho cay xoai",
                Description = "dhajdhasjdahsjdhasjdhasjdhasjdhasjdhasjdhasjdhasjdhasjdhasjdhasjdhasjdhasjdhasjdhasjdhasjdhasjdhsajdhsajdhsajdhasjdhasjhạdhasjdhajdhsajdhasjdhsajdhasjdhasjdhj"
            });
        }
    }
}
