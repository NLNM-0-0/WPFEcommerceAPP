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
		public MUser user => _accountStore.CurrentAccount;

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
            if(user == null) return;
			OrderList?.Clear();

			OrderList = new ObservableCollection<Order>();

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
					var productTemp = await orderInfoRepo.GetSingleAsync(d => d.IdOrder == oi.IdOrder && d.IdProduct == oi.IdProduct, d => d.Product, d => d.Rating);
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
				Order ordertemp = new Order(
						order.Id,
						order.IdCustomer,
						order.IdShop,
						order.Status,
						(double)order.ShipTotal,
						listOrderProduct[i],
						(DateTime)order.DateBegin,
						order.MUser1.Name,
						order.MUser1.SourceImageAva
					) {
				};
				OrderList.Add(ordertemp);
			}
            OrderListChanged?.Invoke();
        }
        public async Task Add(Order p) {
			p.ID = await GenerateID.Gen(typeof(MOrder));
            OrderList.Add(p);
			MOrder temp = new MOrder();
			temp.Id = p.ID;
			temp.IdCustomer = p.IDCustomer;
			temp.IdShop = p.IDShop;
			temp.ShipTotal = (int) p.ShipTotal;
			temp.DateBegin = p.DateBegin;
			temp.DateEnd = p.DateEnd;
			temp.OrderTotal = (int) p.OrderTotal;
			temp.Status = p.Status;
			await orderRepo.Add(temp);

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
				await orderInfoRepo.Add(orderInfo);
			}
            OrderListChanged?.Invoke();
        }
		public async Task Remove(Order p) {
            OrderList.Remove(p);
            MOrder temp = new MOrder();
            temp.Id = p.ID;
            temp.IdCustomer = p.IDCustomer;
            temp.IdShop = p.IDShop;
            temp.ShipTotal = (int)p.ShipTotal;
            temp.DateBegin = p.DateBegin;
            temp.DateEnd = p.DateEnd;
            temp.OrderTotal = (int)p.OrderTotal;
            temp.Status = p.Status;
            await orderRepo.Remove(temp);

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
                await orderInfoRepo.Remove(orderInfo);
            }
            OrderListChanged?.Invoke();
        }
        public async Task Update(Order p) {
			for(int i = 0; i < OrderList.Count; i++) {
				if(OrderList[i].ID == p.ID) OrderList[i] = p;
			}
            MOrder temp = new MOrder();
            temp.Id = p.ID;
            temp.IdCustomer = p.IDCustomer;
            temp.IdShop = p.IDShop;
            temp.ShipTotal = (int)p.ShipTotal;
            temp.DateBegin = p.DateBegin;
            temp.DateEnd = p.DateEnd;
            temp.OrderTotal = (int)p.OrderTotal;
            temp.Status = p.Status;
            await orderRepo.Update(temp);

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
                await orderInfoRepo.Update(orderInfo);
            }
            OrderListChanged?.Invoke();
        }
    }
}
