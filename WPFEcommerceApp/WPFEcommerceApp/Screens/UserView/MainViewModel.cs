using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFEcommerceApp {
    public class MainViewModel : BaseViewModel {
        private readonly NavigationStore _navigationStore;
        public DrawerVM DrawerVM { get; }
        public BaseViewModel CurrentViewModel => _navigationStore.CurrentViewModel;

        public MainViewModel(NavigationStore navigationStore,
            DrawerVM drawerVM) {
            DrawerVM = drawerVM;

            _navigationStore = navigationStore;

            _navigationStore.CurrentVMChanged += OnCurrentVMChanged;
        }
        private void OnCurrentVMChanged() {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
