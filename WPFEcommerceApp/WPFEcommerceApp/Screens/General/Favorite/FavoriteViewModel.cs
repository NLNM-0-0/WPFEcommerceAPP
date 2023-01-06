using DataAccessLayer;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class FavoriteViewModel:BaseViewModel
    {
        private GenericDataRepository<Models.Product> proRepo;
        private GenericDataRepository<MUser> userRepo;
        private ObservableCollection<ProductBlockViewModel> products;
        public ObservableCollection<ProductBlockViewModel> Products
        {
            get { return products; }
            set
            {
                products = value;
                OnPropertyChanged();
            }

        }


        public ICommand TestCommand { get; set; }
        public FavoriteViewModel()
        {
            MainViewModel.IsLoading = true;
            proRepo = new GenericDataRepository<Models.Product>();
            userRepo = new GenericDataRepository<MUser>();
            TestCommand = new RelayCommandWithNoParameter(Test);

            FavoriteStore.instance.FavoriteListChanged += OnFavoriteListChanged;

            Task.Run(async () =>
            {
                MainViewModel.IsLoading = true;
                await Load();
            }).ContinueWith((first) =>
            {
                MainViewModel.IsLoading = false;

            });

            MainViewModel.IsLoading = false;
        }

        private async void OnFavoriteListChanged()
        {
            await Load();
        }

        private void Test()
        {
            Products.Clear();
        }

        public async Task Load()
        {
            MainViewModel.IsLoading = true;
            var favProducts = await userRepo.GetSingleAsync(
                item=>item.Id==AccountStore.instance.CurrentAccount.Id, 
                item=>item.Products1);

            var pro = new ObservableCollection<ProductBlockViewModel>();

            foreach (var favProduct in favProducts.Products1)
            {
                var prod = await proRepo.GetSingleAsync(
                    pr => pr.Id == favProduct.Id, 
                    pr => pr.ImageProducts, 
                    pr => pr.Category, 
                    pr => pr.Brand);

                pro.Add(new ProductBlockViewModel(prod));
            }

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                Products = new ObservableCollection<ProductBlockViewModel>(pro);
            }));
            
            MainViewModel.IsLoading = false;
        }
    }
}
