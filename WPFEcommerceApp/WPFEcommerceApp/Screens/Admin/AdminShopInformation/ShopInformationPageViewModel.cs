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
            set { _searchBy = value; OnPropertyChanged(); }
        }

        public List<string> SearchByOptions { get; set; }

        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; OnPropertyChanged(); }
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
            MainViewModel.SetLoading(true);

            userRepo = new GenericDataRepository<MUser>();
            requestRepo = new GenericDataRepository<ShopRequest>();
            noteRepo = new GenericDataRepository<Models.Notification>();

            SearchByOptions = new List<string> { "ID", "Name", "Des" };
            SearchBy = SearchByOptions[1];

            RequestList = new ShopRequestListViewModel();
            ShopRequestItemViewModel.RemoveRequestCommand = new RelayCommandWithNoParameter(async () => await RemoveRequest());
            ShopRequestItemViewModel.AddRequestCommand = new RelayCommandWithNoParameter(async () => await AddRequest());
            RemoveShopCommand = new RelayCommand<object>(p => p != null, async (p) => await RemoveShop(p));
            SearchCommand = new RelayCommandWithNoParameter(async () => await Search());
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearch);
            OpenRequestCommand = new RelayCommand<object>(p => p != null, async (p) => await OpenDialog(p));

            Task.Run(async () =>
            {
                MainViewModel.SetLoading(true);
                await Load();
            }).ContinueWith((first) =>
            {
                MainViewModel.SetLoading(false);

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
            var addressRepo = new GenericDataRepository<Models.Address>();
            RequestList.Items = new ObservableCollection<ShopRequestItemViewModel>();
            foreach (Models.ShopRequest request in query)
            {
                Models.Address address = await addressRepo.GetSingleAsync(a => a.IdUser == request.IdUser && a.Id == request.MUser.DefaultAddress);
                ShopRequestItemViewModel item = new ShopRequestItemViewModel
                {
                    RequestId = request.Id,
                    Id = request.IdUser,
                    Name = request.MUser.Name,
                    Description = request.Description,
                    SourceImageAva = request.MUser.SourceImageAva,
                    Email = request.MUser.Email,
                    PhoneNumber = request.MUser.PhoneNumber,
                    Address = (address == null || String.IsNullOrEmpty(address.Address1))?"":address.Address1
                };
                RequestList.Items.Add(item);
            }
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                FilteredShops = new ObservableCollection<MUser>(FilteredShops);
                RequestList.Items = new ObservableCollection<ShopRequestItemViewModel>(RequestList.Items);
            }));
        }

        #endregion

        #region Command Methods

        public async Task Search()
        {
            MainViewModel.SetLoading(true);
            await Task.Run(() =>
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
                else if (SearchBy == "Des")
                {
                    _lastSearchOption = "Des";
                    FilteredShops = new ObservableCollection<MUser>(shopsToSearch.Where(br => br.Description.ToLower().Contains(SearchText.ToLower())));
                }
            });
            MainViewModel.SetLoading(false);
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

            List<Models.Notification> notes = new List<Models.Notification>();
            if (removeShop.StatusShop == Status.Banned.ToString() && removeShop.StatusUser == Status.Banned.ToString())
            {
                var view = new ConfirmDialog
                {
                    Header = "No!",
                    Content = "The user is currently ban, make sure to unban it before unbaning its shop."
                };
                await DialogHost.Show(view, "Main");
                return;
            }
            MainViewModel.SetLoading(true);
            if (removeShop.StatusShop == Status.Banned.ToString())
            {
                removeShop.StatusShop = Status.NotBanned.ToString();
                var note = new Models.Notification
                {
                    Id = await GenerateID.Gen(typeof(Models.Notification)),
                    IdSender = AccountStore.instance.CurrentAccount.Id,
                    IdReceiver = removeShop.Id,
                    Content = "Your shop has been unbanned. Feel free to start selling again.",
                    Date = DateTime.Now,
                    HaveSeen = false
                };
                notes.Add(note);
            }
            else
            {
                removeShop.StatusShop = Status.Banned.ToString();
                var note = new Models.Notification
                {
                    Id = await GenerateID.Gen(typeof(Models.Notification)),
                    IdSender = AccountStore.instance.CurrentAccount.Id,
                    IdReceiver = removeShop.Id,
                    Content = "Your shop has been banned. Contact us for further information.",
                    Date = DateTime.Now,
                    HaveSeen = false
                };
                notes.Add(note);
            }
            MainViewModel.SetLoading(true);

            await BanShop(removeShop);
            await userRepo.Update(removeShop);
            await noteRepo.Add(notes.ToArray());

            await Load();

            MainViewModel.SetLoading(false);

        }

        public async Task RemoveRequest()
        {
            MainViewModel.SetLoading(true);
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
                HaveSeen = false
            };

            await requestRepo.Remove(item);
            await noteRepo.Add(note);

            await Load();

            MainViewModel.SetLoading(false);

            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public async Task AddRequest()
        {
            if (AccountStore.instance.CurrentAccount == null)
            {
                return;
            }

            MainViewModel.SetLoading(true);

            var item = await requestRepo.GetSingleAsync(t => t.Id.Equals(RequestSelectedItem.RequestId));

            var newShop = await userRepo.GetSingleAsync(user => user.Id.Equals(item.IdUser));
            newShop.Role = "Shop";
            newShop.StatusShop = Status.NotBanned.ToString();
            newShop.Description = RequestSelectedItem.Description;

            var note = new Models.Notification
            {
                Id = await GenerateID.Gen(typeof(Models.Notification)),
                IdSender = AccountStore.instance.CurrentAccount.Id,
                IdReceiver = item.IdUser,
                Content = "Your request to be a shop has been accepted. Check out your shop and start selling.",
                Date = DateTime.Now,
                HaveSeen = false
            };

            await userRepo.Update(newShop);
            await requestRepo.Remove(item);
            await noteRepo.Add(note);
            await Load();

            MainViewModel.SetLoading(false);

            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public static async Task BanShop(MUser removeShop)
        {
            var prodRepo=new GenericDataRepository<Models.Product>();
            var orderRepo=new GenericDataRepository<MOrder>();
            var newNoteRepo=new GenericDataRepository<Models.Notification>();

            var list = new List<Models.Product>(await prodRepo.GetListAsync(item => item.IdShop == removeShop.Id));
            for (int i = 0; i < list.Count; i++)
            {
                if (removeShop.StatusShop == Status.NotBanned.ToString())
                {
                    list[i].BanLevel -= 1;
                    if (list[i].BanLevel < 0)
                        list[i].BanLevel = 0;
                }
                else
                {
                    list[i].BanLevel += 1;
                }
            }

            if(removeShop.StatusShop == Status.Banned.ToString())
            {
                var orders = new List<Models.MOrder>(
                        await orderRepo.GetListAsync(
                            item => item.IdShop == removeShop.Id
                            && (item.Status == OrderStatus.Processing.ToString()
                            || item.Status == OrderStatus.Delivering.ToString())));
                for (int j = 0; j < orders.Count; j++)
                {
                    orders[j].Status = OrderStatus.Cancelled.ToString();
                    orders[j].DateEnd = DateTime.Now;
                    var cancelNote = new Models.Notification
                    {
                        Id = await GenerateID.Gen(typeof(Models.Notification)),
                        IdSender = AccountStore.instance.CurrentAccount.Id,
                        IdReceiver = orders[j].IdCustomer,
                        Content = $@"Your order ID {orders[j].Id} is cancelled because the shop is banned. Please checkout other similar products
Sorry for the inconvenience!",
                        Date = DateTime.Now,
                        HaveSeen = false
                    };
                    await newNoteRepo.Add(cancelNote);
                }

                await orderRepo.Update(orders.ToArray());
            }
            await prodRepo.Update(list.ToArray());

        }
        #endregion
    }

}
