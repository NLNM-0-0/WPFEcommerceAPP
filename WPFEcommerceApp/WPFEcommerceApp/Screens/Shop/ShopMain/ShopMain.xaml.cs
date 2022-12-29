using WPFEcommerceApp.Models;
using System.Windows.Controls;
using System.Threading.Tasks;
using DataAccessLayer;
using System.Linq;

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
        }

        private void scroll_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scv = sender as ScrollViewer;
            if (scv == null) return;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
