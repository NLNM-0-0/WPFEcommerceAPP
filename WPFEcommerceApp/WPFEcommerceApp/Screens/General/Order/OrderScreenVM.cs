﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;

namespace WPFEcommerceApp {
    public class OrderScreenVM : BaseViewModel {
        private readonly OrderStore _orderStore;

        private ObservableCollection<Order> OrderList => _orderStore.OrderList;


        private ObservableCollection<Order> processingList;

        public ObservableCollection<Order> ProcessingList {
            get { return processingList; }
            set { processingList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Order> deliveringList;

        public ObservableCollection<Order> DeliveringList {
            get { return deliveringList; }
            set { deliveringList = value; OnPropertyChanged(); }
        }
        private ObservableCollection<Order> deliveredList;

        public ObservableCollection<Order> DeliveredList {
            get { return deliveredList; }
            set { deliveredList = value; OnPropertyChanged(); }
        }
        private ObservableCollection<Order> cancelList;

        public ObservableCollection<Order> CancelledList {
            get { return cancelList; }
            set { cancelList = value; OnPropertyChanged(); }
        }

        private int currentPage;

        public int CurrentPage {
            get { return currentPage; }
            set { currentPage = value; }
        }

        public ICommand OnCancel { get; set; }
        public ICommand OnDetailView { get; set; }
        public ICommand OnReorder { get; set; }
        public ICommand OnReviewProduct { get; set; }

        public OrderScreenVM(int currentPage = 0) {
            MainViewModel.IsLoading = true;

            CurrentPage = currentPage;

            _orderStore = OrderStore.instance;
            _orderStore.OrderListChanged += onOrderListChange;
            Task.Run(async () => await _orderStore.Load());

            ICommand CanCelCM = new RelayCommand<object>((p) => true, async (p) => {
                MainViewModel.IsLoading = true;

                (p as Order).Status = "Cancelled";
                await _orderStore.Update(p as Order);

                MainViewModel.IsLoading = false;
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

            OnDetailView = new RelayCommand<object>((p) => true, (p) => {
                NavigateProvider.OrderDetailScreen().Navigate(p);
            });

            ICommand ReOrderCM = new ReOrderCM();
            OnReorder = new RelayCommand<object>(p => true, async p => {
                var view = new ConfirmDialog() {
                    CM = ReOrderCM,
                    Param = p
                };
                await DialogHost.Show(view, "Main");
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
            MainViewModel.IsLoading = false;
        }

        private void onOrderListChange() {
            MainViewModel.IsLoading = true;
            ProcessingList = new ObservableCollection<Order>();
            DeliveringList = new ObservableCollection<Order>();
            DeliveredList = new ObservableCollection<Order>();
            CancelledList = new ObservableCollection<Order>();
            if(OrderList != null)
                for(int i = 0; i < OrderList.Count; i++) {
                    string stt = OrderList[i].Status;
                    if(stt == "Processing") {
                        ProcessingList.Add(OrderList[i]);
                    }
                    else if(stt == "Delivering") {
                        DeliveringList.Add(OrderList[i]);
                    }
                    else if(stt == "Delivered" || stt == "Completed") {
                        DeliveredList.Add(OrderList[i]);
                    }
                    else if(stt == "Cancelled") {
                        CancelledList.Add(OrderList[i]);
                    }
                }
            MainViewModel.IsLoading = false;
        }

        public override void Dispose() {
            _orderStore.OrderListChanged -= onOrderListChange;
            base.Dispose();
        }
    }

    public class ReviewProduct {
        public string IdOrder { get; set; }
        public Product product { get; set; }
        public int rating { get; set; }
        public ReviewProduct(Product product, string IdOrder, int rating = 5) {
            this.IdOrder = IdOrder;
            this.product = product;
            this.rating = rating;
        }
    }
}