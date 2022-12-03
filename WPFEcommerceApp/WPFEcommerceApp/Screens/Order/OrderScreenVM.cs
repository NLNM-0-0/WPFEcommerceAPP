using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPFEcommerceApp {
    public class OrderScreenVM : BaseViewModel {
        private ObservableCollection<Order> orderList;

        public ObservableCollection<Order> OrderList {
            get { return orderList; }
            set { orderList = value; OnPropertyChanged(); }
        }
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
            INavigationService successNavService,
            INavigationService orderNavService) {
			OrderList = new ObservableCollection<Order>() {
				new Order("001", "Processing"),
                new Order("002", "Dellivering"),
                new Order("003", "Dellivered"),
                new Order("004", "Processing"),
                new Order("005", "Dellivering"),
                new Order("006", "Dellivered"),

            };
            ProcessingList = new ObservableCollection<Order>();
            DeliveringList = new ObservableCollection<Order>();
            DeliveredList = new ObservableCollection<Order>();
            CancelledList = new ObservableCollection<Order>();

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

            OnCancel = new RelayCommand<object>((p) => true, (p) => {
                for(int i = 0; i < OrderList.Count; i++) {
                    if(OrderList[i] == (p as Order)) {
                        OrderList[i].Status = "Cancelled";
                    }
                }
                (p as Order).Status = "Cancelled";
                CancelledList.Add(p as Order);
                ProcessingList.Remove(p as Order);
			});
            OnDetailView = new RelayCommand<object>((p) => true, (p) => {
                var param = p as Order;
                var nav = new ParamNavigationService<Order, ProductDetailsVM>(navigationStore,
                    (parameter) => new ProductDetailsVM(parameter, navigationStore, successNavService, orderNavService));
                nav.Navigate(param);
            });
            OnReorder = new ReOrderCM(navigationStore, successNavService);
        }
    }
}
