using DataAccessLayer;
using LiveCharts.Helpers;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
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

        #region Shitprop
        private string _name;
        private string _address;
        private bool _isCheckAgree;
        #endregion
        #region Shit Props2
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
                _name = null;
                if(string.IsNullOrEmpty(value)) {
                    return;
                }
                _name = value;
            }
        }
        public string Email { get; set; }

        private string password;
        public string Password { get => password;
            set {
                password = value;
                ConfirmPassword = null;
                ConfirmPassword = "";
            }
        }

        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        public string Address {
            get => _address;
            set {
                _address = null;
                if(string.IsNullOrEmpty(value)) {
                    return;
                }
                _address = value;
                OnPropertyChanged();
            }
        }
        public bool Gender { get; set; }

        #endregion

        public bool isCreated { get; set; }
        public string ID { get; set; }

        public ICommand Regist { get; set; }
        public ICommand LoginHandle { get; set; }
        public ICommand KeyHandle_PolicyEnter { get; }


        public RegisterViewModel(string email, string password, string id = null) {

            try { Email = email; } catch { }
            try { Password = password; } catch { }
            isCreated = id != null;
            if(isCreated) ID = id;

            Gender = false;

            Regist = new RelayCommand<object>((p) => {
                if(isCreated) {
                    return (!string.IsNullOrEmpty(Name) &&
                    PhoneValidateRule.Validate(Phone) &&
                    !string.IsNullOrEmpty(Address));
                }
                return (
                    EmailValidateRule.Validate(Email) &&
                    PasswordValidateRule.Validate(Password) &&
                    PasswordValidateRule.Validate(ConfirmPassword) &&
                    IsCheckAgree == true);
            }, (p) => {
                
                if(!isCreated) {
                    var flag = false;
                    Task task = Task.Run(async () => {
                        flag = await CreateLogin();
                    })
                    .ContinueWith((t) => {
                        ConfirmDialog dl = null;
                        if(!flag) {
                            dl = new ConfirmDialog() {
                                Header = "Whoops",
                                Content = "Email already exist, back to Login.",
                            };
                        }
                        else {
                            isCreated = true;
                            CommandManager.InvalidateRequerySuggested();
                            return;
                        }
                        DialogHost.Show(dl, "Regist");
                        return;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else {
                    var flag = false;
                    Task task = Task.Run(async () => {
                        flag = await CreateAccount();
                    })
                    .ContinueWith((t) => {
                        var dl = new ConfirmDialog() {
                            Header = "Success",
                            Content = "Back to Login.",
                            CM = new RelayCommand<object>(pr => true, pr => {
                                DialogHost.Close("Login");
                                LoginHandle.Execute(new Tuple<string, string>(Email, Password));
                            }),
                            Param = ""
                        };
                        if(!flag) {
                            dl.Header = "Whoops";
                            dl.Content = "Something is wrong, try again!";
                        }
                        DialogHost.Show(dl, "Regist");
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            });

            KeyHandle_PolicyEnter = new ImmediateCommand<object>(p => {
                IsCheckAgree = !IsCheckAgree;
            });
        }

        private async Task<bool> CreateLogin() {
            var temp = await loginRepo.GetListAsync(d => d.Username == Email);
            if(temp.Count > 0) return false;

            ID = await GenerateID.Gen(typeof(UserLogin), "IdUser");
            var hasher = new Hashing();
            var salt = Convert.ToBase64String(hasher.GenerateSalt());
            var encrypted = new Hashing().Encrypt(salt, Password);
            try {
                await loginRepo.Add(new UserLogin() {
                    IdUser = ID,
                    Password = encrypted,
                    Username = Email,
                    Salt = salt,
                    Provider = 0
                });
            } catch (Exception e){
                Debug.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        public async Task<bool> CreateAccount() {
            try {
                var user = new Models.MUser() {
                    Id = ID,
                    Name = Name,
                    Email = Email,
                    PhoneNumber = Phone,
                    Gender = Gender,
                    Description = "",
                    StatusShop = "NotExist",
                    StatusUser = "NotBanned",
                    Role = "User",
                };
                await userRepo.Add(user);

                var addressId = GenerateID.DateTimeID();
                Address address = new Address() {
                    Id = addressId,
                    IdUser = ID,
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
    }
}
