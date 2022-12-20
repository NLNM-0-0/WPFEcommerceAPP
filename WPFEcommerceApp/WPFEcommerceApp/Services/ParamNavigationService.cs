using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class ParamNavigationService<TParameter, TViewModel>
        where TViewModel : BaseViewModel{
        private readonly NavigationStore _navigationStore;
        private readonly CreateViewModel<TParameter, TViewModel> _createViewModel;

        public ParamNavigationService(CreateViewModel<TParameter, TViewModel> createViewModel) {
            _navigationStore = NavigationStore.instance;
            _createViewModel = createViewModel;
        }

        public void Navigate(TParameter parameter) {
            _navigationStore.CurrentViewModel = _createViewModel(parameter);
        }
    }
}
