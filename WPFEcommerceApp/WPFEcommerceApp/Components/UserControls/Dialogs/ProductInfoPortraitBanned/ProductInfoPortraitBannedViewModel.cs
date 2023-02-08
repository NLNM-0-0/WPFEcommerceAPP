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
        private GenericDataRepository<Models.Product> productRepository = new GenericDataRepository<Models.Product>();
        public ICommand ContactUsCommand { get; set; }
        public ICommand OpenProductInfoLandscapeCommand { get; set; }
        public ICommand DoubleClickCommand { get; set; } = new RelayCommandWithNoParameter(() => { });
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
        public ProductInfoPortraitBannedViewModel(Models.Product product)
        {
            Task.Run(() => { }).ContinueWith(second =>
            {
                SelectedProduct = product;

                OpenProductInfoLandscapeCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
                {
                    IsLoadingCheck.IsLoading = 3;
                    ProductInfoLandscape productInfoLandscape = new ProductInfoLandscape();
                    productInfoLandscape.DataContext = new ProductInfoLandscapeViewModel(SelectedProduct) { IsBanned = true };
                    IsLoadingCheck.IsLoading--;
                    await DialogHost.Show(productInfoLandscape, "Main");
                });
                ContactUsCommand = new RelayCommandWithNoParameter(async () =>
                {
                    MainViewModel.SetLoading(true);
                    NotificationDialog notificationDialog = new NotificationDialog();
                    notificationDialog.Header = "Contact Info";
                    notificationDialog.ContentDialog = $"Please contact us with phone number {Properties.Resources.PhoneNumber} or email {Properties.Resources.Email}.";
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(notificationDialog, "Main");
                });
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            });
        }
    }
}
