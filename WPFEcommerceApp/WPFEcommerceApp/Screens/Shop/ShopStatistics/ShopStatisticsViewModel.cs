using LiveCharts;
using System;
using LiveCharts.Wpf;
using System.Windows.Media;
using LiveCharts.Defaults;
using DataAccessLayer;
using WPFEcommerceApp.Models;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Globalization;
using System.Security.Cryptography;
using MaterialDesignThemes.Wpf;

namespace WPFEcommerceApp
{
    public class ShopStatisticsViewModel : BaseViewModel
    {

        #region Public Properties
        private GenericDataRepository<OrderInfo> orderinfoRepo;
        private GenericDataRepository<MOrder> orderRepo;
        public List<OrderInfo> OrderInfos;

        private DateTime _fromSelectedDate;
        public DateTime FromSelectedDate
        {
            get => _fromSelectedDate;
            set
            {
                _fromSelectedDate = value;
                OnPropertyChanged();
            }
        }
        public DateTime FromSelectedDateMax => ToSelectedDate - new TimeSpan(1, 0, 0, 0);
        public DateTime ToSelectedDateMin => FromSelectedDate + new TimeSpan(1, 0, 0, 0);
        private DateTime _toSelectedDate;
        public DateTime ToSelectedDate
        {
            get => _toSelectedDate;
            set
            {
                _toSelectedDate = value;
                OnPropertyChanged();
            }
        }
        public string TotalSales { get; set; }
        public string Orders { get; set; }

        public Func<double, string> yRevenueFormatter { get; set; }
        public SeriesCollection RevenueSeriesCollection { get; set; }
        public string[] RevenueLabels { get; set; }

        public SeriesCollection OrderStatusSeriesCollection { get; set; }
        public string[] OrderStatusLabels { get; set; }
        public Func<double, string> OrderStatusFormatter { get; set; }

        public SeriesCollection ReviewSeriesCollection { get; set; }

        #endregion

        #region Commands
        public ICommand LoadCommand { get; set; }

        #endregion

        #region Constructor
        public ShopStatisticsViewModel()
        {

            orderinfoRepo = new GenericDataRepository<OrderInfo>();
            LoadCommand = new RelayCommandWithNoParameter(async () =>
            {
                if (FromSelectedDate > ToSelectedDate)
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    var view = new ConfirmDialog { Header = "Day invalid", Content = "The beginning day is later than the end day, choose again!" };
                    await DialogHost.Show(view, "Main");
                    FromSelectedDate = ToSelectedDate - new TimeSpan(30, 0, 0, 0);
                    return;
                }
                else
                    await Load();
            });

            FromSelectedDate = DateTime.Now - new TimeSpan(30, 0, 0, 0);
            ToSelectedDate = DateTime.Now;

