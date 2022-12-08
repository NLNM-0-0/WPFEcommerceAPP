using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class HeaderViewModel: BaseViewModel
    {
        #region Commands
        public ICommand OpenSearchCommand { get; set; }
        public ICommand CloseSearchCommand { get; set; }
        public ICommand ClosePopupCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        #endregion

        #region Properties
        private bool _isSearchOpen = false;
        public bool IsSearchOpen
        {
            get { return _isSearchOpen; }
            set { _isSearchOpen = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SearchItemViewModel> _itemsSource;
        public ObservableCollection<SearchItemViewModel> ItemsSource
        {
            get { return _itemsSource; }
            set { _itemsSource = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SearchItemViewModel> _allItems;
        public ObservableCollection<SearchItemViewModel> AllItems
        {
            get { return _allItems; }
            set { _allItems = value; OnPropertyChanged(); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText == value)
                    return;
                _searchText = value;
                OnPropertyChanged("SearchText");
                Search();
            }
        }

        private ObservableCollection<SearchItemViewModel> _itemsDefault;
        public ObservableCollection<SearchItemViewModel> ItemsDefault
        {
            get { return _itemsDefault; }
            set { _itemsDefault = value; OnPropertyChanged(); }
        }
        #endregion

        public HeaderViewModel()
        {
            OpenSearchCommand= new RelayCommandWithNoParameter(OpenSearch);
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearchText);
            ClosePopupCommand = new RelayCommandWithNoParameter(ClosePopup);
            SearchCommand= new RelayCommandWithNoParameter(Search);

            ItemsDefault = new ObservableCollection<SearchItemViewModel>
            {
                new SearchItemViewModel{ Name="Say hi"},
                new SearchItemViewModel{ Name="A nice day"},
            };

            SearchText = string.Empty;
            ItemsSource = ItemsDefault;

            AllItems = new ObservableCollection<SearchItemViewModel>
            {
                new SearchItemViewModel{Name="This is a nice day"},
                new SearchItemViewModel{ Name="Say hi"},
                new SearchItemViewModel{ Name="A nice day"},
                new SearchItemViewModel{Name="That's fine"},
                new SearchItemViewModel{Name="Trousers"},
                new SearchItemViewModel{Name="Sweater"},

            };

        }

        #region Command Methods
        public void OpenSearch()
        {
            IsSearchOpen= true;
        }

        public void CloseSearchText()
        {
            SearchText = string.Empty;
        }

        public void ClosePopup()
        {
            IsSearchOpen= false;
            SearchText = string.Empty;
            ItemsSource = ItemsDefault;
        }

        public void Search()
        {
            if (SearchText == string.Empty)
               ItemsSource = new ObservableCollection<SearchItemViewModel>( ItemsDefault);

            else 
                ItemsSource = new ObservableCollection<SearchItemViewModel>(AllItems.Where(item => (item.Name.ToLower()).Contains(SearchText.ToLower())));
        }
        #endregion
    }
}
