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
    public class RegisterShopDialogViewModel : BaseViewModel
    {
        public GenericDataRepository<Models.ShopRequest> shopRequestRepository = new GenericDataRepository<Models.ShopRequest>();
        public ICommand RegisterCommand { get; set; }
        private string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
            }
        }
        public RegisterShopDialogViewModel()
        {
            RegisterCommand = new RelayCommand<object>((p) =>
            {
                return !String.IsNullOrEmpty(Description);
            }, async (p) =>
            {
                DialogHost.CloseDialogCommand.Execute(true, null);
                MainViewModel.SetLoading(true);
                Models.ShopRequest shopRequest = new Models.ShopRequest();
                shopRequest.Id = await GenerateID.Gen(typeof(Models.ShopRequest));
                shopRequest.IdUser = AccountStore.instance.CurrentAccount.Id;
                shopRequest.Description = Description.Trim();
                await shopRequestRepository.Add(shopRequest);
                MainViewModel.SetLoading(false);
            });
        }
    }
}