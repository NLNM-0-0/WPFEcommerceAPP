using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    internal class LoginViewModel : BaseViewModel {
        public GenericDataRepository<Models.UserLogin> UserRepository { get; set; }

        private ObservableCollection<Models.UserLogin> accounts;
        public ObservableCollection<Models.UserLogin> Accounts {
            get { return accounts; }
            set {
                accounts = value;
                OnPropertyChanged();
            }
        }

        private string username;
        private string password;
        public string Username {
            get => username; set { username = value; OnPropertyChanged(); }
        }
        public string Password {
            get => password; set { password = value; OnPropertyChanged(); }
        }

        public ICommand OnLogin { get; set; }
        public ICommand CloseCM { get; set; }
        public ICommand OnSignUp { get; set; }
        public LoginViewModel(object o) {
            UserRepository = new GenericDataRepository<Models.UserLogin>();

            //_accountStore.CurrentAccount = ;
            OnLogin = new RelayCommand<object>(p => true, async p => {
                if(await Login()) {
                    (p as Window).Hide();
                    (o as Window).Show();
                    (p as Window).Close();
                }
            });

            CloseCM = new RelayCommand<object>(p => true, p => {
                (p as Window).Hide();
                (o as Window).Show();
                (p as Window).Close();
            });

            OnSignUp = new ImmediateCommand<object>(p => {
                Register register = new Register() {
                    DataContext = new RegisterViewModel(Username, Password)
                };
                DialogHost.Show(register, "Login");
            });
        }
        private async Task<bool> Login() {
            var encode = new Hashing().Encrypt(Username, Password);

            UserLogin acc = await UserRepository.GetSingleAsync(
                x => (x.Username == username
                && x.Password == encode),
                x => x.MUser);
            if(acc != null && acc.MUser.StatusUser != "Banned") {
                var userRepo = new GenericDataRepository<MUser>();
                var user = await userRepo.GetSingleAsync(d => d.Id == acc.MUser.Id, d => d.Products1);
                AccountStore.instance.CurrentAccount = user;
                return true;
            }

            var dl = new ConfirmDialog() {
                Header = "Whoops",
                Content = acc?.MUser.StatusUser != "Banned"
                ? "Email or password is wrong. Try again!"
                : "Your account has been banned!",
            };
            await DialogHost.Show(dl, "Login");
            return false;
        }
    }
}
