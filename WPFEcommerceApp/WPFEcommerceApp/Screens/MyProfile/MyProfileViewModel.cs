using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class MyProfileViewModel:BaseViewModel
    {
        #region Public Properties

        private GenericDataRepository<MUser> userRepo;
        public MUser EditUser { get; set; }

        private string _sourceImageAva;
        public string SourceImageAva
        {
            get
            {
                return _sourceImageAva;
            }
            set
            {
                _sourceImageAva = value;
                OnPropertyChanged();
            }
        }
        private string _sourceImageAvaTemp;
        public string SourceImageAvaTemp
        {
            get
            {
                return _sourceImageAvaTemp;
            }
            set
            {
                _sourceImageAvaTemp = value;
                OnPropertyChanged();
            }
        }


        private string _sourceImageBackground;
        public string SourceImageBackground
        {
            get
            {
                return _sourceImageBackground;
            }
            set
            {
                _sourceImageBackground = value;
                OnPropertyChanged();
            }
        }

        private string _editSourceImageBackground;
        public string EditSourceImageBackground
        {
            get
            {
                return _editSourceImageBackground;
            }
            set
            {
                _editSourceImageBackground = value;
                OnPropertyChanged();
            }
        }

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
        #endregion

        #region Constructor

        public MyProfileViewModel()
        {
            userRepo = new GenericDataRepository<MUser>();

            Task.Run(async () => await Load());

            ChangeAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if (op.FileName != "")
                {
                    SourceImageAva = op.FileName;


                }
            });

            ChangeToDefaultAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                SourceImageAva = null;
            });

            ChangeBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if (op.FileName != "")
                {
                    SourceImageBackground = op.FileName;
                }
            });
            ChangeToDefaultBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                SourceImageBackground = string.Empty;
            });
            SaveBackgroundShopCommand = new RelayCommandWithNoParameter(async()=>await SaveBackground());

            SaveProfileCommand = new RelayCommand<object>(p => p != null,async(p)=>await SaveProfile(p));
            SaveAvaShopCommand = new RelayCommandWithNoParameter(async()=>await SaveAva());
            CancelProfileCommand = new RelayCommand<object>(p => p != null, async (p) => await CancelProfile(p));
        }
        #endregion

        #region Command Methods
        public async Task Load()
        {
            EditUser = await userRepo.GetSingleAsync(usr => usr.Id == AccountStore.instance.CurrentAccount.Id);
            if (!string.IsNullOrEmpty(AccountStore.instance.CurrentAccount.SourceImageAva))
            {
                SourceImageAvaTemp = AccountStore.instance.CurrentAccount.SourceImageAva;
                SourceImageAva = AccountStore.instance.CurrentAccount.SourceImageAva;
            }
            if(!string.IsNullOrEmpty(AccountStore.instance.CurrentAccount.SourceImageBackground))
            {
                EditSourceImageBackground = AccountStore.instance.CurrentAccount.SourceImageBackground;
                SourceImageBackground= AccountStore.instance.CurrentAccount.SourceImageBackground;
            }

        }

        public async Task SaveProfile(object obj)
        {
            var user = obj as MUser;
            if (user == null)
                return;
            await AccountStore.instance.Update(user);
            await Load();
        }
        public async Task SaveAva()
        {

            var link = await FireStorageAPI.Push(SourceImageAva, "User", $"Ava_{AccountStore.instance.CurrentAccount.Id}");

            SourceImageAva = link;
            SourceImageAvaTemp = link;
            EditUser.SourceImageAva = link;
            var nav = NavigationStore.instance.stackScreen;
            AccountStore.instance.CurrentAccount.SourceImageAva = link;
            await AccountStore.instance.Update(AccountStore.instance.CurrentAccount);
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public async Task SaveBackground()
        {
            var link = await FireStorageAPI.Push(SourceImageBackground, "User", $"Background_{AccountStore.instance.CurrentAccount.Id}");

            SourceImageBackground = link;
            EditSourceImageBackground = link;
            EditUser.SourceImageBackground = link;

            AccountStore.instance.CurrentAccount.SourceImageBackground=link;
            await AccountStore.instance.Update(AccountStore.instance.CurrentAccount);
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public async Task CancelProfile(object obj)
        {
            var user = obj as MUser;
            if (user == null)
                return;

            await Load();

        }
        #endregion
    }


}
