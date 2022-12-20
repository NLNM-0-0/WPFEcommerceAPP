using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp {
    public class SuccessScreenVM : BaseViewModel {
        public ICommand OnBackHome { get; set; }
        public SuccessScreenVM() {
            //need to be home screen here
            OnBackHome = new NavigateCommand(NavigateProvider.CheckoutScreen());
        }
    }
}
