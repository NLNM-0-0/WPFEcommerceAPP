using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace WPFEcommerceApp {
    public class MainViewModel : BaseViewModel {

        #region Loading Field
        private static bool isLoading;
        public static bool IsLoading {
            get { return isLoading; }
            set {
                if(isLoading != value) {
                    isLoading = value;
                    NotifyStaticPropertyChanged();
                }
            }
        }

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        private static void NotifyStaticPropertyChanged([CallerMemberName] string name = null) {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
        }
        #endregion

        private readonly NavigationStore _navigationStore;
        
        public DrawerVM DrawerVM { get; }
        public BaseViewModel CurrentViewModel => _navigationStore.CurrentViewModel;
        public ICommand CloseCM { get; set; }
        public ICommand MinimizeCM { get; set; }
        public ICommand MaximizeCM { get; set; }

        public MainViewModel(NavigationStore navigationStore,
            DrawerVM drawerVM) {
            DrawerVM = drawerVM;
            IsLoading = false;

            _navigationStore = navigationStore;

            _navigationStore.CurrentVMChanged += OnCurrentVMChanged;

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
        private void OnCurrentVMChanged() {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public void DragWindow(object sender, MouseEventArgs e) {
            var tmp = getParent(sender as Grid);
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
        static public void OnClosing(object sender, CancelEventArgs e) {
            e.Cancel = true;
            if((sender as Window).WindowState == WindowState.Minimized)
                (sender as Window).WindowState = WindowState.Normal;
            var view = new ConfirmDialog() {
                Param = sender,
                CM = new RelayCommand<object>(t => true, t => {
                    (t as Window).Closing -= MainViewModel.OnClosing;
                    (t as Window).Close();
                }),
                Header = "Are you sure?",
                Content = "Your process may not be saved if you close the app. Please check your work before closing the app!"
            };
            DialogHost.Show(view, "App");
        }

        FrameworkElement getParent(Grid p) {
            FrameworkElement parent = p;
            while(parent.Parent != null) {
                parent = parent.Parent as FrameworkElement;
            }
            return parent;
        }
    }
}
