using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AdminBrandViewModel : BaseViewModel
    {
        #region Properties
        private GenericDataRepository<Brand> BrandRepository;
        private GenericDataRepository<BrandRequest> RequestRepo;
        private GenericDataRepository<Models.Product> ProdRepo;
        private GenericDataRepository<Models.Notification> NoteRepo;

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
            set { _searchText = value; OnPropertyChanged(); }
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
                    RemoveOrRestore = "Ban";
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

        #endregion

        #region Constructor
        public AdminBrandViewModel()
        {
            MainViewModel.SetLoading(true);

            BrandRepository = new GenericDataRepository<Brand>();
            RequestRepo = new GenericDataRepository<BrandRequest>();
            ProdRepo = new GenericDataRepository<Models.Product>();
            NoteRepo = new GenericDataRepository<Models.Notification>();
            RequestList = new BrandRequestListViewModel();
            SearchByOptions = new List<string> { "ID", "Name" };
            SearchBy = SearchByOptions[1];

            AddNewBrandCommand = new RelayCommand<string>(p => p != string.Empty, async (p) => await AddNewBrand(p));
            RemoveBrandCommand = new RelayCommand<object>(p => p != null, async (p) => await RemoveBrand(p));
            RemoveRequestCommand = new RelayCommand<object>(p => p != null, async (p) => await RemoveRequest(p));
            AddRequestCommand = new RelayCommand<object>(p => p != null, async (p) => await AddRequest(p));
            SearchCommand = new RelayCommandWithNoParameter(async()=>await Search());
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearch);

            _lastSearchText = string.Empty;
            _lastSearchOption = string.Empty;

            Task.Run(async () =>
            {
                MainViewModel.SetLoading(true);
                await Load();
            }).ContinueWith((first) =>
            {
                MainViewModel.SetLoading(false);

            });

            

        }

        #endregion

        #region Command Methods
        public async Task Load()
        {
            IsChecked = true;
            RemoveOrRestore = "Ban";

            FilteredBrands = new ObservableCollection<Brand>(
                await BrandRepository.GetListAsync(br => br.Status.Equals(Status.NotBanned.ToString())));

            bannedBrands = new ObservableCollection<Brand>(
                await BrandRepository.GetListAsync(br => br.Status.Equals(Status.Banned.ToString())));

            _brandToSearch = FilteredBrands;
            notBannedBrands = FilteredBrands;

            var query = await RequestRepo.GetAllAsync(item => item.MUser);

            
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                FilteredBrands = new ObservableCollection<Brand>(FilteredBrands);
                RequestList.Items = new ObservableCollection<BrandRequestItemViewModel>(
                        query.Select(item => new BrandRequestItemViewModel
                        {
                            RequestId = item.Id,
                            Id = item.MUser.Id,
                            Name = item.MUser.Name,
                            BrandName = item.Name,
                            SourceImageAva = item.MUser.SourceImageAva,
                            Reason = item.Reason,
                        }));
            }));
        }
        public async Task AddNewBrand(string brandName)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);

            MainViewModel.SetLoading(true);

            brandName=brandName.Trim();
            NewBrandName = string.Empty;
            var brand = await BrandRepository.GetSingleAsync(item => item.Name.ToLower().Equals(brandName.ToLower()));
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
                await BrandRepository.Add(new Brand {Id=await GenerateID.Gen(typeof(Brand)) ,Name = brandName, Status = Status.NotBanned.ToString() });
            }
            await Load();

            MainViewModel.SetLoading(false);

        }

        public async Task RemoveBrand(object obj)
        {
            MainViewModel.SetLoading(true);

            var removeBrand = obj as Brand;
            if (removeBrand == null)
                return;

            if (removeBrand.Status == Status.NotBanned.ToString())
                removeBrand.Status = Status.Banned.ToString();
            else
                removeBrand.Status = Status.NotBanned.ToString();

            var list = new List<Models.Product>(await ProdRepo.GetListAsync(item=>item.IdBrand==removeBrand.Id));
            for(int i = 0; i < list.Count; i++)
            {
                if (removeBrand.Status == Status.Banned.ToString())
                    list[i].BanLevel += 1;
                else if (removeBrand.Status == Status.NotBanned.ToString())
                {
                    list[i].BanLevel -= 1;
                    if (list[i].BanLevel < 0)
                        list[i].BanLevel = 0;
                }
            }

            await ProdRepo.Update(list.ToArray());
            await BrandRepository.Update(removeBrand);
            await Load();
            MainViewModel.SetLoading(false);

        }

        public async Task RemoveRequest(object obj)
        {
            MainViewModel.SetLoading(true);

            var request = obj as BrandRequestItemViewModel;
            if (request == null)
                return;

            if (AccountStore.instance.CurrentAccount == null)
                return;

            var removeRequest = await RequestRepo.GetSingleAsync(item => item.Id.Equals(request.RequestId));

            var note = new Models.Notification
            {
                Id = await GenerateID.Gen(typeof(Models.Notification)),
                IdSender = AccountStore.instance.CurrentAccount.Id,
                IdReceiver = request.Id,
                Content = "Your brand request has been rejected. Contact us for further information.",
                Date = DateTime.Now,
                HaveSeen = false,
            };

            await RequestRepo.Remove(removeRequest);
            await NoteRepo.Add(note);
            await Load();
            MainViewModel.SetLoading(false);

        }

        public async Task AddRequest(object obj)
        {
            MainViewModel.SetLoading(true);

            if (AccountStore.instance.CurrentAccount == null)
                return;

            var request = obj as BrandRequestItemViewModel;
            if (request == null)
                return;

            var brand = await BrandRepository.GetSingleAsync(item => item.Name.ToLower().Trim().Equals(request.BrandName.ToLower().Trim()));
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
                await BrandRepository.Add(new Brand {Id=await GenerateID.Gen(typeof(Brand)) ,Name = request.BrandName, Status = Status.NotBanned.ToString() });
            }

            var note = new Models.Notification
            {
                Id = await GenerateID.Gen(typeof(Models.Notification)),
                IdSender = AccountStore.instance.CurrentAccount.Id,
                IdReceiver = request.Id,
                Content = "Your brand request has been accepted.",
                Date = DateTime.Now,
                HaveSeen = false,
            };

            var removeRequest = await RequestRepo.GetSingleAsync(item => item.Id.Equals(request.RequestId));
            await NoteRepo.Add(note);
            await RequestRepo.Remove(removeRequest);
            await Load();
            MainViewModel.SetLoading(false);


        }

        public async Task Search()
        {
            MainViewModel.SetLoading(true);

            await Task.Run(async () =>
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
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    FilteredBrands = new ObservableCollection<Brand>(FilteredBrands);
                }));
            });
            MainViewModel.SetLoading(false);

        }

        public void CloseSearch()
        {
            SearchText = string.Empty;
        }
        #endregion
    }
}