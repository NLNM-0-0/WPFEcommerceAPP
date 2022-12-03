using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class ReOrderCM : CommandBase {
        private readonly NavigationStore navigationStore;
        private readonly INavigationService successNavService;

        public ReOrderCM(NavigationStore navigationStore, INavigationService successNavService) {
            this.navigationStore=navigationStore;
            this.successNavService=successNavService;
        }

        public override void Execute(object p) {
            var nav = new ParamNavigationService<Order, CheckoutScreenVM>(navigationStore,
                    (parameter) => new CheckoutScreenVM(successNavService, parameter));
            nav.Navigate(p as Order);
        }
    }
}
