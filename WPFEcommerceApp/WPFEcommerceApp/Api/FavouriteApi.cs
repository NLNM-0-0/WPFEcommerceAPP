using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class FavouriteApi {
        public static async Task<bool> Add(string user, string product) {
            bool res = true;
            await Task.Run(() => {
                using(var context = new EcommerceAppEntities()) {
                    var sql = $"insert into dbo.FavouriteProduct values ('{user}', '{product}')";
                    try {
                        context.Database.ExecuteSqlCommand(sql);
                    }
                    catch {
                        res = false;
                    }
                }
            });
            return res;
        }

        public static async Task<bool> Delete(string user, string product) {
            bool res = true;
            await Task.Run(() => {
                using(var context = new EcommerceAppEntities()) {
                    var sql = $"delete from dbo.FavouriteProduct where IdUser = '{user}' and IdProduct = '{product}'";
                    try {
                        context.Database.ExecuteSqlCommand(sql);
                    }
                    catch {
                        res = false;
                    }
                }
            });
            return res;
        }
    }
}
