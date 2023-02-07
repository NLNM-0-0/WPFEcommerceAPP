using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;
using static System.Windows.Forms.LinkLabel;

namespace WPFEcommerceApp
{
    public class ProfileShopDialogViewModel: BaseViewModel
    {
        private GenericDataRepository<Models.MUser> userRepository= new GenericDataRepository<Models.MUser>();
        public ICommand OpenProfileShopBackgroundDialog { get; set; }
        public ICommand OpenProfileShopAvaDialog { get; set; }
        public ICommand SaveProfileShopCommand { get; set; }
        public ICommand EditProfileShopCommand { get; set; }
        public ICommand ChangeEditStatusCommand { get; set; }
        public ICommand DoubleClickCommand { get; set; }
        private Models.MUser shop;
        public Models.MUser Shop 
        {
            get => shop;
            set
            {
                shop = value;
                OnPropertyChanged(nameof(shop));
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
        public ImageSource SourceImageAva
        {
            get
            {
                return (ImageSource)ImageAva;
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
        public ImageSource SourceImageBackground
        {
            get
            {
                return (ImageSource)ImageBackground;
            }
        }
        private bool isEditing = true;
        public bool IsEditing
        {
            get => isEditing;
            set
            {
                isEditing = value;
                OnPropertyChanged();
            }
        }
        private bool isGGAccount = false;
        public bool IsGGAccount
        {
            get => isGGAccount;
            set
            {
                isGGAccount = value;
                OnPropertyChanged();
            }
        }
        private bool isChangedAva = false;
        private bool isChangedBackground = false;
        private System.Windows.Controls.UserControl PreviousItem;
        public ProfileShopDialogViewModel()
        {
            Task.Run(async () =>
            {
                await LoadTempData();
            }).Wait();
            if(Shop.UserLogin.Provider == 1)
            {
                IsGGAccount = true;
            }    
            else
            {
                IsGGAccount = false;
            }
            if (Shop == null || string.IsNullOrEmpty(Shop.SourceImageAva))
            {
                ImageAva = new CroppedBitmap(new BitmapImage(new Uri(Properties.Resources.DefaultShopAvaImage)), new Int32Rect(0, 0, 0, 0));
            }
            else
            {
                ImageAva = new CroppedBitmap(new BitmapImage(new Uri(shop.SourceImageAva)), new Int32Rect(0, 0, 0, 0));
            }
            if (Shop == null || string.IsNullOrEmpty(Shop.SourceImageBackground))
            {
                ImageBackground = new CroppedBitmap(new BitmapImage(new Uri(Properties.Resources.DefaultShopBackgroundImage)), new Int32Rect(0, 0, 0, 0));
            }
            else
            {
                ImageBackground = new CroppedBitmap(new BitmapImage(new Uri(Shop.SourceImageBackground)), new Int32Rect(0, 0, 0, 0));
            }
            OpenProfileShopBackgroundDialog = new RelayCommandWithNoParameter(async () =>
            {
                PreviousItem = MainViewModel.UpdateDialog("Main");
                MainViewModel.SetLoading(true);
                ProfileShopBackgroundDialog profileShopBackgroundDialog = new ProfileShopBackgroundDialog();
                profileShopBackgroundDialog.DataContext = new ProfileShopBackgroundDialogViewModel(ImageBackground, PreviousItem);
                MainViewModel.SetLoading(false);
                await DialogHost.Show(profileShopBackgroundDialog, "Main");
            });
            OpenProfileShopAvaDialog = new RelayCommandWithNoParameter(async () =>
            {
                PreviousItem = MainViewModel.UpdateDialog("Main");
                MainViewModel.SetLoading(true);
                ProfileShopAvaDialog profileShopAvaDialog = new ProfileShopAvaDialog();
                profileShopAvaDialog.DataContext = new ProfileShopAvaDialogViewModel(ImageAva, PreviousItem);
                MainViewModel.SetLoading(false);
                await DialogHost.Show(profileShopAvaDialog, "Main");
            });
            SaveProfileShopCommand = new RelayCommandWithNoParameter(async () =>
            {
                PreviousItem = MainViewModel.UpdateDialog("Main");
                MainViewModel.SetLoading(true);
                IsEditing = false;
                await SaveImage();
                Shop.Email = Shop.Email.Trim();
                Shop.Name = Shop.Name.Trim();
                Shop.Description = Shop.Description.Trim();
                await UpdateShop();
                await LoadTempData();
                List<SearchItemViewModel> searchItemViewModels = new List<SearchItemViewModel>(HeaderViewModel.AllItems.Where(p => p.IsProduct == false && (p.Model as MUser).Id == Shop.Id));
                if (searchItemViewModels != null && searchItemViewModels.Count != 0)
                {
                    SearchItemViewModel searchItemViewModel = searchItemViewModels.First();
                    searchItemViewModel.Model = Shop;
                    searchItemViewModel.Name = Shop.Name;
                    searchItemViewModel.SourceImage = String.IsNullOrEmpty(Shop.SourceImageAva) ? null : Shop.SourceImageAva;
                }
            });
            EditProfileShopCommand = new RelayCommandWithNoParameter(() =>
            {
                IsEditing = true;
            });
            ChangeEditStatusCommand = new RelayCommand<bool?>((p) => p != null, async (p) =>
            {
                if (IsEditing && (p ?? false))
                {
                    MainViewModel.SetLoading(true);
                    IsEditing = false;
                    await AccountStore.instance.Update(Shop);
                    await LoadTempData();
                    MainViewModel.SetLoading(false);
                }
                else if (!isEditing)
                {
                    IsEditing = true;
                }
            });
            DoubleClickCommand = new RelayCommandWithNoParameter(() =>
            {

            });
        }
        public void LoadBackground(CroppedBitmap croppedBitmap)
        {
            OnPropertyChanged(nameof(Shop));
            ImageBackground = croppedBitmap;
            isChangedBackground = true;
        }

        public void LoadAva(CroppedBitmap croppedBitmap)
        {
            OnPropertyChanged(nameof(Shop));
            ImageAva = croppedBitmap;
            isChangedAva = true;
        }

        public async Task LoadTempData()
        {
            Shop = await userRepository.GetSingleAsync(u => u.Id == AccountStore.instance.CurrentAccount.Id, 
                                                        u => u.UserLogin);
        }
        public async Task UpdateShop()
        {
            GenericDataRepository<Models.UserLogin> loginRepo = new GenericDataRepository<UserLogin>();
            Models.UserLogin userlogin = await loginRepo.GetSingleAsync(l => l.IdUser == Shop.Id);
            if (Shop.UserLogin.Username != Shop.Email)
            {
                var check = await loginRepo.GetSingleAsync(d => d.Username == Shop.Email);
                if (check == null)
                {
                    await AccountStore.instance.Update(Shop);
                    userlogin.Username = Shop.Email;
                    AccountStore.instance.CurrentAccount.UserLogin = userlogin;
                    await loginRepo.Update(userlogin);
                    MainViewModel.SetLoading(false);
                }
                else
                {
                    var dl = new NotificationDialog()
                    {
                        ContentDialog = "This email is already exist, please try again!",
                        Header = "Oops", 
                        CloseCommand = new RelayCommandWithNoParameter(()=>
                        {
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            if (PreviousItem != null)
                            {
                                DialogHost.Show(PreviousItem, "Main");
                            }
                        })
                    };
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(dl, "Main");
                    return;
                }
            }
            else {
                await AccountStore.instance.Update(Shop);
            }
            MainViewModel.SetLoading(false);
        }
        public async Task SaveImage()
        {
            if (isChangedAva || isChangedBackground)
            {
                Models.MUser mUser = await userRepository.GetSingleAsync(p => p.Id == Shop.Id);
                string link;

                if (isChangedBackground)
                {
                    if (String.IsNullOrEmpty(mUser.SourceImageBackground) || mUser.SourceImageBackground.Contains("https://firebasestorage.googleapis.com"))
                    {
                        link = await FireStorageAPI.PushFromImage((BitmapSource)SourceImageBackground, "User", $"Background", mUser.SourceImageBackground, $"{Shop.Id}");
                    }
                    else
                    {
                        link = await FireStorageAPI.PushFromImage((BitmapSource)SourceImageBackground, "User", $"Background", null, $"{Shop.Id}");
                    }
                    Shop.SourceImageBackground = link;
                }

                if (isChangedAva)
                {
                    if (String.IsNullOrEmpty(mUser.SourceImageAva) || mUser.SourceImageAva.Contains("https://firebasestorage.googleapis.com"))
                    {
                        link = await FireStorageAPI.PushFromImage((BitmapSource)SourceImageAva, "User", $"Ava", mUser.SourceImageAva, $"{Shop.Id}");
                    }
                    else
                    {
                        link = await FireStorageAPI.PushFromImage((BitmapSource)SourceImageAva, "User", $"Ava", null, $"{Shop.Id}");
                    }
                    Shop.SourceImageAva = link;
                }   
            }
        }
    }
}
