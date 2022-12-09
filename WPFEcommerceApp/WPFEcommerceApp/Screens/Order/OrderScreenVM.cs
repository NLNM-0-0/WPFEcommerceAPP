using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

        public ICommand OnCancel { get; set; }
        public ICommand OnDetailView { get; set; }
        public ICommand OnReorder { get; set; }


        public OrderScreenVM(
            NavigationStore navigationStore, 
            AccountStore accountStore,
            OrderStore orderStore,
            INavigationService successNavService,
            INavigationService orderNavService) {

            _orderStore = orderStore;
            _orderStore.OrderListChanged += onOrderListChange;
            ProcessingList = new ObservableCollection<Order>();
            DeliveringList = new ObservableCollection<Order>();
            DeliveredList = new ObservableCollection<Order>();
            CancelledList = new ObservableCollection<Order>();

            if(OrderList != null)
                for(int i = 0; i < OrderList.Count; i++) {
                    if(OrderList[i].Status == "Processing") {
                        ProcessingList.Add(OrderList[i]);
                    }
                    else if(OrderList[i].Status == "Dellivering") {
                        DeliveringList.Add(OrderList[i]);
                    }
                    else if(OrderList[i].Status == "Dellivered") {
                        DeliveredList.Add(OrderList[i]);
                    }
                    else if(OrderList[i].Status == "Cancelled") {
                        CancelledList.Add(OrderList[i]);
                    }
                }

            ICommand CanCelCM = new RelayCommand<object>((p) => true,async (p) => {
                (p as Order).Status = "Cancelled";
                await _orderStore.Update(p as Order);
                //CancelledList.Add(p as Order);
                //ProcessingList.Remove(p as Order);
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
                var param = p as Order;
                var nav = new ParamNavigationService<Order, ProductDetailsVM>(navigationStore,
                    (parameter) => new ProductDetailsVM(parameter, navigationStore, accountStore, orderStore, successNavService, orderNavService));
                nav.Navigate(param);
            });
            
            ICommand ReOrderCM = new ReOrderCM(navigationStore, accountStore, orderStore, successNavService);
            OnReorder = new RelayCommand<object>(p => true,async p => {
                var view = new ConfirmDialog() {
                    CM = ReOrderCM,
                    Param = p
                };
                await DialogHost.Show(view, "Main");
            });

        }

        private void onOrderListChange() {
            ProcessingList.Clear();
            DeliveringList.Clear();
            DeliveredList.Clear();
            CancelledList.Clear();
            if(OrderList != null)
                for(int i = 0; i < OrderList.Count; i++) {
                    if(OrderList[i].Status == "Processing") {
                        ProcessingList.Add(OrderList[i]);
                    }
                    else if(OrderList[i].Status == "Dellivering") {
                        DeliveringList.Add(OrderList[i]);
                    }
                    else if(OrderList[i].Status == "Dellivered") {
                        DeliveredList.Add(OrderList[i]);
                    }
                    else if(OrderList[i].Status == "Cancelled") {
                        CancelledList.Add(OrderList[i]);
                    }
                }
        }

        public override void Dispose() {
            _orderStore.OrderListChanged -= onOrderListChange;
            base.Dispose();
        }
    }
}
