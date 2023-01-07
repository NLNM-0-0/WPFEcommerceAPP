using DataAccessLayer;
using LiveCharts.Helpers;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    internal class RegisterViewModel : BaseViewModel {
        private readonly GenericDataRepository<Models.MUser> userRepo = new GenericDataRepository<Models.MUser>();
        private readonly GenericDataRepository<Models.UserLogin> loginRepo = new GenericDataRepository<Models.UserLogin>();
        private readonly GenericDataRepository<Address> addressRepo = new GenericDataRepository<Address>();


        private string _name;
        private string _email;
        private string _password;
        private string _phone;
        private string _address;
        private bool _isMen;
        private bool _isWomen;
        private bool _isCheckAgree;
        public bool Gt;

        #region Props
        public bool IsCheckAgree {
            get => _isCheckAgree;
            set {
                _isCheckAgree = value;
                OnPropertyChanged();
            }
        }
        public string Name {
            get => _name;
            set {
                if(string.IsNullOrEmpty(value)) {
                    throw new ArgumentException("*Can not empty");
                }
                _name = value;
                OnPropertyChanged();
            }
        }
        public string Email {
            get => _email;
            set {
                if(string.IsNullOrEmpty(value)) {
                    throw new ArgumentException("*Can not empty");
                }
                string rgStr = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex regex = new Regex(rgStr);
                if(!regex.IsMatch(value)) {
                    throw new ArgumentException("*Wrong type");
                }
                _email = value;
                OnPropertyChanged();
            }
        }
        public bool IsMen {
            get => _isMen; set {
                _isMen = value;
                if(value) {
                    Gt = false;
                }
            }
        }
        public bool IsWomen {
            get => _isWomen; set {
                _isWomen = value;
                if(value) {
                    Gt = true;
                }
            }
        }
        public string Password {
            get => _password; set {
                if(string.IsNullOrEmpty(value)) {
                    throw new ArgumentException("*Can not empty");
                }
                _password = value;
                OnPropertyChanged();
            }
        }
        public string Phone {
            get => _phone;
            set {
                if(string.IsNullOrEmpty(value)) {
                    throw new ArgumentException("*Can not empty");
                }
                if(value.Length <= 6 || value.Length >= 12) {
                    throw new ArgumentException("*Wrong type");
                }
                string rgStr = @"^([+]?[\s0-9]+)?(\d{3}|[(]?[0-9]+[)])?([-]?[\s]?[0-9])+$";
                Regex regex = new Regex(rgStr);
                if(!regex.IsMatch(value)) {
                    throw new ArgumentException("*Wrong type");
                }
                _phone = value;
                OnPropertyChanged();
            }
        }
        public string Address {
            get => _address;
            set {
                if(string.IsNullOrEmpty(value)) {
                    throw new ArgumentException("*Can not empty");
                }
                _address = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public ICommand Regist { get; set; }
        public async Task<bool> CreateAccount() {
            var temp = await loginRepo.GetListAsync(d => d.Username == Email);
            if(temp.Count > 0) return false;
            
            string idUser = await GenerateID.Gen(typeof(Models.MUser));

            var encrypted = new Hashing().Encrypt(Email, Password);
            try {
                var addressId = GenerateID.DateTimeID();

                var user = new Models.MUser() {
                    Id = idUser,
                    Name = Name,
                    Email = Email,
                    PhoneNumber = Phone,
                    Gender = Gt,
                    Address = Address,
                    Description = "",
                    StatusShop = "NotExist",
                    StatusUser = "NotBanned",
                    Role = "User",
                };
                await userRepo.Add(user);
                await loginRepo.Add(new Models.UserLogin() {
                    IdUser = idUser,
                    Password = encrypted,
                    Username = Email,
                    Provider = 0
                });
                Address address = new Address() {
                    Id = addressId,
                    IdUser = idUser,
                    Name = Name,
                    PhoneNumber = Phone,
                    Address1 = Address,
                    Status = true
                };
                await addressRepo.Add(address);
                user.DefaultAddress = addressId;
                await userRepo.Update(user);
            } catch(Exception e) {
                Debug.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        public RegisterViewModel(string em, string pw) {
            try { Email = em; } catch { }
            try { Password = pw; } catch { }

            Regist = new RelayCommand<object>((p) => {
                return (!string.IsNullOrEmpty(Name)&& !string.IsNullOrEmpty(Email)&& !string.IsNullOrEmpty(Phone)&& !string.IsNullOrEmpty(Address)&& !string.IsNullOrEmpty(Password) && (IsMen == true || IsWomen == true) && IsCheckAgree == true);
            }, (p) => {
                var flag = false;
                Task task = Task.Run(async () => { 
                    flag = await CreateAccount();
                })
                .ContinueWith((t) => {
                    var dl = new ConfirmDialog() {
                        Header = "Success",
                        Content = "Back to Login",
                        CM = new RelayCommand<object>(pr => true, pr => {
                            DialogHost.Close("Login");
                            em = Email;
                            pw = Password;
                        }),
                        Param = ""
                    };
                    if(!flag) {
                        dl.Header = "Whoops";
                        dl.Content = "Email already exist, back to Login.";
                    }
                    DialogHost.Show(dl, "Regist");
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });
        }
    }
}
