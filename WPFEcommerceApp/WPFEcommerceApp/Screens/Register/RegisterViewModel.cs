using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    internal class RegisterViewModel:BaseViewModel
    {
        public GenericDataRepository<Models.MUser> GenericDataRepository { get; set; }
        public GenericDataRepository<Models.UserLogin> LoginRepo { get; set; }
        private string _name;
        private string _email;
        private string _password;
        private string _phone;
        private string _address;
        private bool _isMen;
        private bool _isWomen;
        public bool Gt;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }
        public bool IsMen
        {
            get => _isMen; set
            {
                _isMen = value;
                if(value)
                {
                    Gt = true;
                }
            }
        }
        public bool IsWomen
        {
            get => _isWomen; set
            {
                _isWomen = value;
                if (value)
                {
                    Gt = false;
                }
            }
        }
        public string Password
        {
            get => _password; set
            {
                _password = value;
                OnPropertyChanged();
            }
        }
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged();
            }
        }
        public string Address
        {
            get=> _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }
        public ICommand Regist { get; set; }
        public async Task Load()
        {
            string idUser = await GenerateID.Gen(typeof(Models.MUser));
            GenericDataRepository<Models.MUser> dataRepository = new GenericDataRepository<Models.MUser>();
            GenericDataRepository<Models.UserLogin> loginRepository = new GenericDataRepository<Models.UserLogin>();
            try
            {
                
                
               
                await dataRepository.Add(new Models.MUser()
                {
                    Id = idUser,
                    Name = Name,
                    Email = Email,
                    PhoneNumber = Phone,
                    Gender = Gt,
                    Address = Address,
                    Description = "",
                    StatusShop = "NotExist",
                    StatusUser = "NotBanned",
                    Role = "User"
                });
                await loginRepository.Add(new Models.UserLogin()
                {
                    IdUser = idUser,
                    Password = Password,
                    Username = Email
                });
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public RegisterViewModel()
        {
            Regist = new RelayCommandWithNoParameter(async () =>
            {
                Task task = Task.Run(async () => await Load());
                while (!task.IsCompleted) ;
                MessageBox.Show("Register successfully");
            });
        }
    }
}
