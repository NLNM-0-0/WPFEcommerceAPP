using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AdminBrandViewModel : BaseViewModel, IAsyncInitialization
    {
        #region Properties
        public GenericDataRepository<Brand> BrandRepository { get; set; }
        public GenericDataRepository<BrandRequest> RequestRepo { get; set; }

        private ObservableCollection<Brand> _filteredBrands;

        public ObservableCollection<Brand> FilteredBrands
        {
            get
            {
                return _filteredBrands;
            }

            set
            {
                _filteredBrands = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Brand> _brandToSearch;
        private ObservableCollection<Brand> bannedBrands;
        private ObservableCollection<Brand> notBannedBrands;

        private BrandRequestListViewModel _requestList;

        public BrandRequestListViewModel RequestList
        {
            get
            {
                return _requestList;
            }

            set
            {
                _requestList = value;
                OnPropertyChanged();
            }
        }

        private string _newBrandName { get; set; }
        public string NewBrandName
        {
            get => _newBrandName;
            set { _newBrandName = value; OnPropertyChanged(); }
        }

        private string _searchBy;

        public string SearchBy
        {
            get { return _searchBy; }
            set { _searchBy = value; OnPropertyChanged(); }
        }

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; Search(); OnPropertyChanged(); }
        }

        private string _lastSearchText;
        private string _lastSearchOption;

        private Brand _selectedItem;

        public Brand SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }


        public List<string> SearchByOptions { get; set; }
        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value)
                {
                    StatusText = "Not Banned";
                    FilteredBrands = _brandToSearch = notBannedBrands;
                    RemoveOrRestore = "Remove";
                }
                else
                {
                    StatusText = "Banned";
                    FilteredBrands = _brandToSearch = bannedBrands;
                    RemoveOrRestore = "Restore";
                }

                _isChecked = value;
            }
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        private string _removeOrUnBanned;
        public string RemoveOrRestore
        {
            get => _removeOrUnBanned;
            set { _removeOrUnBanned = value; OnPropertyChanged(); }
        }


        #endregion

        #region Commands
        public ICommand AddNewBrandCommand { get; set; }
        public ICommand RemoveBrandCommand { get; set; }

        public ICommand RemoveRequestCommand { get; set; }
        public ICommand AddRequestCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand CloseSearchCommand { get; set; }

        public Task InitializeAsync { get; private set; }

        #endregion

        #region Constructor
        public AdminBrandViewModel()
        {
            BrandRepository = new GenericDataRepository<Brand>();
            RequestRepo = new GenericDataRepository<BrandRequest>();
            var repo = new GenericDataRepository<Notification>();

            InitializeAsync = Load();

            RequestList = new BrandRequestListViewModel();
            SearchByOptions = new List<string> { "ID", "Name" };
            SearchBy = SearchByOptions[1];

            AddNewBrandCommand = new RelayCommand<string>(p => p != string.Empty, async (p) => await AddNewBrand(p));
            RemoveBrandCommand = new RelayCommand<object>(p => p != null, async (p) => await RemoveBrand(p));
            RemoveRequestCommand = new RelayCommand<object>(p => p != null, async (p) => await RemoveRequest(p));
            AddRequestCommand = new RelayCommand<object>(p => p != null, async (p) => await AddRequest(p));
            SearchCommand = new RelayCommandWithNoParameter(Search);
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearch);

            _lastSearchText = string.Empty;
            _lastSearchOption = string.Empty;
        }


        #endregion

        #region Command Methods
        public async Task Load()
        {

            IsChecked = true;
            RemoveOrRestore = "Remove";

            FilteredBrands = new ObservableCollection<Brand>(
                await BrandRepository.GetListAsync(br => br.Status.Equals(Status.NotBanned.ToString())));

            bannedBrands = new ObservableCollection<Brand>(
                await BrandRepository.GetListAsync(br => br.Status.Equals(Status.Banned.ToString())));

            _brandToSearch = FilteredBrands;
            notBannedBrands = FilteredBrands;

            var query = await RequestRepo.GetAllAsync(item => item.MUser);

            RequestList.Items = new ObservableCollection<BrandRequestItemViewModel>(
                        query.Select(item => new BrandRequestItemViewModel
                        {
                            RequestId = item.Id,
                            Id = item.MUser.Id,
                            Name = item.Name,
                            BrandName = item.Name,
                            SourceImageAva = new BitmapImage(new Uri(item.MUser.SourceImageAva)),
                            Reason = item.Reason,
                        }));


        }
        public async Task AddNewBrand(string brandName)
        {
            NewBrandName = string.Empty;
            var brand = await BrandRepository.GetSingleAsync(item => item.Name.Equals(brandName));
            if (brand != null)
            {
                if (brand.Status == Status.Banned.ToString())
                {
                    brand.Status = Status.NotBanned.ToString();
                    await BrandRepository.Update(brand);
                }
            }
            else
            {
                await BrandRepository.Add(new Brand { Name = brandName, Status = Status.NotBanned.ToString() });
            }
            await Load();
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public async Task RemoveBrand(object obj)
        {
            var removeBrand = obj as Brand;
            if (removeBrand == null)
                return;

            if (removeBrand.Status == Status.NotBanned.ToString())
                removeBrand.Status = Status.Banned.ToString();
            else
                removeBrand.Status = Status.NotBanned.ToString();

            await BrandRepository.Update(removeBrand);
            await Load();
        }

        public async Task RemoveRequest(object obj)
        {
            var request = obj as BrandRequestItemViewModel;
            if (request == null)
                return;

            var removeRequest = await RequestRepo.GetSingleAsync(item => item.Id.Equals(request.RequestId));
            await RequestRepo.Remove(removeRequest);
            await Load();
        }

        public async Task AddRequest(object obj)
        {

            var request = obj as BrandRequestItemViewModel;
            if (request == null)
                return;

            var brand = await BrandRepository.GetSingleAsync(item => item.Name.Equals(request.BrandName));
            if (brand != null)
            {
                if (brand.Status == Status.Banned.ToString())
                {
                    brand.Status = Status.NotBanned.ToString();
                    await BrandRepository.Update(brand);
                }
            }
            else
            {
                await BrandRepository.Add(new Brand { Name = request.BrandName, Status = Status.NotBanned.ToString() });
            }

            var removeRequest = await RequestRepo.GetSingleAsync(item => item.Id.Equals(request.RequestId));
            await RequestRepo.Remove(removeRequest);
            await Load();


        }

        public void Search()

        {
            if (string.IsNullOrEmpty(SearchBy))
                FilteredBrands = _brandToSearch;

            if (string.IsNullOrEmpty(_lastSearchText) && string.IsNullOrEmpty(SearchText) ||
                (string.Equals(_lastSearchText, SearchText) && _lastSearchOption == SearchBy))
            {
                FilteredBrands = _brandToSearch;
            }

            if (string.IsNullOrEmpty(SearchText) || _brandToSearch.Count <= 0 || _brandToSearch == null)
            {
                FilteredBrands = _brandToSearch;
                return;
            }

            if (SearchBy == "Name")
            {
                _lastSearchOption = "Name";
                FilteredBrands = new ObservableCollection<Brand>(_brandToSearch.Where(br => br.Name.ToLower().Contains(SearchText.ToLower())));
            }
            else if (SearchBy == "ID")
            {
                _lastSearchOption = "ID";
                FilteredBrands = new ObservableCollection<Brand>(_brandToSearch.Where(br => br.Id.ToString().ToLower().Contains(SearchText.ToLower())));
            }



        }

        public void CloseSearch()
        {
            SearchText = string.Empty;
        }
        #endregion
    }
}