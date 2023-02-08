using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;
using static System.Windows.Forms.LinkLabel;

namespace WPFEcommerceApp
{
    public class MyProfileViewModel:BaseViewModel
    {
        #region Public Properties

        private GenericDataRepository<MUser> userRepo;
        private GenericDataRepository<Address> addressRepo;
        private readonly GenericDataRepository<UserLogin> loginRepo = new GenericDataRepository<UserLogin>();
        public MUser EditUser { get; set; }
        public bool IsAdmin
        {
            get => AccountStore.instance.CurrentAccount != null
                && AccountStore.instance.CurrentAccount.Role == "Admin";
        }

        public ObservableCollection<AddressItemViewModel> AddressList { get; set; }

        public string SourceImageBackground
        {
            get
            {
                if (AccountStore.instance.CurrentAccount == null || string.IsNullOrEmpty(AccountStore.instance.CurrentAccount.SourceImageBackground))
                {
                    return Properties.Resources.DefaultShopBackgroundImage;
                }
                else
                {
                    return AccountStore.instance.CurrentAccount.SourceImageBackground;
                }
            }
        }
        public string SourceImageAva
        {
            get
            {
                if (AccountStore.instance.CurrentAccount == null || string.IsNullOrEmpty(AccountStore.instance.CurrentAccount.SourceImageAva))
                {
                    return Properties.Resources.DefaultShopAvaImage;
                }
                else
                {
                    return AccountStore.instance.CurrentAccount.SourceImageAva;
                }
            }
        }

        private CroppedBitmap imageAva;
        public CroppedBitmap ImageAva
        {
            get => imageAva;
            set
            {
                imageAva = value;
                OnPropertyChanged(nameof(SourceImageAva));
                OnPropertyChanged();
            }
        }

        private CroppedBitmap imageBackground;
        public CroppedBitmap ImageBackground
        {
            get => imageBackground;
            set
            {
                imageBackground = value;
                OnPropertyChanged(nameof(imageBackground));
                OnPropertyChanged();
            }
        }
        public bool IsGoogle { get; set; }
        public List<string> GenderOptions => new List<string> { "Female", "Male" };
        public string CurrentPassword { get; set; }
        private string newPassword;
        public string NewPassword {
            get => newPassword;
            set {
                newPassword = value;
                ConfirmNewPassword = null;
                ConfirmNewPassword = "";
            }
        }
        public string ConfirmNewPassword { get; set; }
        #endregion

        #region Commands
        public ICommand SaveProfileCommand { get; set; }
        public ICommand SaveAvaShopCommand { get; set; }
        public ICommand CancelProfileCommand { get; set; }
        public ICommand OpenAvaDialog { get; set; }
        public ICommand OpenBackgroundDialog { get; set; }
        public ICommand AddAddressCommand { get; set; }
        public ICommand SavePasswordCommand { get; set; }
        public ICommand DoubleClickCommand { get; set; }
        #endregion

        #region Constructor

        public MyProfileViewModel()
        {
            userRepo = new GenericDataRepository<MUser>();
            addressRepo = new GenericDataRepository<Address>();
            if (AccountStore.instance.CurrentAccount.UserLogin.Provider == 1)
            {
                IsGoogle = true;
            }

            OpenAvaDialog = new RelayCommandWithNoParameter(async () =>
            {
                var avaDialog = new ProfileShopAvaDialog();
                ImageAva = new CroppedBitmap(new BitmapImage(new Uri(SourceImageAva)), new Int32Rect(0, 0, 0, 0));
                avaDialog.DataContext = new ProfileShopAvaDialogViewModel(ImageAva);
                await DialogHost.Show(avaDialog, "Main", null, null,SaveAva);
            });

            OpenBackgroundDialog = new RelayCommandWithNoParameter(async () =>
            {
                ProfileShopBackgroundDialog bgrDialog = new ProfileShopBackgroundDialog();
                ImageBackground = new CroppedBitmap(new BitmapImage(new Uri(SourceImageBackground)), new Int32Rect(0, 0, 0, 0));
                bgrDialog.DataContext = new ProfileShopBackgroundDialogViewModel(ImageBackground);
                await DialogHost.Show(bgrDialog, "Main", null, null, SaveBackground);
            });
            DoubleClickCommand = new RelayCommandWithNoParameter(() =>
            {

            });

            Task.Run(async () =>
            {
                MainViewModel.SetLoading(true);
                await Load();
            }).ContinueWith((task) =>
            {
                SaveProfileCommand = new RelayCommand<object>(p => p != null, async (p) => await SaveProfile(p));
                CancelProfileCommand = new RelayCommand<object>(p => p != null, async (p) => await CancelProfile(p));
                AddAddressCommand = new RelayCommandWithNoParameter(async () => await OpenAddAddressDialog());
                AddressDialogViewModel.AddressCommand = new RelayCommand<object>(p => p != null, async (p) => await AddNewAddress(p));
                AddressItemViewModel.EditAddressCommand = new RelayCommand<object>(p => p != null, async (p) => await OpenEditAddress(p));
                AddressDialogViewModel.RemoveAddressCommand = new RelayCommand<object>(p => p != null, async (p) => await RemoveAddress(p));
                SavePasswordCommand = new RelayCommand<object>(p => {
                    return !string.IsNullOrEmpty(CurrentPassword) &&
                    !string.IsNullOrEmpty(NewPassword) &&
                    !string.IsNullOrEmpty(ConfirmNewPassword) &&
                    ConfirmNewPassword == NewPassword;
                }, async p => {
                    var hasher = new Hashing();
                    var user = AccountStore.instance.CurrentAccount.UserLogin;
                    var hash = hasher.Encrypt(user.Salt, CurrentPassword);
                    var dl = new ConfirmDialog() {
                        Content = "Your password is wrong, please try again!",
                        Header = "Oops",
                    };
                    if(hash != user.Password) {
                        await DialogHost.Show(dl, "Main");
                        return;
                    }
                    dl.Content = "Your password has changed!";
                    dl.Header = "Success";

                    var salt = Convert.ToBase64String(hasher.GenerateSalt());
                    user.Password = hasher.Encrypt(salt, NewPassword);
                    user.Salt = salt;
                    if(user.Provider == -1) user.Provider = 0;
                    await loginRepo.Update(user);
                    await DialogHost.Show(dl, "Main");
                    CurrentPassword = "";
                    NewPassword = "";
                    ConfirmNewPassword ="";
                });
                MainViewModel.SetLoading(false);
            });
            
        }
        #endregion

