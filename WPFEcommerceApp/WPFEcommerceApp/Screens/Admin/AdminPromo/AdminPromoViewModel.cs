
using DataAccessLayer;
using LiveCharts;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AdminPromoViewModel : BaseViewModel
    {
        #region Properties

        private GenericDataRepository<Promo> promoRepo;
        private GenericDataRepository<Models.Notification> noteRepo;
        public ObservableCollection<ShopPromoBlockViewModel> Items { get; set; }
        public ObservableCollection<ShopPromoBlockViewModel> RequestItems { get; set; }
        private ObservableCollection<ShopPromoBlockViewModel> _allItems;
        public List<string> SearchByOptions { get; set; }
        private string _searchBy;

        public string SearchBy
        {
            get { return _searchBy; }
            set { _searchBy = value; Search(); OnPropertyChanged(); }
        }

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; Search(); OnPropertyChanged(); }
        }

        private string _lastSearchText;
        private string _lastSearchOption;
        #endregion

        #region Commands

        public ICommand ViewPromoCommand { get; set; }
        public ICommand AcceptPromoCommand { get; set; }
        public ICommand RemovePromoCommand { get; set; }
        public ICommand BanPromoCommand { get; set; }
        public ICommand CloseSearchCommand { get; set; }

        #endregion

        #region Constructor

        public AdminPromoViewModel()
        {
            promoRepo = new GenericDataRepository<Models.Promo>();
            noteRepo = new GenericDataRepository<Models.Notification>();

            Task.Run(async () =>
            {
                MainViewModel.IsLoading = true;
                await Load();
            }).ContinueWith((first) =>
            {
                SearchByOptions = new List<string> { "Id", "IdShop", "Name", "Code" };
                ViewPromoCommand = new RelayCommand<object>(p => p != null, async (p) => await ViewPromo(p));
                AcceptPromoCommand = new RelayCommand<object>(p => p != null, async (p) => await AcceptPromo(p));
                RemovePromoCommand = new RelayCommand<object>(p => p != null, async (p) => await RemovePromo(p));
                BanPromoCommand = new RelayCommand<object>(p => p != null, async (p) => await BanPromo(p));
                CloseSearchCommand = new RelayCommandWithNoParameter(() => { SearchText = ""; });
                MainViewModel.IsLoading = false;
            });
        }

        public async Task Load()
        {
            var items = new ObservableCollection<Promo>(
                await promoRepo.GetListAsync(pro => pro.Status == 1 && pro.DateEnd > DateTime.Now));

            var reqItems = new ObservableCollection<Promo>(
                await promoRepo.GetListAsync(pro => pro.Status == 0 && pro.DateEnd > DateTime.Now));

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                Items = new ObservableCollection<ShopPromoBlockViewModel>(items.Select(item => new ShopPromoBlockViewModel(item)));
                RequestItems = new ObservableCollection<ShopPromoBlockViewModel>(reqItems.Select(item => new ShopPromoBlockViewModel(item)));
                _allItems = new ObservableCollection<ShopPromoBlockViewModel>(items.Select(item => new ShopPromoBlockViewModel(item)));
            }));
        }

        #endregion

        #region Command Methods

        public async Task ViewPromo(object obj)
        {

        }

        public async Task AcceptPromo(object obj)
        {
            MainViewModel.IsLoading = true;

            var pro = obj as ShopPromoBlockViewModel;
            if (pro == null)
                return;

            pro.Promo.Status = 1;

            var note = new Models.Notification
            {
                Id = await GenerateID.Gen(typeof(Models.Notification)),
                IdSender = AccountStore.instance.CurrentAccount.Id,
                IdReceiver = pro.Promo.IdShop,
                Content = "Your promo request is accepted.",
                Date = DateTime.Now,
            };

            await promoRepo.Update(pro.Promo);
            await noteRepo.Add(note);

            await Load();

            MainViewModel.IsLoading = false;

        }
        public async Task RemovePromo(object obj)
        {
            MainViewModel.IsLoading = true;

            var pro = obj as ShopPromoBlockViewModel;
            if (pro == null)
                return;

            pro.Promo.Status = 2;

            var note = new Models.Notification
            {
                Id = await GenerateID.Gen(typeof(Models.Notification)),
                IdSender = AccountStore.instance.CurrentAccount.Id,
                IdReceiver = pro.Promo.IdShop,
                Content = "Your promo request is rejected. Contact us for more information.",
                Date = DateTime.Now,
            };

            await promoRepo.Update(pro.Promo);
            await noteRepo.Add(note);

            await Load();

            MainViewModel.IsLoading = false;
        }

        public async Task BanPromo(object obj)
        {
            MainViewModel.IsLoading = true;

            var pro = obj as ShopPromoBlockViewModel;
            if (pro == null)
                return;

            if (pro.Promo.DateBegin < DateTime.Now)
            {
                var view = new ConfirmDialog()
                {
                    Header = "No!",
                    Content = "Promo is in use, you cannot ban it",
                };
                await DialogHost.Show(view, "Main");
            }
            else
            {
                pro.Promo.Status = 2;
                var note = new Models.Notification
                {
                    Id = await GenerateID.Gen(typeof(Models.Notification)),
                    IdSender = AccountStore.instance.CurrentAccount.Id,
                    IdReceiver = pro.Promo.IdShop,
                    Content = "Your promo is rejected. Contact us for more information.",
                    Date = DateTime.Now,
                };

                await promoRepo.Update(pro.Promo);
                await noteRepo.Add(note);
            }

            await Load();

            MainViewModel.IsLoading = false;


        }
        public void Search()
        {
            if (string.IsNullOrEmpty(SearchBy))
                Items = _allItems;

            if (string.IsNullOrEmpty(_lastSearchText) && string.IsNullOrEmpty(SearchText) ||
                (string.Equals(_lastSearchText, SearchText) && _lastSearchOption == SearchBy))
            {
                Items = _allItems;
            }

            if (string.IsNullOrEmpty(SearchText) || _allItems.Count <= 0 || _allItems == null)
            {
                Items = _allItems;
                return;
            }

            if (SearchBy == "Name")
            {
                _lastSearchOption = "Name";
                Items = new ObservableCollection<ShopPromoBlockViewModel>(_allItems.Where(br => br.Promo.Name.ToLower().Contains(SearchText.ToLower())));
            }
            else if (SearchBy == "Id")
            {
                _lastSearchOption = "Id";
                Items = new ObservableCollection<ShopPromoBlockViewModel>(_allItems.Where(br => br.Promo.Id.ToString().ToLower().Contains(SearchText.ToLower())));
            }
            else if (SearchBy == "IdShop")
            {
                _lastSearchOption = "IdShop";
                Items = new ObservableCollection<ShopPromoBlockViewModel>(_allItems.Where(br => br.Promo.IdShop.ToString().ToLower().Contains(SearchText.ToLower())));
            }
            else if (SearchBy == "Code")
            {
                _lastSearchOption = "Code";
                Items = new ObservableCollection<ShopPromoBlockViewModel>(_allItems.Where(br => br.Promo.Code.ToString().ToLower().Contains(SearchText.ToLower())));
            }
        }

        #endregion
    }
}
