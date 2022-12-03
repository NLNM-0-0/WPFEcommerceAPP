using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class BProduct {
        public BProduct(
            string shopImage = "..\\..\\..\\..\\Assests\\Images\\2.jpg", 
            string shopName = "Shop name temp") {
            ShopImage=shopImage;
            ShopName=shopName;
            ProductList=new List<Product>() {
                new Product(),
                new Product(),
                new Product(),
            };
            Subtotal = 0;
            for(int i = 0; i<ProductList.Count; i++) {
                Subtotal += ProductList[i].Subtotal;
            }
        }

        public string ShopImage { get; set; }
        public string ShopName { get; set; }
        public List<Product> ProductList { get; set; }
        public double Subtotal { get; set; }

    }
}
