using DataAccessLayer;
using LiveCharts.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private bool _isCheckAgree;
        public bool Gt;

        public bool IsCheckAgree
        {
            get => _isCheckAgree;
            set
            {
                _isCheckAgree = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("*Can not empty");
                }
                _name = value;
                OnPropertyChanged();
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("*Can not empty");
                }
                string rgStr = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex regex = new Regex(rgStr);
                if (!regex.IsMatch(value))
                {
                    throw new ArgumentException("*Wrong type");
                }
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
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("*Can not empty");
                }
                _password = value;
                OnPropertyChanged();
            }
        }
        public string Phone
        {
            get => _phone;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("*Can not empty");
                }
                if(value.Length <= 6 || value.Length >= 12)
                {
                    throw new ArgumentException("*Wrong type");
                }
                string rgStr = @"^([+]?[\s0-9]+)?(\d{3}|[(]?[0-9]+[)])?([-]?[\s]?[0-9])+$";
                Regex regex = new Regex(rgStr);
                if (!regex.IsMatch(value))
                {
                    throw new ArgumentException("*Wrong type");
                }
                _phone = value;
                OnPropertyChanged();
            }
        }
        public string Address
        {
            get=> _address;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("*Can not empty");
                }
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
            Regist = new RelayCommand<object>((p) =>
            {
                return (!string.IsNullOrEmpty(Name)&& !string.IsNullOrEmpty(Email)&& !string.IsNullOrEmpty(Phone)&& !string.IsNullOrEmpty(Address)&& !string.IsNullOrEmpty(Password) && (IsMen == true || IsWomen == true) && IsCheckAgree == true);
            },(p) =>
            {
                Task task = Task.Run(async () => await Load());
                while (!task.IsCompleted) ;
                MessageBox.Show("Register successfully");
            });
        }
    }
}
