using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class OrderDetailsVM :BaseViewModel {
		private readonly GenericDataRepository<Models.Product> productRepo = new GenericDataRepository<Models.Product>();
        private readonly GenericDataRepository<MUser> userRepo = new GenericDataRepository<MUser>();

        public Order OrderDetail { get; set; }
        public int Status =>
            OrderDetail == null 
			? 0 
			: OrderDetail.Status == "Processing"
			? 1 
			: OrderDetail.Status == "Delivering"
			? 2
			: OrderDetail.Status == "Delivered"
			? 3
			: OrderDetail.Status == "Cancelled"
			? 0 : 5;

		public ObservableCollection<bool> OrderStatus { get; set; }
		public ObservableCollection<Product> ProductList { get; set; }

		public ICommand OnReOrder { get; }
		public ICommand OnCancel { get; }
		public ICommand OnViewProduct { get; }
		public ICommand OnReviewProduct { get; }
		public ICommand OnVisitShop { get; }

		public OrderDetailsVM(Order order) {

			OrderDetail = order;

			ProductList = new ObservableCollection<Product>(OrderDetail.ProductList);

			OrderStatus = new ObservableCollection<bool>() {
				false, false, false, false, false
			};
			for(int i = 0; i < Status; i++) {
				OrderStatus[i] = true;
			}
			//OnReOrder = new ReOrderCM(navigationStore, successNavService);
			ICommand ReOrderCM = new ReOrderCM();
			OnReOrder = new ImmediateCommand<object>(async p => {
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
                        Content = "This shop has been banned!"
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


            ICommand CanCelCM = new ImmediateCommand<object>(async (p) => {
				MainViewModel.SetLoading(true);

                (p as Order).Status = "Cancelled";
                await OrderStore.instance.Update(p as Order);
                NavigateProvider.OrderScreen().Navigate();

				MainViewModel.SetLoading(false);

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

			OnViewProduct = new ImmediateCommand<object>( async p => {
				var t = await productRepo.GetSingleAsync(d => d.Id == (p as Product).ID, d => d.Category, d => d.ImageProducts, d => d.Brand, d => d.MUser);
				NavigateProvider.ProductDetailScreen().Navigate(t);
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

			OnVisitShop = new ImmediateCommand<object>(async p => {
				var u = await userRepo.GetSingleAsync(d => d.Id == OrderDetail.IDShop);
				NavigateProvider.ShopViewScreen().Navigate(u);
			});
        }

    }
}
