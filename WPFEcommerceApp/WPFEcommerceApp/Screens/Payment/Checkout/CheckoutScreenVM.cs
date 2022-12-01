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

namespace WPFEcommerceApp {
    public class CheckoutScreenVM : BaseViewModel {
        #region props private
        private double subtotal;
        private double shipTotal;
        private double totalPayment;
        private double discount;
		private ObservableCollection<bool> stateArea;
        private ObservableCollection<BProduct> productList;
        private bool paymentState;
		private string userName;
		private string userPhone;
		private string userAddress;
		#endregion
		#region props
		public double SubTotal {
			get { return subtotal; }
			set { subtotal = value; OnPropertyChanged(); }
		}

		public double ShipTotal {
			get { return shipTotal; }
			set { shipTotal = value; OnPropertyChanged(); }
		}

		public double TotalPayment {
			get { return totalPayment; }
			set { totalPayment = value; OnPropertyChanged(); }
		}

		public double Discount {
			get { return discount; }
			set { discount = value; OnPropertyChanged(); }
		}
        public ObservableCollection<BProduct> ProductList {
            get { return productList; }
            set { productList = value; OnPropertyChanged(); }
        }
		public ObservableCollection<bool> StateArea {
			get { return stateArea; }
			set { stateArea = value; OnPropertyChanged(); }
		}

		public bool PaymentState {
			get { return paymentState; }
			set { paymentState = value; OnPropertyChanged(); }
		}
        public string UserAddress {
            get { return userAddress; }
            set { userAddress = value; OnPropertyChanged(); }
        }
        public string UserPhone {
            get { return userPhone; }
            set { userPhone = value; OnPropertyChanged(); }
        }
        public string Username {
            get { return userName; }
            set { userName = value; OnPropertyChanged(); }
        }
        #endregion
        #region command
        public ICommand OnPayment { get; set; }
		public ICommand PaymentAlertDialogCM { get; set; }
		public ICommand OnEditInfor { get; set; }
        
        #endregion

        public CheckoutScreenVM() {
			Username = "Name";
			UserPhone = "012345678";
			UserAddress = "Nothing";
            ProductList= new ObservableCollection<BProduct>() {
                new BProduct(),
                new BProduct(),
                new BProduct(),
                new BProduct(),
            };
			ShipTotal = 23.89;
			Discount = 5;
			for(int i = 0; i < ProductList.Count; i++) {
				SubTotal += ProductList[i].Subtotal;
			}
			TotalPayment = (SubTotal + ShipTotal) * (100 - discount) / 100;
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
			PaymentAlertDialogCM = new RelayCommand<object>((p)=>true, (p)=>PaymentAlertDialog(p));
			OnEditInfor = new RelayCommand<object>((p) => true, (p) => EditInforDialog(p));
        }

		private async void PaymentAlertDialog(object p) {
			var view = new EnterPaymentDialog() {
				DataContext = this
			};
			//var result = await DialogHost.Show(view, "RootDialog", null, ClosingEventHandler, ClosedEventHandler);
			await DialogHost.Show(view);
        }
        private async void EditInforDialog(object p) {
            var view = new EditInforDialog() {
                Username = Username,
				Phone = UserPhone,
				Address = UserAddress,
				EditData = this,
            };
            //var result = await DialogHost.Show(view, "RootDialog", null, ClosingEventHandler, ClosedEventHandler);
            await DialogHost.Show(view);
        }
    }
}
