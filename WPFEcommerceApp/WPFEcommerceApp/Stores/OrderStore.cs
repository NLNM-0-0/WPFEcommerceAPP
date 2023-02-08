using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        private List<Order> orderList;
        public List<Order> OrderList {
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

        #region Load
        public async Task Load() {
            OrderList?.Clear();
            Debug.WriteLine("Thread x");
            OrderList = new List<Order>();
            if(user == null) {
                MainViewModel.SetLoading(false);
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
                        d.IdProduct == oi.IdProduct && 
                        d.Size == oi.Size,
                        d => d.Product,
                        d => d.Rating
                    );
                    var productRepo = new GenericDataRepository<Models.Product>();
                    Models.Product product = await productRepo.GetSingleAsync(d => d.Id == productTemp.Product.Id, d => d.ImageProducts);

                    Product tmp = new Product(product, oi.Size, oi.Count);
                    //    ID = oi.IdProduct,
                    //    ProductImage = oi.ImageProduct,
                    //    Name = product.Name,
                    //    Size = oi.Size,
                    //    Color = product.Color,
                    //    Description = product.Description,
                    //    Price = product.Price,
                    //    Amount = oi.Count,
                    //    Subtotal = oi.Count * product.Price,
                    //    Discount = product.Sale
                    //};

                    listProduct.Add(tmp);
                }
                listOrderProduct.Add(listProduct);
            }

            for(int i = 0; i < mOrders.Count; i++) {
                var order = await orderRepo.GetSingleAsync(d => d.Id == mOrders[i].Id, d => d.MUser1);
                var t = order.ShippingSpeedMethod;
                double subtotal = 0;
                foreach(var prd in listOrderProduct[i]) {
                    subtotal += prd.Subtotal;
                }
                Order ordertemp = new Order() {
                    SubTotal = subtotal,
                    OrderTotal = order.OrderTotal,
                    Discount = order.Discounted != null ? (double)order.Discounted : 0,
                    ID = order.Id,
                    IDCustomer = order.IdCustomer,
                    IDShop = order.IdShop,
                    ProductList = listOrderProduct[i],
                    Status =order.Status,
                    ShipTotal = (double)order.ShipTotal,
                    DateBegin = (DateTime)order.DateBegin,
                    ShopName = order.MUser1.Name,
                    ShopImage = order.MUser1.SourceImageAva,
                    ShippingSpeedMethod = order.ShippingSpeedMethod,
                    DateEnd = order.DateEnd,
                };
                if(!OrderList.Contains(ordertemp))
                    OrderList.Add(ordertemp);
            }
            OrderListChanged?.Invoke();
        }
        #endregion

        #region Generator
        MOrder GenerateOrder(Order p) {
            MOrder temp = new MOrder();
            temp.Id = p.ID;
            temp.IdCustomer = p.IDCustomer;
            temp.IdShop = p.IDShop;
            temp.ShipTotal = p.ShipTotal;
            temp.Discounted = p.Discount;
            temp.DateBegin = p.DateBegin;
            temp.DateEnd = p.DateEnd;
            temp.Promo = p.Promo;
            temp.AddressIndex = p.AddressIndex;
            temp.OrderTotal = p.OrderTotal;
            temp.Status = p.Status;
            temp.ShippingSpeedMethod = p.ShippingSpeedMethod;
            return temp;
        }

        async Task genOrderInfor(Order p, string type) {
            List<Product> products = p.ProductList;
            for(int i = 0; i < products.Count; i++) {
                OrderInfo orderInfo = new OrderInfo() {
                    IdOrder = p.ID,
                    IdProduct = products[i].ID,
                    IdRating = null,
                    ImageProduct = products[i].ProductImage,
                    Size = products[i].Size,
                    Count = products[i].Amount,
                    TotalPrice = products[i].Subtotal,
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
        #endregion

        #region Api
        public async Task Add(Order p) {
            MainViewModel.SetLoading(true);
            p.ID = await GenerateID.Gen(typeof(MOrder));
            OrderList.Add(p);
            MOrder temp = GenerateOrder(p);
            await orderRepo.Add(temp);

            await genOrderInfor(p, "Add");
            MainViewModel.SetLoading(false);
        }
        public async Task Remove(Order p) {
            MainViewModel.SetLoading(true);
            OrderList.Remove(p);
            MOrder temp = GenerateOrder(p);
            await orderRepo.Remove(temp);

            await genOrderInfor(p, "Remove");
            MainViewModel.SetLoading(false);
        }
        public async Task Update(Order p) {
            MainViewModel.SetLoading(true);
            for(int i = 0; i < OrderList.Count; i++) {
                if(OrderList[i].ID == p.ID) OrderList[i] = p;
            }
            var temp = await orderRepo.GetSingleAsync(d => d.Id == p.ID);
            temp.IdCustomer = p.IDCustomer;
            temp.IdShop = p.IDShop;
            temp.ShipTotal = p.ShipTotal;
            temp.DateBegin = p.DateBegin;
            temp.DateEnd = p.DateEnd;
            temp.OrderTotal = p.OrderTotal;
            temp.Status = p.Status;
            temp.ShippingSpeedMethod = p.ShippingSpeedMethod;
            await orderRepo.Update(temp);
            OrderListChanged?.Invoke();
            MainViewModel.SetLoading(false);
        }
        public Order GetOrder(string id) {
            foreach(Order p in OrderList) {
                if(p.ID == id) return p;
            }
            return null;
        }
        #endregion
    }
}
