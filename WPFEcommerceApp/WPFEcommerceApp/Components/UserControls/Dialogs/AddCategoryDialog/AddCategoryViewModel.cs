using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AddCategoryDialogViewModel : BaseViewModel
    {
        private GenericDataRepository<Models.CategoryRequest> categoryRequestReposition = new GenericDataRepository<Models.CategoryRequest>();
        private string categoryName = "";
        public string CategoryName
        {
            get { return categoryName; }
            set
            {
                categoryName = value;
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
        public ICommand RequestCategoryCommand { get; set; }
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
        public AddCategoryDialogViewModel(System.Windows.Controls.UserControl item)
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
            RequestCategoryCommand = new RelayCommand<object>((p) =>
            {
                return !String.IsNullOrEmpty(CategoryName) && !String.IsNullOrEmpty(Reason);
            }, (async (p) =>
            {
                DialogHost.CloseDialogCommand.Execute(true, null);
                MainViewModel.SetLoading(true);
                await AddCategoryRequest();
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
        public async Task AddCategoryRequest()
        {
            try
            {
                await categoryRequestReposition.Add(new CategoryRequest()
                {
                    Id = await GenerateID.Gen(typeof(Category)),
                    IdShop = AccountStore.instance.CurrentAccount.Id,
                    Name = CategoryName.Trim(),
                    Reason = this.Reason.Trim()
                }) ;
                stringCloseDialog = "Update sucessfully. Please wait for us to apply.";
            }
            catch
            {
                stringCloseDialog = "This category name already exists in Category Request. Please wait for us to accept";
            }
        }
    }
}