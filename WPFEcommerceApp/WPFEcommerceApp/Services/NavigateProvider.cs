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
        static public INavigationService BagScreen() {
            return new NavigationService<BagViewModel>(
                serviceProvider.GetRequiredService<BagViewModel>);
        }
        static public INavigationService FavouriteScreen() {
            return new NavigationService<FavoriteViewModel>(
                serviceProvider.GetRequiredService<FavoriteViewModel>);
        }
        static public INavigationService HomeScreen() {
            return new NavigationService<HomeViewModel>(
                serviceProvider.GetRequiredService<HomeViewModel>);
        }
        static public INavigationService NotificationScreen() {
            return new NavigationService<NotificationViewModel>(
                serviceProvider.GetRequiredService<NotificationViewModel>);
        }
        #endregion

        #region Checkout and Payment
        //Checkout and Payment
        static public INavigationService CheckoutScreen() {
            return new ParamNavigationService<CheckoutScreenVM>(
                (p) => new CheckoutScreenVM(p as Order));
        }
        static public INavigationService OrderParamScreen() {
            return new ParamNavigationService<OrderScreenVM>(
                (p) => new OrderScreenVM((int)p));
        }
        static public INavigationService OrderScreen() {
            return new NavigationService<OrderScreenVM>(
                serviceProvider.GetRequiredService<OrderScreenVM>);
        }
        static public INavigationService SuccessScreen() {
            return new NavigationService<SuccessScreenVM>(
                serviceProvider.GetRequiredService<SuccessScreenVM>);
        }
        static public INavigationService OrderDetailScreen() {
            return new ParamNavigationService<OrderDetailsVM>(
                    (parameter) => new OrderDetailsVM(parameter as Order));
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
        static public INavigationService AdminAdsScreen() {
            return new NavigationService<AdsManagerViewModel>(
                serviceProvider.GetRequiredService<AdsManagerViewModel>);
        }
        #endregion

        #region Shop
        //Shop
        static public INavigationService ShopOrderScreen() {
            return new NavigationService<ShopOrderViewModel>(
                serviceProvider.GetRequiredService<ShopOrderViewModel>);
        }

        static public INavigationService ShopProductScreen() {
            return new NavigationService<ShopProductViewModel>(
                serviceProvider.GetRequiredService<ShopProductViewModel>);
        }

        static public INavigationService ShopRatingScreen() {
            return new NavigationService<ShopRatingViewModel>(
                serviceProvider.GetRequiredService<ShopRatingViewModel>);
        }
        static public INavigationService ShopStatisticScreen() {
            return new NavigationService<ShopStatisticsViewModel>(
                serviceProvider.GetRequiredService<ShopStatisticsViewModel>);
        }
        #endregion
    }
}
