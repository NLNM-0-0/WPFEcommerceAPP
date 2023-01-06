using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class LoginViewModel : BaseViewModel {
        private readonly GenericDataRepository<Models.MUser> userRepo = new GenericDataRepository<MUser>();
        private readonly GenericDataRepository<Models.UserLogin> loginRepo = new GenericDataRepository<UserLogin>();


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
        public ICommand OnGoogleSignIn { get; set; }
        public LoginViewModel(object o) {

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
            OnGoogleSignIn = new ImmediateCommand<object>(async p => {
                var auth = new OAuth();
                (p as Window).Activate();
                var x = await auth.Authentication();
                Debug.WriteLine(x);
                if(x.Item1 == null || x.Item2 == null) {
                    ErrorMessage();
                    return;
                }
                try {
                    var ins1 = (x.Item2 as Tuple<object, object>).Item1 as Dictionary<string, string>;
                    var ins2 = (x.Item2 as Tuple<object, object>).Item2 as Dictionary<string, object>;
                    string birthdays = null, coverPhotos = null, phoneNumbers = null, genders = null;
                    try {
                        birthdays = ins2["birthdays"]?.ToString();
                        coverPhotos = ins2["coverPhotos"]?.ToString();
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
                        ins1["coverPhoto"] =  (cp as Dictionary<string, object>)["url"].ToString();
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
                    var user = await userRepo.GetSingleAsync(d => d.Id == ins1["sub"], d => d.Products1);
                    AccountStore.instance.CurrentAccount = user;
                    (p as Window).Hide();
                    (o as Window).WindowState = WindowState.Minimized;
                    (o as Window).Show();
                    (o as Window).WindowState = WindowState.Normal;
                    (p as Window).Close();

                } catch { ErrorMessage(); }
            });
        }

        public async Task<bool> CreateAccount(Dictionary<string, string> user) {
            try {
                string phoneNumber = "", gender = null, cover = null, birthday = null;
                user.TryGetValue("phoneNumber",out phoneNumber);
                user.TryGetValue("gender", out gender);
                user.TryGetValue("coverPhoto", out cover);
                user.TryGetValue("birthday", out birthday);
                if(phoneNumber == null) {
                    phoneNumber = "";
                }
                bool flag = true;
                DateTime t = DateTime.Now;
                try {
                    Convert.ToDateTime(birthday);
                } catch { flag = false; }
                var us = new Models.MUser() {
                    Id = user["sub"],
                    Name = user["name"],
                    Email = user["email"],
                    PhoneNumber = phoneNumber,
                    Gender = gender == "Nam" ? true : false,
                    SourceImageAva = user["picture"],
                    SourceImageBackground = cover,
                    Address = "",
                    Description = "",
                    StatusShop = "NotExist",
                    StatusUser = "NotBanned",
                    Role = "User"
                };
                if(flag) us.DOB = Convert.ToDateTime(birthday);

                await userRepo.Add(us);
                await loginRepo.Add(new Models.UserLogin() {
                    IdUser = user["sub"],
                    Password = null,
                    Username = user["email"]
                });
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
                Content = "Something is wrong, please try again!"
            };
            DialogHost.Show(dlg, "Login");
        }
        private async Task<bool> Login() {
            var encode = new Hashing().Encrypt(Username, Password);

            UserLogin acc = await loginRepo.GetSingleAsync(
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

