using System;
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
        //private readonly IServiceProvider serviceProvider;
        public App() {
            //serviceProvider = DPIService.serviceProvider;
        }

        protected override void OnStartup(StartupEventArgs e) {
            //INavigationService initial = serviceProvider.GetRequiredService<INavigationService>();
            //initial.Navigate();

            //MainWindow = serviceProvider.GetRequiredService<MainWindow>();
            //MainWindow.Show();

            SplashScreen splashScreen = new SplashScreen();
            splashScreen.Show();

            base.OnStartup(e);
        }
    }
}
