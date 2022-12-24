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
            Task t= Task.Run(async () => await Load());
            while (!t.IsCompleted) ;
            MainViewModel.IsLoading = false;
        }

        private void Test()
        {
            Products.Clear();
        }

        public async Task Load()
        {
            
            var favProducts = await userRepo.GetSingleAsync(
                item=>item.Id==AccountStore.instance.CurrentAccount.Id, 
                item=>item.Products1);

            Products = new ObservableCollection<ProductBlockViewModel>();

            foreach (var favProduct in favProducts.Products1)
            {
                var prod = await proRepo.GetSingleAsync(
                    pr => pr.Id == favProduct.Id, 
                    pr => pr.ImageProducts, 
                    pr => pr.Category, 
                    pr => pr.Brand);

                Products.Add(new ProductBlockViewModel(prod));
            }
        }
    }
}
