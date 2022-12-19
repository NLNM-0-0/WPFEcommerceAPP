using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class ReOrderCM : CommandBase {
        private readonly INavigationService successNavService;

        public ReOrderCM(INavigationService successNavService) {
            this.successNavService=successNavService;
        }

        public override void Execute(object p) {
            var nav = new ParamNavigationService<Order, CheckoutScreenVM>(
                    (parameter) => new CheckoutScreenVM(successNavService, parameter));
            nav.Navigate(p as Order);
        }
    }
}
