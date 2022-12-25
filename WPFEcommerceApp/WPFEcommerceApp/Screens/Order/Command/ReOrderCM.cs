using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class ReOrderCM : CommandBase {
        public override void Execute(object p) {

            NavigateProvider.CheckoutScreen().Navigate(p);
        }
    }
}
