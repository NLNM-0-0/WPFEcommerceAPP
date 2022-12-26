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
        public ICommand OnBack { get; }
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
			OnReOrder = new RelayCommand<object>(p => true, async p => {
				var view = new ConfirmDialog() {
					CM = ReOrderCM,
					Param = p,
				};
				await DialogHost.Show(view, "Main");
			});


            ICommand CanCelCM = new RelayCommand<object>((p) => true, async (p) => {
				MainViewModel.IsLoading = true;

                (p as Order).Status = "Cancelled";
                await OrderStore.instance.Update(p as Order);
                NavigateProvider.OrderScreen().Navigate();

				MainViewModel.IsLoading = false;

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
			OnBack = new RelayCommand<object>(p => true, p => {
                //Actually I need to handle the tab index
                //But nahh, we'll do it later
                var param = OrderDetail.Status == "Processing"
							? 0
							: OrderDetail.Status == "Delivering"
							? 1
							: OrderDetail.Status == "Delivered"
							? 2
							: OrderDetail.Status == "Cancelled"
							? 3 : 2;
				NavigateProvider.OrderParamScreen().Navigate(param);
			});
			OnViewProduct = new RelayCommand<object>(p => true, async p => {
				var t = await productRepo.GetSingleAsync(d => d.Id == (p as Product).ID);
				NavigateProvider.ProductDetailScreen().Navigate(t);
            });
			OnReviewProduct = new RelayCommand<object>(p => true, async p => {
                var t = p as Order;
                List<ReviewProduct> products = new List<ReviewProduct>();
                for(int i = 0; i < t.ProductList.Count; i++) {
                        products.Add(new ReviewProduct(t.ProductList[i], t.ID));
                }
                var view = new ReviewProductDialog() {
                    ProductList = products,
                };
                await DialogHost.Show(view, "Main");
            });

			OnVisitShop = new RelayCommand<object>(p => true, async p => {
				var u = await userRepo.GetSingleAsync(d => d.Id == OrderDetail.IDShop);
				NavigateProvider.ShopViewScreen().Navigate(u);
			});
        }

    }
}
