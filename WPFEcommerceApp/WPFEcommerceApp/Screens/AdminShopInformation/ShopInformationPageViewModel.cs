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
    public class ShopInformationPageViewModel: BaseViewModel, IAsyncInitialization
    {
        #region Public Properties
        private ObservableCollection<MUser> _shops;
        public ObservableCollection<MUser> Shops
        {
            get { return _shops; }
            set
            {
                _shops = value;
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

        private MUser _datagridSelectedItem;

        public MUser DatagridSelectedItem
        {
            get
            {
                return _datagridSelectedItem;
            }

            set
            {
                _datagridSelectedItem = value;
                OnPropertyChanged();
            }
        }


        private string _searchBy;

        public string SearchBy
        {
            get { return _searchBy; }
            set { _searchBy = value; OnPropertyChanged(); }
        }
        private string searchText;

        public string SearchString
        {
            get { return searchText; }
            set { searchText = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands
        public ICommand RemoveRequestCommand { get; set; }
        public ICommand RemoveShopCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand CloseSearchCommand { get; set; }
        public ICommand OpenDialogCommand { get; set; }
        public ICommand OpenRequestCommand { get; set; }
        public ICommand ChangeCommand { get; set; }
        public Task InitializeAsync { get; private set; }
        #endregion

        #region Constructor
        public ShopInformationPageViewModel()
        {
            InitializeAsync = Load();
            RequestList= new ShopRequestListViewModel();
            ShopRequestItemViewModel.RemoveRequestCommand = new RelayCommandWithNoParameter(async()=>await RemoveRequest());
            ShopRequestItemViewModel.AddRequestCommand = new RelayCommandWithNoParameter(async()=>await AddRequest());
            RemoveShopCommand = new RelayCommand<object>(p => p != null, async(p)=>await RemoveShop(p));
            SearchCommand = new RelayCommandWithNoParameter(Search);
            CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearch);
            OpenRequestCommand = new RelayCommand<object>(p => p != null, async (p) => await OpenDialog(p));
        }

        private async Task OpenDialog(object obj)
        {
            try
            {
                var temp = (ShopRequestItemViewModel)obj;

                if(RequestSelectedItem == temp)
                {
                    ShowInformation();
                }
                RequestSelectedItem = temp;
            }
            catch { }
        }

        public async Task Load()
        {
            try
            {
                var userRepository = new GenericDataRepository<MUser>();
                //var newUser = new MUser
                //{
                //    Id = "Us004",
                //    Name = "Winter Clothing",
                //    Role = "User",
                //    PhoneNumber = "0987654321",
                //    Email = "goodnight@gmail.com",
                //    Address = "None",
                //    Gender = true,
                //    Description = "None",
                //    Status = "NotBanned"

                //};
                //await userRepository.Add(newUser);

                Shops = new ObservableCollection<MUser>(await userRepository.
                    GetListAsync(item => item.Role.Equals("Shop")&&item.Status.Equals("NotBanned")));

                using (var context = new EcommerceAppEntities())
                {
                    try
                    {
                        var query = (from request in context.ShopRequests
                                    join user in context.MUsers
                                    on request.IdUser equals user.Id
                                    select new
                                    {
                                        RequestId=request.Id,
                                        IdUser=request.IdUser,
                                        Name=user.Name,
                                        SourceImageAva=user.SourceImageAve,
                                        Description=request.Description,
                                        Email=user.Email,
                                        PhoneNumber=user.PhoneNumber,
                                        Address=user.Address,
                                    }).ToList();
                        RequestList.Items = new ObservableCollection<ShopRequestItemViewModel>(
                            query.Select(item=> new ShopRequestItemViewModel
                            {
                                RequestId = item.RequestId,
                                Id=item.IdUser,
                                Name=item.Name,
                                Description=item.Description,
                                SourceImageAva=new BitmapImage(new Uri(item.SourceImageAva)),
                                Email=item.Email,
                                PhoneNumber=item.PhoneNumber,
                                Address=item.Address
                            }));

                    }
                    catch { }
                }
                
                
            }
            catch { }
           
        }

        #endregion

        #region Command Methods

        public void Search()
        {

        }

        public void CloseSearch()
        {
            SearchString = string.Empty;
        }

        public async Task RemoveShop(object obj)
        {
            try
            {
                var removeShop = (MUser)obj;
                Shops.Remove(removeShop);
                removeShop.Status = "Banned";
                await new GenericDataRepository<MUser>().Update(removeShop);
            }
            catch { };
        }

        public async Task RemoveRequest()
        {
            try
            {
                var repo = new GenericDataRepository<ShopRequest>();
                var item = await repo.GetSingleAsync(t => t.Id.Equals(RequestSelectedItem.RequestId));
                await repo.Remove(item);
                await Load();
                DialogHost.CloseDialogCommand.Execute(null, null);
            }
            catch { };
        }

        public async Task AddRequest()
        {
            try
            {
                var repoShopRequest = new GenericDataRepository<ShopRequest>();
                var item = await repoShopRequest.GetSingleAsync(t => t.Id.Equals(RequestSelectedItem.RequestId));

                var repoUser = new GenericDataRepository<MUser>();
                var newShop = await repoUser.GetSingleAsync(user => user.Id.Equals(item.IdUser));
                newShop.Role = "Shop";
                await repoUser.Update(newShop);
                await repoShopRequest.Remove(item);
                await Load();
                DialogHost.CloseDialogCommand.Execute(null, null);
            }
            catch { }
        }
        #endregion
    }

}
