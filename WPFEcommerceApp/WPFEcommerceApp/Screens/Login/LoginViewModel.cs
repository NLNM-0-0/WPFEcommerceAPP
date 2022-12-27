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

namespace WPFEcommerceApp
{
    internal class LoginViewModel:BaseViewModel
    {
        public GenericDataRepository<Models.UserLogin> UserRepository { get; set; }

        private ObservableCollection<Models.UserLogin> accounts;
        public ObservableCollection<Models.UserLogin> Accounts
        {
            get { return accounts; }
            set
            {
                accounts = value;
                OnPropertyChanged();
            }
        }

        public ICommand isLogin { get; set; }
        private string username;
        private string password;
        public string Username
        {
            get => username; set { username = value; OnPropertyChanged(); } 
        }
        public string Password
        {
            get => password; set { password = value; OnPropertyChanged(); }
        }
        public LoginViewModel(object o)
        {
            UserRepository = new GenericDataRepository<Models.UserLogin>();
            
            //_accountStore.CurrentAccount = ;
            isLogin = new RelayCommand<object>(p => true, async p => {
                if(await Login()) {
                    (p as Window).Hide();
                    (o as Window).Show();
                    (p as Window).Close();
                }
            });
        }
        private async Task<bool> Login()
        {
            Models.UserLogin acc = await UserRepository.GetSingleAsync(
                x => (x.Username == username 
                && x.Password == password),
                x => x.MUser);
            if(acc != null && acc.MUser.StatusUser != "Banned")
            {
                AccountStore.instance.CurrentAccount = acc.MUser;
                return true;
            }

            var dl = new ConfirmDialog() {
                Header = "Oops",
                Content = acc.MUser.StatusUser != "Banned" 
                ? "Email or password is wrong. Try again!"
                : "Your account has been banned!",
            };
            await DialogHost.Show(dl, "Login");
            return false;
        }
    }
}
