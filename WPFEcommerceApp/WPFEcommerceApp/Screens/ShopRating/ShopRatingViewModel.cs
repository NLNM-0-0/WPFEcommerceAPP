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
        private readonly AccountStore _accountStore;
        private GenericDataRepository<Models.MOrder> orderReposition = new GenericDataRepository<Models.MOrder>();
        private GenericDataRepository<Models.OrderInfo> orderInfoReposition = new GenericDataRepository<Models.OrderInfo>();
        private GenericDataRepository<Models.Brand> brandReposition = new GenericDataRepository<Models.Brand>();
        private GenericDataRepository<Models.Category> categoryReposition = new GenericDataRepository<Models.Category>();
        public ICommand ChangeRatingStarStyleCommand { get; set; }
        public ICommand ChangeRatingStatusCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ResetCommand { get; set; }
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
        private ObservableCollection<Boolean> ratingStarStyles;
        public ObservableCollection<Boolean> RatingStarStyles
        {
            get { return ratingStarStyles; }
            set
            {
                ratingStarStyles = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ShopRatingBlockModel> shopRatingBlockModels;
        public ObservableCollection<ShopRatingBlockModel> ShopRatingBlockModels
        {
            get { return shopRatingBlockModels; }
            set
            {
                shopRatingBlockModels = value;
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
                OnPropertyChanged();
            }
        }
        public ShopRatingViewModel(AccountStore accountStore)
        {
            _accountStore = accountStore;
            Load();
            Task t = Task.Run(async () =>
            {
                await LoadListData();
                await LoadBrand();
                await LoadCategory();
            });
            while(!t.IsCompleted);
            DisplayShopRatingBlockModels = ShopRatingBlockModels;
            ChangeRatingStarStyleCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                string style = (p as RadioButton).Content.ToString();
                int index = 0;
                if (style == "All")
                {
                    index = 0;
                }
                else if (style == "5 Star")
                {
                    index = 1;
                }
                else if (style == "4 Star")
                {
                    index = 2;
                }
                else if (style == "3 Star")
                {
                    index = 3;
                }
                else if (style == "2 Star")
                {
                    index = 4;
                }
                else
                {
                    index = 5;
                }
                for (int i = 0; i < RatingStarStyles.Count; i++)
                {
                    RatingStarStyles[i] = false;
                }
                RatingStarStyles[index] = true;
            });
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
                MainViewModel.IsLoading = true;
                Search();
                MainViewModel.IsLoading = false;
            });
            ResetCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                MainViewModel.IsLoading = true;
                ProductName = "";
                UserName = "";
                DateFrom = null;
                DateTo = null;
                SelectedCategory = null;
                SelectedBrand = null;
                for (int i = 0; i < RatingStarStyles.Count; i++)
                {
                    RatingStarStyles[i] = false;
                }
                RatingStarStyles[0] = true;
                Search();
                MainViewModel.IsLoading = false;
            });
        }
        public void Search()
        {
            if (DateFrom != null && DateTo != null && DateFrom > DateTo)
            {
                MessageBox.Show("Date Wrong");
                return;
            }
            int ratingPoint = 6;
            for (int i = 0; i < RatingStarStyles.Count; i++)
            {
                if (RatingStarStyles[i])
                {
                    ratingPoint = 6 - i;
                }
            }
            DisplayShopRatingBlockModels = new ObservableCollection<ShopRatingBlockModel>(ShopRatingBlockModels.Where(x => x.OrderInfo.Product.Name.Contains(ProductName ?? "") &&
                                                                                                                 x.Customer.Name.Contains(UserName ?? "") &&
                                                                                                                ((SelectedCategory == null) ? true : (x.OrderInfo.Product.IdCategory == SelectedCategory.Id)) &&
                                                                                                                ((SelectedBrand == null) ? true : (x.OrderInfo.Product.IdBrand == SelectedBrand.Id)) &&
                                                                                                                ((DateFrom == null) ? true : (x.OrderInfo.Rating.DateRating >= DateFrom)) &&
                                                                                                                ((DateTo == null) ? true : (x.OrderInfo.Rating.DateRating <= DateTo)) &&
                                                                                                                (ratingPoint == 6 ? true : (x.OrderInfo.Rating.Rating1 == ratingPoint))).ToList());
        }
        public void Load()
        {
            ShopRatingBlockModels = new ObservableCollection<ShopRatingBlockModel>();
            RatingStarStyles = new ObservableCollection<Boolean>();
            DateFrom = null;
            DateTo = null;
            RatingStarStyles.Add(true);
            RatingStarStyles.Add(false);
            RatingStarStyles.Add(false);
            RatingStarStyles.Add(false);
            RatingStarStyles.Add(false);
            RatingStarStyles.Add(false);
        }
        public async Task LoadListData()
        {
            ObservableCollection<Models.MOrder> orders = new ObservableCollection<Models.MOrder>(await orderReposition.GetListAsync(r => (r != null && r.IdShop == _accountStore.CurrentAccount.Id && r.Status == "Completed"),
                                                                                                                                    r => r.MUser));
            orders = new ObservableCollection<Models.MOrder>(orders.OrderByDescending(p => p.DateBegin));
            foreach (Models.MOrder order in orders)
            {
                ObservableCollection<Models.OrderInfo> orderInfos = new ObservableCollection<OrderInfo>(await orderInfoReposition.GetListAsync(oi => (oi.IdOrder == order.Id && oi.IdRating != null),
                                                                                                                                               oi => oi.Product,
                                                                                                                                               oi => oi.Rating));
                for (int i = 0; i < orderInfos.Count; i++)
                {
                    ShopRatingBlockModel shopRatingBlockModel = new ShopRatingBlockModel()
                    {
                        Order = order,
                        Customer = order.MUser,
                        OrderInfo = orderInfos[i],
                        ImageProduct = orderInfos[i].ImageProduct
                    };
                    ShopRatingBlockModels.Add(shopRatingBlockModel);
                }
            }
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
