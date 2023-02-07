using DataAccessLayer;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ShopPromoViewModel:BaseViewModel
    {
        public ICommand SearchCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand EditPromoCommand { get; set; }
        public ICommand CopyPromoCommand { get; set; }
        public ICommand RemovePromoCommand { get; set; }
        public ICommand ViewPromoCommand { get; set; }
        public ICommand AcceptPromoCommand { get; set; }
        public ICommand AddPromoCommand { get; set; }
        private ObservableCollection<ShopPromoBlockViewModel> allPromos;
        public ObservableCollection<ShopPromoBlockViewModel> AllPromos
        {
            get => allPromos;
            set
            {
                allPromos = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopPromoBlockViewModel> inProcessPromos;
        public ObservableCollection<ShopPromoBlockViewModel> InProcessPromos
        {
            get => inProcessPromos;
            set
            {
                inProcessPromos = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopPromoBlockViewModel> upcommingPromos;
        public ObservableCollection<ShopPromoBlockViewModel> UpcommingPromos
        {
            get => upcommingPromos;
            set
            {
                upcommingPromos = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopPromoBlockViewModel> expiredPromos;
        public ObservableCollection<ShopPromoBlockViewModel> ExpiredPromos
        {
            get => expiredPromos;
            set
            {
                expiredPromos = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopPromoBlockViewModel> requestingPromos;
        public ObservableCollection<ShopPromoBlockViewModel> RequestingPromos
        {
            get => requestingPromos;
            set
            {
                requestingPromos = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopPromoBlockViewModel> deletedPromos;
        public ObservableCollection<ShopPromoBlockViewModel> DeletedPromos
        {
            get => deletedPromos;
            set
            {
                deletedPromos = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopPromoBlockViewModel> filterPromos;
        public ObservableCollection<ShopPromoBlockViewModel> FilterPromos
        {
            get => filterPromos;
            set
            {
                filterPromos = value;
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
        private DateTime? dateFrom;
        public DateTime? DateFrom
        {
            get => dateFrom;
            set
            {
                dateFrom = value;
                OnPropertyChanged();
            }
        }
        private DateTime? dateTo;
        public DateTime? DateTo
        {
            get => dateTo;
            set
            {
                dateTo = value;
                OnPropertyChanged();
            }
        }
        private bool statusAll;
        public bool StatusAll
        {
            get => statusAll;
            set
            {
                statusAll = value;
                if(value)
                {
                    FilterPromos = AllPromos;
                    Search();
                }    
                OnPropertyChanged();
            }
        }
        
        private bool statusInProcess;
        public bool StatusInProcess
        {
            get => statusInProcess;
            set
            {
                statusInProcess = value;
                if(value)
                {
                    FilterPromos = InProcessPromos;
                    Search();
                }    
                OnPropertyChanged();
            }
        }
        private bool statusUpcoming;
        public bool StatusUpcoming
        {
            get => statusUpcoming;
            set
            {
                statusUpcoming = value;
                if(value)
                {
                    FilterPromos = UpcommingPromos;
                    Search();
                }    
                OnPropertyChanged();
            }
        }
        private bool statusExpired;
        public bool StatusExpired
        {
            get => statusExpired;
            set
            {
                statusExpired = value;
                if(value)
                {
                    FilterPromos = ExpiredPromos;
                    Search();
                }    
                OnPropertyChanged();
            }
        }
        private bool statusRequesting;
        public bool StatusRequesting
        {
            get => statusRequesting;
            set
            {
                statusRequesting = value;
                if(value)
                {
                    FilterPromos = RequestingPromos;
                    Search();
                }    
                OnPropertyChanged();
            }
        }
        private bool statusDeleted;
        public bool StatusDeleted
        {
            get => statusDeleted;
            set
            {
                statusDeleted = value;
                if(value)
                {
                    FilterPromos = DeletedPromos;
                    Search();
                }    
                OnPropertyChanged();
            }
        }
        public ShopPromoViewModel()
        {
            IsLoadingCheck.IsLoading = 2;
            Task.Run(async () =>
            {
                AllPromos = new ObservableCollection<ShopPromoBlockViewModel>();
                InProcessPromos = new ObservableCollection<ShopPromoBlockViewModel>();
                UpcommingPromos = new ObservableCollection<ShopPromoBlockViewModel>();
                ExpiredPromos = new ObservableCollection<ShopPromoBlockViewModel>();
                RequestingPromos = new ObservableCollection<ShopPromoBlockViewModel>();
                DeletedPromos = new ObservableCollection<ShopPromoBlockViewModel>();
                await LoadPromos();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            }).ContinueWith((first) =>
            {
                DateFrom = null;
                DateTo = null;
                SearchBy = "Promo Code";
                SearchByValue = "";
                StatusAll = true;
                SearchCommand = new RelayCommand<object>((p) =>
                {
                    return (DateFrom != null && DateTo != null && DateFrom < DateTo) || (DateFrom == null) || (DateTo == null);
                }, ((p) =>
                {
                    if (StatusAll)
                    {
                        FilterPromos = AllPromos;
                    }
                    else if (StatusInProcess)
                    {
                        FilterPromos = InProcessPromos;
                    }
                    else if (statusUpcoming)
                    {
                        FilterPromos = UpcommingPromos;
                    }
                    else if (StatusExpired)
                    {
                        FilterPromos = ExpiredPromos;
                    }
                    else if (StatusRequesting)
                    {
                        FilterPromos = RequestingPromos;
                    }
                    else if (StatusDeleted)
                    {
                        FilterPromos = DeletedPromos;
                    }
                    Search();
                }));
                ResetCommand = new RelayCommandWithNoParameter(() =>
                {
                    DateFrom = null;
                    DateTo = null;
                    SearchBy = "Promo Code";
                    SearchByValue = "";
                    if (StatusAll)
                    {
                        FilterPromos = AllPromos;
                        Search();
                    }
                    else
                    {
                        StatusAll = true;
                    }
                });
                EditPromoCommand = new RelayCommand<object>((p) => p != null, (p) =>
                {
                    var temp = new PromoVMConstructor((p as ShopPromoBlockViewModel).Promo);
                    NavigateProvider.PromoInfomationScreen().Navigate(temp);
                });
                CopyPromoCommand = new RelayCommand<object>((p) => p != null, (p) =>
                {
                    Clipboard.SetText((p as ShopPromoBlockViewModel).Promo.Code);
                });
                RemovePromoCommand = new RelayCommand<object>((p) => p != null,async (p) =>
                {
                    MainViewModel.SetLoading(true);
                    ShopPromoBlockViewModel shopPromoBlockViewModel = p as ShopPromoBlockViewModel;
                    await DeletePromo(shopPromoBlockViewModel.Promo);
                    if (StatusInProcess)
                    {
                        InProcessPromos.Remove(shopPromoBlockViewModel);
                        FilterPromos.Remove(shopPromoBlockViewModel);
                    }
                    else if (StatusUpcoming)
                    {
                        UpcommingPromos.Remove(shopPromoBlockViewModel);
                        FilterPromos.Remove(shopPromoBlockViewModel);
                    }
                    else if (StatusExpired)
                    {
                        ExpiredPromos.Remove(shopPromoBlockViewModel);
                        FilterPromos.Remove(shopPromoBlockViewModel);
                    }
                    else if (StatusRequesting)
                    {
                        RequestingPromos.Remove(shopPromoBlockViewModel);
                        FilterPromos.Remove(shopPromoBlockViewModel);
                    }
                    else if (StatusAll)
                    {
                        if (shopPromoBlockViewModel.Promo.Status == 1)
                        {
                            if (shopPromoBlockViewModel.Promo.DateBegin > DateTime.Now)
                            {
                                UpcommingPromos.Remove(shopPromoBlockViewModel);
                            }
                            else if (shopPromoBlockViewModel.Promo.DateBegin <= DateTime.Now && shopPromoBlockViewModel.Promo.DateEnd > DateTime.Now)
                            {
                                InProcessPromos.Remove(shopPromoBlockViewModel);
                            }
                        }
                        else if (shopPromoBlockViewModel.Promo.Status == 0 && shopPromoBlockViewModel.Promo.DateEnd > DateTime.Now)
                        {
                            RequestingPromos.Remove(shopPromoBlockViewModel);
                        }
                        else
                        {
                            ExpiredPromos.Remove(shopPromoBlockViewModel);
                        }
                    }
                    DeletedPromos.Insert(0, shopPromoBlockViewModel);
                    shopPromoBlockViewModel.Promo.Status = 2;
                    shopPromoBlockViewModel.ChangeStatus();
                    MainViewModel.SetLoading(false);
                });
                AddPromoCommand = new RelayCommandWithNoParameter(() =>
                {
                    NavigateProvider.ShopAddPromoScreen().Navigate();
                });
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        public void Search()
        {
            if (SearchBy == "Promo Code")
            {
                FilterPromos = new ObservableCollection<ShopPromoBlockViewModel>(FilterPromos.Where(p => p.Promo.Code.ToLower().Trim().Contains(SearchByValue==null?"":SearchByValue.ToLower().Trim()) &&
                                                                                                    ((DateFrom == null) ? true : (p.Promo.DateBegin >= DateFrom.Value.Subtract(new TimeSpan(12,0,0)))) &&
                                                                                                    ((DateTo == null) ? true : (p.Promo.DateEnd <= DateTo.Value.Add(new TimeSpan(12, 0, 0))))));
            }
            else
            {
                FilterPromos = new ObservableCollection<ShopPromoBlockViewModel>(FilterPromos.Where(p => p.Promo.Name.ToLower().Trim().Contains(SearchByValue == null ? "" : SearchByValue.ToLower().Trim())&&
                                                                                                    ((DateFrom == null) ? true : (p.Promo.DateBegin >= DateFrom.Value.Subtract(new TimeSpan(12, 0, 0)))) &&
                                                                                                    ((DateTo == null) ? true : (p.Promo.DateEnd <= DateTo.Value.Add(new TimeSpan(12, 0, 0))))));
            }    
        }
        public async Task LoadPromos()
        {
            GenericDataRepository<Models.Promo> promoRepository = new GenericDataRepository<Models.Promo>();
            ObservableCollection<Models.Promo>  allPromos = new ObservableCollection<Models.Promo>((await promoRepository.GetListAsync(p => p.IdShop == AccountStore.instance.CurrentAccount.Id,
                                                                                                                                        p=> p.MUser, 
                                                                                                                                        p => p.Products,
                                                                                                                                        p => p.Products.Select(pr => pr.Brand),
                                                                                                                                        p => p.Products.Select(pr => pr.Category),
                                                                                                                                        p => p.Products.Select(pr => pr.ImageProducts))).OrderByDescending(p => (p.Status == 1 && p.DateBegin < DateTime.Now)).
                                                                                                                                                                                        ThenByDescending(p=>(p.Status == 1 && p.DateBegin >= DateTime.Now && p.DateEnd < DateTime.Now)).
                                                                                                                                                                                        ThenByDescending(p => (p.Status == 1 && p.DateEnd >= DateTime.Now)).
                                                                                                                                                                                        ThenByDescending(p => (p.Status == 0 && p.DateEnd < DateTime.Now)).
                                                                                                                                                                                        ThenByDescending(p => (p.Status == 0 && p.DateEnd >= DateTime.Now)));
            foreach(var promo in allPromos) 
            {
                ShopPromoBlockViewModel shopPromoBlockViewModel = new ShopPromoBlockViewModel(promo);
                AllPromos.Add(shopPromoBlockViewModel);
                if (promo.Status == 2)
                {
                    DeletedPromos.Add(shopPromoBlockViewModel);
                }
                else if (promo.Status == 1)
                {
                    if (promo.DateBegin > DateTime.Now)
                    {
                        UpcommingPromos.Add(shopPromoBlockViewModel);
                    }
                    else if (promo.DateBegin <= DateTime.Now && promo.DateEnd > DateTime.Now)
                    {
                        InProcessPromos.Add(shopPromoBlockViewModel);
                    }
                }
                else if (promo.Status == 0 && promo.DateEnd > DateTime.Now)
                {
                    RequestingPromos.Add(shopPromoBlockViewModel);
                }
                else
                {
                    ExpiredPromos.Add(shopPromoBlockViewModel);
                }
            }
        }
        public async Task DeletePromo(Models.Promo promo)
        {
            GenericDataRepository<Models.Promo> promoRepository = new GenericDataRepository<Models.Promo>();
            Models.Promo temp = await promoRepository.GetSingleAsync(p=>p.Id == promo.Id);
            temp.Status = 2;
            await promoRepository.Update(temp);
        }
    }
}
