using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using System.Windows.Navigation;
using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
	public class CheckoutScreenVM : BaseViewModel {
        private readonly GenericDataRepository<Address> addressRepo = new GenericDataRepository<Address>();
        private readonly GenericDataRepository<Promo> promoRepo = new GenericDataRepository<Promo>();

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

        public string _vouchercode;
        public string VoucherCode {
            get => _vouchercode; set {
                _vouchercode = value;
                if(IsVoucherError)
                    IsVoucherError = false;
            }
        }
        public bool IsVoucherError { get; set; }
        public int LeftColumnChoice { get; set; }

        public int LeftState { get; set; }

        #endregion
        #region command
        public ICommand OnChooseVoucher { get; }
		public ICommand PaymentAlertDialogCM { get; }
		public ICommand OnEditAddress { get; }
		public ICommand OnSuccessPayment { get; }
		public ICommand OnPaymentFieldChoice { get; }
        public ICommand OnDeliFieldChoice { get; }
		public ICommand OnEditOrder { get; }
        public ICommand OnLeftChange { get; }
        public ICommand TestFeature { get; }
        public ICommand OnOpenPayment { get; }
        public ICommand OnViewConditionVoucher { get; }
        public ICommand OnApplyVoucher { get; }
        public ICommand OnCloseErrorAlert { get; }
        #endregion

        public CheckoutScreenVM(
			Order order = null,
			List<Order> orders = null) {

            ListVoucher = new List<Promo>();
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

            OnChooseVoucher = new RelayCommand<object>(p => {
                return address != null;
            },(p) => {
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
                order.Promo = PromoChoosen?.Id;
                order.AddressIndex = address.Id;
                PaymentAlertDialog(order);
			});
            OnEditAddress = new ImmediateCommand<object>((p) => {
                var dl = new AddressChoiceDialog() {
                    DataContext = new AddressChoiceDialogVM() {
                        SelectedItem = address,
                        ListAddress = new ObservableCollection<Address>(ListAddress.Values.ToList()),
                        ChangeAddressHandle = new ImmediateCommand<object>(async o => {
                            address = o as Address;
                            OnPropertyChanged(nameof(address));
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
            OnViewConditionVoucher = new ImmediateCommand<object>(p => {
                var dl = new VoucherConditionDialog() {
                    Data = p as Promo
                };
                DialogHost.Show(dl, "Main");
            });
            OnApplyVoucher = new RelayCommand<object>(p => !string.IsNullOrEmpty(p as string), p => {
                bool flag = false;
                foreach(var o in ListVoucher) {
                    if(o.Code == (p as string).ToUpper()) {
                        PromoChoosen = o;
                        flag = true;
                        break;
                    }
                }
                if(!flag) IsVoucherError = true;
            });
            OnCloseErrorAlert = new ImmediateCommand<object>(p => {
                IsVoucherError = false;
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
            var listVoucher = await promoRepo.GetListAsync(d => d.DateEnd > DateTime.Now);

            foreach(var o in listVoucher) {
                ListVoucher.Add(o);
            }

            foreach(var o in list) {
                ListAddress.Add(o.Id, o);
            }

            App.Current.Dispatcher.Invoke(() => {
                OnPropertyChanged(nameof(ListVoucher));
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
