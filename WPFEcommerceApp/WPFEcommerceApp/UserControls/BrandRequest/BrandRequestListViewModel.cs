using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class BrandRequestListViewModel:BaseViewModel
    {
        private ObservableCollection<BrandRequestItemViewModel> _items;
        public ObservableCollection<BrandRequestItemViewModel> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged(); }
        }

        public BrandRequestListViewModel()
        {
            Items = new ObservableCollection<BrandRequestItemViewModel>
            {
                new BrandRequestItemViewModel{BrandName="BrandName1", Reason="Some reason"},
                new BrandRequestItemViewModel{BrandName="BrandName2", Reason="Some reason"},
            };
        }
    }
}
