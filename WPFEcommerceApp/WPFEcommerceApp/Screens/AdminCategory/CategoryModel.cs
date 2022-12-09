using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp
{
    public class CategoryModel
    {
        public static int countID = 0;
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }

        public CategoryModel(string categoryName)
        {
            countID = countID + 1;
            CategoryID = countID.ToString();
            CategoryName = categoryName;
        }
    }
}
