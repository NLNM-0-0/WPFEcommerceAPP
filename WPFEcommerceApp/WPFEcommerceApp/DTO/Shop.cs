using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFEcommerceApp
{
    public partial class Shop
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public ImageSource SourceImageAva { get; set; }
        public static ImageSource DefaultSourceImageAva = new BitmapImage(new Uri("file:///C:/Users/ASUS/Downloads/UserDefaultAvatar.png"));
        public ImageSource SourceImageBackground { get; set; }
        public static ImageSource DefaultSourceImageBackground = new BitmapImage(new Uri("https://htmlcolorcodes.com/assets/images/colors/light-gray-color-solid-background-1920x1080.png"));
        public Shop() {
            SourceImageAva = DefaultSourceImageAva;
            SourceImageBackground = DefaultSourceImageBackground;
        }
    }
}
