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
        //static public DPIService instance;
        public IServiceProvider serviceProvider;
        public DPIService() {
            IServiceCollection services = new ServiceCollection();

            #region Set Store and some initial dependences

            AccountStore.instance = new AccountStore();
            OrderStore.instance = new OrderStore();
            NavigationStore.instance = new NavigationStore();
            FavoriteStore.instance = new FavoriteStore();

            services.AddTransient<DrawerVM>();
            #endregion

            //Set service
            //setup Transient ViewModel

            //Normal
            #region Checkout and Order
            //Normal - Checkout and Order
            services.AddTransient<OrderScreenVM>();
            services.AddTransient<SuccessScreenVM>();
            #endregion

            #region Shop
            services.AddTransient<ShopRatingViewModel>();
            services.AddTransient<ShopProductViewModel>();
            services.AddTransient<ShopOrderViewModel>();
            services.AddTransient<ShopStatisticsViewModel>();
            services.AddTransient<ShopPromoViewModel>();
            services.AddTransient<AddShopPromoViewModel>();
            #endregion

            //Admin
            #region Admin
            services.AddTransient<ShopInformationPageViewModel>();
            services.AddTransient<AdminCategoryViewModel>();
            services.AddTransient<AdminBrandViewModel>();
            services.AddTransient<AdminProductManagerViewModel>();
            services.AddTransient<AdminUserManagerViewModel>();
            services.AddTransient<AdsManagerViewModel>();
            services.AddTransient<AdminPromoViewModel>();
            #endregion

            #region General
            services.AddTransient<MyProfileViewModel>();
            //services.AddTransient<FilterViewModel>();
            services.AddTransient<MyHomeViewModel>();
            services.AddTransient<BagViewModel>();
            services.AddTransient<FavoriteViewModel>();
            services.AddTransient<NotificationViewModel>();
            services.AddSingleton<OfflineScreenVM>();

            #endregion


            //Setup MainWindow
            #region MainWindow Setup
            services.AddSingleton<INavigationService>(s => NavigateProvider.HomeScreen());

            services.AddSingleton<MainViewModel>();

            services.AddSingleton<MainWindow>(s => new MainWindow() {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            services.AddTransient<LoginViewModel>();
            services.AddSingleton<Login>(s => new Login() {
                DataContext = s.GetRequiredService<LoginViewModel>()
            });

            serviceProvider = services.BuildServiceProvider();

            NavigateProvider.serviceProvider = serviceProvider;
            App.serviceProvider = serviceProvider;
            #endregion
        }
    }
}
