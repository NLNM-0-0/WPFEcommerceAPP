using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class NotRegisterShopViewModel : BaseViewModel
    {
        public GenericDataRepository<Models.ShopRequest> shopRequestRepository = new GenericDataRepository<Models.ShopRequest>();
        private bool isRequest;
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
        public NotRegisterShopViewModel()
        {
            Task.Run(async () =>
            {
                await Load();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    IsLoadingCheck.IsLoading--;
                }));
            }).ContinueWith((p) =>
            {
                if (!isRequest)
                {
                    LoadNotRegistered();
                }
                else
                {
                    LoadRegistered();
                }
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    IsLoadingCheck.IsLoading--;
                }));
            }, TaskContinuationOptions.RunContinuationsAsynchronously);
        }

        private void ClosedRegisterShopDialog(object sender, DialogClosedEventArgs eventArgs)
        {
            if (eventArgs.Parameter == null)
            {
                return;
            }
            else
            {
                isRequest = false;
                LoadRegistered();
            }
        }
        private void LoadRegistered()
        {
            Icon = PackIconKind.EmoticonExcitedOutline;
            LabelExcuteContent = null;
            TextContent = "You have registered. Please wait for us to apply.";
            UnShopCommand = new RelayCommand<bool>(p => p, p => { });
        }
        private void LoadNotRegistered()
        {
            Icon = PackIconKind.EmoticonSadOutline;
            TextContent = "You have not registed a shop yet.";
            LabelExcuteContent = "Register now.";
            UnShopCommand = new RelayCommand<bool>(p=>p,async p=>
            {
                MainViewModel.SetLoading(true);
                RegisterShopDialog registerShopDialog = new RegisterShopDialog();
                registerShopDialog.DataContext = new RegisterShopDialogViewModel();
                MainViewModel.SetLoading(false);
                await DialogHost.Show(registerShopDialog, "Main", null, null, ClosedRegisterShopDialog);
            });
        }
        private async Task Load()
        {
            MainViewModel.SetLoading(true);
            isRequest = (await shopRequestRepository.GetSingleAsync(sr => sr.IdUser == AccountStore.instance.CurrentAccount.Id) == null)?false:true;
            MainViewModel.SetLoading(false);
        }
    }
}
