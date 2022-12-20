﻿using DataAccessLayer;
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
        private readonly AccountStore _accountStore;
        private GenericDataRepository<Models.CategoryRequest> categoryRequestReposition =new GenericDataRepository<Models.CategoryRequest>();
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
        public AddCategoryDialogViewModel(AccountStore accountStore)
        {
            _accountStore = accountStore;
            RequestCategoryCommand = new RelayCommandWithNoParameter(()=>
            {
                Task task = Task.Run(async () => await AddCategoryRequest());
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
                if (button.IsEnabled)
                {
                    button.Command.Execute(null);
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
                    IdShop = _accountStore.CurrentAccount.Id,
                    Name = CategoryName,
                    Reason = this.Reason
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
