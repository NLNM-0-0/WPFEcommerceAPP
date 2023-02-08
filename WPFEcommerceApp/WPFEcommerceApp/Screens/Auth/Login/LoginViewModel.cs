using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class LoginViewModel : BaseViewModel {
        private readonly GenericDataRepository<Models.MUser> userRepo = new GenericDataRepository<MUser>();
        private readonly GenericDataRepository<Models.UserLogin> loginRepo = new GenericDataRepository<UserLogin>();
        private readonly GenericDataRepository<Address> addressRepo = new GenericDataRepository<Address>();

        private string email;
        public string Email {
            get => email;
            set {
                email = value;
            } 
        }
        public string Password { get; set; }
        public bool KeepSignIn { get; set; }

        private bool adminAccess;
        public bool AdminAccess {
            get => adminAccess;
            set {
                adminAccess = value;
                Email = "";
                Password = "";
            }
        }
        public static bool IsLoading { get; set; } = false;
        public ICommand OnLogin { get; set; }
        public ICommand CloseCM { get; set; }
        public ICommand OnSignUp { get; set; }
        public ICommand OnGoogleSignIn { get; set; }
        public ICommand OnForgotPassword { get; set; }
        #region Keyhandle
        public ICommand KeyHandle_EnterKeepSignIn { get; }
        public ICommand KeyHandle_AdminAccess { get; }
        public ICommand KeyHandle_EnterLogin { get; }
        #endregion
        public LoginViewModel() {
            //_accountStore.CurrentAccount = ;
            OnLogin = new RelayCommand<object>(p => {
                return EmailValidateRule.Validate(Email, AdminAccess) &&
                        PasswordValidateRule.Validate(Password);
            }, async p => {
                if(await Login()) {
                    AdminAccess = false;
                    NavigateProvider.LoginScreenHandle(false);
                }
            });

            CloseCM = new RelayCommand<object>(p => true, p => {
                (p as Window).Close();
            });

            OnSignUp = new ImmediateCommand<object>(p => {
                string id = null;
                if(!string.IsNullOrEmpty(p as string)) id = p as string;
                Register register = new Register() {
                    DataContext = new RegisterViewModel(Email, Password, id) {
                        LoginHandle = new ImmediateCommand<object>(t => {
                            var temp = t as Tuple<string, string>;
                            Email = temp.Item1;
                            Password = temp.Item2;
                            OnPropertyChanged(nameof(Password));
                            OnPropertyChanged(nameof(Email));
                        }),
                        Gender = false
                    }
                };
                DialogHost.Show(register, "Login");
            });
            #region Google SignIn
            OnGoogleSignIn = new ImmediateCommand<object>(async p => {
                var auth = new OAuth();
                (p as Window).Activate();
                Tuple<string, object> x = new Tuple<string, object>("timeout", null);

                var cts = new CancellationTokenSource();
                cts.CancelAfter(9229);

                try {
                    x = await auth.Authentication().AsCancellable(cts.Token);
                } catch { }
                if(AccountStore.instance.CurrentAccount != null) return;

                (p as Window).WindowState = WindowState.Minimized;
                (p as Window).WindowState = WindowState.Normal;
                IsLoading = false;

                if(x == null) {
                    return;
                }
                if(x.Item1 == "timeout") {
                    var dl = new ConfirmDialog() {
                        Header = "Oops",
                        Content = "The process took too long, please try again!"
                    };
                    await DialogHost.Show(dl, "Login");
                    return;
                }
                if(x.Item1 == null || x.Item2 == null) {
                    ErrorMessage();
                    return;
                }
                try {
                    var ins1 = (x.Item2 as Tuple<object, object>).Item1 as Dictionary<string, string>;
                    var ins2 = (x.Item2 as Tuple<object, object>).Item2 as Dictionary<string, object>;
                    string birthdays = null, coverPhotos = null, phoneNumbers = null, genders = null;
                    try {
                        coverPhotos = ins2["coverPhotos"]?.ToString();
                        birthdays = ins2["birthdays"]?.ToString();
                        phoneNumbers = ins2["phoneNumbers"]?.ToString();
                        genders = ins2["genders"]?.ToString();
                    } catch { }
                    if(birthdays != null) {
                        Stdize(ref birthdays);
                        var bd = JsonConvert.DeserializeObject<Dictionary<string, object>>(birthdays);
                        var bd2 = JsonConvert.DeserializeObject<Dictionary<string, string>>((bd as Dictionary<string, object>)["date"].ToString());
                        ins1["birthday"] = bd2["month"] + '/' + bd2["day"] + '/' + bd2["year"];
                    }
                    if(coverPhotos != null) {
                        Stdize(ref coverPhotos);
                        var cp = JsonConvert.DeserializeObject<Dictionary<string, object>>(coverPhotos);
                        ins1["coverPhoto"] =  cp["url"].ToString();
                    }
                    if(phoneNumbers != null) {
                        //handle here
                    }
                    if(genders != null) {
                        //handle here
                    }
                    var check = await loginRepo.GetSingleAsync(d => d.IdUser == ins1["sub"]);

                    if(check == null) {
                        await CreateAccount(ins1);
                    }

                    var user = await userRepo.GetSingleAsync(d => d.Id == ins1["sub"], d => d.Products1, d => d.UserLogin);
                    IsLoading = false;
                    AccountStore.instance.CurrentAccount = user;
                    if(KeepSignIn) {
                        WPFEcommerceApp.Properties.Settings.Default.Cookie = Hashing.Base64Encode(user.Id);
                        WPFEcommerceApp.Properties.Settings.Default.Save();
                    }
                    else {
                        WPFEcommerceApp.Properties.Settings.Default.Cookie = "";
                        WPFEcommerceApp.Properties.Settings.Default.Save();
                    }
                    (p as Window).Hide();
                    App.Current.MainWindow.Show();
                } catch { ErrorMessage(); }
            });
            #endregion
            OnForgotPassword = new ImmediateCommand<object>(p => {
                var view = new ForgotPassword() {
                    DataContext = new ForgotPasswordVM()
                };
                DialogHost.Show(view, "Login");
            });
            KeyHandle_EnterKeepSignIn = new ImmediateCommand<object>(p => {
                KeepSignIn = !KeepSignIn;
            });
            KeyHandle_AdminAccess = new ImmediateCommand<object>(p => {
                AdminAccess = !AdminAccess;
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(Password));
            });
            KeyHandle_EnterLogin = new RelayCommand<object>(p => {
                return (bool)p;
            }, p => {
                OnLogin.Execute(p);
            });
        }

        #region Google helper
        public async Task<bool> CreateAccount(Dictionary<string, string> user) {
            try {
                string phoneNumber = null, gender = null, cover = null, birthday = null;
                user.TryGetValue("phoneNumber", out phoneNumber);
                user.TryGetValue("gender", out gender);
                user.TryGetValue("coverPhoto", out cover);
                user.TryGetValue("birthday", out birthday);

                if(phoneNumber == null) phoneNumber = "";
                if(gender == null) gender = "";
                if(cover == null) cover = "";
                if(birthday == null) birthday = "";

                bool flag = true;
                DateTime t = DateTime.Now;
                try {
                    Convert.ToDateTime(birthday);
                } catch { flag = false; }

                await loginRepo.Add(new Models.UserLogin() {
                    IdUser = user["sub"],
                    Password = null,
                    Username = user["email"],
                    Provider = 1
                });

                var us = new Models.MUser() {
                    Id = user["sub"],
                    Name = user["name"],
                    Email = user["email"],
                    PhoneNumber = phoneNumber,
                    Gender = gender == "Male" ? false : true,
                    SourceImageAva = user["picture"],
                    SourceImageBackground = cover,
                    Description = "",
                    StatusShop = "NotExist",
                    StatusUser = "NotBanned",
                    Role = "User"
                };
                if(flag) us.DOB = Convert.ToDateTime(birthday);
                await userRepo.Add(us);

            } catch(Exception e) {
                Debug.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        void Stdize(ref string t) {
            if(t[0] == '[') {
                t = t.Remove(0, 1);
                t = t.Remove(t.Length - 1, 1);
            }
        }
        void ErrorMessage() {
            var dlg = new ConfirmDialog() {
                Header = "Whoops",
                Content = "Unable to authenticate your account, please try again!"
            };
            DialogHost.Show(dlg, "Login");
        }
        #endregion
        private async Task<bool> Login() {

            UserLogin acc = await loginRepo.GetSingleAsync(
                x => x.Username == Email,
                x => x.MUser);

            if(acc != null && acc.Provider != 1) {
                var encode = new Hashing().Encrypt(acc.Salt, Password);
                if(acc.Password != encode) acc = null;
            }

            if(acc != null && acc.MUser == null) {
                OnSignUp.Execute(acc.IdUser);
                return false;
            }

            if(acc != null && acc.MUser.StatusUser != "Banned" && acc.Provider != 1) {
                var userRepo = new GenericDataRepository<MUser>();
                var user = await userRepo.GetSingleAsync(d => d.Id == acc.MUser.Id, d => d.Products1, d => d.UserLogin);
                AccountStore.instance.CurrentAccount = user;

                if(KeepSignIn) {
                    WPFEcommerceApp.Properties.Settings.Default.Cookie = Hashing.Base64Encode(user.Id);
                    WPFEcommerceApp.Properties.Settings.Default.Save();
                }
                else {
                    WPFEcommerceApp.Properties.Settings.Default.Cookie = "";
                    WPFEcommerceApp.Properties.Settings.Default.Save();
                }

                if(acc.Provider == -1) {
                    var changePass = new ForgotPassword(){
                        DataContext = new ForgotPasswordVM() {
                            Email = Email,
                            FunctionName = "RESET PASSWORD",
                            FunctionNumber = 3
                        }
                    };
                    await DialogHost.Show(changePass, "Login");
                    //return false;
                }

                Email = "";
                Password = "";
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
        static public void OnClosing(object sender, CancelEventArgs e) {
            if(DialogHost.IsDialogOpen("Login"))
                DialogHost.Close("Login");

            e.Cancel = true;
            if(App.Current.MainWindow != null)
                App.Current.MainWindow.Show();
            else return;
            (sender as Window).Hide();
        }
    }
}

