using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFEcommerceApp {
    public class WindowControlBarVM : BaseViewModel {
        public ICommand CloseCM { get; set; }
        public ICommand MinimizeCM { get; set; }
        public ICommand MaximizeCM { get; set; }

        public WindowControlBarVM() {
            CloseCM = new RelayCommand<object>(p => true, p => {
                (p as Window).Close();
            });
            MinimizeCM = new RelayCommand<object>(p => true, p => {
                (p as Window).WindowState = WindowState.Minimized;
            });
            MaximizeCM = new RelayCommand<object>(p => true, p => {
                if((p as Window).WindowState == WindowState.Normal) {
                    (p as Window).WindowState = WindowState.Maximized;
                }
                else {
                    (p as Window).WindowState = WindowState.Normal;
                }
            });
        }
        public void DragWindow(object sender, MouseEventArgs e) {
            var tmp = getWindow(sender as FrameworkElement);
            Window w = tmp as Window;
            if(e.LeftButton == MouseButtonState.Pressed) {
                Point p = e.GetPosition(sender as IInputElement);
                if(w.WindowState == WindowState.Maximized) {
                    w.Width = 1440;
                    w.Height = 950;
                    w.Left = p.X < 850 ? 10
                             : p.X < 1250
                             ? 350 : 600;
                    w.Top = p.Y - 20;
                    w.WindowStartupLocation = WindowStartupLocation.Manual;
                    w.WindowState = WindowState.Normal;
                }
                w.DragMove();
            }
        }
        public static FrameworkElement getWindow(FrameworkElement p) {
            FrameworkElement parent = p;
            while(parent.Parent != null) {
                parent = parent.Parent as FrameworkElement;
            }
            return parent;
        }
    }
}
