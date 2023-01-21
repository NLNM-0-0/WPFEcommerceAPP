

using DataAccessLayer;
using LiveCharts;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AdminUserManagerViewModel:BaseViewModel
    {
        #region Public Properties
        public GenericDataRepository<MUser> userRepo;
        private readonly GenericDataRepository<UserLogin> loginRepo = new GenericDataRepository<UserLogin>();

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
            set { _searchText = value; OnPropertyChanged(); }
        }

        private string _searchBy;

        public string SearchBy
        {
            get { return _searchBy; }
            set { _searchBy = value; OnPropertyChanged(); }
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
                    RemoveOrUnBanned = "Ban";
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
        public List<string> GenderOptions { get; set; }

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
            MainViewModel.SetLoading(true);
            userRepo = new GenericDataRepository<MUser>();
            SearchByOptions = new List<string> { "ID", "Name" };
            RoleOptions = new List<string> { "User", "Shop", "Admin" };
            GenderOptions = new List<string> { "Male", "Female" };

            RemoveUserCommand = new RelayCommand<object>(p => p != null, async(p)=>await RemoveUser(p));
            SearchCommand = new RelayCommandWithNoParameter(async()=>await Search());
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearch);
            AddUserCommand = new RelayCommand<object>((p)=>CheckNewUser(p), async(p)=>await AddUser(p));

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
            NewUser = new MUser();
            Role = string.Empty;
            IsChecked = true;
            RemoveOrUnBanned = "Ban";
            SearchBy = SearchByOptions[1];

            notBannedUsers = new ObservableCollection<MUser>(await userRepo.GetListAsync(
                user => user.StatusUser != Status.Banned.ToString() && 
                user.Id != AccountStore.instance.CurrentAccount.Id)
            );
            bannedUsers = new ObservableCollection<MUser>(await userRepo.GetListAsync(
                user => user.StatusUser == Status.Banned.ToString() &&
                user.Id != AccountStore.instance.CurrentAccount.Id)
            );
            FilteredUsers = usersToSearch = notBannedUsers;

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                FilteredUsers = new ObservableCollection<MUser>(FilteredUsers);
                
            }));
        }
        #endregion

        #region Command Methods


        public async Task RemoveUser(object obj)
        {
            MainViewModel.SetLoading(true);

            var removeUser = obj as MUser;
            if (removeUser == null)
                return;

            if (removeUser.StatusUser == Status.NotBanned.ToString())
            {
                removeUser.StatusUser = Status.Banned.ToString();
                if (removeUser.Role == "Shop")
                {
                    removeUser.StatusShop = Status.Banned.ToString();
                    await ShopInformationPageViewModel.BanShop(removeUser);
                }
            }
            else
            {
                removeUser.StatusUser = Status.NotBanned.ToString();
            }

            await userRepo.Update(removeUser);
            await Load();
            MainViewModel.SetLoading(false);

        }

        public async Task Search()
        {
            MainViewModel.SetLoading(true);
            await Task.Run(() =>
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
            });
            MainViewModel.SetLoading(false);
        }

        public void CloseSearch()
        {
            SearchText = string.Empty;
        }

        public async Task AddUser(object p)
        {
            MainViewModel.SetLoading(true);
            var user = p as MUser;

            var check = await loginRepo.GetSingleAsync(d => d.Username == user.Email);
            if(check != null) {
                var dl = new ConfirmDialog() {
                    Header = "Oops",
                    Content = "This username/email already exists"
                };
                await DialogHost.Show(dl, "AddUser");
                MainViewModel.SetLoading(false);
                return;
            }

            var userLogin = new UserLogin();
            userLogin.Username = user.Email;
            var hasher = new Hashing();
            var salt = Convert.ToBase64String(hasher.GenerateSalt());
            userLogin.Password = hasher.Encrypt(salt, "WANO123");
            userLogin.Salt = salt;
            userLogin.Provider = -1;
            userLogin.IdUser = await GenerateID.Gen(typeof(UserLogin), "IdUser");
            await loginRepo.Add(userLogin);

            user.Role = Role;
            user.Id = userLogin.IdUser;
            user.StatusUser = Status.NotBanned.ToString();
            if (Role == "Shop")
                user.StatusShop = Status.NotBanned.ToString();
            else
            {
                user.StatusShop = Status.NotExist.ToString();
                user.Description = string.Empty;
            }
            await userRepo.Add(user);
            await Load();

            MainViewModel.SetLoading(false);

            Role = null;
            NewUser = new MUser();
            DialogHost.CloseDialogCommand.Execute(null, null);

        }

        public bool CheckNewUser(object p)
        {
            var user=p as MUser;
            if (user == null)
                return false;

            //VHCMT => Bỏ address
            if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.PhoneNumber) ||
                string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(Role))
                return false;

            if (Role == "Shop" && string.IsNullOrEmpty(user.Description))
                return false;

            return true;
        }
        #endregion
    }


}
