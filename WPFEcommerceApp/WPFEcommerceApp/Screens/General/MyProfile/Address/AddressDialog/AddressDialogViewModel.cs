
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class AddressDialogViewModel: BaseViewModel
    {
        #region Properties
        public string Header { get; set; }
        public string CommandContent { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Address.Name = value;
            }
        }

        private string _phoneNumber { get; set; }
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                Address.PhoneNumber = value;
            }
        }

        private string _thisAddress;
        public string ThisAddress
        {
            get => _thisAddress;
            set
            {
                _thisAddress = value;
                Address.Address1 = value;
            }
        }
        private Models.Address _address;
        public Models.Address Address
        {
            get => _address;
            set
            {
                _address = value;
                tempAddress = new Models.Address { Name = _address.Name, PhoneNumber = _address.PhoneNumber, Address1 = _address.Address1 };
                Name=tempAddress.Name;
                PhoneNumber=tempAddress.PhoneNumber;
                ThisAddress = tempAddress.Address1;
            }
        }
        public bool IsAdding { get; set; }
        public bool IsSetAsDefault { get; set; }
        public bool IsDefault { get; set; }
        private Models.Address tempAddress;
        #endregion

        public static ICommand AddressCommand { get; set; }
        public static ICommand CancelEditCommand { get; set; }
        public static ICommand RemoveAddressCommand { get; set; }

        #region Constructor

        public AddressDialogViewModel()
        {
            CancelEditCommand = new RelayCommandWithNoParameter(() =>
            {
                Name=tempAddress.Name;
                PhoneNumber=tempAddress.PhoneNumber;
                ThisAddress = tempAddress.Address1;
                IsSetAsDefault =IsDefault;
            });
        }

        #endregion
    }
}
