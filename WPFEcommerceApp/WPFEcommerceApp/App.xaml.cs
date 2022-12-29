﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public static IServiceProvider serviceProvider;

        public App() {
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            SplashScreen splashScreen = new SplashScreen();
            splashScreen.Show();

            Task.Run(async () => {
                DPIService dpi = new DPIService();
                await load();
            }).ContinueWith(_ => {
                MainWindow = null;
                INavigationService initial;
                if(AccountStore.instance.CurrentAccount == null ||
                AccountStore.instance.CurrentAccount.Role != "Admin") {
                    initial = serviceProvider.GetRequiredService<INavigationService>();
                }
                else {
                    initial = NavigateProvider.AdminUserScreen();
                }
                initial.Navigate();

                var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                //Subcribe access SplashScreen from MainWindow
                mainWindow.Loaded += (sender, args) => {
                    splashScreen.Close();
                    splashScreen = null;
                };

                MainWindow = mainWindow;
                MainWindow.Show();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        //All the shit need to be load in here
        async Task load() {
            var t = new GenericDataRepository<MUser>();
            var u = await t.GetSingleAsync(d => d.Id.Equals("user01"));
            AccountStore.instance.CurrentAccount = null;
        }
    }
}
