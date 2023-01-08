using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFEcommerceApp
{
    public class PromoProductBlockViewModel : BaseViewModel
    {
        private Models.Product selectedProduct;
        public Models.Product SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                selectedProduct = value;
                OnPropertyChanged();
            }    
        }
        private ICommand deleteCommand;
        public ICommand DeleteCommand
        {
            get { return deleteCommand; }
            set
            {
                deleteCommand = value;
                OnPropertyChanged();
            }
        }
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                OnPropertyChanged();
            }
        }
        private bool isAdmin;
        public bool IsAdmin
        {
            get { return isAdmin; }
            set
            {
                isAdmin = value;
                OnPropertyChanged();
            }
        }
        public ImageSource ProductImage
        {
            get
            {
                if(SelectedProduct == null || SelectedProduct.ImageProducts == null || SelectedProduct.ImageProducts.Count() == 0)
                {
                    return new BitmapImage(new Uri(Properties.Resources.DefaultProductImage));
                }   
                return new BitmapImage(new Uri(SelectedProduct.ImageProducts.ElementAt(0).Source));
            }
        }
        public PromoProductBlockViewModel(Models.Product product) 
        {
            SelectedProduct = product;
            DeleteCommand = new RelayCommand<object>((p)=> { return p != null; }, (p) =>
            {

            });
        }
    }
}
