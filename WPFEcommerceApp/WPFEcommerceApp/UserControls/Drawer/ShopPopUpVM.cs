using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp {
    public class ShopPopUpVM : BaseViewModel {
        public ICommand OnChecked { get; set; }
        public ShopPopUpVM(
            INavigationService ShopMainNavigateService,
            INavigationService ShopOrderNavigateService,
            INavigationService ShopProductNavigateService,
            INavigationService ShopRatingNavigateService) {
            OnChecked = new RelayCommand<object>(p => true, p => {
                var temp = p as string;
                if(temp == "1") {
                    ShopMainNavigateService.Navigate();
                }
                else if(temp == "2") {
                    ShopOrderNavigateService.Navigate();
                }
                else if(temp == "3") {
                    ShopProductNavigateService.Navigate();
                }
                else {
                    ShopRatingNavigateService.Navigate();
                }
            });
        }
    }
}
