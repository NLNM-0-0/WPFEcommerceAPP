
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class AddressItemViewModel: BaseViewModel
    {
        #region Properties

        public Models.Address Address { get; set; }
        public bool IsDefault { get; set; }

        #endregion

        public static ICommand EditAddressCommand { get; set; }
        #region Constructor

        public AddressItemViewModel(Models.Address addr, bool isDefault=false)
        {
            if (addr == null)
                return;

            Address = addr;
            IsDefault = isDefault;
        }
        #endregion
    }
}
