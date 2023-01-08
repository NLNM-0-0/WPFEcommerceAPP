using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ShopInformationPageViewModel : BaseViewModel
    {
        #region Public Properties
        private GenericDataRepository<MUser> userRepo;
        private GenericDataRepository<ShopRequest> requestRepo;
        private GenericDataRepository<Models.Notification> noteRepo;

        private ObservableCollection<MUser> shopsToSearch;

        private ObservableCollection<MUser> bannedShops;
        private ObservableCollection<MUser> notBannedShops;

        private ObservableCollection<MUser> _filteredShops;
        public ObservableCollection<MUser> FilteredShops
        {
            get
            {
                return _filteredShops;
            }

            set
            {
                _filteredShops = value;
                OnPropertyChanged();
            }
        }

        private ShopRequestListViewModel _requestList;

        public ShopRequestListViewModel RequestList
        {
            get
            {
                return _requestList;
            }

            set
            {
                if (value == _requestList)
                    return;

                _requestList = value;
                OnPropertyChanged();
            }
        }
        private ShopRequestItemViewModel _requestSelectedItem;

        public ShopRequestItemViewModel RequestSelectedItem
        {
            get
            {
                return _requestSelectedItem;
            }

            set
            {
                _requestSelectedItem = value;
                ShowInformation();

                OnPropertyChanged();
            }
        }

        private async void ShowInformation()
        {
            var view = new ShopRequestInformationDialog()
            {
                DataContext = this,
            };
            if (RequestSelectedItem == null)
                return;
            await DialogHost.Show(view, "show");
        }



        private string _searchBy;

        public string SearchBy
        {
            get { return _searchBy; }
            set { _searchBy = value; Search(); OnPropertyChanged(); }
        }

        public List<string> SearchByOptions { get; set; }

        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; Search(); OnPropertyChanged(); }
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
                    FilteredShops = shopsToSearch = notBannedShops;
                    RemoveOrUnBanned = "Ban";
                }
                else
                {
                    StatusText = "Banned";
                    FilteredShops = shopsToSearch = bannedShops;
                    RemoveOrUnBanned = "Unban";
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
        private string _lastSearchText;
        private string _lastSearchOption;
        #endregion

        #region Commands
        public ICommand RemoveRequestCommand { get; set; }
        public ICommand RemoveShopCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand CloseSearchCommand { get; set; }
        public ICommand OpenDialogCommand { get; set; }
        public ICommand OpenRequestCommand { get; set; }
        public ICommand ChangeCommand { get; set; }
        #endregion

        #region Constructor
        public ShopInformationPageViewModel()
        {
            MainViewModel.IsLoading = true;

            userRepo = new GenericDataRepository<MUser>();
            requestRepo = new GenericDataRepository<ShopRequest>();
            noteRepo = new GenericDataRepository<Models.Notification>();

            SearchByOptions = new List<string> { "ID", "Name", "Des" };
            SearchBy = SearchByOptions[1];

            RequestList = new ShopRequestListViewModel();
            ShopRequestItemViewModel.RemoveRequestCommand = new RelayCommandWithNoParameter(async () => await RemoveRequest());
            ShopRequestItemViewModel.AddRequestCommand = new RelayCommandWithNoParameter(async () => await AddRequest());
            RemoveShopCommand = new RelayCommand<object>(p => p != null, async (p) => await RemoveShop(p));
            SearchCommand = new RelayCommandWithNoParameter(Search);
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearch);
            OpenRequestCommand = new RelayCommand<object>(p => p != null, async (p) => await OpenDialog(p));

            Task.Run(async () =>
            {
                MainViewModel.IsLoading = true;
                await Load();
            }).ContinueWith((first) =>
            {
                MainViewModel.IsLoading = false;

            });


        }

        private async Task OpenDialog(object obj)
        {
            try
            {
                var temp = (ShopRequestItemViewModel)obj;

                if (RequestSelectedItem == temp)
                {
                    ShowInformation();
                }
                RequestSelectedItem = temp;
            }
            catch { }
        }

        public async Task Load()
        {
            IsChecked = true;
            RemoveOrUnBanned = "Ban";

            notBannedShops = new ObservableCollection<MUser>(await userRepo.
                GetListAsync(item => item.Role.Equals("Shop") && item.StatusShop.Equals(Status.NotBanned.ToString())));

            FilteredShops = shopsToSearch = notBannedShops;
            bannedShops = new ObservableCollection<MUser>(await userRepo.
                GetListAsync(item => item.Role.Equals("Shop") && item.StatusShop.Equals(Status.Banned.ToString())));
            var query = await requestRepo.GetAllAsync(s => s.MUser);

            RequestList.Items = new ObservableCollection<ShopRequestItemViewModel>(
                query.Select(item => new ShopRequestItemViewModel
                {
                    RequestId = item.Id,
                    Id = item.IdUser,
                    Name = item.MUser.Name,
                    Description = item.Description,
                    SourceImageAva = item.MUser.SourceImageAva,
                    Email = item.MUser.Email,
                    PhoneNumber = item.MUser.PhoneNumber,
                    Address = item.MUser.Address
                }));
        }

        #endregion

        #region Command Methods

        public void Search()
        {
            if (string.IsNullOrEmpty(SearchBy))
                FilteredShops = shopsToSearch;

            if (string.IsNullOrEmpty(_lastSearchText) && string.IsNullOrEmpty(SearchText) ||
                (string.Equals(_lastSearchText, SearchText) && _lastSearchOption == SearchBy))
            {
                FilteredShops = shopsToSearch;
            }

            if (string.IsNullOrEmpty(SearchText) || shopsToSearch.Count <= 0 || shopsToSearch == null)
            {
                FilteredShops = shopsToSearch;
                return;
            }

            if (SearchBy == "Name")
            {
                _lastSearchOption = "Name";
                FilteredShops = new ObservableCollection<MUser>(shopsToSearch.Where(br => br.Name.ToLower().Contains(SearchText.ToLower())));
            }
            else if (SearchBy == "ID")
            {
                _lastSearchOption = "ID";
                FilteredShops = new ObservableCollection<MUser>(shopsToSearch.Where(br => br.Id.ToLower().Contains(SearchText.ToLower())));
            }
            else if(SearchBy=="Des")
            {
                _lastSearchOption = "Des";
                FilteredShops = new ObservableCollection<MUser>(shopsToSearch.Where(br => br.Description.ToLower().Contains(SearchText.ToLower())));

            }
        }

        public void CloseSearch()
        {
            SearchText = string.Empty;
        }

        public async Task RemoveShop(object obj)
        {
            var removeShop = obj as MUser;
            if (obj == null)
                return;

            if (AccountStore.instance.CurrentAccount == null)
                return;

            Models.Notification note;

            if (removeShop.StatusShop == Status.Banned.ToString())
            {
                removeShop.StatusShop = Status.NotBanned.ToString();
                note = new Models.Notification
                {
                    Id = await GenerateID.Gen(typeof(Models.Notification)),
                    IdSender = AccountStore.instance.CurrentAccount.Id,
                    IdReceiver = removeShop.Id,
                    Content = "Your shop has been unbanned. Feel free to start selling again.",
                    Date = DateTime.Now,
                };
            }
            else
            {
                removeShop.StatusShop = Status.Banned.ToString();
                note = new Models.Notification
                {
                    Id = await GenerateID.Gen(typeof(Models.Notification)),
                    IdSender = AccountStore.instance.CurrentAccount.Id,
                    IdReceiver = removeShop.Id,
                    Content = "Your shop has been banned. Contact us for further information.",
                    Date = DateTime.Now,
                };
            }

            MainViewModel.IsLoading = true;

            await userRepo.Update(removeShop);
            await noteRepo.Add(note);

            await Load();

            MainViewModel.IsLoading = false;

        }

        public async Task RemoveRequest()
        {
            MainViewModel.IsLoading = true;
            if (AccountStore.instance.CurrentAccount == null)
                return;

            var item = await requestRepo.GetSingleAsync(t => t.Id.Equals(RequestSelectedItem.RequestId));

            var note = new Models.Notification
            {
                Id = await GenerateID.Gen(typeof(Models.Notification)),
                IdSender = AccountStore.instance.CurrentAccount.Id,
                IdReceiver = item.IdUser,
                Content = "Your shop request has been rejected.",
                Date = DateTime.Now,
            };

            await requestRepo.Remove(item);
            await noteRepo.Add(note);

            await Load();

            MainViewModel.IsLoading = false;

            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public async Task AddRequest()
        {
            if(AccountStore.instance.CurrentAccount==null)
            {
                return;
            }

            MainViewModel.IsLoading = true;

            var item = await requestRepo.GetSingleAsync(t => t.Id.Equals(RequestSelectedItem.RequestId));

            var newShop = await userRepo.GetSingleAsync(user => user.Id.Equals(item.IdUser));
            newShop.Role = "Shop";
            newShop.StatusShop = Status.NotBanned.ToString();
            newShop.Description=RequestSelectedItem.Description;

            var note = new Models.Notification
            {
                Id = await GenerateID.Gen(typeof(Models.Notification)),
                IdSender=AccountStore.instance.CurrentAccount.Id,
                IdReceiver=item.IdUser,
                Content="Your request to be a shop has been accepted. Check out your shop and start selling.",
                Date=DateTime.Now,
            };

            await userRepo.Update(newShop);
            await requestRepo.Remove(item);
            await noteRepo.Add(note);
            await Load();

            MainViewModel.IsLoading = false;

            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        #endregion
    }

}
