﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

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
            }
        }
        public bool IsShop
        {
            get
            {
                return Promo.MUser.StatusShop == "NotBanned";
            }
        }
        public bool IsRequest
        {
            get
            {
                return Promo.Status == 0;
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
                    return "Deleted";
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