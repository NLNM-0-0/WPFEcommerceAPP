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
            Icon = PackIconKind.CartOff;
            TextContent = "This shop have been banned by us.";
            LabelExcuteContent = "Contact us.";
            UnShopCommand = new RelayCommand<bool>(p => p, p =>
            {
                NotificationDialog notificationDialog = new NotificationDialog();
                notificationDialog.Header = "Contact Info";
                notificationDialog.ContentDialog = $"Please contact us with phone number {Properties.Resources.PhoneNumber} or email {Properties.Resources.Email}.";
                DialogHost.Show(notificationDialog, "Main");
            });
        }
    }
}
