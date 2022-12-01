using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


		public ProductDetailsVM() {
			OrderDetail = new Order();
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
        }

    }
}
