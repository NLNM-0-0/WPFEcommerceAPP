using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            if(navigationStore.CurrentViewModel != null && navigationStore.CurrentViewModel.GetType().Equals(typeof(TViewModel))) {
                return;
            }
            navigationStore.CurrentViewModel = CreateVM();
            navigationStore.stackScreen.Add(new Tuple<INavigationService, object>(this, null));
            if(navigationStore.stackScreen.Count == 6) {
                navigationStore.stackScreen.RemoveAt(0);
            }
        }

        public void NoBackNavigate() {
            navigationStore.CurrentViewModel = CreateVM();
        }
        public void NoBackNavigate(object o) {
            throw new NotImplementedException();
        }

        public Type GetViewModel() {
            Type viewModelType = typeof(TViewModel);
            return viewModelType;
        }

        public void Navigate(object o) {
            throw new NotImplementedException();
        }
    }
}
