using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AddBrandDialogViewModel : BaseViewModel
    {
        private GenericDataRepository<Models.BrandRequest> brandRequestReposition = new GenericDataRepository<BrandRequest>();
        private string brandName = "";
        public string BrandName
        {
            get { return brandName; }
            set
            {
                brandName = value;
                OnPropertyChanged();
            }
        }
        private string reason = "";
        public string Reason
        {
            get { return reason; }
            set
            {
                reason = value;
                OnPropertyChanged();
            }
        }
        private string stringCloseDialog = "";
        public ICommand RequestBrandCommand { get; set; }
        public ICommand KeyDownEnterCommand { get; set; }
        private ICommand closeNotification;
        public ICommand CloseNotification 
        {
            get => closeNotification;
            set
            {
                closeNotification = value;
                OnPropertyChanged();
            }
        }
        public ICommand DoubleClickCommand { get; set; } = new RelayCommandWithNoParameter(() => { });
        private System.Windows.Controls.UserControl PreviousItem;
        public AddBrandDialogViewModel(System.Windows.Controls.UserControl item)
        {
            PreviousItem = item;
            CloseNotification = new RelayCommandWithNoParameter(() =>
            {
                DialogHost.CloseDialogCommand.Execute(null, null);
                if (PreviousItem != null)
                {
                    DialogHost.Show(PreviousItem, "Main");
                }
            });
            RequestBrandCommand = new RelayCommand<object>((p) =>
            {
                return !String.IsNullOrEmpty(Reason) &&
                        !String.IsNullOrEmpty(BrandName);
            }, (async (p) =>
            {
                DialogHost.CloseDialogCommand.Execute(true, null);
                MainViewModel.SetLoading(true);
                await AddBrandRequest();
                NotificationDialog notification = new NotificationDialog()
                {
                    Header = "Notification",
                    ContentDialog = stringCloseDialog,
                    CloseCommand = CloseNotification
                };
                MainViewModel.SetLoading(false);
                await DialogHost.Show(notification, "Main");
            }));
            KeyDownEnterCommand = new RelayCommand<object>((p) => p != null, (p) =>
            {
                System.Windows.Controls.Button button = p as System.Windows.Controls.Button;
                if (button.IsEnabled)
                {
                    button.Command.Execute(button);
                }
            });
        }

        private async Task AddBrandRequest()
        {
            try
            {
                await brandRequestReposition.Add(new BrandRequest()
                {
                    Id = await GenerateID.Gen(typeof(Brand)),
                    IdShop = AccountStore.instance.CurrentAccount.Id,
                    Name = BrandName.Trim(),
                    Reason = this.Reason.Trim()
                });
                stringCloseDialog = "Update sucessfully. Please wait for us to apply.";
            }
            catch
            {
                stringCloseDialog = "There is exist this brand in the brand request. Please wait for us to apply.";
            }
        }
    }
}