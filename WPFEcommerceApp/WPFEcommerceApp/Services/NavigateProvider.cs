using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WPFEcommerceApp {
    public class NavigateProvider {
        public static IServiceProvider serviceProvider;

        #region General
        static public INavigationService ProfileScreen() {
            return new NavigationService<MyProfileViewModel>(
                serviceProvider.GetRequiredService<MyProfileViewModel>);
        }
        #endregion

        #region Checkout and Payment
        //Checkout and Payment
        static public INavigationService CheckoutScreen() {
            return new NavigationService<CheckoutScreenVM>(
                serviceProvider.GetRequiredService<CheckoutScreenVM>);
        }
        static public INavigationService OrderScreen() {
            return new NavigationService<OrderScreenVM>(
                serviceProvider.GetRequiredService<OrderScreenVM>);
        }
        static public INavigationService SuccessScreen() {
            return new NavigationService<SuccessScreenVM>(
                serviceProvider.GetRequiredService<SuccessScreenVM>);
        }
        #endregion

        #region Admin
        //Admin

        static public INavigationService ShopInformationScreen() {
            return new NavigationService<ShopInformationPageViewModel>(
                serviceProvider.GetRequiredService<ShopInformationPageViewModel>);
        }
        static public INavigationService AdminCategoryScreen() {
            return new NavigationService<AdminCategoryViewModel>(
                serviceProvider.GetRequiredService<AdminCategoryViewModel>);
        }
        static public INavigationService AdminBrandScreen() {
            return new NavigationService<AdminBrandViewModel>(
                serviceProvider.GetRequiredService<AdminBrandViewModel>);
        }

        static public INavigationService AdminProductScreen() {
            return new NavigationService<AdminProductManagerViewModel>(
                serviceProvider.GetRequiredService<AdminProductManagerViewModel>);
        }

        static public INavigationService AdminUserScreen() {
            return new NavigationService<AdminUserManagerViewModel>(
                serviceProvider.GetRequiredService<AdminUserManagerViewModel>);
        }
        #endregion

        #region Shop
        //Shop
        static public INavigationService ShopMainScreen() {
            return new NavigationService<ShopInformationPageViewModel>(
                serviceProvider.GetRequiredService<ShopInformationPageViewModel>);
        }
        static public INavigationService ShopOrderScreen() {
            return new NavigationService<ShopInformationPageViewModel>(
                serviceProvider.GetRequiredService<ShopInformationPageViewModel>);
        }

        static public INavigationService ShopProductScreen() {
            return new NavigationService<ShopInformationPageViewModel>(
                serviceProvider.GetRequiredService<ShopInformationPageViewModel>);
        }

        static public INavigationService ShopRatingScreen() {
            return new NavigationService<ShopRatingViewModel>(
                serviceProvider.GetRequiredService<ShopRatingViewModel>);
        }
        #endregion
    }
}
