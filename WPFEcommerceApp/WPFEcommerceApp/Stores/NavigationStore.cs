using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class NavigationStore {
        public static NavigationStore instance;
        public event Action CurrentVMChanged;

        public BaseViewModel currentViewModel;
        public BaseViewModel CurrentViewModel {
            get => currentViewModel;
            set {
                currentViewModel?.Dispose();
                currentViewModel = value;
                OnCurrentVMChange();
            }
        }

        private void OnCurrentVMChange() {
            CurrentVMChanged?.Invoke();
        }
    }
}
