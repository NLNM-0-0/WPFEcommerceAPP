using DataAccessLayer;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AdminCategoryViewModel : BaseViewModel, IAsyncInitialization
    {
        #region Properties
        public GenericDataRepository<Category> cateRepo;
        public GenericDataRepository<CategoryRequest> cateRequestRepo;

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
            set { _searchText = value; Search(); OnPropertyChanged(); }
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
                    RemoveOrUnBanned = "Remove";
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

        public Task InitializeAsync { get; private set; }

        #endregion

        #region Constructor
        public AdminCategoryViewModel()
        {
            cateRepo = new GenericDataRepository<Category>();
            cateRequestRepo = new GenericDataRepository<CategoryRequest>();

            InitializeAsync = Load();

            RequestList = new CategoryRequestListViewModel();
            SearchByOptions = new List<string> { "ID", "Name" };
            SearchBy = SearchByOptions[1];


            AddNewCategoryCommand = new RelayCommand<string>(p => p != string.Empty, AddNewCategory);
            RemoveCategoryCommand = new RelayCommand<object>(p => p != null, RemoveCategory);
            RemoveRequestCommand = new RelayCommand<object>(p => p != null, RemoveRequest);
            AddRequestCommand = new RelayCommand<object>(p => p != null, AddRequest);
            SearchCommand = new RelayCommandWithNoParameter(Search);
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearch);
        }

        private async Task Load()
        {
            IsChecked = true;
            RemoveOrUnBanned = "Remove";

            categoriesToSearch = new ObservableCollection<Category>(
                await cateRepo.GetListAsync(item => item.Status.Equals(Status.NotBanned.ToString())));

            bannedCategories = new ObservableCollection<Category>(
                await cateRepo.GetListAsync(item => item.Status.Equals(Status.Banned.ToString())));

            FilteredCategories = notBannedCategories = categoriesToSearch;

            _lastSearchOption = null;
            _lastSearchText = string.Empty;

            var query = await cateRequestRepo.GetAllAsync(item => item.MUser);
            RequestList.Items = new ObservableCollection<CategoryRequestItemViewModel>(
                query.Select(item => new CategoryRequestItemViewModel
                {
                    RequestId = item.Id,
                    Id = item.MUser.Id,
                    Name = item.Name,
                    CategoryName = item.Name,
                    SourceImageAva = new BitmapImage(new Uri(item.MUser.SourceImageAva)),
                    Reason = item.Reason,
                }));

        }

        #endregion

        #region Command Methods

        public async void AddNewCategory(string cateName)
        {
            NewName = string.Empty;
            var cate = await cateRepo.GetSingleAsync(item => item.Name.Equals(cateName));
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
                await cateRepo.Add(new Category { Name = cateName, Status = Status.NotBanned.ToString() });
            }

            await Load();
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public async void RemoveCategory(object obj)
        {
            var removeCate = obj as Category;
            if (removeCate == null)
                return;

            if (removeCate.Status == Status.NotBanned.ToString())
                removeCate.Status = Status.Banned.ToString();
            else
                removeCate.Status = Status.NotBanned.ToString();

            await cateRepo.Update(removeCate);
            await Load();
        }

        public async void RemoveRequest(object obj)
        {
            var request = obj as CategoryRequestItemViewModel;
            if (request == null)
                return;

            var removeRequest = await cateRequestRepo.GetSingleAsync(item => item.Id.Equals(request.RequestId));
            await cateRequestRepo.Remove(removeRequest);
            await Load();
        }

        public async void AddRequest(object obj)
        {
            var request = obj as CategoryRequestItemViewModel;
            if (request == null)
                return;

            var cate = await cateRepo.GetSingleAsync(item => item.Name.Equals(request.CategoryName));
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
                await cateRepo.Add(new Category { Name = request.CategoryName, Status = Status.NotBanned.ToString() });
            }

            var removeRequest = await cateRequestRepo.GetSingleAsync(item => item.Id.Equals(request.RequestId));
            await cateRequestRepo.Remove(removeRequest);
            await Load();
        }

        public void Search()
        {
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
                FilteredCategories = new ObservableCollection<Category>(categoriesToSearch.Where(br => br.Id.ToString().ToLower().Contains(SearchText.ToLower())));
            }
        }

        public void CloseSearch()
        {
            SearchText = string.Empty;
        }
        #endregion
    }
}
