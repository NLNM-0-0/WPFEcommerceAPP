using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO.Packaging;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFEcommerceApp
{
    public class ProfileShopDialogViewModel: BaseViewModel
    {
        private readonly AccountStore _accountStore;
        private GenericDataRepository<Models.MUser> userRepository= new GenericDataRepository<Models.MUser>();
        public ICommand OpenProfileShopBackgroundDialog { get; set; }
        public ICommand OpenProfileShopAvaDialog { get; set; }
        public ICommand SaveProfileShopCommand { get; set; }
        public ICommand EditProfileShopCommand { get; set; }
        public ICommand ChangeEditStatusCommand { get; set; }
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
        private string sourceImageAva;
        public string SourceImageAva
        {
            get => sourceImageAva;
            set
            {
                sourceImageAva = value;
                OnPropertyChanged();
            }
        }
        private string sourceImageBackground;
        public string SourceImageBackground
        {
            get => sourceImageBackground;
            set
            {
                sourceImageBackground = value;
                OnPropertyChanged();
            }
        }
        private bool isEditing = false;
        public bool IsEditing
        {
            get => isEditing;
            set
            {
                isEditing = value;
                OnPropertyChanged();
            }
        }
        public ProfileShopDialogViewModel(AccountStore accountStore)
        {
            _accountStore = accountStore;
            Task task = Task.Run(async () => await LoadTempData());
            while (!task.IsCompleted) ;
            if (Shop == null || string.IsNullOrEmpty(Shop.SourceImageAva))
            {
                SourceImageAva = Properties.Resources.DefaultShopAvaImage;
            }
            else
            {
                SourceImageAva = Shop.SourceImageAva;
            }
            if (Shop == null || string.IsNullOrEmpty(Shop.SourceImageBackground))
            {
                SourceImageBackground = Properties.Resources.DefaultShopBackgroundImage;
            }
            else
            {
                SourceImageBackground = Shop.SourceImageBackground;
            }
            OpenProfileShopBackgroundDialog = new RelayCommandWithNoParameter(() =>
            {
                ProfileShopBackgroundDialog profileShopBackgroundDialog = new ProfileShopBackgroundDialog();
                profileShopBackgroundDialog.DataContext = new ProfileShopBackgroundDialogViewModel(Shop);
                DialogHost.Show(profileShopBackgroundDialog, "SecondDialog", null, null, LoadBackground);
            });
            OpenProfileShopAvaDialog = new RelayCommandWithNoParameter(() =>
            {
                ProfileShopAvaDialog profileShopAvaDialog = new ProfileShopAvaDialog();
                profileShopAvaDialog.DataContext = new ProfileShopAvaDialogViewModel(Shop);
                DialogHost.Show(profileShopAvaDialog, "SecondDialog", null, null, LoadAva);
            });
            SaveProfileShopCommand = new RelayCommandWithNoParameter(() =>
            {
                ChangeToEdittable();
            });
            EditProfileShopCommand = new RelayCommandWithNoParameter(() =>
            {
                IsEditing = true;
            });
            ChangeEditStatusCommand = new RelayCommand<bool?>((p) => p!=null,(p)=>
            {
                if(IsEditing && (p??false))
                {
                    ChangeToEdittable();
                }
                else if(!isEditing)
                {
                    IsEditing = true;
                }    
            });
        }
        private void ChangeToEdittable()
        {
            IsEditing = false;
            Task.Run(async () =>
            {
                await _accountStore.Update(Shop);
                await LoadTempData();
            });
        }
        private void LoadBackground(object sender, DialogClosedEventArgs eventArgs)
        {
            Task.Run(async () => await UpdateAccountStore());
            OnPropertyChanged(nameof(Shop));
            if (Shop == null || string.IsNullOrEmpty(Shop.SourceImageBackground))
            {
                SourceImageBackground = Properties.Resources.DefaultShopBackgroundImage;
            }
            else
            {
                SourceImageBackground = Shop.SourceImageBackground;
            }
            OnPropertyChanged(nameof(SourceImageBackground));
            
        }
        private void LoadAva(object sender, DialogClosedEventArgs eventArgs)
        {
            Task.Run(async () => await UpdateAccountStore());
            OnPropertyChanged(nameof(Shop));
            if (Shop == null || string.IsNullOrEmpty(Shop.SourceImageAva))
            {
                SourceImageAva = Properties.Resources.DefaultShopAvaImage;
            }
            else
            {
                SourceImageAva = Shop.SourceImageAva;
            }
            OnPropertyChanged(nameof(SourceImageAva));
        }

        public async Task LoadTempData()
        {
            Shop = await userRepository.GetSingleAsync(u => u.Id == _accountStore.CurrentAccount.Id);
        }
        public async Task UpdateAccountStore()
        {
            await _accountStore.Update(Shop);
        }
    }
}
