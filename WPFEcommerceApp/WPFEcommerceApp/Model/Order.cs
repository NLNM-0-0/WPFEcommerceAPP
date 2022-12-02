using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class Order {
        public Order(string iD = "000", string status = "Processing") {
            ID=iD;
            Status=status;
            ShopProduct = new List<BProduct>() {
                new BProduct(),
                new BProduct(),
            };
            ProductPreview = new List<Product>();
            for(int i = 0; i<ShopProduct.Count; i++) {
                bool flag = false;
                for(int j = 0; j < ShopProduct[i].ProductList.Count; j++) {
                    ProductPreview.Add(ShopProduct[i].ProductList[j]);
                    if(ProductPreview.Count == 2) {
                        flag = true;
                        break;
                    }
                }
                if(flag) break;
            }
        }

        public string ID { get; set; }
        public string Status { get; set; }
        public List<BProduct> ShopProduct { get; set; }
        public List<Product> ProductPreview { get; set; }
        public double OrderTotal { get; set; }

    }
}
