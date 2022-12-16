using DataAccessLayer;
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
        private AccountStore _accountStore;

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

        public LoginViewModel(AccountStore accountStore)
        {
            UserRepository = new GenericDataRepository<Models.UserLogin>();
            
            _accountStore = accountStore;
            //_accountStore.CurrentAccount = ;
            isLogin = new RelayCommand<object>((p) => { return true; },(p)=>{
                Login();
            });
        }
        private async Task Login()
        {
            var acc = new ObservableCollection<Models.UserLogin>(await UserRepository.GetListAsync(x => x.Username == username && x.Password == password)).Count;
            if(acc > 0)
            {
                MessageBox.Show("Login successfully");
            }

        }
    }
}
