
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
            Items = new ObservableCollection<CategoryRequestItemViewModel>
            {
                new CategoryRequestItemViewModel{CategoryName="a random one", Reason="There is no reason at all"},
                new CategoryRequestItemViewModel{CategoryName="another random name", Reason="There is a lot of reason"},
            };
        }
    }
}
