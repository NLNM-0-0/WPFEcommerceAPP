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
        public static FavoriteStore instance;
        private readonly AccountStore accountStore;
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
            accountStore = AccountStore.instance;
            accountStore.AccountChanged += OnAccountChange;
            Load();
        }

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
                MainViewModel.SetLoading(false);
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
            MainViewModel.SetLoading(false);
            await FavouriteApi.Add(AccountStore.instance.CurrentAccount.Id, p.Id);
            FavoriteProductList.Add(p);
            FavoriteListChanged?.Invoke();
            MainViewModel.SetLoading(false);
        }
        public async Task Delete(Models.Product p)
        {
            MainViewModel.SetLoading(true);
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
            MainViewModel.SetLoading(false);
        }
    }
}
