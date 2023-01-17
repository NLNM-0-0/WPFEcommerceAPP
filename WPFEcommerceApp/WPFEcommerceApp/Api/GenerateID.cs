using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.JScript;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class GenerateID {
        static char reVal(long num) {
            if(num > -1 && num < 5)
                return (char)(num +'5');
            else if(num > 4 && num < 10)
                return (char)(num - 5 + '0');
            else if(num > 9 && num < 36)
                return (char)(num - 10 + 'A');
            else if(num > 35 && num < 62)
                return (char)(num - 36 + 'a');
            else if(num == 62) return '=';
            else if(num == 63) return '-';
            return '?';
        }

        static string encode(long inputNum) {
            inputNum += 261816;
            inputNum = (long)(inputNum * 1.62);
            long index = 0;
            string res = "";
            while(inputNum > 0) {
                res += reVal(inputNum % 64);
                index++;
                inputNum /= 64;
            }
            return res;
        }
        static public async Task<string> Gen(Type type, string checkProperty = "Id") {
            long res = 0;
            await Task.Run(() => {
                using(var context = new EcommerceAppEntities()) {
                    string t = type.Name;
                    var sql = $"SELECT COUNT(*) FROM dbo.{t}";
                    res = context.Database.SqlQuery<int>(sql).Single();
                }
            });

            #region Fake ID
            //string id = type.Name.Substring(0, 4);
            //id = id + (res + 50).ToString();
            //50 thay bằng các số bắt đầu của mỗi người
            #endregion

            #region Real ID
            bool check = false;
            string id = "";
            while(!check) {
                id = encode(res);
                if(id.Length < 6)
                    for(int i = 0; i <= 6 - id.Length; i++)
                        id = "#" + id;
                char r1 = (char)(new Random().Next(0, 10) + '0');
                char r2 = (char)(new Random().Next(36, 62) - 36 + 'a');
                id = r1 + id;
                id += r2;

                await Task.Run(() => {
                    using(var context = new EcommerceAppEntities()) {
                        string t = type.Name;
                        var sql = $"SELECT COUNT(1) FROM dbo.{t} WHERE {checkProperty} = '{id}'";
                        check = context.Database.SqlQuery<int>(sql).Single() == 0 ? true : false;
                    }
                });
                if(!check) res = (long)(res * 1.5);
            }
            #endregion

            return id;
        }
        static public string RandomID(int length = 6) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static public string DateTimeID() {
            var time = DateTime.Now;
            string res = time.Year.ToString("D2") + time.Month.ToString("D2")
                + time.Day.ToString("D2") + time.Hour.ToString("D2")
                + time.Minute.ToString("D2") + time.Second.ToString("D2");
            return res;
        }
    }
}
