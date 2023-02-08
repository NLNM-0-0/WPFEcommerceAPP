
using DataAccessLayer;
using LiveCharts;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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
        private DispatcherTimer timer;
        #endregion

        #region Commands

        public ICommand ViewPromoCommand { get; set; }
        public ICommand AcceptPromoCommand { get; set; }
        public ICommand RemovePromoCommand { get; set; }
        public ICommand BanPromoCommand { get; set; }
        public ICommand CloseSearchCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        #endregion

        #region Constructor

        public AdminPromoViewModel()
        {
            promoRepo = new GenericDataRepository<Models.Promo>();
            noteRepo = new GenericDataRepository<Models.Notification>();
            SearchBy = "Code";
            timer=new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(600);
            timer.Tick += async delegate { await Search(); };

            Task.Run(async () =>
            {
                MainViewModel.SetLoading(true);
                await Load();
            }).ContinueWith((first) =>
            {
                SearchByOptions = new List<string> { "Id", "IdShop", "Name", "Code" };
                ViewPromoCommand = new RelayCommand<object>(p => p != null, async (p) => await ViewPromo(p));
                AcceptPromoCommand = new RelayCommand<object>(p => p != null, async (p) => await AcceptPromo(p));
                RemovePromoCommand = new RelayCommand<object>(p => p != null, async (p) => await RemovePromo(p));
                BanPromoCommand = new RelayCommand<object>(p => p != null, async (p) => await BanPromo(p));
                SearchCommand = new RelayCommandWithNoParameter(() =>
                {
                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        if(!string.Equals(SearchText, _lastSearchText)||!string.Equals(_lastSearchOption, SearchBy))
                        {
                            if (!timer.IsEnabled)
                                timer.Stop();
                            timer.Start();
                        }
                    }
                    else
                    {
                        if (timer.IsEnabled)
                            timer.Stop();
                        if (string.IsNullOrEmpty(SearchText) || _allItems.Count <= 0 || _allItems == null)
                        {
                            Items = _allItems;
                        }
                    }
                    
                });
                CloseSearchCommand = new RelayCommandWithNoParameter(() => { SearchText = ""; });
                MainViewModel.SetLoading(false);
            });
        }

        public async Task Load()
        {
            var items = new ObservableCollection<Promo>(
                await promoRepo.GetListAsync(pro => pro.Status == 1 && pro.DateEnd > DateTime.Now, pro=>pro.Products, pro => pro.Products.Select(p => p.ImageProducts)));

            var reqItems = new ObservableCollection<Promo>(
                await promoRepo.GetListAsync(pro => pro.Status == 0 && pro.DateEnd > DateTime.Now, pro=>pro.Products, pro => pro.Products.Select(p => p.ImageProducts)));

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
            var promo=obj as ShopPromoBlockViewModel;
            if(promo!=null)
            {
                var temp = new PromoVMConstructor(promo.Promo, true);
                NavigateProvider.PromoInfomationScreen().Navigate(temp);
            }
        }

        public async Task AcceptPromo(object obj)
        {
            MainViewModel.SetLoading(true);

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
                HaveSeen = false
            };

            await promoRepo.Update(pro.Promo);
            await noteRepo.Add(note);

            await Load();

            MainViewModel.SetLoading(false);

        }
        public async Task RemovePromo(object obj)
        {
            MainViewModel.SetLoading(true);

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
                HaveSeen=false
            };

            await promoRepo.Update(pro.Promo);
            await noteRepo.Add(note);

            await Load();

            MainViewModel.SetLoading(false);
        }

        public async Task BanPromo(object obj)
        {
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
                MainViewModel.SetLoading(true);

                pro.Promo.Status = 2;
                var note = new Models.Notification
                {
                    Id = await GenerateID.Gen(typeof(Models.Notification)),
                    IdSender = AccountStore.instance.CurrentAccount.Id,
                    IdReceiver = pro.Promo.IdShop,
                    Content = "Your promo is rejected. Contact us for more information.",
                    Date = DateTime.Now,
                    HaveSeen = false
                };

                await promoRepo.Update(pro.Promo);
                await noteRepo.Add(note);
            }
            MainViewModel.SetLoading(true);

            await Load();

            MainViewModel.SetLoading(false);


        }
        public async Task Search()
        {
            MainViewModel.SetLoading(true);
            await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(SearchBy))
                {
                    Items = _allItems;
                }

                if (string.IsNullOrEmpty(_lastSearchText) && string.IsNullOrEmpty(SearchText) ||
                    (string.Equals(_lastSearchText, SearchText) && _lastSearchOption == SearchBy))
                {
                    Items = _allItems;
                }

                if (string.IsNullOrEmpty(SearchText) || _allItems.Count <= 0 || _allItems == null)
                {
                    Items = _allItems;
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
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    Items = new ObservableCollection<ShopPromoBlockViewModel>(Items);
                }));
                if (timer.IsEnabled)
                    timer.Stop();
            });
            MainViewModel.SetLoading(false);

        }

        #endregion
    }
}
