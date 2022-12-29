using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class CompositeNavigationService : INavigationService {
        private readonly IEnumerable<INavigationService> _navigationServices;

        public CompositeNavigationService(params INavigationService[] navigationServices) {
            _navigationServices = navigationServices;
        }

        public void Navigate() {
            foreach(INavigationService navigationService in _navigationServices) {
                navigationService.Navigate();
            }
        }
        public void Navigate(object o) {
            throw new NotImplementedException();
        }
        public Type GetViewModel() {
            return null;
        }
    }
}
