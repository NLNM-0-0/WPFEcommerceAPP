using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class ShopRatingViewModel : BaseViewModel
    {
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
        private DateTime dateFrom;
        public DateTime DateFrom
        {
            get { return dateFrom; }
            set
            {
                dateFrom = value;
                OnPropertyChanged();
            }
        }
        private DateTime dateTo;
        public DateTime DateTo
        {
            get { return dateTo; }
            set
            {
                dateTo = value;
                OnPropertyChanged();
            }
        }
        private string category;
        public string Category
        {
            get { return category; }
            set
            {
                category = value;
                OnPropertyChanged();
            }
        }
        private string brand;
        public string Brand
        {
            get { return brand; }
            set
            {
                brand = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Boolean> statusStyles;
        public ObservableCollection<Boolean> StatusStyles
        {
            get { return statusStyles; }
            set
            {
                statusStyles = value;
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

        public ShopRatingViewModel()
        {
            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
            StatusStyles = new ObservableCollection<Boolean>();
            StatusStyles.Add(true);
            StatusStyles.Add(false);
            StatusStyles.Add(false);
            RatingStarStyles = new ObservableCollection<Boolean>();
            RatingStarStyles.Add(true);
            RatingStarStyles.Add(false);
            RatingStarStyles.Add(false);
            RatingStarStyles.Add(false);
            RatingStarStyles.Add(false);
            RatingStarStyles.Add(false);
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
                else if (style == "4 Star")
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
            ChangeRatingStatusCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                string status = (p as RadioButton).Content.ToString();
                int index = 0;
                if (status == "All")
                {
                    index = 0;
                }
                else if (status == "To Reply")
                {
                    index = 1;
                }
                else if (status == "Replied")
                {
                    index = 2;
                }
                for (int i = 0; i < StatusStyles.Count; i++)
                {
                    StatusStyles[i] = false;
                }
                StatusStyles[index] = true;
            });
            ResetCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                ProductName = "";
                UserName = "";
                DateFrom = DateTime.Now;
                DateTo = DateTime.Now;
                Category = "";
                Brand = "";
                for (int i = 0; i < RatingStarStyles.Count; i++)
                {
                    RatingStarStyles[i] = false;
                }
                RatingStarStyles[0] = true;
                for (int i = 0; i < StatusStyles.Count; i++)
                {
                    StatusStyles[i] = false;
                }
                StatusStyles[0] = true;
            });
        }
    }
}