            Task.Run(async () => await Load()).ContinueWith((p) =>
            {
                FromSelectedDate = DateTime.Now - new TimeSpan(30, 0, 0, 0);
                ToSelectedDate = DateTime.Now;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        public async Task Load()
        {
            MainViewModel.SetLoading(true);

            if(AccountStore.instance.CurrentAccount == null) {
                MainViewModel.SetLoading(false);
                return;
            }

            OrderInfos = new List<OrderInfo>(await orderinfoRepo.GetListAsync(
                ord => ord.MOrder.IdShop == AccountStore.instance.CurrentAccount.Id,
                ord => ord.MOrder, ord => ord.Rating));
            var id = AccountStore.instance.CurrentAccount.Id;

            var fromDate = FromSelectedDate.Date;
            var toDate = ToSelectedDate.Date;

            #region 4 number on the left
            orderRepo = new GenericDataRepository<MOrder>();
            var orders = new List<MOrder>(
                await orderRepo.GetListAsync(
                    ord => ord.IdShop == AccountStore.instance.CurrentAccount.Id
                    && (ord.DateBegin.Value.Date >= fromDate
                    && ord.DateBegin.Value.Date <= toDate
                    || (ord.DateEnd != null && (ord.DateEnd.Value.Date >= fromDate && ord.DateEnd.Value.Date <= toDate)))));

            double totalSale = 0;
            foreach (var ord in orders)
            {
                if (ord.Status == OrderStatus.Completed.ToString() || ord.Status == OrderStatus.Delivered.ToString())
                {
                    if (!string.IsNullOrEmpty(ord.DateEnd.ToString()))
                    {
                        var ordDate = ord.DateEnd;
                        if (ordDate.Value.Date >= fromDate && ordDate.Value.Date <= toDate)
                            totalSale += ord.OrderTotal;
                    }
                }
            }
            TotalSales = totalSale.ToString();
            Orders = orders.Count.ToString();

            #endregion

            #region Revenue

            var labels = new List<string>();
            var dayList = new List<DateTime>();

            var revenueList = new List<double>();


            for (int i = 0; ; i++)
            {
                var day = (fromDate + new TimeSpan(i, 0, 0, 0));
                labels.Add(day.ToString("dd/MM/yyyy"));
                dayList.Add(day);
                if (day >= toDate)
                    break;
            }

            foreach (var day in dayList)
            {
                double temp = 0;
                foreach (var ord in orders)
                {
                    if (ord.DateEnd == null)
                        continue;
                    if ((ord.Status == OrderStatus.Delivered.ToString() || ord.Status == OrderStatus.Completed.ToString())
                        && ord.DateEnd.Value.Date == day)
                        temp += ord.OrderTotal;
                }
                revenueList.Add(temp);
            }

            RevenueLabels = labels.ToArray();

            yRevenueFormatter = (value) => value.ToString("C");

            RevenueSeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title="Revenue",
                    Values=new ChartValues<double>(revenueList),
                    PointGeometrySize=0,
                },
            };
            #endregion

            #region Order Status
            var statusList = new List<double> { 0, 0, 0, 0, 0 };
            foreach (var ord in orders)
            {
                if (ord.Status == OrderStatus.Processing.ToString())
                    statusList[0]++;
                else if (ord.Status == OrderStatus.Delivering.ToString())
                    statusList[1]++;
                else if (ord.Status == OrderStatus.Delivered.ToString())
                    statusList[2]++;
                else if (ord.Status == OrderStatus.Cancelled.ToString())
                    statusList[3]++;
                else if (ord.Status == OrderStatus.Completed.ToString())
                    statusList[4]++;
            }

            OrderStatusSeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = String.Empty,
                    Values = new ChartValues<double> (statusList)
                }
            };


            OrderStatusLabels = new[] { "Processing", "Delivering", "Delivered", "Cancelled", "Completed" };
            OrderStatusFormatter = value => value.ToString("g");


            #endregion

            #region Review
            List<int> starList = new List<int> { 0, 0, 0, 0, 0 };
            foreach (var ord in OrderInfos)
            {
                if (ord.MOrder.DateBegin.HasValue && ord.MOrder.DateBegin.Value.Date >= fromDate && ord.MOrder.DateBegin.Value.Date <= toDate
                    || ord.MOrder.DateEnd.HasValue && ord.MOrder.DateEnd.Value.Date >= fromDate && ord.MOrder.DateEnd.Value.Date <= toDate)
                {
                    if (ord.Rating == null)
                        continue;
                    if (ord.Rating.Rating1 == 1)
                        starList[0]++;
                    else if (ord.Rating.Rating1 == 2)
                        starList[1]++;
                    else if (ord.Rating.Rating1 == 3)
                        starList[2]++;
                    else if (ord.Rating.Rating1 == 4)
                        starList[3]++;
                    else if (ord.Rating.Rating1 == 5)
                        starList[4]++;
                }

            }

            ReviewSeriesCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "5 stars",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(starList[4]) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "4 stars",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(starList[3]) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "3 stars",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(starList[2]) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "2 stars",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(starList[1]) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "1 star",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(starList[0]) },
                    DataLabels = true
                }
            };
            #endregion

            MainViewModel.SetLoading(false);
        }
    }
}
