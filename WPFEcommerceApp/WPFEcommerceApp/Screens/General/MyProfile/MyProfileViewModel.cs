using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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
        public MUser EditUser { get; set; }

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

        public List<string> GenderOptions => new List<string> { "Female", "Male" };
        #endregion

        #region Commands

        public ICommand ChangeAvaShopCommand { get; set; }
        public ICommand ChangeToDefaultAvaShopCommand { get; set; }
        public ICommand ChangeBackgroundShopCommand { get; set; }
        public ICommand ChangeToDefaultBackgroundShopCommand { get; set; }
        public ICommand SaveBackgroundShopCommand { get; set; }

        public ICommand SaveProfileCommand { get; set; }
        public ICommand SaveAvaShopCommand { get; set; }

        public ICommand CancelProfileCommand { get; set; }

        public ICommand OpenAvaDialog { get; set; }
        public ICommand OpenBackgroundDialog { get; set; }

        #endregion

        #region Constructor

        public MyProfileViewModel()
        {
            userRepo = new GenericDataRepository<MUser>();


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


            Task.Run(async () =>
            {
                MainViewModel.IsLoading = true;
                await Load();
                MainViewModel.IsLoading = false;
            });

            SaveProfileCommand = new RelayCommand<object>(p => p != null, async (p) => await SaveProfile(p));
            CancelProfileCommand = new RelayCommand<object>(p => p != null, async (p) => await CancelProfile(p));
        }
        #endregion

        #region Command Methods
        public async Task Load()
        {
            EditUser = await userRepo.GetSingleAsync(usr => usr.Id == AccountStore.instance.CurrentAccount.Id);


        }

        private async void SaveAva(object sender, DialogClosedEventArgs eventArgs)
        {
            MainViewModel.IsLoading = true;
            if (eventArgs.Parameter != null && eventArgs.Parameter.GetType() == typeof(CroppedBitmap))
            {
                ImageAva = (eventArgs.Parameter as CroppedBitmap);

                string link;
                if (String.IsNullOrEmpty(SourceImageAva) || SourceImageAva.Contains("https://firebasestorage.googleapis.com"))
                {
                    link = await FireStorageAPI.PushFromImage((BitmapSource)ImageAva, "User", $"Ava_{AccountStore.instance.CurrentAccount.Id}", AccountStore.instance.CurrentAccount.SourceImageAva);
                }
                else
                {
                    link = await FireStorageAPI.PushFromImage((BitmapSource)ImageAva, "User", $"Ava_{AccountStore.instance.CurrentAccount.Id}");
                }

                AccountStore.instance.CurrentAccount.SourceImageAva = link;
                await AccountStore.instance.Update(AccountStore.instance.CurrentAccount);
                await Load();
            }

            MainViewModel.IsLoading = false;
        }

        private async void SaveBackground(object sender, DialogClosedEventArgs eventArgs)
        {
            MainViewModel.IsLoading = true;
            if (eventArgs.Parameter != null && eventArgs.Parameter.GetType() == typeof(CroppedBitmap))
            {
                ImageBackground = (eventArgs.Parameter as CroppedBitmap);

                string link;
                if (String.IsNullOrEmpty(SourceImageBackground) || SourceImageBackground.Contains("https://firebasestorage.googleapis.com"))
                {
                    link = await FireStorageAPI.PushFromImage((BitmapSource)ImageBackground, "User", $"Background_{AccountStore.instance.CurrentAccount.Id}", AccountStore.instance.CurrentAccount.SourceImageAva);
                }
                else
                {
                    link = await FireStorageAPI.PushFromImage((BitmapSource)ImageBackground, "User", $"Background_{AccountStore.instance.CurrentAccount.Id}");
                }

                AccountStore.instance.CurrentAccount.SourceImageBackground = link;
                await AccountStore.instance.Update(AccountStore.instance.CurrentAccount);
                await Load();
            }

            MainViewModel.IsLoading = false;
        }

        public async Task SaveProfile(object obj)
        {
            MainViewModel.IsLoading = true;
            var user = obj as MUser;
            if (user == null)
                return;
            await AccountStore.instance.Update(user);
            await Load();
            MainViewModel.IsLoading = false;

        }

        public async Task CancelProfile(object obj)
        {
            MainViewModel.IsLoading = true;

            var user = obj as MUser;
            if (user == null)
                return;

            await Load();
            MainViewModel.IsLoading = false;

        }
        #endregion
    }


}
