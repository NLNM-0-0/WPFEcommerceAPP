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
        private readonly GenericDataRepository<Cart> cartRepo = new GenericDataRepository<Cart>();

        private readonly AccountStore _accountStore;
        public List<Order> ListOrder { get; set; }
        public MUser CurrentUser => _accountStore?.CurrentAccount;
        public Dictionary<string, Address> ListAddress { get; set; }

        #region props
        public Promo PromoChoosen { get; set; } = null;
        public Promo PrevPromo { get; set; } = null;

        private Address _address;
        public Address address {
            get { return _address; }
            set { _address = value; OnPropertyChanged(); }
        }

        public List<Promo> ListVoucher { get; set; }
        private bool shippingMethod;
        public bool ShippingMethod {
            get => shippingMethod;
            set {
                shippingMethod = value;
                if(value == false) ShipTotal = ListOrder.Count * 2.5;
                else ShipTotal = 0;
                OnPropertyChanged();
            }
        }
        private double subTotal;
        public double SubTotal {
            get { return subTotal; }
            set { subTotal = value; OnPropertyChanged(); }
        }

        private double shipTotal;

        public double ShipTotal {
            get { return shipTotal; }
            set {
                shipTotal = value;
                OnPropertyChanged();
            }
        }

        public double TotalPayment {
            get { return ShipTotal + SubTotal - Discount; }
        }

        public double ProductDiscount{get; set;}

        public double Discount {
            get {
                if(PromoChoosen == null) return ProductDiscount;
                Nullable<double> promoSale = SubTotal* PromoChoosen.Sale / 100 > PromoChoosen.MaxSale 
                    ? PromoChoosen.MaxSale
                    : SubTotal* PromoChoosen.Sale / 100;
                var sale = promoSale + ProductDiscount;
                if(sale > SubTotal) sale = SubTotal;
                return (double) sale;
            }
        }


        public string _vouchercode;
        public string VoucherCode {
            get => _vouchercode; set {
                _vouchercode = value;
                if(IsVoucherError) {
                    IsVoucherError = false;
                    VoucherErrorMessage = "No voucher found";
                }
            }
        }
        public bool IsVoucherError { get; set; }
        public int LeftColumnChoice { get; set; }
        public int LeftState { get; set; }
        public string VoucherErrorMessage { get; set; } = "No voucher found";

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
        public ICommand CheckVoucher { get; }
        #endregion

        #region Constructor
        public CheckoutScreenVM(List<Order> orders = null) {
            //var order = param.Item1;
            //var orders = param.Item2;

            ListAddress = new Dictionary<string, Address>();
            ShippingMethod = true;
            Task.Run(async () => await Load());

			_accountStore = AccountStore.instance;
			_accountStore.AccountChanged += OnAccountChange;
			//_order = param.Item1;

			if(orders != null)
				ListOrder = orders;

            SubTotal = 0;
            ProductDiscount = 0;
            for(int i = 0; i < ListOrder?.Count; i++) {
                SubTotal += ListOrder[i].SubTotal;
                ProductDiscount += ListOrder[i].Discount;
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
                    foreach(var order in orders) {
                        order.ShippingSpeedMethod = 0;
                    }
                }
                else {
                    foreach(var order in orders)
                        order.ShippingSpeedMethod = 1;
                }
                foreach(var order in orders) {
                    order.Promo = PromoChoosen?.Id;
                    order.Discount = Discount;
                    order.ShipTotal = ShipTotal;
                    order.OrderTotal = TotalPayment;
                    order.SubTotal = SubTotal;
                    order.AddressIndex = address.Id;
                }
                PaymentAlertDialog(orders);
            });
            OnEditAddress = new ImmediateCommand<object>((p) => {
                var listAddress = ListAddress.Values.ToList();
                if(listAddress.Count != 0 && address != null && address.Id != listAddress[0].Id)
                    for(int i = 0; i < listAddress.Count; i++) {
                        if(listAddress[i].Id == address.Id) {
                            listAddress.RemoveAt(i);
                            listAddress.Insert(0, address);
                        }
                    }
                var dl = new AddressChoiceDialog() {
                    DataContext = new AddressChoiceDialogVM() {
                        SelectedItem = address,
                        ListAddress = new ObservableCollection<Address>(listAddress),
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
                MainViewModel.SetLoading(true);

                foreach(var order in orders) {
                    var temp = new Order(order);
                    temp.Status = "Processing";
                    await OrderStore.instance.Add(temp);
                    await Task.Run(async () => {
                        foreach(var item in order.ProductList) {
                            var t = await cartRepo.GetSingleAsync(d =>
                                d.IdProduct == item.ID &&
                                d.IdUser == order.IDCustomer &&
                                d.Size == item.Size);
                            if(t != null) await cartRepo.Remove(t);
                        }
                    }).ConfigureAwait(false);
                }
				NavigationStore.instance.clearHistory();
				NavigationStore.instance.stackScreen.Add(new Tuple<INavigationService, object>(NavigateProvider.OrderScreen(), null));
                NavigateProvider.SuccessScreen().Navigate();
                MainViewModel.SetLoading(false);

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

            #region Voucher Command
            OnViewConditionVoucher = new ImmediateCommand<object>(p => {
                var x = p as Promo;
                var dl = new VoucherConditionDialog() {
                    Data = x
                };
                DialogHost.Show(dl, "Main");
            });
            OnApplyVoucher = new RelayCommand<object>(p => !string.IsNullOrEmpty(p as string), p => {
                bool flag = false;
                foreach(var o in ListVoucher) {
                    if(o.Code == (p as string).ToUpper()) {
                        if(!ValidVoucherList.ContainsKey(o.Id)) {
                            flag = false;
                            VoucherErrorMessage = "Not meet conditions";
                            break;
                        }
                        PromoChoosen = o;
                        flag = true;
                        break;
                    }
                }
                if(!flag) IsVoucherError = true;
            });
            OnCloseErrorAlert = new ImmediateCommand<object>(p => {
                IsVoucherError = false;
                VoucherErrorMessage = "No voucher found";
            });
            CheckVoucher = new ImmediateCommand<object>(p => {
                if(PromoChoosen == null) {
                    return;
                }
                if(!ValidVoucherList.ContainsKey(PromoChoosen.Id)) {
                    PromoChoosen = PrevPromo;
                }
                PrevPromo = PromoChoosen;
            });
            #endregion
        }
        #endregion
        public void OnAccountChange() {
			OnPropertyChanged(nameof(CurrentUser));
        }
        public override void Dispose() {
			_accountStore.AccountChanged -= OnAccountChange;
			base.Dispose();
		}

        public List<string> productList = new List<string>();
        public Dictionary<string, bool> ValidVoucherList = new Dictionary<string, bool>();
        public async Task Load() {
            MainViewModel.SetLoading(true, haveTimeout: false);

            var list = await addressRepo.GetListAsync(d => d.IdUser == CurrentUser.Id && d.Status == true);
            foreach(var o in list) {
                ListAddress.Add(o.Id, o);
            }

            ListVoucher = new List<Promo>();
            var valid = new List<Promo>();
            var invalid = new List<Promo>();

            var listShop = ListOrder.Select(s => s.IDShop).ToList();

            var listVoucher = await promoRepo.GetListAsync(
                d => d.DateEnd > DateTime.Now &&
                d.Status == 1 &&
                listShop.Contains(d.IdShop),
                d => d.Products
            );

            if(productList.Count < 1) {
                foreach(var order in ListOrder) {
                    foreach(var product in order.ProductList) {
                        productList.Add(product.ID);
                    }
                }
            }

            foreach(var o in listVoucher) {
                var flag = true;
                //if(o.Products.Count == 0) flag = false;
                //if(CurrentUser)
                o.DateEnd = o.DateEnd.Value.Subtract(new TimeSpan(12, 0, 0));
                o.DateBegin = o.DateBegin.Value.Add(new TimeSpan(12, 0, 0));
                if(o.Products.Count < productList.Count || 
                    o.Products.Count == 0 ||
                    (o.CustomerType == 0 && DateTime.Now - CurrentUser.UserLogin.CreatedDate > TimeSpan.FromDays(7)) ||
                    o.MinCost > SubTotal) {
                    flag = false;
                }
                else {
                    List<string> listVoucherId = o.Products.Select(d => d.Id).ToList();
                    flag = !productList.Except(listVoucherId).Any();
                }

                if(flag) {
                    valid.Add(o);
                    ValidVoucherList.Add(o.Id, true);
                }
                else invalid.Add(o);
            }
            valid.Sort(new VoucherCompare());
            invalid.Sort(new VoucherCompare());
            ListVoucher = valid.Concat(invalid).ToList();

            App.Current.Dispatcher.Invoke(() => {
                OnPropertyChanged(nameof(ListVoucher));
                OnPropertyChanged(nameof(ListAddress));
                try {
                    address = ListAddress[CurrentUser.DefaultAddress];
                } catch { address = null; }
            });
            MainViewModel.SetLoading(false, haveTimeout: false);
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
    class VoucherCompare : IComparer<Promo> {
        public int Compare(Promo x, Promo y) {
            double t1 = (double) x.Sale;
            double t2 = (double) y.Sale;
            return t1.CompareTo(t2);
        }
    }
}
