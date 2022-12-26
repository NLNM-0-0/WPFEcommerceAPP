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
            OnBackHome = new NavigateCommand(NavigateProvider.HomeScreen());
        }
    }
}
