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
        public LoginViewModel()
        {
            UserRepository = new GenericDataRepository<Models.UserLogin>();
            
            //_accountStore.CurrentAccount = ;
            isLogin = new RelayCommandWithNoParameter(() => {
                Login();
            });
        }
        private async Task Login()
        {
            Models.UserLogin acc = await UserRepository.GetSingleAsync(x => (x.Username == username && x.Password == password),
                                                                       x => x.MUser);
            if(acc != null )
            {
                AccountStore.instance.CurrentAccount = acc.MUser;
                MessageBox.Show("Login successfully");
            }
            else
            {
                MessageBox.Show("Error");
            }

        }
    }
}
