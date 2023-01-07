using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class ForgotPasswordVM : BaseViewModel {
        private readonly GenericDataRepository<UserLogin> loginRepo = new GenericDataRepository<UserLogin>();
        public bool IsLoading { get; set; } = false;
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string ResetCode { get; set; }
        private string Code { get; set; }
        public int FunctionNumber { get; set; }
        public string FunctionName { get; set; }
        public ICommand InvokeFunction { get; set; }
        public ForgotPasswordVM() {
            FunctionName = "SEND CODE";
            FunctionNumber = 1;
            InvokeFunction = new RelayCommand<object>(p => true, async p => {
                if(FunctionNumber == 1) {
                    var x = await loginRepo.GetSingleAsync(d => d.Username == Email);
                    if(x == null || x.Provider == 1) {
                        var dl = new ConfirmDialog() {
                            Header = "Oops",
                            Content = x == null
                            ? "Email is not registed!"
                            : "Email is registed with Google",
                        };
                        await DialogHost.Show(dl, "ForgotPassword");
                        return;
                    }

                    var t = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    string body = File.ReadAllText($"{t}\\Screens\\Auth\\ForgotPassword\\email.html");
                    Code = GenerateID.RandomID();
                    body = body.Replace("[SECRETCODE]", Code);
                    IsLoading = true;
                    await Task.Run(async () => await OAuth.SendEmail(Email, "[WANO] - Reset password Code", body))
                        .ContinueWith(_ => {
                            FunctionName = "ENTER CODE";
                            FunctionNumber++;
                            IsLoading = false;
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else if(FunctionNumber == 2) {
                    if(ResetCode == Code) {
                        FunctionName = "RESET PASSWORD";
                        FunctionNumber++;
                    }
                }
                else {
                    IsLoading = true;
                    var pw = new Hashing().Encrypt(Email, Password);
                    var x = await loginRepo.GetSingleAsync(d => d.Username == Email);
                    x.Password = pw;
                    await loginRepo.Update(x);
                    IsLoading = false;

                    var dl = new ConfirmDialog() {
                        Header = "Success",
                        Content = "Back to Login",
                        CM = new RelayCommand<object>(pr => true, pr => {
                            DialogHost.Close("Login");
                        }),
                        Param = ""
                    };
                    await DialogHost.Show(dl, "ForgotPassword");
                    return;
                }
            });
        }
    }
}
