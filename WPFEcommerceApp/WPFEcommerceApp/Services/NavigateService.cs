using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class NavigationService<TViewModel> : INavigationService
        where TViewModel : BaseViewModel{
        public NavigationService(CreateViewModel<TViewModel> createVM) {
            this.navigationStore=NavigationStore.instance;
            CreateVM=createVM;
        }

        private readonly NavigationStore navigationStore;
        public readonly CreateViewModel<TViewModel> CreateVM;

        public void Navigate() {
            navigationStore.CurrentViewModel = CreateVM();
        }
        public void Navigate(object o) {
            throw new NotImplementedException();
        }
    }
}
