using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.JScript;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class GenerateID {
        static char reVal(int num) {
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

        static string encode(int inputNum) {
            inputNum += 262145;
            int index = 0;
            string res = "";
            while(inputNum > 0) {
                res += reVal(inputNum % 64);
                index++;
                inputNum /= 64;
            }
            return res;
        }
        static public async Task<string> Gen(Type type) {
            int res = 0;
            await Task.Run(() => {
                using(var context = new EcommerceAppEntities()) {
                    string t = type.Name;
                    var sql = $"SELECT COUNT(*) FROM dbo.{t}";
                    res = context.Database.SqlQuery<int>(sql).Single();
                }
            });
            string id = encode(res);
            char r1 = (char) (new Random().Next(0, 10) + '0');
            char r2 = (char) (new Random().Next(36, 62) - 36 + 'a');
            id += r1;
            id += r2;
            return id;
        }
    }
}
