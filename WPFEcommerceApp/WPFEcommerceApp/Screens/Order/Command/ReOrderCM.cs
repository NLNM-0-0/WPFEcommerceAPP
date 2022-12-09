using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class ReOrderCM : CommandBase {
        private readonly NavigationStore navigationStore;
        private readonly INavigationService successNavService;
        private readonly OrderStore orderStore;
        private readonly AccountStore accountStore;

        public ReOrderCM(NavigationStore navigationStore, AccountStore accountStore, OrderStore orderStore, INavigationService successNavService) {
            this.navigationStore=navigationStore;
            this.successNavService=successNavService;
            this.orderStore = orderStore;
            this.accountStore = accountStore;
        }

        public override void Execute(object p) {
            var nav = new ParamNavigationService<Order, CheckoutScreenVM>(navigationStore,
                    (parameter) => new CheckoutScreenVM(successNavService, accountStore, orderStore, parameter));
            nav.Navigate(p as Order);
        }
    }
}
