
using System.Collections.ObjectModel;

namespace WPFEcommerceApp
{
    public class CategoryRequestListViewModel: BaseViewModel
    {
        private ObservableCollection<CategoryRequestItemViewModel> _items;
        public ObservableCollection<CategoryRequestItemViewModel> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged(); }
        }

        public CategoryRequestListViewModel()
        {

        }
    }
}
