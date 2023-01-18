using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFEcommerceApp.Models;
namespace WPFEcommerceApp
{
    public class ShopContactViewModel:BaseViewModel
    {
        public Models.Address address;
        public Models.Address Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged();
            }
        }
        private Models.MUser shop;
        public Models.MUser Shop
        {
            get => shop;
            set
            {
                shop = value;
                OnPropertyChanged();
            }
        }

        public ShopContactViewModel(Models.MUser shop)
        {
            Shop = shop;
            IsLoadingCheck.IsLoading++;
            Task.Run(async () =>
            {
                await Load();
                IsLoadingCheck.IsLoading--;
            });
        }
        public async Task Load()
        {
            GenericDataRepository<Models.Address> addressRepository = new GenericDataRepository<Address>();
            Address = await addressRepository.GetSingleAsync(p => p.IdUser == shop.Id && p.Id == shop.DefaultAddress);
        }    
    }
}
