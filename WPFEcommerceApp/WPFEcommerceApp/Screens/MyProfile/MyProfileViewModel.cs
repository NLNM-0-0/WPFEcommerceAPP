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
        public MUser ThisUser { get; set; }
        public MUser EditUser { get; set; }

        private ImageSource _sourceImageAva;
        public ImageSource SourceImageAva
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
        private ImageSource _sourceImageAvaTemp;
        public ImageSource SourceImageAvaTemp
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


        private ImageSource _sourceImageBackground;
        public ImageSource SourceImageBackground
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

        private ImageSource _editSourceImageBackground;
        public ImageSource EditSourceImageBackground
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

        private readonly AccountStore _accountStore;
        public MyProfileViewModel(AccountStore accountStore)
        {
            _accountStore = accountStore;
            userRepo = new GenericDataRepository<MUser>();
            //ThisUser = _accountStore.CurrentAccount;
            Load();

            ChangeAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if (op.FileName != "")
                {
                    var temp = SourceImageAva;
                    var src = new Uri(op.FileName);
                    SourceImageAva = new BitmapImage(src);


                }
            });

            ChangeToDefaultAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                SourceImageAva = new BitmapImage(new Uri("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTWyXl_ES0Jwg7dn_W559ya9pk8X_8B_e9IEQ&usqp=CAU"));
            });

            ChangeBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if (op.FileName != "")
                {
                    SourceImageBackground = new BitmapImage(new Uri(op.FileName));
                }
            });
            ChangeToDefaultBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                SourceImageBackground = new BitmapImage(new Uri("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRFU7U2h0umyF0P6E_yhTX45sGgPEQAbGaJ4g&usqp=CAU"));
            });
            SaveBackgroundShopCommand = new RelayCommandWithNoParameter(SaveBackground);

            SaveProfileCommand = new RelayCommand<object>(p => p != null, SaveProfile);
            SaveAvaShopCommand = new RelayCommandWithNoParameter(SaveAva);
            CancelProfileCommand = new RelayCommand<object>(p => p != null, CancelProfile);
        }
        #endregion

        #region Command Methods
        public async void Load()
        {
            ThisUser = await userRepo.GetSingleAsync(usr => usr.Id == "user01");
            EditUser = await userRepo.GetSingleAsync(usr => usr.Id == "user01");
            if(!string.IsNullOrEmpty(ThisUser.SourceImageAva))
                SourceImageAvaTemp = new BitmapImage(new Uri(ThisUser.SourceImageAva));
            if(!string.IsNullOrEmpty(ThisUser.SourceImageBackground))
                EditSourceImageBackground = new BitmapImage(new Uri(ThisUser.SourceImageBackground));

        }

        public async void SaveProfile(object obj)
        {
            var user = obj as MUser;
            if (user == null)
                return;
            ThisUser = user;
            await userRepo.Update(user);
            Load();
        }
        public async void SaveAva()
        {
            SourceImageAvaTemp = SourceImageAva;
            ThisUser.SourceImageAva = SourceImageAvaTemp.ToString();
            await userRepo.Update(ThisUser);
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public async void SaveBackground()
        {
            EditSourceImageBackground=SourceImageBackground;
            ThisUser.SourceImageBackground=SourceImageBackground.ToString();
            await userRepo.Update(ThisUser);
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public void CancelProfile(object obj)
        {
            var user = obj as MUser;
            if (user == null)
                return;

            Load();

        }
        #endregion
    }


}
