using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WPFEcommerceApp.Models;

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

        private string _sourceImage;
        public string SourceImage
        {
            get { return string.IsNullOrEmpty(_sourceImage)?Properties.Resources.DefaultShopAvaImage:_sourceImage; }
            set { _sourceImage = value; OnPropertyChanged(); }
        }
        public object Model { get; set; }
        public bool IsProduct { get; set; }

        public ICommand MouseDownCommand { get; set; }
        public SearchItemViewModel()
        {
            MouseDownCommand = new RelayCommand<object>(p=>p!=null,ToProduct);
        }

        private void ToProduct(object p) {
            var header = p as HeaderViewModel;
            header.IsSearchOpen = false;
            header.SearchText = "";
            if(IsProduct) {
                NavigateProvider.ProductDetailScreen().Navigate(Model);
            }
            else {
                if((Model as MUser).StatusShop == "NotBanned")
                    NavigateProvider.ShopViewScreen().Navigate(Model);
                else MessageBox.Show("lel");
            }
        }
    }
}