        #region Command Methods
        public async Task Load()
        {
            EditUser = await userRepo.GetSingleAsync(usr => usr.Id == AccountStore.instance.CurrentAccount.Id,
                usr => usr.Addresses, usr=>usr.UserLogin
                );

            if (AccountStore.instance != null)
            {
                AccountStore.instance.AccountChanged += OnAccountChange;
            }

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                AddressList = new ObservableCollection<AddressItemViewModel>();
                foreach (var item in EditUser.Addresses)
                {
                    if (item.Status != true)
                        continue;

                    bool isDefault = false;
                    if (item.Id == EditUser.DefaultAddress)
                    {
                        isDefault = true;
                        AddressList.Insert(0, new AddressItemViewModel(item, isDefault));
                        continue;
                    }
                    AddressList.Add(new AddressItemViewModel(item, isDefault));
                }
            }));
        }
        private void OnAccountChange()
        {
            OnPropertyChanged(nameof(IsAdmin));
        }
        private async void SaveAva(object sender, DialogClosedEventArgs eventArgs)
        {
            MainViewModel.SetLoading(true);
            if (eventArgs.Parameter != null && eventArgs.Parameter.GetType() == typeof(CroppedBitmap))
            {
                ImageAva = (eventArgs.Parameter as CroppedBitmap);

                string link;
                if (String.IsNullOrEmpty(SourceImageAva) || SourceImageAva.Contains("https://firebasestorage.googleapis.com"))
                {
                    link = await FireStorageAPI.PushFromImage((BitmapSource)ImageAva, "User", $"Ava", AccountStore.instance.CurrentAccount.SourceImageAva, $"{AccountStore.instance.CurrentAccount.Id}");
                }
                else
                {
                    link = await FireStorageAPI.PushFromImage((BitmapSource)ImageAva, "User", "Ava", null, $"{AccountStore.instance.CurrentAccount.Id}");
                }

                AccountStore.instance.CurrentAccount.SourceImageAva = link;
                OnPropertyChanged(nameof(SourceImageAva));
                await AccountStore.instance.Update(AccountStore.instance.CurrentAccount);
                List<SearchItemViewModel> searchItemViewModels = new List<SearchItemViewModel>(HeaderViewModel.AllItems.Where(p => p.IsProduct == false && (p.Model as MUser).Id == AccountStore.instance.CurrentAccount.Id));
                if (searchItemViewModels != null && searchItemViewModels.Count != 0)
                {
                    SearchItemViewModel searchItemViewModel = searchItemViewModels.First();
                    searchItemViewModel.SourceImage = String.IsNullOrEmpty(link) ? null : link;
                }
                await Load();
            }

            MainViewModel.SetLoading(false);
        }
        private async void SaveBackground(object sender, DialogClosedEventArgs eventArgs)
        {
            MainViewModel.SetLoading(true);
            if (eventArgs.Parameter != null && eventArgs.Parameter.GetType() == typeof(CroppedBitmap))
            {
                ImageBackground = (eventArgs.Parameter as CroppedBitmap);

                string link;
                if (String.IsNullOrEmpty(SourceImageBackground) || SourceImageBackground.Contains("https://firebasestorage.googleapis.com"))
                {
                    link = await FireStorageAPI.PushFromImage((BitmapSource)ImageBackground, "User", $"Background", AccountStore.instance.CurrentAccount.SourceImageAva, $"{AccountStore.instance.CurrentAccount.Id}");
                }
                else
                {
                    link = await FireStorageAPI.PushFromImage((BitmapSource)ImageBackground, "User", "Background", null, $"{AccountStore.instance.CurrentAccount.Id}");
                }

                AccountStore.instance.CurrentAccount.SourceImageBackground = link;
                OnPropertyChanged(nameof(SourceImageBackground));
                await AccountStore.instance.Update(AccountStore.instance.CurrentAccount);
                await Load();
            }

            MainViewModel.SetLoading(false);
        }
        public async Task SaveProfile(object obj)
        {
            MainViewModel.SetLoading(true);

            var userlogin = AccountStore.instance.CurrentAccount.UserLogin;

            var user = obj as MUser;
            if (user == null)
                return;
            if(userlogin.Username != EditUser.Email) {
                var check = await loginRepo.GetSingleAsync(d => d.Username == EditUser.Email);
                if(check == null) {
                    await AccountStore.instance.Update(user);
                    userlogin.Username = EditUser.Email;
                    AccountStore.instance.CurrentAccount.UserLogin = userlogin;
                    List<SearchItemViewModel> searchItemViewModels =new List<SearchItemViewModel>(HeaderViewModel.AllItems.Where(p => p.IsProduct == false && (p.Model as MUser).Id == user.Id));
                    if(searchItemViewModels != null && searchItemViewModels.Count!=0)
                    {
                        SearchItemViewModel searchItemViewModel = searchItemViewModels.First();
                        searchItemViewModel.Model = user;
                        searchItemViewModel.Name = user.Name;
                    }    
                    await loginRepo.Update(userlogin);
                } 
                else {
                    var dl = new ConfirmDialog() {
                        Content = "This email is already exist, please try again!",
                        Header = "Oops"
                    };
                    await DialogHost.Show(dl, "Main");
                    MainViewModel.SetLoading(false);
                    return;
                }
            }
            else {
                await AccountStore.instance.Update(user);
                List<SearchItemViewModel> searchItemViewModels = new List<SearchItemViewModel>(HeaderViewModel.AllItems.Where(p => p.IsProduct == false && (p.Model as MUser).Id == user.Id));
                if (searchItemViewModels != null && searchItemViewModels.Count != 0)
                {
                    SearchItemViewModel searchItemViewModel = searchItemViewModels.First();
                    searchItemViewModel.Model = user;
                    searchItemViewModel.Name = user.Name;
                }
            }
            await Load();
            MainViewModel.SetLoading(false);

        }
        public async Task CancelProfile(object obj)
        {
            MainViewModel.SetLoading(true);

            var user = obj as MUser;
            if (user == null)
                return;

            await Load();
            MainViewModel.SetLoading(false);

        }
        public async Task OpenAddAddressDialog()
        {
            var dialog = new AddAddressDialog();
            var add = new Address();
            dialog.DataContext = new AddressDialogViewModel
            {
                Header = "Add new address",
                CommandContent = "Add",
                Address = add,
                IsAdding = true,
                IsDefault = false,
            };

            await DialogHost.Show(dialog, "Main", null, null, null);
        }
        public async Task OpenEditAddress(object obj)
        {
            var dialog = new AddAddressDialog();
            var add = obj as AddressItemViewModel;

            if (add == null)
                return;

            dialog.DataContext = new AddressDialogViewModel
            {
                Header = "Edit address",
                CommandContent = "Save",
                Address = add.Address,
                IsAdding = false,
                IsDefault = add.IsDefault,
            };

            await DialogHost.Show(dialog, "Main", null, null, Reload);
        }
        private async void Reload(object sender, DialogClosedEventArgs eventArgs)
        {
            await Load();
        }
        public async Task AddNewAddress(object obj)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);

            MainViewModel.SetLoading(true);
            var ad = obj as AddressDialogViewModel;
            if (ad == null)
                return;

            var address = ad.Address;

            if (ad.IsAdding)
            {
                address.IdUser = AccountStore.instance.CurrentAccount.Id;
                address.Status = true;
                address.Id = GenerateID.DateTimeID();
                await addressRepo.Add(address);
            }
            else
            {
                await addressRepo.Update(address);
            }

            if (ad.IsSetAsDefault)
            {
                AccountStore.instance.CurrentAccount.DefaultAddress = ad.Address.Id;
                await AccountStore.instance.Update(AccountStore.instance.CurrentAccount);
            }

            await Load();
            MainViewModel.SetLoading(false);

        }
        public async Task RemoveAddress(object obj)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            MainViewModel.SetLoading(true);

            var add = obj as AddressDialogViewModel;
            if (add == null)
                return;

            if (add.IsDefault)
            {
                MainViewModel.SetLoading(false);

                var view = new ConfirmDialog
                {
                    Header = "No!",
                    Content = "You cannot remove default address"
                };
                await DialogHost.Show(view, "Main");
            }
            else
            {
                add.Address.Status = false;
                await addressRepo.Update(add.Address);
                await Load();
            }
            MainViewModel.SetLoading(false);

        }
        #endregion

        public override void Dispose()
        {
            AccountStore.instance.AccountChanged -= OnAccountChange;
            base.Dispose();
        }

    }
}
