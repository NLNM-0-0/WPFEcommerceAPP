using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace WPFEcommerceApp {
    public class OfflineScreenVM : BaseViewModel {
        public ICommand OnReconnect { get; set; }
        public OfflineScreenVM() {
            OnReconnect = new RelayCommand<object>(p => true, async p => {
                MainViewModel.SetLoading(true);
                if(Internet.CheckConnection()) {
                    Internet.OnlineNav();
                    MainViewModel.SetLoading(false);
                    return;
                }
                await Task.Delay(600);
                MainViewModel.SetLoading(false);
            });
        }
    }
}
