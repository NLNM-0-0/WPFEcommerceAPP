using WPFEcommerceApp.Models;
using System.Windows.Controls;


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
