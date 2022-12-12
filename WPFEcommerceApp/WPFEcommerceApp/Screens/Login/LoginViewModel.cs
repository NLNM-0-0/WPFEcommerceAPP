using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    internal class LoginViewModel:BaseViewModel
    {
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
            isLogin = new RelayCommand<object>((p) => { return true; },(p)=>{
                
            });
        }

    }
}
