using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AddShopPromoViewModel: BaseViewModel
    {
        public ICommand AddNewProductCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand DoubleClickCommand { get; set; } = new RelayCommandWithNoParameter(() => { });

        private string searchByValue;
        public string SearchByValue
        {
            get => searchByValue;
            set
            {
                searchByValue = value;
                OnPropertyChanged();    
            }
        }
        private bool isMaxSale;
        public bool IsMaxSale
        {
            get
            {
                return isMaxSale;
            }
            set
            {
                isMaxSale = value;
                if(!value)
                {
                    MaxSale = "0";
                }    
                OnPropertyChanged();
            }
        }
        private bool isLimitedAmount;
        public bool IsLimitedAmount
        {
            get
            {
                return isLimitedAmount;
            }
            set
            {
                isLimitedAmount = value;
                if (!value)
                {
                    Amount = "1";
                }
                OnPropertyChanged();
            }
        }
        private ObservableCollection<PromoProductBlockViewModel> selectedProductPromos;
        public ObservableCollection<PromoProductBlockViewModel> SelectedProductPromos
        {
            get => selectedProductPromos;
            set
            {
                selectedProductPromos = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<PromoProductBlockViewModel> filterProductPromos;
        public ObservableCollection<PromoProductBlockViewModel> FilterProductPromos
        {
            get => filterProductPromos;
            set
            {
                filterProductPromos = value;
                OnPropertyChanged();
            }
        }
        private string searchBy;
        public string SearchBy
        {
            get => searchBy;
            set
            {
                searchBy = value;
                OnPropertyChanged();
            }
        }
        private bool isAdmin;
        public bool IsAdmin
        {
            get { return isAdmin; }

            set
            {
                isAdmin = value;
                OnPropertyChanged();
            }
        }
        private bool isNewCustomer;
        public bool IsNewCustomer
        {
            get => isNewCustomer;
            set
            {
                isNewCustomer = value;
                OnPropertyChanged();
            }
        }
        private bool isAllCustomer;
        public bool IsAllCustomer
        {
            get => isAllCustomer;
            set
            {
                isAllCustomer = value;
                OnPropertyChanged();
            }
        }
        private string code;
        public string Code
        {
            get => code;
            set
            {
                code = value;
                OnPropertyChanged();
            }    
        }
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        private DateTime? dateBegin;
        public DateTime? DateBegin
        {
            get => dateBegin;
            set
            {
                dateBegin = value;
                OnPropertyChanged();
            }
        }
        private DateTime? dateEnd;
        public DateTime? DateEnd
        {
            get => dateEnd;
            set
            {
                dateEnd = value;
                OnPropertyChanged();
            }
        }
        private string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }
        private string amount;
        public string Amount
        {
            get => amount;
            set
            {
                amount = value;
                OnPropertyChanged();
            }
        }
        private string sale;
        public string Sale
        {
            get => sale;
            set
            {
                sale = value;
                OnPropertyChanged();
            }
        }
        private string minCost;
        public string MinCost
        {
            get => minCost;
            set
            {
                minCost = value;
                OnPropertyChanged();
            }
        }
        private string maxSale;
        public string MaxSale
        {
            get => maxSale;
            set
            {
                maxSale = value;
                OnPropertyChanged();
            }
        }
        public AddShopPromoViewModel()
        {
            Task.Run(() =>
            {
            }).ContinueWith((first) =>
            {
                MainViewModel.SetLoading(true);
                IsAdmin = isAdmin;
                IsMaxSale = false;
                SelectedProductPromos = new ObservableCollection<PromoProductBlockViewModel>();
                FilterProductPromos = SelectedProductPromos;
                SearchBy = "Id";
                IsNewCustomer = true;
                IsAllCustomer = false;
                Amount = "1";
                MaxSale = "0";
                AddNewProductCommand = new RelayCommandWithNoParameter( async() =>
                {
                    MainViewModel.SetLoading(true);
                    AddNewProductPromo addNewProductPromo = new AddNewProductPromo();
                    addNewProductPromo.DataContext = new AddNewProductPromoViewModel(SelectedProductPromos);
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(addNewProductPromo, "Main", null, null, LoadList);
                });
                SaveCommand = new RelayCommand<object>((p) =>
                {
                    return !String.IsNullOrEmpty(Code) &&
                            !String.IsNullOrEmpty(Name) &&
                            DateBegin != null &&
                            DateEnd != null &&
                            !String.IsNullOrEmpty(Description) &&
                            (String.IsNullOrEmpty(Amount)?1:int.Parse(Amount)) > 0 &&
                            (String.IsNullOrEmpty(Sale)?0:int.Parse(Sale)) > 0 &&
                            (String.IsNullOrEmpty(MinCost) ? 0 : double.Parse(MinCost)) >= 0 &&
                            (String.IsNullOrEmpty(MaxSale) ? 0 : double.Parse(MaxSale)) >= 0 &&
                            (DateBegin <= DateEnd);
                },async (p) =>
                {
                    if (DateEnd.Value.Date < DateTime.Now.Date)
                    {
                        MainViewModel.SetLoading(true);
                        ConfirmDialog confirmDialog = new ConfirmDialog()
                        {
                            Header = "Oops",
                            Content = "Please input Date End of promo bigger than Today"
                        };
                        MainViewModel.SetLoading(false);
                        await DialogHost.Show(confirmDialog, "Main");
                    }
                    else
                    {
                        MainViewModel.SetLoading(true);
                        await SaveProduct();
                        MainViewModel.SetLoading(false);
                    }
                });
                DeleteProductCommand = new RelayCommand<object>((p) => p != null, (p) =>
                {
                    PromoProductBlockViewModel promoProductBlockViewModel = p as PromoProductBlockViewModel;
                    SelectedProductPromos.Remove(promoProductBlockViewModel);
                    FilterProductPromos.Remove(promoProductBlockViewModel);
                });
                SearchCommand = new RelayCommandWithNoParameter(() =>
                {
                    MainViewModel.SetLoading(true);
                    Task.Run(() => { }).ContinueWith((second) =>
                    {
                        Search();
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            FilterProductPromos = new ObservableCollection<PromoProductBlockViewModel>(FilterProductPromos);
                        }));
                        MainViewModel.SetLoading(false);
                    });
                });
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
                MainViewModel.SetLoading(false);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private async Task SaveProduct()
        {
            GenericDataRepository<Models.Promo> promoRepository = new GenericDataRepository<Promo>();
            Models.Promo promo = await promoRepository.GetSingleAsync(p=> (p.IdShop == AccountStore.instance.CurrentAccount.Id && p.Code.Trim() == Code.Trim() && (p.DateEnd >= DateTime.Now)));
            NotificationDialog notification = new NotificationDialog();
            if (promo != null)
            {
                notification = new NotificationDialog();
                notification.Header = "Oops";
                notification.ContentDialog = "This promo is already in process or in our promo request list. Please check again.";
                MainViewModel.SetLoading(false);
                await DialogHost.Show(notification, "Main");
                return;
            }
            string id = await GenerateID.Gen(typeof(Models.Promo));
            await promoRepository.Add(new Models.Promo()
            {
                Id = id,
                IdShop = AccountStore.instance.CurrentAccount.Id,
                Code = this.Code.Trim(),
                Description = this.Description.Trim(),
                DateBegin = this.DateBegin.Value.Subtract(new TimeSpan(12, 0, 0)),
                DateEnd = this.DateEnd.Value.Add(new TimeSpan(12, 0, 0)),
                Amount = (IsLimitedAmount ? int.Parse(this.Amount) : -1),
                AmountUsed = 0,
                MaxSale = (IsMaxSale ? double.Parse(this.MaxSale) : double.MaxValue),
                MinCost = double.Parse(this.MinCost),
                Sale = double.Parse(this.Sale),
                CustomerType = IsNewCustomer ? 0 : 1,
                Status = 0,
                Name = this.Name
            }) ;
            foreach (PromoProductBlockViewModel promoProductBlock in SelectedProductPromos)
            {
                await PromoDetailAPI.Add(id, promoProductBlock.SelectedProduct.Id);   
            }
            notification.Header = "Notification";
            notification.ContentDialog = "This promo is requested successfully. Please wait for us to accept.";
            MainViewModel.SetLoading(false);
            await DialogHost.Show(notification, "Main");
            NavigateProvider.ShopPromoScreen().Navigate();
        }
        private void LoadList(object sender, DialogClosedEventArgs eventArgs)
        {
            if (eventArgs == null || eventArgs.Parameter == null || eventArgs.Parameter.GetType() != typeof(ObservableCollection<PromoProductBlockViewModel>))
            {
                return;
            } 
            else
            {
                ObservableCollection<PromoProductBlockViewModel> result = eventArgs.Parameter as ObservableCollection<PromoProductBlockViewModel>;
                SelectedProductPromos = result;
                foreach (PromoProductBlockViewModel product in SelectedProductPromos)
                {
                    product.DeleteCommand = DeleteProductCommand;
                }
                Search();
            }       
        }
        private void Search()
        {
            if(SearchBy == "Id")
            {
                FilterProductPromos = new ObservableCollection<PromoProductBlockViewModel>(SelectedProductPromos.Where(p => p.SelectedProduct.Id.ToLower().Trim().Contains(SearchByValue==null?"":SearchByValue.ToLower().Trim())));
            }    
            else if(SearchBy == "Product Name")
            {
                FilterProductPromos = new ObservableCollection<PromoProductBlockViewModel>(SelectedProductPromos.Where(p => p.SelectedProduct.Name.ToLower().Trim().Contains(SearchByValue==null?"": SearchByValue.ToLower().Trim())));
            }    
        }
    }
}
