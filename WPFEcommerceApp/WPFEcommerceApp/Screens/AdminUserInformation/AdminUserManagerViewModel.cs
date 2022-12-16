

using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.AccessControl;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AdminUserManagerViewModel:BaseViewModel
    {
        #region Public Properties
        public GenericDataRepository<MUser> userRepo;
        private ObservableCollection<MUser> _filteredUsers;
        public ObservableCollection<MUser> FilteredUsers
        {
            get { return _filteredUsers; }
            set { _filteredUsers = value; OnPropertyChanged(); }
        }

        private ObservableCollection<MUser> usersToSearch;

        private ObservableCollection<MUser> bannedUsers;
        private ObservableCollection<MUser> notBannedUsers;


        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; Search(); OnPropertyChanged(); }
        }

        private string _searchBy;

        public string SearchBy
        {
            get { return _searchBy; }
            set { _searchBy = value; Search(); OnPropertyChanged(); }
        }

        private string _lastSearchText;
        private string _lastSearchOption;
        public List<string> SearchByOptions { get; set; }

        public MUser NewUser { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value)
                {
                    StatusText = "Not Banned";
                    FilteredUsers = usersToSearch = notBannedUsers;
                    RemoveOrUnBanned = "Remove";
                }
                else
                {
                    StatusText = "Banned";
                    FilteredUsers = usersToSearch = bannedUsers;
                    RemoveOrUnBanned = "Restore";
                }

                _isChecked = value;
            }
        }
        private string _role;
        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                if(_role=="Shop")
                    IsShop= true;
                else
                    IsShop= false;
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
        public List<string> RoleOptions { get; set; }

        public bool IsShop { get; set; }
        private bool _isShop
        {
            get => _isShop;
            set { _isShop = value; OnPropertyChanged(); }
        }
        #endregion


        #region Commands
        public ICommand AddUserCommand { get; set; }
        public ICommand RemoveUserCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand CloseSearchCommand { get; set; }

        #endregion

        #region Constructor
        public AdminUserManagerViewModel()
        {
            userRepo = new GenericDataRepository<MUser>();
            SearchByOptions = new List<string> { "ID", "Name" };
            RoleOptions = new List<string> { "User", "Shop", "Admin" };
            Load();

            RemoveUserCommand = new RelayCommand<object>(p => p != null, RemoveUser);
            SearchCommand = new RelayCommandWithNoParameter(Search);
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearch);
            AddUserCommand = new RelayCommand<object>((p)=>CheckNewUser(p),AddUser);
        }

        private async void Load()
        {
            NewUser = new MUser();
            Role = string.Empty;
            IsChecked = true;
            RemoveOrUnBanned = "Remove";
            SearchBy = SearchByOptions[1];

            notBannedUsers = new ObservableCollection<MUser>(await userRepo.GetListAsync(user => user.StatusUser != Status.Banned.ToString()));
            bannedUsers = new ObservableCollection<MUser>(await userRepo.GetListAsync(user => user.StatusUser == Status.Banned.ToString()));
            FilteredUsers = usersToSearch = notBannedUsers;
        }
        #endregion

        #region Command Methods


        public async void RemoveUser(object obj)
        {
            var removeUser = obj as MUser;
            if (removeUser == null)
                return;

            if (removeUser.StatusUser == Status.NotBanned.ToString())
                removeUser.StatusUser = Status.Banned.ToString();
            else
                removeUser.StatusUser = Status.NotBanned.ToString();

            await userRepo.Update(removeUser);
            Load();
        }

        public void Search()
        {
            if (string.IsNullOrEmpty(SearchBy))
                FilteredUsers = usersToSearch;

            if (string.IsNullOrEmpty(_lastSearchText) && string.IsNullOrEmpty(SearchText) ||
                (string.Equals(_lastSearchText, SearchText) && _lastSearchOption == SearchBy))
            {
                FilteredUsers = usersToSearch;
            }

            if (string.IsNullOrEmpty(SearchText) || usersToSearch.Count <= 0 || usersToSearch == null)
            {
                FilteredUsers = usersToSearch;
                return;
            }

            if (SearchBy == "Name")
            {
                _lastSearchOption = "Name";
                FilteredUsers = new ObservableCollection<MUser>(usersToSearch.Where(br => br.Name.ToLower().Contains(SearchText.ToLower())));
            }
            else if (SearchBy == "ID")
            {
                _lastSearchOption = "ID";
                FilteredUsers = new ObservableCollection<MUser>(usersToSearch.Where(br => br.Id.ToLower().Contains(SearchText.ToLower())));
            }
        }

        public void CloseSearch()
        {
            SearchText = string.Empty;
        }

        public async void AddUser(object p)
        {
            var user = p as MUser;
            user.Role = Role;

            user.Id =await GenerateID.Gen(typeof(MUser));
            user.StatusUser = Status.NotBanned.ToString();
            if (Role == "Shop")
                user.StatusShop = Status.NotBanned.ToString();
            else
            {
                user.StatusShop = Status.NotExist.ToString();
                user.Description = string.Empty;
            }
                await userRepo.Add(user);
            Load();
            DialogHost.CloseDialogCommand.Execute(null, null);

        }

        public bool CheckNewUser(object p)
        {
            var user=p as MUser;
            if (user == null)
                return false;

            if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.PhoneNumber) ||
                string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Address) ||
                string.IsNullOrEmpty(Role))
                return false;

            if (Role == "Shop" && string.IsNullOrEmpty(user.Description))
                return false;

            return true;
        }
        #endregion
    }


}
