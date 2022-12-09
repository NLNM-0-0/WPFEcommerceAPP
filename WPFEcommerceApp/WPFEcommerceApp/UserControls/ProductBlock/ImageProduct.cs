using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
namespace WPFEcommerceApp
{
    /// <summary>
    /// This class will be deleted after having database
    /// </summary>
    partial class ImageProduct:BaseViewModel
    {
        public string Path { get; set; }
        public Image Image { get; private set; }
        public ImageProduct(string path = null)
        {
            if (path != null)
            {
                Path = path;
            }
            else
            {
                Path = "http://www.sitech.co.id/assets/img/products/default.jpg";
            }
            Image = new Image();
            Image.Source = new BitmapImage(new Uri(Path));
        }
    }
}
