﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;

namespace WPFEcommerceApp {
    public class ProductDetailsVM :BaseViewModel {
		private readonly OrderStore _orderStore;
		public Order OrderDetail { get; set; }
        public int Status =>
            OrderDetail == null 
			? 0 
			: OrderDetail.Status == "Processing"
			? 1 
			: OrderDetail.Status == "Delivering"
			? 2
			: OrderDetail.Status == "Delivered"
			? 3
			: OrderDetail.Status == "Cancelled"
			? 0 : 5;

		public ObservableCollection<bool> OrderStatus { get; set; }
		public ObservableCollection<Product> ProductList { get; set; }

		public ICommand OnReOrder { get; }
		public ICommand OnCancel { get; }
        public ICommand OnBack { get; }
		public ICommand OnViewProduct { get; }
		public ICommand OnReviewProduct { get; }
		public ICommand OnVisitShop { get; }

        public ProductDetailsVM(
			Order order, 
			NavigationStore navigationStore,
            AccountStore accountStore,
            OrderStore orderStore,
			INavigationService successNavService, 
			INavigationService orderNavService) {

            OrderDetail = order;
			_orderStore = orderStore;

			ProductList = new ObservableCollection<Product>(OrderDetail.ProductList);

			OrderStatus = new ObservableCollection<bool>() {
				false, false, false, false, false
			};
			for(int i = 0; i < Status; i++) {
				OrderStatus[i] = true;
			}
			//OnReOrder = new ReOrderCM(navigationStore, successNavService);
			ICommand ReOrderCM = new ReOrderCM(navigationStore, accountStore, orderStore, successNavService);
			OnReOrder = new RelayCommand<object>(p => true, async p => {
				var view = new ConfirmDialog() {
					CM = ReOrderCM,
					Param = p,
				};
				await DialogHost.Show(view, "Main");
			});


            ICommand CanCelCM = new RelayCommand<object>((p) => true, async (p) => {
				//Do something with OrderStore
				MainViewModel.IsLoading = true;

                var timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(2000);
                timer.Tick += delegate {
                    MainViewModel.IsLoading = false;
                };
                timer.Start();

                (p as Order).Status = "Cancelled";
                await _orderStore.Update(p as Order);
                orderNavService.Navigate();
                //MainViewModel.IsLoading = false;

            });
			OnCancel = new RelayCommand<object>(p => true, async p => {
				var view = new ConfirmDialog() {
					Header = "Are you sure?",
					Content = "You will not be able to take this action back.",
					CM = CanCelCM,
					Param = p,
				};
				await DialogHost.Show(view, "Main");
			});
			OnBack = new RelayCommand<object>(p => true, p => {
                //Actually I need to handle the tab index
                //But nahh, we'll do it later
                var param = OrderDetail.Status == "Processing"
							? 0
							: OrderDetail.Status == "Delivering"
							? 1
							: OrderDetail.Status == "Delivered"
							? 2
							: OrderDetail.Status == "Cancelled"
							? 3 : 2;
                var nav = new ParamNavigationService<int, OrderScreenVM>(navigationStore,
                    (parameter) => new OrderScreenVM(navigationStore, accountStore, orderStore, successNavService, orderNavService, parameter));
                nav.Navigate(param);
			});
			OnViewProduct = new RelayCommand<object>(p => true, p => {
				MessageBox.Show($"Navigate to {(p as Product).ID}");
            });
			OnReviewProduct = new RelayCommand<object>(p => true, async p => {
                var t = p as Order;
                List<ReviewProduct> products = new List<ReviewProduct>();
                for(int i = 0; i < t.ProductList.Count; i++) {
                        products.Add(new ReviewProduct(t.ProductList[i], t.ID));
                }
                var view = new ReviewProductDialog() {
                    ProductList = products,
                };
                await DialogHost.Show(view, "Main");
            });

			OnVisitShop = new RelayCommand<object>(p => true, p => {
				MessageBox.Show($"Navigate to Shop {OrderDetail.IDShop}");
			});
        }

    }
}
