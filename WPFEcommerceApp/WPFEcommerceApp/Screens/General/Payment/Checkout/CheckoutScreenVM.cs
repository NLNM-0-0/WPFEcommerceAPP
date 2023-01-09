using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
	public class CheckoutScreenVM : BaseViewModel {
        private readonly GenericDataRepository<Address> addressRepo = new GenericDataRepository<Address>();
		private readonly AccountStore _accountStore;
		private readonly Order _order;
		public List<Order> ListOrder { get; set; }
		public MUser CurrentUser => _accountStore?.CurrentAccount;
        public Dictionary<string, Address> ListAddress { get; set; }

        #region props
        public Promo PromoChoosen { get; set; } = null;

        private Address _address;

        public Address address {
            get { return _address; }
            set { _address = value; OnPropertyChanged(); }
        }

        public Dictionary<string, Promo> VoucherMap { get; set; }
        public List<Promo> ListVoucher { get; set; }
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

        public int LeftColumnChoice { get; set; }

        public int LeftState { get; set; }

        #endregion
        #region command
        public ICommand OnChooseVoucher { get; set; }
		public ICommand PaymentAlertDialogCM { get; set; }
		public ICommand OnEditAddress { get; set; }
		public ICommand OnSuccessPayment { get; set; }
		public ICommand OnPaymentFieldChoice { get; set; }
        public ICommand OnDeliFieldChoice { get; set; }
		public ICommand OnEditOrder { get; set; }
        public ICommand OnLeftChange { get; set; }
        public ICommand TestFeature { get; set; }
        public ICommand OnOpenPayment { get; set; }
        #endregion

        public CheckoutScreenVM(
			Order order = null,
			List<Order> orders = null) {

            VoucherMap = new Dictionary<string, Promo>();
            ListVoucher = new List<Promo>();
            for(int i = 0; i < 9; i++) {
                ListVoucher.Add(new Promo() {
                    Name = $"Promo {i}",
                    DateEnd = DateTime.Now,
                });
            }
            ListAddress = new Dictionary<string, Address>();
            Task.Run(async () => await Load());
			_accountStore = AccountStore.instance;
			_accountStore.AccountChanged += OnAccountChange;
			_order = order;
			if(orders != null)
				ListOrder = orders;
			else {
				ListOrder = new List<Order>();
				if(order != null)
					ListOrder.Add(order);
			}
			LeftColumnChoice = 1;
            LeftState = 0;
            //command
            OnLeftChange = new RelayCommand<object>(p => true, p => {
                var t = Convert.ToInt32((string)p);
                LeftColumnChoice = t;
                if(LeftColumnChoice != 3) 
                    if(LeftState != 0)
                        LeftState = 1;
                OnPropertyChanged(nameof(LeftColumnChoice));
            });

            OnChooseVoucher = new ImmediateCommand<object>((p) => {
				LeftColumnChoice = 2;
                LeftState = 1;
            });
            OnOpenPayment = new ImmediateCommand<object>(p => {
                LeftColumnChoice = 3;
                LeftState = 2;
            });
            PaymentAlertDialogCM = new ImmediateCommand<object>((p)=> {
                if((bool)p == true) {
                    order.ShippingSpeedMethod = 0;
                }
                else order.ShippingSpeedMethod = 1;
                PaymentAlertDialog(order);
			});
            OnEditAddress = new ImmediateCommand<object>((p) => {
                var dl = new AddressChoiceDialog() {
                    DataContext = new AddressChoiceDialogVM() {
                        SelectedItem = address,
                        ListAddress = new ObservableCollection<Address>(ListAddress.Values.ToList()),
                        ChangeAddressHandle = new ImmediateCommand<object>(async o => {
                            address = o as Address;
                            try {
                                var t = ListAddress[address.Id];
                            } catch {
                                ListAddress[address.Id] = address;
                                await addressRepo.Add(address);
                            }
                        }),
                        AddAddressHandle = new ImmediateCommand<object>(o => {
                            ListAddress[(o as Address).Id] = o as Address;
                        })
                    }
                };
                DialogHost.Show(dl, "Main");
            });
			OnSuccessPayment = new ImmediateCommand<object>(async (p) => {
                //Do something with store here
                MainViewModel.IsLoading = true;

                var temp = new Order(order);
				temp.Status = "Processing";
				await OrderStore.instance.Add(temp);
				NavigationStore.instance.clearHistory();
				NavigationStore.instance.stackScreen.Add(new Tuple<INavigationService, object>(NavigateProvider.OrderScreen(), null));
                NavigateProvider.SuccessScreen().Navigate();
                MainViewModel.IsLoading = false;

            });
            #region Test Feature
            //TestFeature = new RelayCommand<object>(p => true, async (p) => {
            //	FileStream stream = new FileStream("H:\\Dev\\Projects\\WPFEcommerceAPP\\WPFEcommerceApp\\WPFEcommerceApp\\Assets\\Images\\SuccessBag.png", FileMode.Open, FileAccess.Read);
            //	System.Drawing.Image t = System.Drawing.Image.FromStream(stream);

            //	Bitmap img = (Bitmap)t;
            //             BitmapImage bmImg = new BitmapImage();

            //             using(MemoryStream memStream2 = new MemoryStream()) {
            //                 img.Save(memStream2, System.Drawing.Imaging.ImageFormat.Png);
            //                 memStream2.Position = 0;

            //                 bmImg.BeginInit();
            //                 bmImg.CacheOption = BitmapCacheOption.OnLoad;
            //                 bmImg.UriSource = null;
            //                 bmImg.StreamSource = memStream2;
            //                 bmImg.EndInit();
            //             }
            //	BitmapSource bm = bmImg;
            //	await FireStorageAPI.PushFromImage(bm, "Test", "TestImg");
            //});
            #endregion
            OnEditOrder = new ImmediateCommand<object>(p => {
				NavigateProvider.BagScreen().Navigate();
			});
        }

		public void OnAccountChange() {
			OnPropertyChanged(nameof(CurrentUser));
        }
        public override void Dispose() {
			_accountStore.AccountChanged -= OnAccountChange;
			base.Dispose();
		}

        public async Task Load() {
            MainViewModel.IsLoading = true;
            var list = await addressRepo.GetListAsync(d => d.IdUser == CurrentUser.Id);
            foreach(var o in list) {
                ListAddress.Add(o.Id, o);
            }
            App.Current.Dispatcher.Invoke(() => {
                OnPropertyChanged(nameof(ListAddress));
                try {
                    address = ListAddress[CurrentUser.DefaultAddress];
                } catch { address = null; }
            });
            MainViewModel.IsLoading = false;
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
    }
}
