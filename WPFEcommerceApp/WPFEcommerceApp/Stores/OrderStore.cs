using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class OrderStore {
		public event Action OrderListChanged;

		private ObservableCollection<Order> orderList;

		public ObservableCollection<Order> OrderList {
			get { return orderList; }
			set { 
				orderList = value;
				OrderListChanged?.Invoke();
			}
		}

		public void Add(Order p) {
			OrderList.Add(p);
		}
	}
}
