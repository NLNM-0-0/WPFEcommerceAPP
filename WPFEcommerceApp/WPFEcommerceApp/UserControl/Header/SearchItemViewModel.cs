using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPFEcommerceApp
{
    public class SearchItemViewModel:BaseViewModel
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private ImageSource _sourceImage;
        public ImageSource SourceImage
        {
            get { return _sourceImage; }
            set { _sourceImage = value; OnPropertyChanged(); }
        }

        public SearchItemViewModel()
        {
            //SourceImage = Shop.DefaultSourceImageAva;
            Name = "A product or shop name";
        }
    }
}
