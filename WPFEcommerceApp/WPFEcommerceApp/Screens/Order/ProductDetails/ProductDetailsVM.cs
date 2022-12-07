using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace WPFEcommerceApp {
    public class ProductDetailsVM :BaseViewModel {
		private int status;

		public int Status {
			get { return status; }
			set { status = value; OnPropertyChanged(); }
		}

		private ObservableCollection<bool> orderStatus;

		public ObservableCollection<bool> OrderStatus {
			get { return orderStatus; }
			set { orderStatus = value; OnPropertyChanged(); }
		}
		public ObservableCollection<BProduct> ShopProduct { get; set; }
		private Order orderDetail;

		public Order OrderDetail {
			get { return orderDetail; }
			set { orderDetail = value; OnPropertyChanged(); }
		}
		public ICommand OnReOrder { get; }
		public ICommand OnCancel { get; }
        public ICommand OnBack { get; }

        public ProductDetailsVM(Order order, NavigationStore navigationStore, INavigationService successNavService, INavigationService orderNavService) {
			OrderDetail = order;
			if(OrderDetail.Status == "Processing") {
				Status = 1;
			}
			else if(OrderDetail.Status == "Dellivering") {
				Status=2;
			}
			else if(OrderDetail.Status == "Dellivered") {
				Status=3;
			}
			else if(OrderDetail.Status == "Cancelled") {
				Status=0;
			}
			else Status=4;
			ShopProduct = new ObservableCollection<BProduct>(OrderDetail.ShopProduct);
			OrderStatus = new ObservableCollection<bool>() {
				false, false, false, false
			};
			for(int i = 0; i < Status; i++) {
				OrderStatus[i] = true;
			}
			//OnReOrder = new ReOrderCM(navigationStore, successNavService);
			ICommand ReOrderCM = new ReOrderCM(navigationStore, successNavService);
			OnReOrder = new RelayCommand<object>(p => true, async p => {
				var view = new ConfirmDialog() {
					CM = ReOrderCM,
					Param = p,
				};
				await DialogHost.Show(view);
			});


            ICommand CanCelCM = new RelayCommand<object>((p) => true, (p) => {
				//Do something with OrderStore
				orderNavService.Navigate();
			});
			OnCancel = new RelayCommand<object>(p => true, async p => {
				var view = new ConfirmDialog() {
					Header = "Are you sure?",
					Content = "You will not be able to take this action back.",
					CM = CanCelCM,
					Param = p,
				};
				await DialogHost.Show(view);
			});
			OnBack = new RelayCommand<object>(p => true, p => {
				//Actually I need to handle the tab index
				//But nahh, we'll do it later
				orderNavService.Navigate();
			});
        }

    }
}
