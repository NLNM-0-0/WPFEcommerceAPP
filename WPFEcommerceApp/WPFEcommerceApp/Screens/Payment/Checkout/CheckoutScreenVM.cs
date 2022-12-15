using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
	public class CheckoutScreenVM : BaseViewModel {
		private readonly AccountStore _accountStore;
		private readonly Order _order;
		public List<Order> ListOrder { get; set; }
		public MUser CurrentUser => _accountStore?.CurrentAccount;

		#region props private
		private ObservableCollection<bool> stateArea;
		private bool paymentState;
		#endregion
		#region props
		public double SubTotal {
			get {
				if(ListOrder == null) return _order.SubTotal;
				double total = 0;
				for(int i = 0; i < ListOrder?.Count; i++) {
					total += ListOrder[i].SubTotal;
				}
				return total;
			}
		}
        public double ShipTotal {
            get {
                if(ListOrder == null) return _order.ShipTotal;
                double total = 0;
                for(int i = 0; i < ListOrder.Count; i++) {
                    total += ListOrder[i].ShipTotal;
                }
                return total;
            }
        }
        public double TotalPayment {
            get {
                if(ListOrder == null) return _order.OrderTotal;
                double total = 0;
                for(int i = 0; i < ListOrder.Count; i++) {
                    total += ListOrder[i].OrderTotal;
                }
                return total;
            }
        }
        public double Discount {
            get {
                if(ListOrder == null) return _order.Discount;
                double total = 0;
                for(int i = 0; i < ListOrder.Count; i++) {
                    total += ListOrder[i].Discount;
                }
                return total;
            }
        }

		public string UserName => CurrentUser?.Name;
        public string UserPhone => CurrentUser?.PhoneNumber;
        public string UserAddress => CurrentUser?.Address;
		
		public ObservableCollection<bool> StateArea {
			get { return stateArea; }
			set { stateArea = value; OnPropertyChanged(); }
		}

		public bool PaymentState {
			get { return paymentState; }
			set { paymentState = value; OnPropertyChanged(); }
		}

        #endregion
        #region command
        public ICommand OnPayment { get; set; }
		public ICommand PaymentAlertDialogCM { get; set; }
		public ICommand OnEditInfor { get; set; }
		public ICommand OnSuccessPayment { get; set; }
		public ICommand OnPaymentFieldChoice { get; set; }
        public ICommand OnDeliFieldChoice { get; set; }
        #endregion

        public CheckoutScreenVM(
			INavigationService successNavService, 
			AccountStore accountStore,
			OrderStore orderStore,
			Order order = null,
			List<Order> orders = null) {

			_accountStore = accountStore;
			_accountStore.AccountChanged += OnAccountChange;
			_order = order;
			if(orders != null)
				ListOrder = orders;
			else {
				ListOrder = new List<Order>();
				if(order != null)
					ListOrder.Add(order);
			}

			PaymentState = false;
			StateArea = new ObservableCollection<bool> {
				true,
				false,
			};
            //command
            OnPayment = new RelayCommand<object>((p) => { return true; }, (p) => {
                StateArea[0] = false;
                StateArea[1] = true;
                PaymentState = true;
            });
			OnDeliFieldChoice = new RelayCommand<object>(p => true, p => {
				if(PaymentState == false) StateArea[0] = true;
				else { StateArea[0] = true; StateArea[1] = false; PaymentState = false; }
			});

            PaymentAlertDialogCM = new RelayCommand<object>((p)=>true, (p)=>PaymentAlertDialog(order));
			OnEditInfor = new RelayCommand<object>((p) => true, (p) => EditInforDialog(p));
			OnSuccessPayment = new RelayCommand<object>((p) => true, async (p) => {
                //Do something with store here
                MainViewModel.IsLoading = true;

                var temp = new Order(order);
				temp.Status = "Processing";
				await orderStore.Add(temp);
				successNavService.Navigate();
                MainViewModel.IsLoading = false;

            });
        }

		public void OnAccountChange() {
			OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(UserPhone));
            OnPropertyChanged(nameof(UserAddress));
        }
        public override void Dispose() {
			_accountStore.AccountChanged -= OnAccountChange;
			base.Dispose();
		}

		private async void PaymentAlertDialog(object p) {
			var view = new ConfirmDialog() {
				CM = OnSuccessPayment,
				Param = p,
				Header = "Place-Order",
				Content = "Before you place your order, please make sure that your shipping information, billing information and bag summary are true. Are you sure to place your order?"
            };
			await DialogHost.Show(view, "Main");
        }
        private async void EditInforDialog(object p) {
            var view = new EditInforDialog(_accountStore) {
                Username = UserName,
				Phone = UserPhone,
				Address = UserAddress,
				EditData = this,
            };
            await DialogHost.Show(view, "Main");
        }
    }
}
