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
            OnBackHome = new RelayCommand<object>(p => true, p => {
                NavigateProvider.HomeScreen().Navigate();
                NavigationStore.instance.stackScreen.RemoveAt(NavigationStore.instance.stackScreen.Count - 1);
            });
        }
    }
}
