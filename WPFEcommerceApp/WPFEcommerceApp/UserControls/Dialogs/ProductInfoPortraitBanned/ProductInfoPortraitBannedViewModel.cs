using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class ProductInfoPortraitBannedViewModel : BaseViewModel
    {
        private readonly AccountStore _accountStore;
        private GenericDataRepository<Models.Product> productRepository = new GenericDataRepository<Models.Product>();
        public ICommand ContactUsCommand { get; set; }
        public ICommand OpenProductInfoLandscapeCommand { get; set; }
        private Models.Product selectedProduct;
        public Models.Product SelectedProduct
        {
            get
            {
                if (selectedProduct == null)
                {
                    selectedProduct = new Models.Product();
                }
                return selectedProduct;
            }
            set
            {
                selectedProduct = value;
                OnPropertyChanged();
            }
        }
        public ProductInfoPortraitBannedViewModel(AccountStore accountStore, Models.Product product)
        {
            _accountStore = accountStore;
            SelectedProduct = product;

            OpenProductInfoLandscapeCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                ProductInfoLandscape productInfoLandscape = new ProductInfoLandscape();
                productInfoLandscape.DataContext = new ProductInfoLandscapeViewModel(_accountStore, SelectedProduct) { IsBanned = true }; ;
                DialogHost.Show(productInfoLandscape, "App");
            });
            ContactUsCommand = new RelayCommandWithNoParameter(() =>
            {
                NotificationDialog notificationDialog = new NotificationDialog();
                notificationDialog.Header = "Contact Info";
                notificationDialog.ContentDialog = "Please contact us with phone number 0585885214 or email 21520339@uit.gm.edu.vn.";
                DialogHost.Show(notificationDialog, "App");
            });
        }
    }
}
