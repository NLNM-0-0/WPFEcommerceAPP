﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private PackIcon _iconMaximize { get; set; }
        public DrawerVM DrawerVM { get; }
        public BaseViewModel CurrentViewModel => _navigationStore.CurrentViewModel;
        public ICommand CloseCM { get; set; }
        public ICommand MinimizeCM { get; set; }
        public ICommand MaximizeCM { get; set; }

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
                    Content = "Your process may not be saved if you close the app. Please check your work before closing the app!"
                };
                DialogHost.Show(view, "App");
            });
            MinimizeCM = new RelayCommand<object>(p => true, p => {
                (p as Window).WindowState = WindowState.Minimized;
            });
            MaximizeCM = new RelayCommand<object>(p => true, p => {
                var temp = (object[])p;
                if(_iconMaximize == null)
                    _iconMaximize = temp[1] as PackIcon;
                if((temp[0] as Window).WindowState == WindowState.Normal) {
                    (temp[0] as Window).WindowState = WindowState.Maximized;
                    (temp[1] as PackIcon).Kind = PackIconKind.CheckboxMultipleBlankOutline;
                }
                else {
                    (temp[0] as Window).WindowState = WindowState.Normal;
                    (temp[1] as PackIcon).Kind = PackIconKind.CheckboxBlankOutline;
                }
            });
        }
        private void OnCurrentVMChanged() {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public void DragWindow(object sender, MouseEventArgs e) {
            var t = new PackIcon();
            var tmp = getParent(sender as Grid);
            Window w = tmp as Window;
            if(e.LeftButton == MouseButtonState.Pressed) {
                Point p = e.GetPosition(sender as IInputElement);
                if(w.WindowState == WindowState.Maximized) {
                    _iconMaximize.Kind = PackIconKind.CheckboxBlankOutline;
                    w.Width = w.ActualWidth;
                    w.Height = 950;
                    w.Left = p.X < 850 ? 15 
                             : p.X < 1250 ? 350 : 600;
                    w.Top = p.Y - 20;
                    w.WindowStartupLocation = WindowStartupLocation.Manual;
                    w.WindowState = WindowState.Normal;
                }
                w.DragMove();
            }
        }
        static public void OnClosing(object sender, CancelEventArgs e) {
            if((sender as Window).WindowState != WindowState.Minimized) return;
            e.Cancel = true;
            (sender as Window).WindowState = WindowState.Normal;
            var view = new ConfirmDialog() {
                Param = sender,
                CM = new RelayCommand<object>(t => true, t => {
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
