using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class OrderScreenVM : BaseViewModel {
        private readonly GenericDataRepository<MUser> userRepo = new GenericDataRepository<MUser>();
        private readonly OrderStore _orderStore;

        private List<Order> OrderList => _orderStore.OrderList;


        private List<Order> processingList;

        public List<Order> ProcessingList {
            get { return processingList; }
            set { processingList = value; OnPropertyChanged(); }
        }

        private List<Order> deliveringList;

        public List<Order> DeliveringList {
            get { return deliveringList; }
            set { deliveringList = value; OnPropertyChanged(); }
        }
        private List<Order> deliveredList;

        public List<Order> DeliveredList {
            get { return deliveredList; }
            set { deliveredList = value; OnPropertyChanged(); }
        }
        private List<Order> cancelList;

        public List<Order> CancelledList {
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
            CurrentPage = currentPage;

            _orderStore = OrderStore.instance;
            _orderStore.OrderListChanged += onOrderListChange;
            Debug.WriteLine("OrderScreenVM Created");

            Task.Run(async () => {
                MainViewModel.SetLoading(true);
                await _orderStore.Load();
                MainViewModel.SetLoading(false);
            });

            ICommand CanCelCM = new ImmediateCommand<object>(async (p) => {
                (p as Order).Status = "Cancelled";
                await _orderStore.Update(p as Order);
            });

            OnCancel = new ImmediateCommand<object>(async p => {
                var view = new ConfirmDialog() {
                    Header = "Are you sure?",
                    Content = "You will not be able to take this action back.",
                    CM = CanCelCM,
                    Param = p,
                };
                await DialogHost.Show(view, "Main");
            });

            OnDetailView = new ImmediateCommand<object>(p => {
                var t = NavigationStore.instance.stackScreen;
                t[t.Count - 1] = new Tuple<INavigationService, object>(NavigateProvider.OrderParamScreen(), CurrentPage);
                NavigateProvider.OrderDetailScreen().Navigate(p);
            });

            ICommand ReOrderCM = new ReOrderCM();
            OnReorder = new ImmediateCommand<object>(async p => {
                
                var t = await userRepo.GetSingleAsync(d => d.Id == (p as Order).IDShop);

                var rootOrder = p as Order;
                bool bannedCheck = false;
                foreach(var c in rootOrder.ProductList) {
                    if(c.Banned) {
                        bannedCheck = true;
                        break;
                    }
                }

                if(t.StatusShop == "Banned" ||
                    t.StatusUser == "Banned" ||
                    bannedCheck) {
                    var view = new ConfirmDialog() {
                        Header = "Oops",
                        Content = $"This {(t.StatusShop == "Banned" ? "shop" : "order")} has been banned!"
                    };
                    await DialogHost.Show(view, "Main");
                    return;
                }
                else {
                    var tempOrder = new Order(rootOrder.ProductList) {
                         DateBegin = DateTime.Now,
                         IDCustomer = rootOrder.IDCustomer,
                         IDShop = rootOrder.IDShop,
                         ShopImage = rootOrder.ShopImage,
                         ShopName = rootOrder.ShopName,
                    };
                    ReOrderCM.Execute(new List<Order>() { tempOrder });
                }
            });

            OnReviewProduct = new ImmediateCommand<object>(async p => {
                var t = p as Order;
                List<ReviewProduct> products = new List<ReviewProduct>();
                for(int i = 0; i < t.ProductList.Count; i++) {
                    products.Add(new ReviewProduct() {
                        Product = t.ProductList[i],
                        IdOrder = t.ID,
                        Rating = 5
                    });
                }
                var view = new ReviewProductDialog() {
                    ProductList = products,
                };
                await DialogHost.Show(view, "Main");
            });
        }


        //Fix: ObservableCollection:
        //Can't change the Observable in another thread
        //Can fix this by use Dispatcher to Invoke an Action to Current Synchronization Context
        //But it will be dumb if Collection has too many items to add
        //So the Invoke need to be outside the loop
        //The alternative way to do this is reset the list after change and call OnpropertyChanged
        //But I don't know if it legal to call OnPropertyChanged in another thread
        //So I put this this into a Dispatcher.
        private void onOrderListChange() {
            var processing = new List<Order>();
            var delivering = new List<Order>();
            var delivered = new List<Order>();
            var cancelled = new List<Order>();

            if(OrderList != null) { 
                for(int i = 0; i < OrderList.Count; i++) {
                    string stt = OrderList[i].Status;
                    if(stt == "Processing") {
                        processing.Add(OrderList[i]);
                    }
                    else if(stt == "Delivering") {
                        delivering.Add(OrderList[i]);
                    }
                    else if(stt == "Delivered" || stt == "Completed") {
                        delivered.Add(OrderList[i]);
                    }
                    else if(stt == "Cancelled") {
                        cancelled.Add(OrderList[i]);
                    }
                }
            }
            App.Current.Dispatcher.Invoke((Action)(() => {
                ProcessingList = new List<Order>(processing);
                DeliveringList = new List<Order>(delivering);
                DeliveredList = new List<Order>(delivered);
                CancelledList = new List<Order>(cancelled);
            }));
        }

        public override void Dispose() {
            _orderStore.OrderListChanged -= onOrderListChange;
            base.Dispose();
        }
    }

    public class ReviewProduct {
        public string IdOrder { get; set; }
        public Product Product { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        //}
    }
}
