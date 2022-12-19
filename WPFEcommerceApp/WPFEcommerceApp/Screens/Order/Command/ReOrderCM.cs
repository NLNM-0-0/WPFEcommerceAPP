using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class ReOrderCM : CommandBase {
        public override void Execute(object p) {
            var nav = new ParamNavigationService<Order, CheckoutScreenVM>(
                    (parameter) => new CheckoutScreenVM(parameter));
            nav.Navigate(p as Order);
        }
    }
}
