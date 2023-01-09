using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class PromoDetailAPI
    {
        public static async Task<bool> Add(string idPromo, string idProduct)
        {
            bool res = true;
            await Task.Run(() => {
                using (var context = new EcommerceAppEntities())
                {
                    var sql = $"insert into dbo.PromoDetail values ('{idProduct}', '{idPromo}')";
                    try
                    {
                        context.Database.ExecuteSqlCommand(sql);
                    }
                    catch
                    {
                        res = false;
                    }
                }
            });
            return res;
        }

        public static async Task<bool> Delete(string idPromo, string idProduct)
        {
            bool res = true;
            await Task.Run(() => {
                using (var context = new EcommerceAppEntities())
                {
                    var sql = $"delete from dbo.PromoDetail where IdProduct = '{idProduct}' and IdPromo = '{idPromo}'";
                    try
                    {
                        context.Database.ExecuteSqlCommand(sql);
                    }
                    catch
                    {
                        res = false;
                    }
                }
            });
            return res;
        }
    }
}
