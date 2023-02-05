using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ShopRatingViewModel : BaseViewModel
    {
        private GenericDataRepository<Models.MOrder> orderReposition = new GenericDataRepository<Models.MOrder>();
        private GenericDataRepository<Models.OrderInfo> orderInfoReposition = new GenericDataRepository<Models.OrderInfo>();
        private GenericDataRepository<Models.Brand> brandReposition = new GenericDataRepository<Models.Brand>();
        private GenericDataRepository<Models.Category> categoryReposition = new GenericDataRepository<Models.Category>();
        public ICommand ChangeRatingStarStyleCommand { get; set; }
        public ICommand ChangeRatingStatusCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ScrollToHome { get; set; }
        public ICommand ScrollToCategory { get; set; }
        private string productName;
        public string ProductName
        {
            get { return productName; }
            set
            {
                productName = value;
                OnPropertyChanged();
            }
        }
        private string userName;
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                OnPropertyChanged();
            }
        }
        private DateTime? dateFrom;
        public DateTime? DateFrom
        {
            get { return dateFrom; }
            set
            {
                dateFrom = value;
                OnPropertyChanged();
            }
        }
        private DateTime? dateTo;
        public DateTime? DateTo
        {
            get { return dateTo; }
            set
            {
                dateTo = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Category> categories;
        public ObservableCollection<Models.Category> Categories
        {
            get { return categories; }
            set
            {
                categories = value;
                OnPropertyChanged();
            }
        }
        private Models.Category selectedCategory;
        public Models.Category SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Brand> brands;
        public ObservableCollection<Models.Brand> Brands
        {
            get { return brands; }
            set
            {
                brands = value;
                OnPropertyChanged();
            }
        }
        private Models.Brand selectedBrand;
        public Models.Brand SelectedBrand
        {
            get { return selectedBrand; }
            set
            {
                selectedBrand = value;
                OnPropertyChanged();
            }
        }
        private bool statusSearchAll;
        public bool StatusSearchAll
        {
            get => statusSearchAll;
            set
            {
                statusSearchAll = value;
                OnPropertyChanged();
                if (value)
                {
                    ShopRatingBlockModelFilter = ShopRatingBlockModelsAll;
                    Task.Run(() =>
                    {
                        if (MainViewModel.IsLoading)
                        {
                            Search();
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading--;
                                }
                            }));
                        }
                        else
                        {
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading++;
                                }
                            }));

                            Search();

                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading--;
                                }
                            }));
                        }
                    });
                }
            }
        }
        private bool statusSearch5Star;
        public bool StatusSearch5Star
        {
            get => statusSearch5Star;
            set
            {
                statusSearch5Star = value;
                OnPropertyChanged();
                if (value)
                {
                    ShopRatingBlockModelFilter = ShopRatingBlockModels5Star;
                    Task.Run(() =>
                    {
                        if (MainViewModel.IsLoading)
                        {
                            Search();
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading--;
                                }
                            }));
                        }
                        else
                        {
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading++;
                                }
                            }));

                            Search();

                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading--;
                                }
                            }));
                        }
                    });
                }
            }
        }
        private bool statusSearch4Star;
        public bool StatusSearch4Star
        {
            get => statusSearch4Star;
            set
            {
                statusSearch4Star = value;
                OnPropertyChanged();
                if (value)
                {
                    ShopRatingBlockModelFilter = ShopRatingBlockModels4Star;
                    Task.Run(() =>
                    {
                        if (MainViewModel.IsLoading)
                        {
                            Search();
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading--;
                                }
                            }));
                        }
                        else
                        {
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading++;
                                }
                            }));

                            Search();

                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading--;
                                }
                            }));
                        }
                    });
                }
            }
        }
        private bool statusSearch3Star;
        public bool StatusSearch3Star
        {
            get => statusSearch3Star;
            set
            {
                statusSearch3Star = value;
                OnPropertyChanged();
                if (value)
                {
                    ShopRatingBlockModelFilter = ShopRatingBlockModels3Star;
                    Task.Run(() =>
                    {
                        if (MainViewModel.IsLoading)
                        {
                            Search();
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading--;
                                }
                            }));
                        }
                        else
                        {
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading++;
                                }
                            }));

                            Search();

                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading--;
                                }
                            }));
                        }
                    });
                }
            }
        }
        private bool statusSearch2Star;
        public bool StatusSearch2Star
        {
            get => statusSearch2Star;
            set
            {
                statusSearch2Star = value;
                OnPropertyChanged();
                if (value)
                {
                    ShopRatingBlockModelFilter = ShopRatingBlockModels2Star;
                    Task.Run(() =>
                    {
                        if (MainViewModel.IsLoading)
                        {
                            Search();
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading--;
                                }
                            }));
                        }
                        else
                        {
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading++;
                                }
                            }));

                            Search();

                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading--;
                                }
                            }));
                        }
                    });
                }
            }
        }
        private bool statusSearch1Star;
        public bool StatusSearch1Star
        {
            get => statusSearch1Star;
            set
            {
                statusSearch1Star = value;
                OnPropertyChanged();
                if (value)
                {
                    ShopRatingBlockModelFilter = ShopRatingBlockModels1Star;
                    Task.Run(() =>
                    {
                        if (MainViewModel.IsLoading)
                        {
                            Search();
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading--;
                                }
                            }));
                        }
                        else
                        {
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading++;
                                }
                            }));

                            Search();

                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                lock (IsLoadingCheck.IsLoading as object)
                                {
                                    IsLoadingCheck.IsLoading--;
                                }
                            }));
                        }
                    });
                }
            }
        }
        private ObservableCollection<ShopRatingBlockModel> shopRatingBlockModelFilter;
        public ObservableCollection<ShopRatingBlockModel> ShopRatingBlockModelFilter
        {
            get { return shopRatingBlockModelFilter; }
            set
            {
                shopRatingBlockModelFilter = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopRatingBlockModel> shopRatingBlockModelAll;
        public ObservableCollection<ShopRatingBlockModel> ShopRatingBlockModelsAll
        {
            get { return shopRatingBlockModelAll; }
            set
            {
                shopRatingBlockModelAll = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopRatingBlockModel> shopRatingBlockModels5Star;
        public ObservableCollection<ShopRatingBlockModel> ShopRatingBlockModels5Star
        {
            get { return shopRatingBlockModels5Star; }
            set
            {
                shopRatingBlockModels5Star = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopRatingBlockModel> shopRatingBlockModels4Star;
        public ObservableCollection<ShopRatingBlockModel> ShopRatingBlockModels4Star
        {
            get { return shopRatingBlockModels4Star; }
            set
            {
                shopRatingBlockModels4Star = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopRatingBlockModel> shopRatingBlockModels3Star;
        public ObservableCollection<ShopRatingBlockModel> ShopRatingBlockModels3Star
        {
            get { return shopRatingBlockModels3Star; }
            set
            {
                shopRatingBlockModels3Star = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopRatingBlockModel> shopRatingBlockModels2Star;
        public ObservableCollection<ShopRatingBlockModel> ShopRatingBlockModels2Star
        {
            get { return shopRatingBlockModels2Star; }
            set
            {
                shopRatingBlockModels2Star = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopRatingBlockModel> shopRatingBlockModels1Star;
        public ObservableCollection<ShopRatingBlockModel> ShopRatingBlockModels1Star
        {
            get { return shopRatingBlockModels1Star; }
            set
            {
                shopRatingBlockModels1Star = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopRatingBlockModel> displayShopRatingBlockModels;
        public ObservableCollection<ShopRatingBlockModel> DisplayShopRatingBlockModels
        {
            get { return displayShopRatingBlockModels; }
            set
            {
                displayShopRatingBlockModels = value;
                IsLoadingCheck.IsLoading += displayShopRatingBlockModels.Count();
                OnPropertyChanged();
            }
        }
        public ShopRatingViewModel()
        {
            IsLoadingCheck.IsLoading = 2;
            ShopRatingBlockModelFilter = new ObservableCollection<ShopRatingBlockModel>();
            Task.Run(async () =>
            {
                Load();
                await LoadListData();
                await LoadBrand();
                await LoadCategory();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            }).ContinueWith((first) =>
            {
                SearchCommand = new RelayCommandWithNoParameter(() =>
                {
                    if (DateFrom != null && DateTo != null && DateFrom > DateTo)
                    {
                        NotificationDialog notification = new NotificationDialog();
                        notification.Header = "Error";
                        notification.ContentDialog = "Date to is bigger than date from";
                        DialogHost.Show(notification, "Main");
                        return;
                    }
                    MainViewModel.SetLoading(true);
                    Search();
                    MainViewModel.SetLoading(false);
                });
                ResetCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
                {
                    MainViewModel.SetLoading(true);
                    ProductName = "";
                    UserName = "";
                    DateFrom = null;
                    DateTo = null;
                    SelectedCategory = null;
                    SelectedBrand = null;
                    if (statusSearchAll)
                    {
                        ShopRatingBlockModelFilter = ShopRatingBlockModelsAll;
                        Search();
                    }
                    else
                    {
                        StatusSearchAll = true;
                        StatusSearch5Star = false;
                        StatusSearch4Star = false;
                        StatusSearch3Star = false;
                        StatusSearch2Star = false;
                        StatusSearch1Star = false;
                    }
                    MainViewModel.SetLoading(false);
                });
                ScrollToHome = new RelayCommand<object>((p) => { return p != null; }, p =>
                {
                    ScrollViewer scrollViewer = p as ScrollViewer;
                    scrollViewer.ScrollToHome();
                });
                ScrollToCategory = new RelayCommand<object>((p) => { return p != null; }, p =>
                {
                    ScrollViewer scrollViewer = p as ScrollViewer;
                    scrollViewer.ScrollToVerticalOffset(330);
                });
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    DisplayShopRatingBlockModels = ShopRatingBlockModelsAll;
                    ShopRatingBlockModelFilter = ShopRatingBlockModelsAll;
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            });
        }
        public void Search()
        {
            if (DateFrom != null && DateTo != null && DateFrom > DateTo)
            {
                MessageBox.Show("Date Wrong");
                return;
            }
            DisplayShopRatingBlockModels = new ObservableCollection<ShopRatingBlockModel>(ShopRatingBlockModelFilter.Where(x => x.OrderInfo.Product.Name.ToLower().Trim().Contains(ProductName == null? "": ProductName.ToLower().Trim()) &&
                                                                                                                 x.OrderInfo.MOrder.MUser.Name.ToLower().Trim().Contains(UserName == null? "": UserName.ToLower().Trim()) &&
                                                                                                                ((SelectedCategory == null) ? true : (x.OrderInfo.Product.IdCategory == SelectedCategory.Id)) &&
                                                                                                                ((SelectedBrand == null) ? true : (x.OrderInfo.Product.IdBrand == SelectedBrand.Id)) &&
                                                                                                                ((DateFrom == null) ? true : (x.OrderInfo.Rating.DateRating >= DateFrom.Value.Subtract(new TimeSpan(12, 0, 0)))) &&
                                                                                                                ((DateTo == null) ? true : (x.OrderInfo.Rating.DateRating <= DateTo.Value.Add(new TimeSpan(12, 0, 0))))));
        }
        public void Load()
        {
            ShopRatingBlockModelsAll = new ObservableCollection<ShopRatingBlockModel>();
            DateFrom = null;
            DateTo = null;
            statusSearchAll = true;
            statusSearch5Star = false;
            statusSearch4Star = false;
            statusSearch3Star = false;
            statusSearch2Star = false;
            statusSearch1Star = false;
        }
        public async Task LoadListData()
        {
            ObservableCollection<Models.MOrder> orders = new ObservableCollection<Models.MOrder>((await orderReposition.GetListAsync(r => (r != null && r.IdShop == AccountStore.instance.CurrentAccount.Id && r.Status == "Completed"),
                                                                                                                                    r => r.MUser)));
            foreach (Models.MOrder order in orders)
            {
                ObservableCollection<Models.OrderInfo> orderInfos = new ObservableCollection<OrderInfo>(await orderInfoReposition.GetListAsync(oi => (oi.IdOrder == order.Id && oi.IdRating != null),
                                                                                                                                               oi => oi.Product,
                                                                                                                                               oi => oi.Product.ImageProducts,
                                                                                                                                               oi => oi.Rating,
                                                                                                                                               oi => oi.Rating.RatingInfoes,
                                                                                                                                               oi => oi.Rating.RatingInfoes.Select(ri => ri.MUser),
                                                                                                                                               oi => oi.MOrder, 
                                                                                                                                               oi => oi.MOrder.MUser));
                for (int i = 0; i < orderInfos.Count; i++)
                {
                    ShopRatingBlockModel shopRatingBlockModel = new ShopRatingBlockModel(orderInfos[i]);
                    ShopRatingBlockModelsAll.Add(shopRatingBlockModel);
                }
            }
            ShopRatingBlockModelsAll = new ObservableCollection<ShopRatingBlockModel>(ShopRatingBlockModelsAll.OrderByDescending(r => r.OrderInfo.Rating.DateRating));
            ShopRatingBlockModels5Star = new ObservableCollection<ShopRatingBlockModel>(ShopRatingBlockModelsAll.Where(r => r.OrderInfo.Rating.Rating1 == 5));
            ShopRatingBlockModels4Star = new ObservableCollection<ShopRatingBlockModel>(ShopRatingBlockModelsAll.Where(r => r.OrderInfo.Rating.Rating1 == 4));
            ShopRatingBlockModels3Star = new ObservableCollection<ShopRatingBlockModel>(ShopRatingBlockModelsAll.Where(r => r.OrderInfo.Rating.Rating1 == 3));
            ShopRatingBlockModels2Star = new ObservableCollection<ShopRatingBlockModel>(ShopRatingBlockModelsAll.Where(r => r.OrderInfo.Rating.Rating1 == 2));
            ShopRatingBlockModels1Star = new ObservableCollection<ShopRatingBlockModel>(ShopRatingBlockModelsAll.Where(r => r.OrderInfo.Rating.Rating1 == 1));
        }
        public async Task LoadBrand()
        {
            Brands = new ObservableCollection<Models.Brand>(await brandReposition.GetListAsync(b => b.Status == "NotBanned"));
        }
        public async Task LoadCategory()
        {
            Categories = new ObservableCollection<Models.Category>(await categoryReposition.GetListAsync(c => c.Status == "NotBanned"));
        }
    }
}
