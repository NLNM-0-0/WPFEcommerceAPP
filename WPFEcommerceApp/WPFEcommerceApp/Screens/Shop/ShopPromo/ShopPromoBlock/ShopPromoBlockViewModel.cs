using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ShopPromoBlockViewModel:BaseViewModel
    {
        public ICommand DropdownButtonCommand { get; set; }
        private Models.Promo promo;
        public Models.Promo Promo
        {
            get => promo;
            set
            {
                promo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OverlayBackground));
                OnPropertyChanged(nameof(Icon));
                OnPropertyChanged(nameof(TargetCustomer));
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(StatusOverlay));
                OnPropertyChanged(nameof(IsShop));
                OnPropertyChanged(nameof(IsRequest));
                OnPropertyChanged(nameof(NumberString));
                OnPropertyChanged(nameof(IsDeleted));
            }
        }
        public bool IsShop
        {
            get
            {
                return AccountStore.instance.CurrentAccount.Role=="Shop" && AccountStore.instance.CurrentAccount.StatusShop == "NotBanned";
            }
        }
        public bool IsRequest
        {
            get
            {
                return Promo.Status == 0;
            }
        }
        public bool IsDeleted
        {
            get
            {
                return Promo.Status == 2;
            }
        }
        private bool isDropdown;
        public bool IsDropdown
        {
            get => isDropdown;
            set
            {
                isDropdown = value;    
            }
        }
        public System.Windows.Media.Brush OverlayBackground
        {
            get
            {
                if(Promo.CustomerType == 0)
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(171, 196, 255));
                }
                return new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 169, 255));
            }
        }
        public PackIconKind Icon
        {
            get
            {
                if (Promo.CustomerType == 0)
                {
                    return PackIconKind.AccountStar;
                }
                return PackIconKind.AccountSupervisor;
            }
        }
        public string TargetCustomer
        {
            get
            {
                if (Promo.CustomerType == 0)
                {
                    return "New Customer";
                }
                return "All customer";
            }
        }
        public string Status
        {
            get
            {
                if(Promo.Status == 2)
                {
                    return "Deleted";
                }    
                else if(Promo.Status == 1)
                {
                    if (Promo.DateBegin > DateTime.Now)
                    {
                        return "Upcoming";
                    }
                    else if (Promo.DateBegin <= DateTime.Now && Promo.DateEnd > DateTime.Now)
                    {
                        return "In process";
                    }
                }    
                else if(Promo.Status == 0 && Promo.DateEnd > DateTime.Now)
                {
                    return "Requesting";
                }    
                else if(Promo.Status == 0 && Promo.DateEnd <= DateTime.Now)
                {
                    return "Expired";
                }    
                return "Expired";
            }
        }
        public System.Windows.Media.Brush StatusOverlay
        {
            get
            {
                if (Promo.Status == 2)
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(219, 48, 34));
                }
                else if (Promo.Status == 1)
                {
                    if (Promo.DateBegin > DateTime.Now)
                    {
                        return new SolidColorBrush(System.Windows.Media.Color.FromRgb(42, 169, 82));
                    }
                    else if (Promo.DateBegin <= DateTime.Now && Promo.DateEnd > DateTime.Now)
                    {
                        return new SolidColorBrush(System.Windows.Media.Color.FromRgb(42, 169, 82));
                    }
                }
                else if (Promo.Status == 0 && Promo.DateEnd > DateTime.Now)
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(253, 197, 0));
                }
                return new SolidColorBrush(System.Windows.Media.Color.FromRgb(219, 48, 34));
            }
        }
        public string NumberString
        {
            get 
            {
                if (Promo.Amount != -1)
                {
                    return $"{Promo.AmountUsed} / {Promo.Amount}";
                }
                else
                {
                    return $"{Promo.AmountUsed} / +inf";
                }    
            }
        }
        public void ChangeStatus()
        {
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(StatusOverlay));
            OnPropertyChanged(nameof(IsShop));
            OnPropertyChanged(nameof(IsRequest));
            OnPropertyChanged(nameof(IsDeleted));
        }
        public ShopPromoBlockViewModel(Models.Promo promo)
        {
            Promo = promo;
            IsDropdown = false;
            DropdownButtonCommand = new RelayCommandWithNoParameter(() =>
            {
                IsDropdown = !IsDropdown;
            });
        }
    }
}
