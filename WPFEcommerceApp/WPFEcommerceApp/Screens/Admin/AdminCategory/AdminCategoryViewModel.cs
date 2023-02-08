using DataAccessLayer;
using LiveCharts;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AdminCategoryViewModel : BaseViewModel
    {
        #region Properties
        private GenericDataRepository<Category> cateRepo;
        private GenericDataRepository<CategoryRequest> cateRequestRepo;
        private GenericDataRepository<Models.Notification> noteRepo;
        private GenericDataRepository<Models.Product> prodRepo;

        private ObservableCollection<Category> categoriesToSearch;

        private ObservableCollection<Category> bannedCategories;
        private ObservableCollection<Category> notBannedCategories;

        private ObservableCollection<Category> _filteredCategories;
        public ObservableCollection<Category> FilteredCategories
        {
            get
            {
                return _filteredCategories;
            }

            set
            {
                _filteredCategories = value;
                OnPropertyChanged();
            }
        }

        private CategoryRequestListViewModel _requestList;
        public CategoryRequestListViewModel RequestList
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
        public List<string> SearchByOptions { get; set; }

        private string _newName { get; set; }
        public string NewName
        {
            get => _newName;
            set { _newName = value; OnPropertyChanged(); }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value)
                {
                    StatusText = "Not Banned";
                    FilteredCategories = categoriesToSearch = notBannedCategories;
                    RemoveOrUnBanned = "Ban";
                }
                else
                {
                    StatusText = "Banned";
                    FilteredCategories = categoriesToSearch = bannedCategories;
                    RemoveOrUnBanned = "Restore";
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
        public string RemoveOrUnBanned
        {
            get => _removeOrUnBanned;
            set { _removeOrUnBanned = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand AddNewCategoryCommand { get; set; }
        public ICommand RemoveCategoryCommand { get; set; }

        public ICommand RemoveRequestCommand { get; set; }
        public ICommand AddRequestCommand { get; set; }

        public ICommand SearchCommand { get; set; }
        public ICommand CloseSearchCommand { get; set; }


        #endregion

        #region Constructor
        public AdminCategoryViewModel()
        {
            MainViewModel.SetLoading(true);
            cateRepo = new GenericDataRepository<Category>();
            cateRequestRepo = new GenericDataRepository<CategoryRequest>();
            noteRepo = new GenericDataRepository<Models.Notification>();
            prodRepo = new GenericDataRepository<Models.Product>();

            RequestList = new CategoryRequestListViewModel();
            SearchByOptions = new List<string> { "ID", "Name" };
            SearchBy = SearchByOptions[1];


            AddNewCategoryCommand = new RelayCommand<string>(p => p != string.Empty, async (p) => await AddNewCategory(p));
            RemoveCategoryCommand = new RelayCommand<object>(p => p != null, async(p)=>await RemoveCategory(p));
            RemoveRequestCommand = new RelayCommand<object>(p => p != null,async(p)=>await RemoveRequest(p));
            AddRequestCommand = new RelayCommand<object>(p => p != null, async(p)=>await AddRequest(p));
            SearchCommand = new RelayCommandWithNoParameter(async()=>await Search());
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearch);

            Task.Run(async () =>
            {
                MainViewModel.SetLoading(true);
                await Load();
            }).ContinueWith((first) =>
            {
                MainViewModel.SetLoading(false);

            });
        }

        private async Task Load()
        {
            IsChecked = true;
            RemoveOrUnBanned = "Ban";

            categoriesToSearch = new ObservableCollection<Category>(
                await cateRepo.GetListAsync(item => item.Status.Equals(Status.NotBanned.ToString())));

            bannedCategories = new ObservableCollection<Category>(
                await cateRepo.GetListAsync(item => item.Status.Equals(Status.Banned.ToString())));

            FilteredCategories = notBannedCategories = categoriesToSearch;

            _lastSearchOption = null;
            _lastSearchText = string.Empty;

            var query = await cateRequestRepo.GetAllAsync(item => item.MUser);
            
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                FilteredCategories = new ObservableCollection<Category>(FilteredCategories);
                RequestList.Items = new ObservableCollection<CategoryRequestItemViewModel>(
                query.Select(item => new CategoryRequestItemViewModel
                {
                    RequestId = item.Id,
                    Id = item.MUser.Id,
                    Name = item.MUser.Name,
                    CategoryName = item.Name,
                    SourceImageAva = item.MUser.SourceImageAva,
                    Reason = item.Reason,
                }));
            }));
        }

        #endregion

        #region Command Methods

        public async Task AddNewCategory(string cateName)
        {
            MainViewModel.SetLoading(true);
            cateName = cateName.Trim();
            NewName = string.Empty;
            var cate = await cateRepo.GetSingleAsync(item => item.Name.ToLower().Equals(cateName.ToLower()));
            if (cate != null)
            {
                if (cate.Status == Status.Banned.ToString())
                {
                    cate.Status = Status.NotBanned.ToString();
                    await cateRepo.Update(cate);
                }
            }
            else
            {
                await cateRepo.Add(new Category {Id=await GenerateID.Gen(typeof(Category)) ,Name = cateName, Status = Status.NotBanned.ToString() });
            }

            await Load();
            MainViewModel.SetLoading(false);

            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public async Task RemoveCategory(object obj)
        {
            MainViewModel.SetLoading(true);

            var removeCate = obj as Category;
            if (removeCate == null)
                return;

            if (removeCate.Status == Status.NotBanned.ToString())
                removeCate.Status = Status.Banned.ToString();
            else
                removeCate.Status = Status.NotBanned.ToString();

            var list = new List<Models.Product>(await prodRepo.GetListAsync(item => item.IdCategory == removeCate.Id));
            for (int i = 0; i < list.Count; i++)
            {
                if (removeCate.Status == Status.Banned.ToString())
                    list[i].BanLevel += 1;
                else if (removeCate.Status == Status.NotBanned.ToString())
                {
                    list[i].BanLevel -= 1;
                    if (list[i].BanLevel<0)
                        list[i].BanLevel = 0;
                }
            }

            await prodRepo.Update(list.ToArray());
            await cateRepo.Update(removeCate);
            await Load();
            MainViewModel.SetLoading(false);

        }

        public async Task RemoveRequest(object obj)
        {
            MainViewModel.SetLoading(true);
            var request = obj as CategoryRequestItemViewModel;
            if (request == null)
                return;

            var removeRequest = await cateRequestRepo.GetSingleAsync(item => item.Id.Equals(request.RequestId));

            var note = new Models.Notification
            {
                Id = await GenerateID.Gen(typeof(Models.Notification)),
                IdSender = AccountStore.instance.CurrentAccount.Id,
                IdReceiver = request.Id,
                Content = "Your category request has been rejected. Contact us for further information.",
                Date = DateTime.Now,
                HaveSeen = false,
            };

            await noteRepo.Add(note);
            await cateRequestRepo.Remove(removeRequest);
            await Load();
            MainViewModel.SetLoading(false);
        }

        public async Task AddRequest(object obj)
        {
            MainViewModel.SetLoading(true);

            var request = obj as CategoryRequestItemViewModel;
            if (request == null)
                return;

            var cate = await cateRepo.GetSingleAsync(item => item.Name.ToLower().Trim().Equals(request.CategoryName.ToLower().Trim()));
            if (cate != null)
            {
                if (cate.Status == Status.Banned.ToString())
                {
                    cate.Status = Status.NotBanned.ToString();
                    await cateRepo.Update(cate);
                }
            }
            else
            {
                await cateRepo.Add(new Category {Id=await GenerateID.Gen(typeof(Category)) ,Name = request.CategoryName, Status = Status.NotBanned.ToString() });
            }

            var removeRequest = await cateRequestRepo.GetSingleAsync(item => item.Id.Equals(request.RequestId));

            var note = new Models.Notification
            {
                Id = await GenerateID.Gen(typeof(Models.Notification)),
                IdSender = AccountStore.instance.CurrentAccount.Id,
                IdReceiver = request.Id,
                Content = "Your category request has been accepted.",
                Date = DateTime.Now,
                HaveSeen = false,
            };

            await noteRepo.Add(note);
            await cateRequestRepo.Remove(removeRequest);
            await Load();

            MainViewModel.SetLoading(false);

        }

        public async Task Search()
        {
            await Task.Run(() =>
            {
                MainViewModel.SetLoading(true);
                if (string.IsNullOrEmpty(SearchBy))
                    FilteredCategories = categoriesToSearch;

                if (string.IsNullOrEmpty(_lastSearchText) && string.IsNullOrEmpty(SearchText) ||
                    (string.Equals(_lastSearchText, SearchText) && _lastSearchOption == SearchBy))
                {
                    FilteredCategories = categoriesToSearch;
                }

                if (string.IsNullOrEmpty(SearchText) || categoriesToSearch.Count <= 0 || categoriesToSearch == null)
                {
                    FilteredCategories = categoriesToSearch;
                    return;
                }

                if (SearchBy == "Name")
                {
                    _lastSearchOption = "Name";
                    FilteredCategories = new ObservableCollection<Category>(categoriesToSearch.Where(br => br.Name.ToLower().Contains(SearchText.ToLower())));
                }
                else if (SearchBy == "ID")
                {
                    _lastSearchOption = "ID";
                    FilteredCategories = new ObservableCollection<Category>(categoriesToSearch.Where(br => br.Id.ToLower().Contains(SearchText.ToLower())));
                }
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    FilteredCategories = new ObservableCollection<Category>(FilteredCategories);
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
