using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private readonly IServiceProvider serviceProvider;
        public App() {
            IServiceCollection services = new ServiceCollection();

            //Set Store and some initial dependences
            services.AddSingleton<NavigationStore>();

            services.AddSingleton<DrawerVM>(s => new DrawerVM(CreateCheckoutNavService(serviceProvider), CreateOrderNavService(serviceProvider)));

            //Set service

            //It need to be CreateHomeNavService
            //But I set initial screen is checkout here
            //just for example
            services.AddSingleton<INavigationService>(s => CreateCheckoutNavService(serviceProvider));

            //setup Transient ViewModel
            services.AddTransient<CheckoutScreenVM>(s => new CheckoutScreenVM(CreateSuccessNavService(s)));

            services.AddTransient<OrderScreenVM>(s => new OrderScreenVM(s.GetRequiredService<NavigationStore>()));

            services.AddTransient<SuccessScreenVM>(s => new SuccessScreenVM(CreateCheckoutNavService(serviceProvider)));

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
    }
}
