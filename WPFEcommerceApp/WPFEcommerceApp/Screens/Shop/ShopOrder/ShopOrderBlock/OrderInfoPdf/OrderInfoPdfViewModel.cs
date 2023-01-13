using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp
{
    public class OrderInfoPdfViewModel: BaseViewModel
    {
        private Models.MOrder order;
        public Models.MOrder Order
        {
            get => order;
            set
            {
                order = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UserAdress));
                OnPropertyChanged(nameof(UserPhoneNumber));
                OnPropertyChanged(nameof(ShopAdress));
                OnPropertyChanged(nameof(ShopPhoneNumber));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(PhoneNumber));
                OnPropertyChanged(nameof(UserName));
            }
        }
        public string UserAdress
        {
            get
            {
                return Order.MUser.Addresses.Where(a => a.Id == Order.AddressIndex).ElementAt(0).Address1??"";
            }
        }
        public string UserName
        {
            get
            {
                return Order.MUser.Addresses.Where(a => a.Id == Order.AddressIndex).ElementAt(0).Name??"";
            }
        }
        public string UserPhoneNumber
        {
            get
            {
                return Order.MUser.Addresses.Where(a => a.Id == Order.AddressIndex).ElementAt(0).PhoneNumber??"";
            }
        }
        public string ShopAdress
        {
            get
            {
                return Order.MUser1.Addresses.Where(a => a.Id == Order.MUser1.DefaultAddress).ElementAt(0).Address1??"";
            }
        }
        public string ShopPhoneNumber
        {
            get
            {
                return Order.MUser1.Addresses.Where(a => a.Id == Order.MUser1.DefaultAddress).ElementAt(0).PhoneNumber??"";
            }
        }
        public string PhoneNumber
        {
            get
            {
                return Properties.Resources.PhoneNumber;
            }
        }
        public string Email
        {
            get
            {
                return Properties.Resources.Email;
            }
        }
        public OrderInfoPdfViewModel(Models.MOrder order)
        {
            Order = order;
        }
    }
}
