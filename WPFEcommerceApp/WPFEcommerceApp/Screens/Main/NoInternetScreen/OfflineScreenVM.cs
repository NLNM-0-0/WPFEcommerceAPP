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
                MainViewModel.IsLoading = true;
                if(Internet.CheckConnection()) {
                    Internet.OnlineNav();
                    MainViewModel.IsLoading = false;
                    return;
                }
                await Task.Delay(600);
                MainViewModel.IsLoading = false;
            });
        }
    }
}
