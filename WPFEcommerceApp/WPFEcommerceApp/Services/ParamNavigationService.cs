using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class ParamNavigationService<TViewModel> : INavigationService
        where TViewModel : BaseViewModel{
        private readonly NavigationStore _navigationStore;
        private readonly CreateParamVM<TViewModel> _createViewModel;

        public ParamNavigationService(CreateParamVM<TViewModel> createViewModel) {
            _navigationStore = NavigationStore.instance;
            _createViewModel = createViewModel;
        }

        public void Navigate() { 
            throw new NotImplementedException();
        }
        public void NoBackNavigate() {
            throw new NotImplementedException();
        }
        public void NoBackNavigate(object o) {
            _navigationStore.CurrentViewModel = _createViewModel(o);
        }
        public void Navigate(object parameter) {
            _navigationStore.CurrentViewModel = _createViewModel(parameter);
            _navigationStore.stackScreen.Add(new Tuple<INavigationService, object>(this, parameter));
            if(_navigationStore.stackScreen.Count == 6) {
                _navigationStore.stackScreen.RemoveAt(0);
            }
        }

        public Type GetViewModel() {
            Type viewModelType = typeof(TViewModel);
            return viewModelType;
        }
    }
}
