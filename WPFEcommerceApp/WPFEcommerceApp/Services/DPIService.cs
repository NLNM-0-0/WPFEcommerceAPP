using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using Microsoft.Extensions.DependencyInjection;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class DPIService {
        public static IServiceProvider serviceProvider;
        static DPIService() {
            IServiceCollection services = new ServiceCollection();

            #region Set Store and some initial dependences
            //Set Store and some initial dependences
            var t = new GenericDataRepository<MUser>();
            var u = t.GetSingleAsync(d => d.Id.Equals("user01")).Result;

            AccountStore.instance = new AccountStore();
            AccountStore.instance.CurrentAccount = u;

            OrderStore.instance = new OrderStore();
            NavigationStore.instance = new NavigationStore();


            services.AddTransient<DrawerVM>();
            #endregion

            //Set service
            //setup Transient ViewModel

            //Normal
            #region Checkout and Order
            //Normal - Checkout and Order
            services.AddTransient<CheckoutScreenVM>();
            services.AddTransient<OrderScreenVM>();
            services.AddTransient<SuccessScreenVM>();
            #endregion

            #region Shop
            services.AddTransient<ShopRatingViewModel>();
            #endregion

            //Admin
            #region Admin
            services.AddTransient<ShopInformationPageViewModel>();
            services.AddTransient<AdminCategoryViewModel>();
            services.AddTransient<AdminBrandViewModel>();
            services.AddTransient<AdminProductManagerViewModel>();
            services.AddTransient<AdminUserManagerViewModel>();
            #endregion

            #region General
            services.AddTransient<MyProfileViewModel>(s => new MyProfileViewModel(AccountStore.instance));
            #endregion


            //Setup MainWindow
            //It need to be CreateHomeNavService
            //But I set initial screen is checkout here
            //just for example
            services.AddSingleton<INavigationService>(s => NavigateProvider.CheckoutScreen());

            services.AddSingleton<MainViewModel>();

            services.AddSingleton<MainWindow>(s => new MainWindow() {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            serviceProvider = services.BuildServiceProvider();

            NavigateProvider.serviceProvider = serviceProvider;
        }
    }
}
