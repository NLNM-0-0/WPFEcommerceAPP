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
    public class BannedShopViewModel:BaseViewModel
    {
        private ICommand unShopCommand;
        public ICommand UnShopCommand
        {
            get => unShopCommand;
            set
            {
                unShopCommand = value;
                OnPropertyChanged();
            }
        }
        private PackIconKind icon;
        public PackIconKind Icon
        {
            get => icon;
            set
            {
                icon = value;
                OnPropertyChanged();
            }
        }
        private string textContent;
        public string TextContent
        {
            get => textContent;
            set
            {
                textContent = value;
                OnPropertyChanged();
            }
        }
        private string labelExcuteContent;
        public string LabelExcuteContent
        {
            get => labelExcuteContent;
            set
            {
                labelExcuteContent = value;
                OnPropertyChanged();
            }
        }
        public BannedShopViewModel()
        {
            Task.Run(() =>
            {
                Icon = PackIconKind.CartOff;
                TextContent = "This shop have been banned by us.";
                LabelExcuteContent = "Contact us.";
                UnShopCommand = new RelayCommand<bool>(p => p, async p =>
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
                    IsLoadingCheck.IsLoading--;
                }));
            });
            
        }
    }
}
