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
        public BaseViewModel CurrentViewModel => _navigationStore?.CurrentViewModel;

        public MainViewModel(
            DrawerVM drawerVM) {
            DrawerVM = drawerVM;
            IsLoading = false;
            
            _navigationStore = NavigationStore.instance;

            _navigationStore.CurrentVMChanged += OnCurrentVMChanged;
        }
        private void OnCurrentVMChanged() {
            OnPropertyChanged(nameof(CurrentViewModel));
        }


        static public void OnClosing(object sender, CancelEventArgs e) {
            e.Cancel = true;
            if((sender as Window).WindowState == WindowState.Minimized)
                (sender as Window).WindowState = WindowState.Normal;
            var view = new ConfirmDialogCustom() {
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
    }
}
