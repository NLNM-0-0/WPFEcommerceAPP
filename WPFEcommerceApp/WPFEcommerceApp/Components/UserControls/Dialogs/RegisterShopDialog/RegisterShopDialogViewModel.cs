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
    public class RegisterShopDialogViewModel: BaseViewModel
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
            RegisterCommand = new RelayCommand<bool>((p)=>!p, async (p) =>
            {
                MainViewModel.IsLoading = true;
                await RegisterShop();
                DialogHost.CloseDialogCommand.Execute(true, null);
                MainViewModel.IsLoading = false;
            });
        }
        private async Task RegisterShop()
        {
            Models.ShopRequest shopRequest = new Models.ShopRequest();
            shopRequest.Id = await GenerateID.Gen(typeof(Models.ShopRequest));
            shopRequest.IdUser = AccountStore.instance.CurrentAccount.Id;
            shopRequest.Description = Description;
            await shopRequestRepository.Add(shopRequest);
        }
    }
}
