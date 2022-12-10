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

            //Set Store and some initial dependences
            var t = new GenericDataRepository<MUser>();
            var u = t.GetSingleAsync(d => d.Id.Equals(1)).Result;
            AccountStore ast = new AccountStore();
            ast.CurrentAccount = u;

            services.AddSingleton<NavigationStore>();
            services.AddSingleton<AccountStore>(s => ast);
            services.AddSingleton<OrderStore>();

            services.AddTransient<DrawerVM>(s => new DrawerVM(
                    serviceProvider.GetRequiredService<AccountStore>(),
                    CreateCheckoutNavService(serviceProvider), 
                    CreateOrderNavService(serviceProvider),
                    CreateShopInformationPageNavService(serviceProvider),
                    CreateShopMainNavService(serviceProvider),
                    CreateShopOrderNavService(serviceProvider),
                    CreateShopProductNavService(serviceProvider),
                    CreateShopRatingNavService(serviceProvider)
                )
            );

            //Set service

            //It need to be CreateHomeNavService
            //But I set initial screen is checkout here
            //just for example
            services.AddSingleton<INavigationService>(s => CreateCheckoutNavService(serviceProvider));

            //setup Transient ViewModel
            //Normal
            //Normal - Checkout and Order
            services.AddTransient<CheckoutScreenVM>(s => new CheckoutScreenVM(
                    CreateSuccessNavService(s),
                    s.GetRequiredService<AccountStore>(),
                    s.GetRequiredService<OrderStore>()
                )
            );

            services.AddTransient<OrderScreenVM>(s => new OrderScreenVM(
                s.GetRequiredService<NavigationStore>(),
                s.GetRequiredService<AccountStore>(),
                s.GetRequiredService<OrderStore>(),
                CreateSuccessNavService(serviceProvider),
                CreateOrderNavService(serviceProvider)
                )
            );

            services.AddTransient<SuccessScreenVM>(s => new SuccessScreenVM(CreateCheckoutNavService(serviceProvider))); //Need to be HomeView here
            //Normal - Shop
            services.AddTransient<ShopRatingViewModel>(s => new ShopRatingViewModel());

            //Admin
            services.AddTransient<ShopInformationPageViewModel>(s => new ShopInformationPageViewModel());

            //Setup MainWindow
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

        //Checkout and Payment
        private INavigationService CreateCheckoutNavService(IServiceProvider serviceProvider) {
            return new NavigationService<CheckoutScreenVM>(
                serviceProvider.GetRequiredService<NavigationStore>(), 
                serviceProvider.GetRequiredService<CheckoutScreenVM>);
        }
        private INavigationService CreateOrderNavService(IServiceProvider serviceProvider) {
            return new NavigationService<OrderScreenVM>(
                serviceProvider.GetRequiredService<NavigationStore>(), 
                serviceProvider.GetRequiredService<OrderScreenVM>);
        }
        private INavigationService CreateSuccessNavService(IServiceProvider serviceProvider) {
            return new NavigationService<SuccessScreenVM>(
                serviceProvider.GetRequiredService<NavigationStore>(), 
                serviceProvider.GetRequiredService<SuccessScreenVM>);
        }

        //Admin

        private INavigationService CreateShopInformationPageNavService(IServiceProvider serviceProvider) {
            return new NavigationService<ShopInformationPageViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                serviceProvider.GetRequiredService<ShopInformationPageViewModel>);
        }

        //Shop
        private INavigationService CreateShopMainNavService(IServiceProvider serviceProvider) {
            return new NavigationService<ShopInformationPageViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                serviceProvider.GetRequiredService<ShopInformationPageViewModel>);
        }
        private INavigationService CreateShopOrderNavService(IServiceProvider serviceProvider) {
            return new NavigationService<ShopInformationPageViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                serviceProvider.GetRequiredService<ShopInformationPageViewModel>);
        }

        private INavigationService CreateShopProductNavService(IServiceProvider serviceProvider) {
            return new NavigationService<ShopInformationPageViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                serviceProvider.GetRequiredService<ShopInformationPageViewModel>);
        }

        private INavigationService CreateShopRatingNavService(IServiceProvider serviceProvider) {
            return new NavigationService<ShopRatingViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                serviceProvider.GetRequiredService<ShopRatingViewModel>);
        }
    }
}
