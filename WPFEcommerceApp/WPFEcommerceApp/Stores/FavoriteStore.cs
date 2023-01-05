using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class FavoriteStore
    {
        public static FavoriteStore instance = new FavoriteStore();

        public event Action FavoriteListChanged;

        private List<Models.Product> favoriteProductList = new List<Models.Product>();
        public List<Models.Product> FavoriteProductList
        {
            get { return favoriteProductList; }
            set
            {
                favoriteProductList = value;
            }
        }
        public FavoriteStore()
        {
            AccountStore.instance.AccountChanged += OnAccountChange;
            Load();
        }

        private readonly GenericDataRepository<MOrder> orderRepo = new GenericDataRepository<MOrder>();
        private readonly GenericDataRepository<OrderInfo> orderInfoRepo = new GenericDataRepository<OrderInfo>();
        private readonly GenericDataRepository<MUser> userRepo = new GenericDataRepository<MUser>();

        private void OnAccountChange()
        {
            Load();
        }

        public void Load()
        {
            FavoriteProductList?.Clear();
            FavoriteProductList = new List<Models.Product>();
            if (AccountStore.instance.CurrentAccount == null)
            {
                MainViewModel.IsLoading = false;
                return;
            }
            else
            {
                FavoriteProductList = AccountStore.instance.CurrentAccount.Products1.ToList();
            }    
            FavoriteListChanged?.Invoke();
        }
        public async Task Add(Models.Product p)
        {
            MainViewModel.IsLoading = false;
            await FavouriteApi.Add(AccountStore.instance.CurrentAccount.Id, p.Id);
            FavoriteProductList.Add(p);
            FavoriteListChanged?.Invoke();
            MainViewModel.IsLoading = false;
        }
        public async Task Delete(Models.Product p)
        {
            MainViewModel.IsLoading = true;
            await FavouriteApi.Delete(AccountStore.instance.CurrentAccount.Id, p.Id);
            for(int i = 0; i < FavoriteProductList.Count; i++)
            {
                if (FavoriteProductList[i].Id == p.Id) 
                {
                    FavoriteProductList.RemoveAt(i);
                    break;
                }
            }    
            FavoriteListChanged?.Invoke();
            MainViewModel.IsLoading = false;
        }
    }
}
