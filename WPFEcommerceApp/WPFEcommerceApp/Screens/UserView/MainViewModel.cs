using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace WPFEcommerceApp {
    public class MainViewModel : BaseViewModel {
        private readonly NavigationStore _navigationStore;
        public DrawerVM DrawerVM { get; }
        public BaseViewModel CurrentViewModel => _navigationStore.CurrentViewModel;
        public ICommand CloseCM { get; set; }
        public ICommand MinimizeCM { get; set; }
        public ICommand MaximizeCM { get; set; }
        public ICommand DragWindowCM { get; set; }


        public MainViewModel(NavigationStore navigationStore,
            DrawerVM drawerVM) {
            DrawerVM = drawerVM;

            _navigationStore = navigationStore;

            _navigationStore.CurrentVMChanged += OnCurrentVMChanged;

            CloseCM = new RelayCommand<object>(p => true, p => {
                var view = new ConfirmDialog() {
                    Param = p,
                    CM = new RelayCommand<object>(t => true, t => {
                        (t as Window).Close();
                    }),
                    Header = "Are you sure?",
                    Content = "Close app may not save your process. Please check your work before close the app!"
                };
                DialogHost.Show(view, "App");
            });
            MinimizeCM = new RelayCommand<object>(p => true, p => {
                (p as Window).WindowState = WindowState.Minimized;
            });
            MaximizeCM = new RelayCommand<object>(p => true, p => {
                if((p as Window).WindowState == WindowState.Normal)
                    (p as Window).WindowState = WindowState.Maximized;
                else
                    (p as Window).WindowState = WindowState.Normal;
            });
            DragWindowCM = new RelayCommand<object>(p => true, p => {
                (p as Window).DragMove();
            });
        }
        private void OnCurrentVMChanged() {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
