﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DataAccessLayer;
using Microsoft.Extensions.DependencyInjection;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private readonly IServiceProvider serviceProvider;
        public App() {
            IServiceCollection services = new ServiceCollection();

            #region Set Store and some initial dependences
            //Set Store and some initial dependences
            var t = new GenericDataRepository<MUser>();
            var u = t.GetSingleAsync(d => d.Id.Equals("user01")).Result;
            AccountStore ast = new AccountStore();
            ast.CurrentAccount = u;

            AccountStore.instance = ast;
            OrderStore.instance = new OrderStore();
            NavigationStore.instance = new NavigationStore();

            //Don't need anymore
            services.AddSingleton<AccountStore>(s => ast);

            services.AddTransient<DrawerVM>(s => new DrawerVM(
                    serviceProvider.GetRequiredService<AccountStore>(),
                    CreateCheckoutNavService(serviceProvider), 
                    CreateOrderNavService(serviceProvider),
                    CreateShopInformationPageNavService(serviceProvider),
                    CreateShopMainNavService(serviceProvider),
                    CreateShopOrderNavService(serviceProvider),
                    CreateShopProductNavService(serviceProvider),
                    CreateShopRatingNavService(serviceProvider),
                    CreateAdminCategoryMngNavService(serviceProvider),
                    CreateAdminBrandMngNavService(serviceProvider),
                    CreateAdminUserMngNavService(serviceProvider),
                    CreateAdminProductMngNavService(serviceProvider),
                    CreateProfileNavService(serviceProvider)
                )
            );
            #endregion

            //Set service
            //setup Transient ViewModel

            //Normal
            #region Checkout and Order
            //Normal - Checkout and Order
            services.AddTransient<CheckoutScreenVM>(s => new CheckoutScreenVM(
                    CreateSuccessNavService(s)
                )
            );

            services.AddTransient<OrderScreenVM>(s => new OrderScreenVM(
                CreateSuccessNavService(serviceProvider),
                CreateOrderNavService(serviceProvider)
                )
            );

            services.AddTransient<SuccessScreenVM>(s => new SuccessScreenVM(CreateCheckoutNavService(serviceProvider))); //Need to be HomeView here

            #endregion

            #region Shop
            services.AddTransient<ShopRatingViewModel>(s => new ShopRatingViewModel());
            #endregion

            //Admin
            #region Admin
            services.AddTransient<ShopInformationPageViewModel>(s => new ShopInformationPageViewModel());
            services.AddTransient<AdminCategoryViewModel>(s => new AdminCategoryViewModel());
            services.AddTransient<AdminBrandViewModel>(s => new AdminBrandViewModel());
            services.AddTransient<AdminProductManagerViewModel>(s => new AdminProductManagerViewModel());
            services.AddTransient<AdminUserManagerViewModel>(s => new AdminUserManagerViewModel());
            #endregion

            #region Genaral
            services.AddTransient<MyProfileViewModel>(s => new MyProfileViewModel(
                serviceProvider.GetRequiredService<AccountStore>())
            );
            #endregion


            //Setup MainWindow
            //It need to be CreateHomeNavService
            //But I set initial screen is checkout here
            //just for example
            services.AddSingleton<INavigationService>(s => CreateCheckoutNavService(serviceProvider));

            services.AddSingleton<MainViewModel>();

            services.AddSingleton<MainWindow>(s => new MainWindow() {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            serviceProvider = services.BuildServiceProvider();

        }

        protected override void OnStartup(StartupEventArgs e) {
            INavigationService initial = serviceProvider.GetRequiredService<INavigationService>();
            initial.Navigate();

            MainWindow = serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        #region General
        private INavigationService CreateProfileNavService(IServiceProvider serviceProvider) {
            return new NavigationService<MyProfileViewModel>(
                serviceProvider.GetRequiredService<MyProfileViewModel>);
        }
        #endregion

        #region Checkout and Payment
        //Checkout and Payment
        private INavigationService CreateCheckoutNavService(IServiceProvider serviceProvider) {
            return new NavigationService<CheckoutScreenVM>(
                serviceProvider.GetRequiredService<CheckoutScreenVM>);
        }
        private INavigationService CreateOrderNavService(IServiceProvider serviceProvider) {
            return new NavigationService<OrderScreenVM>(
                serviceProvider.GetRequiredService<OrderScreenVM>);
        }
        private INavigationService CreateSuccessNavService(IServiceProvider serviceProvider) {
            return new NavigationService<SuccessScreenVM>(
                serviceProvider.GetRequiredService<SuccessScreenVM>);
        }
        #endregion

        #region Admin
        //Admin

        private INavigationService CreateShopInformationPageNavService(IServiceProvider serviceProvider) {
            return new NavigationService<ShopInformationPageViewModel>(
                serviceProvider.GetRequiredService<ShopInformationPageViewModel>);
        }
        private INavigationService CreateAdminCategoryMngNavService(IServiceProvider serviceProvider) {
            return new NavigationService<AdminCategoryViewModel>(
                serviceProvider.GetRequiredService<AdminCategoryViewModel>);
        }
        private INavigationService CreateAdminBrandMngNavService(IServiceProvider serviceProvider) {
            return new NavigationService<AdminBrandViewModel>(
                serviceProvider.GetRequiredService<AdminBrandViewModel>);
        }

        private INavigationService CreateAdminProductMngNavService(IServiceProvider serviceProvider) {
            return new NavigationService<AdminProductManagerViewModel>(
                serviceProvider.GetRequiredService<AdminProductManagerViewModel>);
        }

        private INavigationService CreateAdminUserMngNavService(IServiceProvider serviceProvider) {
            return new NavigationService<AdminUserManagerViewModel>(
                serviceProvider.GetRequiredService<AdminUserManagerViewModel>);
        }
        #endregion

        #region Shop
        //Shop
        private INavigationService CreateShopMainNavService(IServiceProvider serviceProvider) {
            return new NavigationService<ShopInformationPageViewModel>(
                serviceProvider.GetRequiredService<ShopInformationPageViewModel>);
        }
        private INavigationService CreateShopOrderNavService(IServiceProvider serviceProvider) {
            return new NavigationService<ShopInformationPageViewModel>(
                serviceProvider.GetRequiredService<ShopInformationPageViewModel>);
        }

        private INavigationService CreateShopProductNavService(IServiceProvider serviceProvider) {
            return new NavigationService<ShopInformationPageViewModel>(
                serviceProvider.GetRequiredService<ShopInformationPageViewModel>);
        }

        private INavigationService CreateShopRatingNavService(IServiceProvider serviceProvider) {
            return new NavigationService<ShopRatingViewModel>(
                serviceProvider.GetRequiredService<ShopRatingViewModel>);
        }
        #endregion
    }
}
