using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly AccountStore _accountStore;
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
        private string stringCloseDialog ="";
        public ICommand RequestBrandCommand { get; set; }
        public ICommand KeyDownEnterCommand { get; set; }
        public AddBrandDialogViewModel(AccountStore accountStore)
        {
            _accountStore = accountStore;
            RequestBrandCommand = new RelayCommandWithNoParameter(()=>
            { 
                Task task = Task.Run(async () => await AddBrandRequest());
                while (!task.IsCompleted) ;
                NotificationDialog notification = new NotificationDialog()
                {
                    Header = "Notification",
                    ContentDialog = stringCloseDialog
                };
                DialogHost.Show(notification, "Notification");
            });
            KeyDownEnterCommand = new RelayCommand<object>((p) => p != null, (p) =>
            {
                System.Windows.Controls.Button button = p as System.Windows.Controls.Button;
                if(button.IsEnabled)
                {
                    button.Command.Execute(null);    
                }
            });
        }
        private async Task AddBrandRequest()
        {
            try
            {
                await brandRequestReposition.Add(new BrandRequest()
                {
                    Id =  await GenerateID.Gen(typeof(Brand)),
                    IdShop = _accountStore.CurrentAccount.Id,
                    Name = BrandName,
                    Reason = this.Reason
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
