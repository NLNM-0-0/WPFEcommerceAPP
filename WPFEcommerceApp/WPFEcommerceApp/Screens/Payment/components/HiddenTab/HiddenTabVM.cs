using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace WPFEcommerceApp {
    internal class HiddenTabVM : BaseViewModel{
        public HiddenTabVM() {

            //MinimizeWindowCM = new RelayCommand<UserControl>((p) => { return true; }, (p) => {
            //    var tmp = getParent(p);
            //    var window = tmp as Window;
            //    if(window != null)
            //        if(window.WindowState != WindowState.Minimized)
            //            window.WindowState = WindowState.Minimized;
            //        else window.WindowState = WindowState.Maximized;
            //});
        }
    }
}
