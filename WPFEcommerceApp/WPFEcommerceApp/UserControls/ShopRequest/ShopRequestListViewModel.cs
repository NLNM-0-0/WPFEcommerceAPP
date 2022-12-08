
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class ShopRequestListViewModel:BaseViewModel
    {
        public ObservableCollection<ShopRequestItemViewModel> Items { get; set; }

        public ShopRequestListViewModel()
        {

        }
    }
}
