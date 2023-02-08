using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ShopOrderBlockViewModel : BaseViewModel
    {
        private Models.MUser customer;
        public Models.MUser Customer
        {
            get => customer;
            set
            {
                customer = value;
                OnPropertyChanged();
            }
        }
        public string SourceImageAva
        {
            get
            {
                if (Customer == null || String.IsNullOrEmpty(Customer.SourceImageAva))
                {
                    return Properties.Resources.DefaultShopAvaImage;
                }
                else
                {
                    return Customer.SourceImageAva;
                }
            }
        }
        private Models.MOrder order;
        public Models.MOrder Order
        {
            get => order;
            set
            {
                order = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCanCommandExcute));
                OnPropertyChanged(nameof(IsProcessing));
                OnPropertyChanged(nameof(ForegroundStatus));
            }
        }
        private ObservableCollection<ShopOrderDetailBlockViewModel> orderDetails;
        public ObservableCollection<ShopOrderDetailBlockViewModel> OrderDetails
        {
            get => orderDetails;
            set
            {
                orderDetails = value;
                OnPropertyChanged();
            }
        }
        private ICommand shopOrderBlockCommand;
        public ICommand ShopOrderBlockCommand
        {
            get => shopOrderBlockCommand;
            set
            {
                shopOrderBlockCommand = value;
                OnPropertyChanged();
            }
        }
        private ICommand cancelledOrderBlockCommand;
        public ICommand CancelledOrderBlockCommand
        {
            get => cancelledOrderBlockCommand;
            set
            {
                cancelledOrderBlockCommand = value;
                OnPropertyChanged();
            }
        }
        private ICommand printCommand;
        public ICommand PrintCommand
        {
            get => printCommand;
            set
            {
                printCommand = value;
                OnPropertyChanged();
            }
        }
        public ICommand DoubleClickCommand { get; set; } = new RelayCommandWithNoParameter(() => { });
        public string DateFrom
        {
            get
            {
                if (Order.DateBegin == null)
                {
                    return "";
                }
                else
                {
                    return String.Format("{0:d}", Order.DateBegin);
                }
            }
        }
        public string DateTo
        {
            get
            {
                if (Order.DateEnd == null)
                {
                    return "";
                }
                else
                {
                    return String.Format("{0:d}", Order.DateEnd);
                }
            }
        }
        public System.Windows.Media.Brush ForegroundStatus
        {
            get
            {
                if (Order.Status == "Cancelled")
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(219, 48, 34));
                }
                else if(Order.Status == "Completed" || Order.Status == "Delivered")
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(42, 169, 82));
                }
                else
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(253, 197, 0));
                }
            }
        }
        public string ShippingSpeed
        {
            get
            {
                if (Order.ShippingSpeedMethod == 0)
                {
                    return "Normal ship";
                }
                else 
                {
                    return "Fast ship";
                }
            }
        }
        private string nextStatusContent;
        public string NextStatusContent
        {
            get => nextStatusContent;
            set
            {
                nextStatusContent = value;
            }
        }
        public bool IsCanCommandExcute
        {
            get => !(Order.Status == "Completed" || Order.Status == "Cancelled");
        }
        public bool IsProcessing
        {
            get => Order.Status == "Processing";
        }
        public ShopOrderBlockViewModel()
        {
            PrintCommand = new RelayCommandWithNoParameter(()=>
            {
                MainViewModel.SetLoading(true);
                OrderInfoPdf pdf = new OrderInfoPdf();
                pdf.DataContext = new OrderInfoPdfViewModel(Order);
                pdf.Print();
                MainViewModel.SetLoading(false); 
            });
        }
    }
}
