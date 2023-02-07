using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class PromoInformationViewModel : BaseViewModel
    {
        public ICommand AddNewProductCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public ICommand AcceptCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
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
                if (!isMaxSale)
                {
                    SelectedPromo.MaxSale = 0;
                    OnPropertyChanged(nameof(SelectedPromo));
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
                if (!isLimitedAmount)
                {
                    SelectedPromo.Amount = 1;
                    OnPropertyChanged(nameof(SelectedPromo));
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
        private Models.Promo selectedPromo;
        public Models.Promo SelectedPromo
        {
            get { return selectedPromo; }
            set
            {
                selectedPromo = value;
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
        private Models.Promo initialPromo;
        public PromoInformationViewModel(PromoVMConstructor param)
        {
            IsLoadingCheck.IsLoading = 2;
            initialPromo = param.promo;
            Task.Run(async () =>
            {
                await Load();
                SelectedPromo.DateBegin = SelectedPromo.DateBegin.Value.Add(new TimeSpan(12, 0, 0));
                SelectedPromo.DateEnd = SelectedPromo.DateEnd.Value.Subtract(new TimeSpan(12, 0, 0));
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
                MainViewModel.SetLoading(false);
            }).ContinueWith((first) =>
            {
                IsNewCustomer = (SelectedPromo.CustomerType == 0);
                IsAllCustomer = (SelectedPromo.CustomerType == 1);  
                AddNewProductCommand = new RelayCommandWithNoParameter(async() =>
                {
                    MainViewModel.SetLoading(true);
                    AddNewProductPromo addNewProductPromo = new AddNewProductPromo();
                    addNewProductPromo.DataContext = new AddNewProductPromoViewModel(SelectedProductPromos, SelectedPromo.MUser);
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(addNewProductPromo, "Main", null, null, LoadList);
                });
                SaveCommand = new RelayCommand<object>((p) =>
                {
                    return !String.IsNullOrEmpty(SelectedPromo.Code) &&
                            !String.IsNullOrEmpty(SelectedPromo.Name) &&
                            SelectedPromo.DateBegin != null &&
                            SelectedPromo.DateEnd != null &&
                            !String.IsNullOrEmpty(SelectedPromo.Description) &&
                            (SelectedPromo.Amount == null || SelectedPromo.Amount == -1 || SelectedPromo.Amount > 0) &&
                            SelectedPromo.Sale > 0 &&
                            SelectedPromo.MinCost >= 0 &&
                            SelectedPromo.MaxSale >= 0 &&
                            (SelectedPromo.DateBegin <= SelectedPromo.DateEnd);
                }, async (p) =>
                {
                    if (SelectedPromo.DateEnd.Value.Date < DateTime.Now.Date)
                    {
                        MainViewModel.SetLoading(true);
                        ConfirmDialog confirmDialog = new ConfirmDialog()
                        {
                            Header = "Oops",
                            Content = "Please input DateEnd of promo bigger than Today"
                        };
                        MainViewModel.SetLoading(false);
                        await DialogHost.Show(confirmDialog, "Main");
                    }
                    else
                    {
                        MainViewModel.SetLoading(true);
                        await SaveProduct();
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
                    Search();
                });
                AcceptCommand = new RelayCommandWithNoParameter(async() => 
                {
                    MainViewModel.SetLoading(true);
                    await AcceptPromo();
                    MainViewModel.SetLoading(false);
                });
                DeleteCommand = new RelayCommandWithNoParameter(async() => 
                {
                    MainViewModel.SetLoading(true);
                    await DeletePromo();
                    MainViewModel.SetLoading(false);
                });
                IsAdmin = param.isAdmin;
                if (SelectedPromo.MaxSale == null || SelectedPromo.MaxSale == double.MaxValue)
                {
                    IsMaxSale = false;
                }
                else
                {
                    IsMaxSale = true;
                }
                if (SelectedPromo.Amount == -1)
                {
                    IsLimitedAmount = false;
                }
                else
                {
                    IsLimitedAmount = true;
                }
                SelectedProductPromos = new ObservableCollection<PromoProductBlockViewModel>();
                foreach (Models.Product product in param.promo.Products)
                {
                    PromoProductBlockViewModel promoProductBlockViewModel = new PromoProductBlockViewModel(product);
                    promoProductBlockViewModel.DeleteCommand = DeleteProductCommand; 
                    promoProductBlockViewModel.IsAdmin = IsAdmin;
                    SelectedProductPromos.Add(promoProductBlockViewModel);
                }
                FilterProductPromos = SelectedProductPromos;
                SearchBy = "Id";
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
        private async Task AcceptPromo()
        {
            GenericDataRepository<Models.Promo> promoRepository = new GenericDataRepository<Promo>();
            Models.Promo promo = await promoRepository.GetSingleAsync(p => p.Id == SelectedPromo.Id);
            promo.Status = 1;
            await promoRepository.Update(promo);
            NotificationDialog notification = new NotificationDialog();
            notification.Header = "Notification";
            notification.ContentDialog = "This promo has been accepted successfully.";
            await DialogHost.Show(notification, "Main");
            NavigateProvider.AdminPromoScreen().Navigate();
        }
        private async Task DeletePromo()
        {
            GenericDataRepository<Models.Promo> promoRepository = new GenericDataRepository<Promo>();
            Models.Promo promo = await promoRepository.GetSingleAsync(p => p.Id == SelectedPromo.Id);
            promo.Status = 2;
            await promoRepository.Update(promo);
            NotificationDialog notification = new NotificationDialog();
            notification.Header = "Notification";
            notification.ContentDialog = "This promo has been removed successfully.";
            MainViewModel.SetLoading(false);
            await DialogHost.Show(notification, "Main");
            NavigateProvider.AdminPromoScreen().Navigate();
        }
        private async Task SaveProduct()
        {
            GenericDataRepository<Models.Promo> promoRepository = new GenericDataRepository<Promo>();
            Models.Promo promo = await promoRepository.GetSingleAsync(p => p.Id == SelectedPromo.Id);
            promo.Code = SelectedPromo.Code;
            promo.Description = SelectedPromo.Description;
            promo.DateBegin = SelectedPromo.DateBegin.Value.Subtract(new TimeSpan(12, 0, 0));
            promo.DateEnd = SelectedPromo.DateEnd.Value.Add(new TimeSpan(12, 0, 0));
            promo.Amount = (IsLimitedAmount ? SelectedPromo.Amount : -1);
            promo.AmountUsed = 0;
            promo.MaxSale = (IsMaxSale ? SelectedPromo.MaxSale : double.MaxValue);
            promo.MinCost = SelectedPromo.MinCost;
            promo.Sale = SelectedPromo.Sale;
            promo.Status = 0;
            if (IsNewCustomer)
            {
                promo.CustomerType = 0;
            }
            if(IsAllCustomer)
            {
                promo.CustomerType = 1;
            }    
            promo.Name = SelectedPromo.Name;
            await promoRepository.Update(promo);
            foreach(Models.Product p in SelectedPromo.Products)
            {
                await PromoDetailAPI.Delete(promo.Id, p.Id);
            }
            SelectedPromo.Products.Clear();
            foreach (PromoProductBlockViewModel promoProductBlock in SelectedProductPromos)
            {
                await PromoDetailAPI.Add(promo.Id, promoProductBlock.SelectedProduct.Id);
                SelectedPromo.Products.Add(promoProductBlock.SelectedProduct);
            }
            NotificationDialog notification = new NotificationDialog();
            notification.Header = "Notification";
            notification.ContentDialog = "This promo has been updated successfully. Please wait for us to accept.";
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
        private async Task Load()
        {
            GenericDataRepository<Models.Promo> promoRepository = new GenericDataRepository<Models.Promo>();
            SelectedPromo = await promoRepository.GetSingleAsync(p => p.Id == initialPromo.Id,
                                                                p => p.MUser,
                                                                p => p.Products, 
                                                                p => p.Products.Select(pr => pr.ImageProducts));
        }
        private void Search()
        {
            if (SearchBy == "Id")
            {
                FilterProductPromos = new ObservableCollection<PromoProductBlockViewModel>(SelectedProductPromos.Where(p => p.SelectedProduct.Id.ToLower().Trim().Contains(SearchByValue == null ? "" : SearchByValue.ToLower().Trim())));
            }
            else if (SearchBy == "Product Name")
            {
                FilterProductPromos = new ObservableCollection<PromoProductBlockViewModel>(SelectedProductPromos.Where(p => p.SelectedProduct.Name.ToLower().Trim().Contains(SearchByValue == null ? "" : SearchByValue.ToLower().Trim())));
            }
        }
    }
    public class PromoVMConstructor {
        public Promo promo { get; set; }
        public bool isAdmin { get; set; }
        public PromoVMConstructor(Promo promo, bool isAdmin = false) {
            this.promo = promo;
            this.isAdmin = isAdmin;
        }
    }
}
