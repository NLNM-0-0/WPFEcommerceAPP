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
            FromSelectedDate = new DateTime(2022, 11, 1);
            ToSelectedDate =DateTime.Now;
            LoadCommand = new RelayCommandWithNoParameter(async () => await Load());

            Task.Run(async()=>await Load());

        }

        #endregion

        public async Task Load()
        {
            OrderInfos = new List<OrderInfo>(await orderinfoRepo.GetListAsync(
                ord => ord.MOrder.IdShop == AccountStore.instance.CurrentAccount.Id,
                ord => ord.MOrder, ord=>ord.Rating));

            var fromDate = FromSelectedDate.Date;
            var toDate = ToSelectedDate.Date;

            #region 4 number on the left
            long totalSale = 0;
            foreach (var ord in OrderInfos)
            {
                if (ord.MOrder.Status == OrderStatus.Completed.ToString()||ord.MOrder.Status==OrderStatus.Delivered.ToString())
                {
                    if (!string.IsNullOrEmpty(ord.MOrder.DateEnd.ToString()))
                    {
                        var ordDate = ord.MOrder.DateEnd;
                        if (ordDate.Value.Date >= fromDate && ordDate.Value.Date <= toDate)
                            totalSale += ord.TotalPrice;
                    }
                }
            }
            orderRepo = new GenericDataRepository<MOrder>();
            var orders = new List<MOrder>(
                await orderRepo.GetListAsync(
                    ord => ord.Status == OrderStatus.Processing.ToString()
                    && ord.IdShop == AccountStore.instance.CurrentAccount.Id
                    && ord.DateBegin>=fromDate
                    && ord.DateBegin<=toDate));

            TotalSales = totalSale.ToString();
            Orders = orders.Count.ToString();

            #endregion

            #region Revenue


            var dayCount = (toDate - fromDate).Days / 30;
            var labels = new List<string>();

            if(dayCount<=0)
            {
                for(int i=0; ;i++)
                {
                    var day = (fromDate + new TimeSpan(i, 0, 0, 0));
                    if (day >= toDate)
                        break;
                    labels.Add(day.ToString("dd/MM/yyyy"));
                }
            }
            else
            {
                for (int i = 0; ; i++)
                {
                    var day = (fromDate + new TimeSpan(i * dayCount, 0, 0, 0));
                    if (day >= toDate)
                        break;
                    labels.Add(day.ToString("dd/MM/yyyy"));
                }
            }   
            labels.Add(toDate.ToString("dd/MM/yyy"));
            RevenueLabels = labels.ToArray();
            yRevenueFormatter = (value) => value.ToString("C");

            var revenueList = new List<long> ();

            var dayList = new List<string>(RevenueLabels).Select(item => DateTime.Parse(item));

            foreach (var day in dayList)
            {
                long temp = 0;
                foreach (var ord in OrderInfos)
                {
                    if (ord.MOrder.DateEnd == null)
                        continue;
                    if ((ord.MOrder.Status == OrderStatus.Delivered.ToString() || ord.MOrder.Status == OrderStatus.Completed.ToString())
                        && ord.MOrder.DateEnd == day)
                        temp += ord.TotalPrice;
                }
                revenueList.Add(temp);
            }

            RevenueSeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title="Revenue",
                    Values=new ChartValues<long>(revenueList)
                },
            };
            #endregion

            #region Order Status
            var statusList = new List<double> { 0, 0, 0, 0, 0};
            foreach(var ord in OrderInfos)
            {
                if(ord.MOrder.DateBegin>=fromDate&& ord.MOrder.DateBegin<=toDate|| ord.MOrder.DateEnd >= fromDate && ord.MOrder.DateEnd <= toDate)
                {
                    if (ord.MOrder.Status == OrderStatus.Processing.ToString())
                        statusList[0]++;
                    else if (ord.MOrder.Status == OrderStatus.Delivering.ToString())
                        statusList[1]++;
                    else if (ord.MOrder.Status == OrderStatus.Delivered.ToString())
                        statusList[2]++;
                    else if (ord.MOrder.Status == OrderStatus.Cancelled.ToString())
                        statusList[3]++;
                    else if (ord.MOrder.Status == OrderStatus.Completed.ToString())
                        statusList[4]++;
                }
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
            OrderStatusFormatter = value => value.ToString("N");


            #endregion

            #region Review
            List<int> starList = new List<int> { 0, 0, 0, 0 ,0};
            foreach (var ord in OrderInfos)
            {
                if (ord.MOrder.DateBegin >= fromDate && ord.MOrder.DateBegin <= toDate || ord.MOrder.DateEnd >= fromDate && ord.MOrder.DateEnd <= toDate)
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
        }
    }
}
