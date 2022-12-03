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
        private readonly NavigationStore navigationStore;
        private readonly DrawerVM drawerVM;
        public App() {
            navigationStore = new NavigationStore();
            drawerVM = new DrawerVM(CreateCheckoutNavService(), CreateOrderNavService());
        }

        protected override void OnStartup(StartupEventArgs e) {
            INavigationService navigationService = CreateCheckoutNavService();
            navigationService.Navigate();
            MainWindow = new MainWindow() {
                DataContext = new MainViewModel(navigationStore,
                drawerVM)
            };
            MainWindow.Show();
            base.OnStartup(e);
        }

        private INavigationService CreateCheckoutNavService() {
            return new NavigationService<CheckoutScreenVM>(navigationStore, () => new CheckoutScreenVM(CreateSuccessNavService()));
        }
        private INavigationService CreateOrderNavService() {
            return new NavigationService<OrderScreenVM>(navigationStore, () => new OrderScreenVM());
        }
        private INavigationService CreateSuccessNavService() {
            return new NavigationService<SuccessScreenVM>(navigationStore, () => new SuccessScreenVM(CreateOrderNavService()));
        }
    }
}
