﻿using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ShopOrderViewModel:BaseViewModel
    {
        private GenericDataRepository<Models.MOrder> orderRepposition = new GenericDataRepository<Models.MOrder>();
        private GenericDataRepository<Models.OrderInfo> orderInfoRepposition = new GenericDataRepository<Models.OrderInfo>();
        private GenericDataRepository<Models.Notification> notificationRepposition = new GenericDataRepository<Models.Notification>();
        public ICommand SearchCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ChooseShippingMethodCommand { get; set; }
        public ICommand ChangeToDeliveredCommand { get; set; }
        public ICommand ChangeToCompletedCommand { get; set; }
        public ICommand ChangeToCancelledCommand { get; set; }
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
        private ObservableCollection<ShopOrderBlockViewModel> searchedShopOrderBlockModels;
        public ObservableCollection<ShopOrderBlockViewModel> SearchedShopOrderBlockModels
        {
            get => searchedShopOrderBlockModels;
            set
            {
                searchedShopOrderBlockModels = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopOrderBlockViewModel> allShopOrderBlockModels;
        public ObservableCollection<ShopOrderBlockViewModel> AllShopOrderBlockModels
        {
            get => allShopOrderBlockModels;
            set
            {
                allShopOrderBlockModels = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopOrderBlockViewModel> processingShopOrderBlockModels;
        public ObservableCollection<ShopOrderBlockViewModel> ProcessingShopOrderBlockModels
        {
            get => processingShopOrderBlockModels;
            set
            {
                processingShopOrderBlockModels = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopOrderBlockViewModel> deliveringShopOrderBlockModels;
        public ObservableCollection<ShopOrderBlockViewModel> DeliveringShopOrderBlockModels
        {
            get => deliveringShopOrderBlockModels;
            set
            {
                deliveringShopOrderBlockModels = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopOrderBlockViewModel> deliveredShopOrderBlockModels;
        public ObservableCollection<ShopOrderBlockViewModel> DeliveredShopOrderBlockModels
        {
            get => deliveredShopOrderBlockModels;
            set
            {
                deliveredShopOrderBlockModels = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopOrderBlockViewModel> completedShopOrderBlockModels;
        public ObservableCollection<ShopOrderBlockViewModel> CompletedShopOrderBlockModels
        {
            get => completedShopOrderBlockModels;
            set
            {
                completedShopOrderBlockModels = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopOrderBlockViewModel> cancelledShopOrderBlockModels;
        public ObservableCollection<ShopOrderBlockViewModel> CancelledShopOrderBlockModels
        {
            get => cancelledShopOrderBlockModels;
            set
            {
                cancelledShopOrderBlockModels = value;
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
                if(value)
                {
                    SearchByStatus(AllShopOrderBlockModels);
                }    
                OnPropertyChanged();
            }
        }
        private bool statusSearchProcessing;
        public bool StatusSearchProcessing
        {
            get => statusSearchProcessing;
            set
            {
                statusSearchProcessing = value;
                if (value)
                {
                    SearchByStatus(ProcessingShopOrderBlockModels);
                }
                OnPropertyChanged();
            }
        }
        private bool statusSearchDelivering;
        public bool StatusSearchDelivering
        {
            get => statusSearchDelivering;
            set
            {
                statusSearchDelivering = value;
                if (value)
                {
                    SearchByStatus(DeliveringShopOrderBlockModels);
                }
                OnPropertyChanged();
            }
        }
        private bool statusSearchDelivered;
        public bool StatusSearchDelivered
        {
            get => statusSearchDelivered;
            set
            {
                statusSearchDelivered = value;
                if (value)
                {
                    SearchByStatus(DeliveredShopOrderBlockModels);
                }
                OnPropertyChanged();
            }
        }
        private bool statusSearchCompleted;
        public bool StatusSearchCompleted
        {
            get => statusSearchCompleted;
            set
            {
                statusSearchCompleted = value;
                if (value)
                {
                    SearchByStatus(CompletedShopOrderBlockModels);
                }
                OnPropertyChanged();
            }
        }
        private bool statusSearchCancelled;
        public bool StatusSearchCancelled
        {
            get => statusSearchCancelled;
            set
            {
                statusSearchCancelled = value;
                if (value)
                {
                    SearchByStatus(CancelledShopOrderBlockModels);
                }
                OnPropertyChanged();
            }
        }
        public ShopOrderViewModel()
        {
            ChooseShippingMethodCommand = new RelayCommand<object>((p) => p != null, async (p) =>
            {
                MainViewModel.IsLoading = true;
                ChooseShippingMethodDialog dialog = new ChooseShippingMethodDialog();
                dialog.DataContext = new ChooseShippingMethodDialogViewModel(p as ShopOrderBlockViewModel);
                MainViewModel.IsLoading = false;
                await DialogHost.Show(dialog, "SecondDialog", null, null, ClosedChooseShippingDialog);
            });
            ChangeToDeliveredCommand = new RelayCommand<object>((p) => p != null, async (p) =>
            {
                MainViewModel.IsLoading = true;
                ShopOrderBlockViewModel shopOrderBlockViewModel = p as ShopOrderBlockViewModel;
                await UpdateStatus(shopOrderBlockViewModel.Order.Id, "Delivered");
                DeliveringShopOrderBlockModels.Remove(shopOrderBlockViewModel);
                shopOrderBlockViewModel.NextStatusContent = "Completed";
                shopOrderBlockViewModel.Order.Status = "Delivered";
                shopOrderBlockViewModel.Order.DateEnd= DateTime.Now;
                shopOrderBlockViewModel.ShopOrderBlockCommand = ChangeToCompletedCommand;
                DeliveredShopOrderBlockModels.Insert(0, shopOrderBlockViewModel);
                await AddNotification
                (
                    $"The order {shopOrderBlockViewModel.Order.Id} is delivered. Please check and notify us if something is wrong.",
                    AccountStore.instance.CurrentAccount.Id,
                    shopOrderBlockViewModel.Customer.Id
                );
                LoadAllShopOrderBlockViewModel();
                Search();
                OnPropertyChanged(nameof(SearchedShopOrderBlockModels));
                MainViewModel.IsLoading = false;
            });
            ChangeToCompletedCommand = new RelayCommand<object>((p) => p != null, async (p) =>
            {
                try
                {
                    MainViewModel.IsLoading = true;
                    ShopOrderBlockViewModel shopOrderBlockViewModel = p as ShopOrderBlockViewModel;
                    await UpdateStatus(shopOrderBlockViewModel.Order.Id, "Completed");
                    DeliveredShopOrderBlockModels.Remove(shopOrderBlockViewModel); 
                    shopOrderBlockViewModel.Order.Status = "Completed";
                    shopOrderBlockViewModel.NextStatusContent = "";
                    shopOrderBlockViewModel.ShopOrderBlockCommand = null;
                    CompletedShopOrderBlockModels.Insert(0, shopOrderBlockViewModel);
                    await AddNotification
                    (
                        $"The order {shopOrderBlockViewModel.Order.Id} is completed. Thanks for your rating.",
                        AccountStore.instance.CurrentAccount.Id,
                        shopOrderBlockViewModel.Customer.Id
                    );
                    LoadAllShopOrderBlockViewModel();
                    Search();
                    OnPropertyChanged(nameof(SearchedShopOrderBlockModels));
                    MainViewModel.IsLoading = false;
                }
                catch
                {
                    MainViewModel.IsLoading = true;
                    NotificationDialog notificationDialog = new NotificationDialog();
                    notificationDialog.Header = "Error";
                    notificationDialog.ContentDialog = "The status of order is changed before. Please reload this page!";
                    MainViewModel.IsLoading = false;
                    await DialogHost.Show(notificationDialog, "Main");
                }
               
            });
            ChangeToCancelledCommand = new RelayCommand<object>((p) => p != null, async (p) =>
            {
                MainViewModel.IsLoading = true;
                ShopOrderBlockViewModel shopOrderBlockViewModel = p as ShopOrderBlockViewModel;
                await UpdateStatus(shopOrderBlockViewModel.Order.Id, "Cancelled");
                ProcessingShopOrderBlockModels.Remove(shopOrderBlockViewModel);
                shopOrderBlockViewModel.Order.Status = "Cancelled";
                shopOrderBlockViewModel.NextStatusContent = "";
                shopOrderBlockViewModel.ShopOrderBlockCommand = null;
                CancelledShopOrderBlockModels.Insert(0, shopOrderBlockViewModel);
                await AddNotification
                (
                    $"The order {shopOrderBlockViewModel.Order.Id} is cancelled by shop.",
                    AccountStore.instance.CurrentAccount.Id,
                    shopOrderBlockViewModel.Customer.Id
                );
                LoadAllShopOrderBlockViewModel();
                Search();
                OnPropertyChanged(nameof(SearchedShopOrderBlockModels));
                MainViewModel.IsLoading = false;
            });
            SearchCommand = new RelayCommandWithNoParameter(() =>
            {
                if(DateFrom != null && DateTo != null && DateFrom > DateTo)
                {
                    NotificationDialog notification = new NotificationDialog();
                    notification.Header = "Error";
                    notification.ContentDialog = "Date to is bigger than date from";
                    DialogHost.Show(notification, "Main");
                    return;
                }    
                MainViewModel.IsLoading = true;
                Search();
                MainViewModel.IsLoading = false;
            });
            ResetCommand = new RelayCommandWithNoParameter(() =>
            {
                DateFrom = null;
                DateTo = null;
                SearchByValue = "";
                SearchBy = "Id";
            });
            ProcessingShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>();
            DeliveringShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>();
            DeliveredShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>();
            CompletedShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>();
            CancelledShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>();
            Task t = Task.Run(async () =>
            {
                await LoadDataProcessingIntoCollection();
                await LoadDataDeliveringIntoCollection();
                await LoadDataDeliveredIntoCollection();
                await LoadDataCompletedIntoCollection();
                await LoadDataCancelledIntoCollection();
            });
            while (!t.IsCompleted) { }
            LoadAllShopOrderBlockViewModel();
            if(string.IsNullOrEmpty(SearchBy))
            {
                SearchBy = "Id";                
            }    
            StatusSearchAll = true;
        }

        private async void ClosedChooseShippingDialog(object sender, DialogClosedEventArgs eventArgs)
        {
            try
            {
                Tuple<int, ShopOrderBlockViewModel> tuple = eventArgs.Parameter as Tuple<int, ShopOrderBlockViewModel>;
                if(tuple == null)
                {
                    return;
                }
                MainViewModel.IsLoading = true;
                ShopOrderBlockViewModel shopOrderBlockViewModel = tuple.Item2;
                await UpdateStatus(shopOrderBlockViewModel.Order.Id, "Delivering");
                ProcessingShopOrderBlockModels.Remove(shopOrderBlockViewModel);
                shopOrderBlockViewModel.NextStatusContent = "Delivered";
                shopOrderBlockViewModel.Order.Status = "Delivering";
                shopOrderBlockViewModel.CancelledOrderBlockCommand = null;
                shopOrderBlockViewModel.ShopOrderBlockCommand = ChangeToDeliveredCommand;
                DeliveringShopOrderBlockModels.Add(shopOrderBlockViewModel);
                await AddNotification
                (
                    $"The order {shopOrderBlockViewModel.Order.Id} is delivering.",
                    AccountStore.instance.CurrentAccount.Id,
                    shopOrderBlockViewModel.Customer.Id
                );
                await UpdateShippingMethod(shopOrderBlockViewModel.Order.Id, tuple.Item1);
                LoadAllShopOrderBlockViewModel();
                Search();
                OnPropertyChanged(nameof(SearchedShopOrderBlockModels));
                MainViewModel.IsLoading = false;
            }
            catch
            {
                MainViewModel.IsLoading = true;
                NotificationDialog notificationDialog = new NotificationDialog();
                notificationDialog.Header = "Error";
                notificationDialog.ContentDialog = "The status of order is changed before. Please reload this page!";
                MainViewModel.IsLoading = false;
                await DialogHost.Show(notificationDialog, "Main");
            }
        }

        public void Search()
        {
            if (StatusSearchAll)
            {
                SearchByStatus(AllShopOrderBlockModels);
            }
            else if (StatusSearchProcessing)
            {
                SearchByStatus(ProcessingShopOrderBlockModels);
            }
            else if (StatusSearchDelivering)
            {
                SearchByStatus(DeliveringShopOrderBlockModels);
            }
            else if (StatusSearchDelivered)
            {
                SearchByStatus(DeliveredShopOrderBlockModels);
            }
            else if (StatusSearchCompleted)
            {
                SearchByStatus(CompletedShopOrderBlockModels);
            }
            else if (StatusSearchCancelled)
            {
                SearchByStatus(CancelledShopOrderBlockModels);
            }
        }
        public void LoadAllShopOrderBlockViewModel()
        {
            AllShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>();
            foreach (var item in ProcessingShopOrderBlockModels)
            {
                AllShopOrderBlockModels.Add(item);
            }
            foreach (var item in DeliveringShopOrderBlockModels)
            {
                AllShopOrderBlockModels.Add(item);
            }
            foreach (var item in DeliveredShopOrderBlockModels)
            {
                AllShopOrderBlockModels.Add(item);
            }
            foreach (var item in CompletedShopOrderBlockModels)
            {
                AllShopOrderBlockModels.Add(item);
            }
            foreach (var item in CancelledShopOrderBlockModels)
            {
                AllShopOrderBlockModels.Add(item);
            }
        }
        public async Task UpdateStatus(string id, string status)
        {
            Models.MOrder order = await orderRepposition.GetSingleAsync(o => o.Id == id);
            if(order.Status == status)
            {
                throw new Exception("The status is already updated.");
            }    
            order.Status = status;
            if(status == "Delivered")
            {
                order.DateEnd = DateTime.Now;
            }
            await orderRepposition.Update(order);
        }
        private void SearchByStatus(ObservableCollection<ShopOrderBlockViewModel> collection)
        {
            if (SearchBy == "Id")
            {
                SearchedShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>(collection.Where(p => (String.IsNullOrEmpty(SearchByValue) ? true : p.Order.Id.Contains(SearchByValue)) &&
                                                                                                                  ((DateFrom == null) ? true : (p.Order.DateBegin >= DateFrom)) &&
                                                                                                                  ((p.Order.DateEnd == null)? ((DateTo == null) ? true : (DateTime.Now <= DateTo)) : ((DateTo == null) ? true : (p.Order.DateEnd <= DateTo)))).ToList());
            }
            else if (SearchBy == "Product Name")
            {
                SearchedShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>(collection.Where(p => (String.IsNullOrEmpty(SearchByValue) ? true : (p.OrderDetails.Where(od=>od.OrderInfo.Product.Name.Contains(SearchByValue)).Count() != 0))&&
                                                                                                                  ((DateFrom == null) ? true : (p.Order.DateBegin >= DateFrom)) &&
                                                                                                                  ((p.Order.DateEnd == null) ? ((DateTo == null) ? true : (DateTime.Now <= DateTo)) : ((DateTo == null) ? true : (p.Order.DateEnd <= DateTo)))).ToList());
            }
            else if (SearchBy == "Customer Name")
            {
                SearchedShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>(collection.Where(p => (String.IsNullOrEmpty(SearchByValue) ? true : p.Customer.Name.Contains(SearchByValue)) &&
                                                                                                                  ((DateFrom == null) ? true : (p.Order.DateBegin >= DateFrom)) &&
                                                                                                                  ((p.Order.DateEnd == null) ? ((DateTo == null) ? true : (DateTime.Now <= DateTo)) : ((DateTo == null) ? true : (p.Order.DateEnd <= DateTo)))).ToList());
            }
        }
        private async Task LoadDataProcessingIntoCollection()
        {
            ProcessingShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>();
            ObservableCollection<Models.MOrder> orders = new ObservableCollection<Models.MOrder>(await orderRepposition.GetListAsync(o => o.IdShop == AccountStore.instance.CurrentAccount.Id && o.Status == "Processing",
                                                                                                                                     o => o.OrderInfoes,
                                                                                                                                     o => o.OrderInfoes.Select(oi=>oi.Rating),
                                                                                                                                     o => o.MUser,
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Product),
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Product).Select(p => p.ImageProducts)));
            orders = new ObservableCollection<Models.MOrder>(orders.OrderByDescending(p => p.DateBegin));
            foreach (var order in orders)
            {
                ObservableCollection<Models.OrderInfo> orderInfos = new ObservableCollection<Models.OrderInfo>(order.OrderInfoes);
                ShopOrderBlockViewModel shopOrderBlockModel = new ShopOrderBlockViewModel();
                ObservableCollection<ShopOrderDetailBlockViewModel> shopOrderDetailBlockModels = new ObservableCollection<ShopOrderDetailBlockViewModel>();
                foreach (var orderInfo in orderInfos)
                {
                    ShopOrderDetailBlockViewModel shopOrderDetailBlockModel = new ShopOrderDetailBlockViewModel(orderInfo.ImageProduct, orderInfo);
                    shopOrderDetailBlockModels.Add(shopOrderDetailBlockModel);
                }
                shopOrderBlockModel.OrderDetails = shopOrderDetailBlockModels;
                shopOrderBlockModel.Order = order;
                shopOrderBlockModel.Customer = order.MUser;
                shopOrderBlockModel.ShopOrderBlockCommand = ChooseShippingMethodCommand;
                shopOrderBlockModel.CancelledOrderBlockCommand = ChangeToCancelledCommand;
                shopOrderBlockModel.NextStatusContent = "Prepare done";
                ProcessingShopOrderBlockModels.Add(shopOrderBlockModel);
            }
        }
        private async Task LoadDataDeliveringIntoCollection()
        {
            DeliveringShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>();
            ObservableCollection<Models.MOrder> orders = new ObservableCollection<Models.MOrder>(await orderRepposition.GetListAsync(o => o.IdShop == AccountStore.instance.CurrentAccount.Id && o.Status == "Delivering",
                                                                                                                                     o => o.OrderInfoes,
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Rating),
                                                                                                                                     o => o.MUser,
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Product),
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Product).Select(p => p.ImageProducts)));
            orders = new ObservableCollection<Models.MOrder>(orders.OrderByDescending(p => p.DateBegin));
            foreach (var order in orders)
            {
                ObservableCollection<Models.OrderInfo> orderInfos = new ObservableCollection<Models.OrderInfo>(order.OrderInfoes);
                ShopOrderBlockViewModel shopOrderBlockModel = new ShopOrderBlockViewModel();
                ObservableCollection<ShopOrderDetailBlockViewModel> shopOrderDetailBlockModels = new ObservableCollection<ShopOrderDetailBlockViewModel>();
                foreach (var orderInfo in orderInfos)
                {
                    ShopOrderDetailBlockViewModel shopOrderDetailBlockModel = new ShopOrderDetailBlockViewModel(orderInfo.ImageProduct, orderInfo);
                    shopOrderDetailBlockModels.Add(shopOrderDetailBlockModel);
                }
                shopOrderBlockModel.OrderDetails = shopOrderDetailBlockModels;
                shopOrderBlockModel.Order = order;
                shopOrderBlockModel.Customer = order.MUser;
                shopOrderBlockModel.NextStatusContent = "Delivered";
                shopOrderBlockModel.ShopOrderBlockCommand = ChangeToDeliveredCommand;
                DeliveringShopOrderBlockModels.Add(shopOrderBlockModel);
            }
        }
        private async Task LoadDataDeliveredIntoCollection()
        {
            DeliveredShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>();
            ObservableCollection<Models.MOrder> orders = new ObservableCollection<Models.MOrder>(await orderRepposition.GetListAsync(o => o.IdShop == AccountStore.instance.CurrentAccount.Id && o.Status == "Delivered",
                                                                                                                                     o => o.OrderInfoes,
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Rating),
                                                                                                                                     o => o.MUser,
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Product),
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Product).Select(p => p.ImageProducts)));
            orders = new ObservableCollection<Models.MOrder>(orders.OrderByDescending(p => p.DateBegin));
            foreach (var order in orders)
            {
                ObservableCollection<Models.OrderInfo> orderInfos = new ObservableCollection<Models.OrderInfo>(order.OrderInfoes);
                ShopOrderBlockViewModel shopOrderBlockModel = new ShopOrderBlockViewModel();
                ObservableCollection<ShopOrderDetailBlockViewModel> shopOrderDetailBlockModels = new ObservableCollection<ShopOrderDetailBlockViewModel>();
                foreach (var orderInfo in orderInfos)
                {
                    ShopOrderDetailBlockViewModel shopOrderDetailBlockModel = new ShopOrderDetailBlockViewModel(orderInfo.ImageProduct, orderInfo);
                    shopOrderDetailBlockModels.Add(shopOrderDetailBlockModel);
                }
                shopOrderBlockModel.OrderDetails = shopOrderDetailBlockModels;
                shopOrderBlockModel.Order = order;
                shopOrderBlockModel.Customer = order.MUser;
                shopOrderBlockModel.NextStatusContent = "Completed";
                shopOrderBlockModel.ShopOrderBlockCommand = ChangeToCompletedCommand;
                DeliveredShopOrderBlockModels.Add(shopOrderBlockModel);
            }
        }
        private async Task LoadDataCompletedIntoCollection()
        {
            CompletedShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>();
            ObservableCollection<Models.MOrder> orders = new ObservableCollection<Models.MOrder>(await orderRepposition.GetListAsync(o => o.IdShop == AccountStore.instance.CurrentAccount.Id && o.Status == "Completed",
                                                                                                                                     o => o.OrderInfoes,
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Rating),
                                                                                                                                     o => o.MUser,
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Product),
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Product).Select(p => p.ImageProducts)));
            orders = new ObservableCollection<Models.MOrder>(orders.OrderByDescending(p => p.DateBegin));
            foreach (var order in orders)
            {
                ObservableCollection<Models.OrderInfo> orderInfos = new ObservableCollection<Models.OrderInfo>(order.OrderInfoes);
                ShopOrderBlockViewModel shopOrderBlockModel = new ShopOrderBlockViewModel();
                ObservableCollection<ShopOrderDetailBlockViewModel> shopOrderDetailBlockModels = new ObservableCollection<ShopOrderDetailBlockViewModel>();
                foreach (var orderInfo in orderInfos)
                {
                    ShopOrderDetailBlockViewModel shopOrderDetailBlockModel = new ShopOrderDetailBlockViewModel(orderInfo.ImageProduct, orderInfo);
                    shopOrderDetailBlockModels.Add(shopOrderDetailBlockModel);
                }
                shopOrderBlockModel.OrderDetails = shopOrderDetailBlockModels;
                shopOrderBlockModel.Order = order;
                shopOrderBlockModel.Customer = order.MUser;
                CompletedShopOrderBlockModels.Add(shopOrderBlockModel);
            }
        }
        private async Task LoadDataCancelledIntoCollection()
        {
            CancelledShopOrderBlockModels = new ObservableCollection<ShopOrderBlockViewModel>();
            ObservableCollection<Models.MOrder> orders = new ObservableCollection<Models.MOrder>(await orderRepposition.GetListAsync(o => o.IdShop == AccountStore.instance.CurrentAccount.Id && o.Status == "Cancelled",
                                                                                                                                     o => o.OrderInfoes,
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Rating),
                                                                                                                                     o => o.MUser,
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Product),
                                                                                                                                     o => o.OrderInfoes.Select(oi => oi.Product).Select(p => p.ImageProducts)));
            orders = new ObservableCollection<Models.MOrder>(orders.OrderByDescending(p => p.DateBegin));
            foreach (var order in orders)
            {
                ObservableCollection<Models.OrderInfo> orderInfos = new ObservableCollection<Models.OrderInfo>(order.OrderInfoes);
                ShopOrderBlockViewModel shopOrderBlockModel = new ShopOrderBlockViewModel();
                ObservableCollection<ShopOrderDetailBlockViewModel> shopOrderDetailBlockModels = new ObservableCollection<ShopOrderDetailBlockViewModel>();
                foreach (var orderInfo in orderInfos)
                {
                    ShopOrderDetailBlockViewModel shopOrderDetailBlockModel = new ShopOrderDetailBlockViewModel(orderInfo.ImageProduct, orderInfo);
                    shopOrderDetailBlockModels.Add(shopOrderDetailBlockModel);
                }
                shopOrderBlockModel.OrderDetails = shopOrderDetailBlockModels;
                shopOrderBlockModel.Order = order;
                shopOrderBlockModel.Customer = order.MUser;
                CancelledShopOrderBlockModels.Add(shopOrderBlockModel);
            }
        }
        private async Task AddNotification(string Content, string idSender, string idReceiver)
        {
            Models.Notification notification = new Models.Notification();
            notification.Id = await GenerateID.Gen(typeof(Models.Notification));
            notification.Content = Content;
            notification.Date = DateTime.Now;
            notification.IdSender = idSender;
            notification.IdReceiver = idReceiver;
            await notificationRepposition.Add(notification);
        }
        private async Task UpdateShippingMethod(string idOrder, int method)
        {
            Models.MOrder order = await orderRepposition.GetSingleAsync(p => p.Id == idOrder);
            order.ShippingMethod = method;
            await orderRepposition.Update(order);
        }
    }
}