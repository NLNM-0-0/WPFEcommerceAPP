using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using DataAccessLayer;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class OrderStore {
		public static OrderStore instance;
		private readonly AccountStore _accountStore;
		public MUser user => _accountStore?.CurrentAccount;

		public event Action OrderListChanged;

		private ObservableCollection<Order> orderList;
		public ObservableCollection<Order> OrderList {
			get { return orderList; }
			set { 
				orderList = value;
			}
		}
		public OrderStore() {
			_accountStore = AccountStore.instance;
			_accountStore.AccountChanged += OnAccountChange;
            Task.Run(async () => await Load());
        }

        private readonly GenericDataRepository<MOrder> orderRepo = new GenericDataRepository<MOrder>();
        private readonly GenericDataRepository<OrderInfo> orderInfoRepo = new GenericDataRepository<OrderInfo>();
        private readonly GenericDataRepository<MUser> userRepo = new GenericDataRepository<MUser>();

        private void OnAccountChange() {
			Task.Run(async () => await Load());
            OrderListChanged?.Invoke();
        }
		public async Task Load() {
			MainViewModel.IsLoading = true;
            OrderList?.Clear();
            OrderList = new ObservableCollection<Order>();
            if(user == null) {
                MainViewModel.IsLoading = false;
                return;
			}

			var userTemp = await userRepo.GetSingleAsync(d => d.Id == user.Id, d => d.MOrders);
			List<MOrder> mOrders = userTemp.MOrders.ToList();

			List<List<OrderInfo>> listOrderInfor = new List<List<OrderInfo>>();

			foreach(var o in mOrders) {
                var oTemp = await orderRepo.GetSingleAsync(d => d.Id == o.Id, d => d.OrderInfoes);
                listOrderInfor.Add(oTemp.OrderInfoes.ToList());
            }

			List<List<Product>> listOrderProduct = new List<List<Product>>();

			foreach(var listOI in listOrderInfor) { 
				List<Product> listProduct = new List<Product>();
				foreach(var oi in listOI) {
					var productTemp = await orderInfoRepo.GetSingleAsync(
						d => d.IdOrder == oi.IdOrder && 
						d.IdProduct == oi.IdProduct, 
						d => d.Product, 
						d => d.Rating
					);
					Models.Product product = new Models.Product();
					product = productTemp.Product;

					Product tmp = new Product() {
						ID = oi.IdProduct,
						ProductImage = oi.ImageProduct,
						Name = product.Name,
						Size = oi.Size,
						Color = product.Color,
						Description = product.Description,
						Price = product.Price,
						Amount = oi.Count,
						Subtotal = oi.Count * product.Price,
						Discount = product.Sale
					};

					listProduct.Add(tmp);
				}
				listOrderProduct.Add(listProduct);
			}

			for(int i = 0; i < mOrders.Count; i++) { 
				var order = await orderRepo.GetSingleAsync(d => d.Id == mOrders[i].Id, d => d.MUser1);
				var t = order.ShippingSpeedMethod;
				Order ordertemp = new Order(
						order.Id,
						order.IdCustomer,
						order.IdShop,
						order.Status,
						(double)order.ShipTotal,
						listOrderProduct[i],
						(DateTime)order.DateBegin,
						order.MUser1.Name,
						order.MUser1.SourceImageAva,
						order.ShippingSpeedMethod
					) {
				};
				OrderList.Add(ordertemp);
			}
            OrderListChanged?.Invoke();
            MainViewModel.IsLoading = false;
        }

        MOrder GenerateOrder(Order p) {
            MOrder temp = new MOrder();
            temp.Id = p.ID;
            temp.IdCustomer = p.IDCustomer;
            temp.IdShop = p.IDShop;
            temp.ShipTotal = (int)p.ShipTotal;
            temp.DateBegin = p.DateBegin;
            temp.DateEnd = p.DateEnd;
            temp.OrderTotal = (int)p.OrderTotal;
            temp.Status = p.Status;
			temp.ShippingSpeedMethod = p.ShippingSpeedMethod;
			return temp;
        }

		async Task genOrderInfor (Order p, string type) {
            List<Product> products = p.ProductList;
            for(int i = 0; i < products.Count; i++) {
                OrderInfo orderInfo = new OrderInfo() {
                    IdOrder = p.ID,
                    IdProduct = products[i].ID,
                    IdRating = null,
                    ImageProduct = products[i].ProductImage,
                    Size = products[i].Size,
                    Count = products[i].Amount,
                    TotalPrice = (int)p.OrderTotal
                };
				if(type == "Add")
					await orderInfoRepo.Add(orderInfo);
				else if(type == "Remove")
                    await orderInfoRepo.Remove(orderInfo);
				else if(type == "Update")
                    await orderInfoRepo.Update(orderInfo);
            }
            OrderListChanged?.Invoke();
        }
        public async Task Add(Order p) {
            MainViewModel.IsLoading = true;
            p.ID = await GenerateID.Gen(typeof(MOrder));
            OrderList.Add(p);
			MOrder temp = GenerateOrder(p);
			await orderRepo.Add(temp);

            await genOrderInfor(p, "Add");
            MainViewModel.IsLoading = false;
        }
        public async Task Remove(Order p) {
            MainViewModel.IsLoading = true;
            OrderList.Remove(p);
            MOrder temp = GenerateOrder(p);
            await orderRepo.Remove(temp);

			await genOrderInfor(p, "Remove");
			MainViewModel.IsLoading = false;
        }
        public async Task Update(Order p) {
            MainViewModel.IsLoading = true;
            for(int i = 0; i < OrderList.Count; i++) {
				if(OrderList[i].ID == p.ID) OrderList[i] = p;
			}
            var temp = await orderRepo.GetSingleAsync(d => d.Id == p.ID);
            temp.IdCustomer = p.IDCustomer;
            temp.IdShop = p.IDShop;
            temp.ShipTotal = (int)p.ShipTotal;
            temp.DateBegin = p.DateBegin;
            temp.DateEnd = p.DateEnd;
            temp.OrderTotal = (int)p.OrderTotal;
            temp.Status = p.Status;
			temp.ShippingSpeedMethod = p.ShippingSpeedMethod;
            await orderRepo.Update(temp);
            OrderListChanged?.Invoke();
            MainViewModel.IsLoading = false;
        }
		public Order GetOrder(string id) {
			foreach(Order p in OrderList) {
				if(p.ID == id) return p;
			}
			return null;
		}
    }
}
