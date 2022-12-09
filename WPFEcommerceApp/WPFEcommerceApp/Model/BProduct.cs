using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class BProduct {
        public BProduct(
            List<Product> listProduct,
            string shopImage = "..\\..\\..\\..\\Assests\\Images\\2.jpg",
            string shopName = "Shop name temp",
            string iDShop = null,
            double discount = 0) {
            ShopImage=shopImage;
            ShopName=shopName;
            ProductList= listProduct;
            Subtotal = 0;
            for(int i = 0; i<ProductList.Count; i++) {
                Subtotal += ProductList[i].Subtotal;
            }
            IDShop=iDShop;
            Discount=discount;
        }

        public string IDShop { get; set; }
        public string ShopImage { get; set; }
        public string ShopName { get; set; }
        public List<Product> ProductList { get; set; }
        public double Discount { get; set; }
        public double Subtotal { get; set; }

    }
}
