using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class ProductDetailBannedViewModel : BaseViewModel
    {
        private ICommand contactCommand;
        public ICommand ContactCommand
        {
            get => contactCommand;
            set
            {
                contactCommand = value;
                OnPropertyChanged();
            }
        }
        public ProductDetailBannedViewModel()
        {
            Task.Run(() => {  }).ContinueWith((first)=>
            {
                ContactCommand = new RelayCommand<bool>(p => p, async p =>
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
