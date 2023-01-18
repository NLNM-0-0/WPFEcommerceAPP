using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WPFEcommerceApp {
    public class NavigateProvider {
        public static IServiceProvider serviceProvider;
        public static bool Back() {
            var nav = NavigationStore.instance.stackScreen;
            if(nav.Count < 2) {
                return false;
            }
            var t = nav[nav.Count - 2];
            if(t.Item2 == null) t.Item1.Navigate();
            else t.Item1.Navigate(t.Item2);
            nav.RemoveAt(nav.Count - 1);
            nav.RemoveAt(nav.Count - 1);
            return true;
        }

        public static bool LoginScreenHandle(bool type = false) {
            try {
                App.Current.Dispatcher.Invoke(() => {
                    Login p = App.serviceProvider.GetRequiredService<Login>();
                    bool IsMainVisible = App.Current.MainWindow.Visibility == System.Windows.Visibility.Visible;
                    if(!IsMainVisible && !type) {
                        App.Current.MainWindow.Show();
                        p.Hide();
                    }
                    else if(IsMainVisible && type) {
                        p.Show();
                        App.Current.MainWindow.Hide();
                    }
                });
                return true;
            } catch { return false; }
        }

        #region General
        static public INavigationService OfflineScreen() {
            return new NavigationService<OfflineScreenVM>(
                serviceProvider.GetRequiredService<OfflineScreenVM>);
        }
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
            return new NavigationService<MyHomeViewModel>(
                serviceProvider.GetRequiredService<MyHomeViewModel>);
        }
        static public INavigationService NotificationScreen() {
            return new NavigationService<NotificationViewModel>(
                serviceProvider.GetRequiredService<NotificationViewModel>);
        }

        static public INavigationService FilterScreen() {
            return new ParamNavigationService<FilterViewModel>(
                p => new FilterViewModel(p as FilterObject));
        }
        static public INavigationService ProductDetailScreen() {
            return new ParamNavigationService<ProductDetailViewModel>(
                (p) => new ProductDetailViewModel(p as Models.Product));
        }
        #endregion

        #region Checkout and Payment
        //Checkout and Payment
        static public INavigationService CheckoutScreen() {
            return new ParamNavigationService<CheckoutScreenVM>(
                (p) => new CheckoutScreenVM(p as List<Order>));
        }
        static public INavigationService OrderParamScreen() {
            return new ParamNavigationService<OrderScreenVM>(
                (p) => new OrderScreenVM((int)p));
        }
        static public INavigationService OrderScreen() {
            Debug.WriteLine("Nav to OrderScreen");
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
        static public INavigationService AdminPromoScreen() {
            return new NavigationService<AdminPromoViewModel>(
                serviceProvider.GetRequiredService<AdminPromoViewModel>);
        }
        #endregion

        #region Shop
        //Shop
        static public INavigationService ShopViewScreen() {
            return new ParamNavigationService<ShopViewViewModel>(
                    (parameter) => new ShopViewViewModel(parameter as Models.MUser));
        }
        static public INavigationService ShopOrderScreen() {
            return new NavigationService<ShopOrderViewModel>(
                serviceProvider.GetRequiredService<ShopOrderViewModel>);
        }

        static public INavigationService ShopAddPromoScreen() {
            return new NavigationService<AddShopPromoViewModel>(
                serviceProvider.GetRequiredService<AddShopPromoViewModel>);
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
        static public INavigationService ShopPromoScreen() {
            return new NavigationService<ShopPromoViewModel>(
                serviceProvider.GetRequiredService<ShopPromoViewModel>);
        }
        static public INavigationService PromoInfomationScreen() {
            return new ParamNavigationService<PromoInformationViewModel>(
                (p) => new PromoInformationViewModel(p as PromoVMConstructor));
        }
        #endregion
    }
}
