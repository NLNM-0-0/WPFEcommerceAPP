using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public interface INavigationService {
        void Navigate();
        void Navigate(object o);
        void NoBackNavigate();
        void NoBackNavigate(object o);

        Type GetViewModel();
    }
}
