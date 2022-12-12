using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class OrderStore {
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
		public OrderStore(AccountStore account) {
			_accountStore = account;
			_accountStore.AccountChanged += OnAccountChange;
			Load();
		}

        private readonly GenericDataRepository<MOrder> orderRepo = new GenericDataRepository<MOrder>();
        private readonly GenericDataRepository<OrderInfo> orderInfoRepo = new GenericDataRepository<OrderInfo>();
        private readonly GenericDataRepository<MUser> userRepo = new GenericDataRepository<MUser>();

        private void OnAccountChange() {
			Load();
            OrderListChanged?.Invoke();
        }
		public async void Load() {
            if(user == null) return;
			OrderList?.Clear();
			OrderList = new ObservableCollection<Order>();

			var userTemp = await userRepo.GetSingleAsync(d => d.Id == user.Id, d => d.MOrders);
			List<MOrder> mOrders = userTemp.MOrders.ToList();

			List<List<OrderInfo>> listOrderInfor = new List<List<OrderInfo>>();
			List<List<BProduct>> listBProduct = new List<List<BProduct>>();

			foreach(var o in mOrders) {
                var oTemp = await orderRepo.GetSingleAsync(d => d.Id == o.Id, d => d.OrderInfoes);
                listOrderInfor.Add(oTemp.OrderInfoes.ToList());
            }

            foreach(var o in listOrderInfor) {
				Dictionary<string, List<Product>> prdtemp = new Dictionary<string, List<Product>>();
				Dictionary<MUser, int> shopref = new Dictionary<MUser, int>();
                foreach(var obj in o) {
					var objTemp = await orderInfoRepo.GetSingleAsync(d => d.IdProduct == obj.IdProduct && d.IdOrder == obj.IdOrder, d => d.Product, d => d.MUser);
                    WPFEcommerceApp.Models.Product product = new WPFEcommerceApp.Models.Product();
					product = objTemp.Product;
					product.MUser = objTemp.MUser;
					Product tmp = new Product(
							iD: obj.IdProduct,
							productImage: obj.ImageProduct,
							name: product.Name,
							size: obj.Size,
							color: product.Color,
							description: product.Description,
							price: product.Price,
							amount: obj.Count
						);
					if(!prdtemp.ContainsKey(product.MUser.Name))
						prdtemp[product.MUser.Name] = new List<Product>();
					prdtemp[product.MUser.Name].Add(tmp);
					if(!shopref.ContainsKey(product.MUser))
						shopref[product.MUser] = 0;
					shopref[product.MUser]++;
				};
				List<List<Product>> shoplisttemp = prdtemp.Values.ToList();
                List<MUser> shopRefList = shopref.Keys.ToList();

                List<BProduct> bProducts = new List<BProduct>();

				for(int i = 0; i < shoplisttemp.Count; i++) {
					BProduct bprdtemp = new BProduct(shoplisttemp[i], shopRefList[i].SourceImageAva, shopRefList[i].Name, shopRefList[i].Id);
					bProducts.Add(bprdtemp);
				}

				listBProduct.Add(bProducts);
			};
			
			for(int i = 0; i < listBProduct.Count; i++) {
				Order ordertemp = new Order(listBProduct[i], mOrders[i].Id, mOrders[i].Status, mOrders[i].IdCustomer, mOrders[i].IdRating);
				OrderList.Add(ordertemp);
			}
            OrderListChanged?.Invoke();
        }
        public async Task Add(Order p) {
			p.ID = await GenerateID.Gen(typeof(MOrder));
            OrderList.Add(p);
			MOrder temp = new MOrder();
			temp.Id = p.ID;
			temp.OrderTotal = (long)p.OrderTotal;
			temp.IdCustomer =p.IDCustomer;
			temp.Status = p.Status;
			temp.IdRating = p.IDrating;
			temp.DateBegin = DateTime.Now;
			temp.DateEnd = null;
			await orderRepo.Add(temp);
			List<BProduct> bProducts = p.ShopProduct;
			for(int i = 0; i < bProducts.Count; i++) {
				for(int j = 0; j < bProducts[i].ProductList.Count; j++) {
					OrderInfo t = new OrderInfo();
					t.IdOrder =p.ID;
					t.IdProduct = bProducts[i].ProductList[j].ID;
					t.ImageProduct = bProducts[i].ProductList[j].ProductImage;
					t.IdShop = bProducts[i].IDShop;
					t.Size = bProducts[i].ProductList[j].Size;
					t.TotalPrice = (long)bProducts[i].Subtotal;
					t.Count = bProducts[i].ProductList[j].Amount;
					await orderInfoRepo.Add(t);
                }
			}
            OrderListChanged?.Invoke();
        }
		public async Task Remove(Order p) {
            OrderList.Remove(p);
            MOrder temp = new MOrder();
            temp.OrderTotal = (long)p.OrderTotal;
            temp.IdCustomer = p.IDCustomer;
            temp.Status = p.Status;
            temp.IdRating =p.IDrating;
            temp.Id =p.ID;
            temp.DateBegin = DateTime.Now;
            temp.DateEnd = null;
            await orderRepo.Remove(temp);
            List<BProduct> bProducts = p.ShopProduct;
            for(int i = 0; i < bProducts.Count; i++) {
                for(int j = 0; j < bProducts[i].ProductList.Count; j++) {
                    OrderInfo t = new OrderInfo();
                    t.IdOrder = p.ID;
                    t.IdProduct =bProducts[i].ProductList[j].ID;
                    t.ImageProduct = bProducts[i].ProductList[j].ProductImage;
                    t.IdShop = bProducts[i].IDShop;
                    t.Size = bProducts[i].ProductList[j].Size;
                    t.TotalPrice = (long)bProducts[i].Subtotal;
                    t.Count = bProducts[i].ProductList[j].Amount;
                    await orderInfoRepo.Remove(t);
                }
            }
            OrderListChanged?.Invoke();
        }
        public async Task Update(Order p) {
			for(int i = 0; i < OrderList.Count; i++) {
				if(OrderList[i].ID == p.ID) OrderList[i] = p;
			}

            MOrder temp = new MOrder();
            temp.OrderTotal = (long)p.OrderTotal;
            temp.IdCustomer =p.IDCustomer;
            temp.Status = p.Status;
            temp.IdRating = p.IDrating;
            temp.Id = p.ID;
            temp.DateBegin = DateTime.Now;
            temp.DateEnd = null;
            await orderRepo.Update(temp);
            List<BProduct> bProducts = p.ShopProduct;
            for(int i = 0; i < bProducts.Count; i++) {
                for(int j = 0; j < bProducts[i].ProductList.Count; j++) {
                    OrderInfo t = new OrderInfo();
                    t.IdOrder = p.ID;
                    t.IdProduct = bProducts[i].ProductList[j].ID;
                    t.ImageProduct = bProducts[i].ProductList[j].ProductImage;
                    t.IdShop = bProducts[i].IDShop;
                    t.Size = bProducts[i].ProductList[j].Size;
                    t.TotalPrice = (long)bProducts[i].Subtotal;
                    t.Count = bProducts[i].ProductList[j].Amount;
                    await orderInfoRepo.Update(t);
                }
            }
            OrderListChanged?.Invoke();
        }
    }
}
