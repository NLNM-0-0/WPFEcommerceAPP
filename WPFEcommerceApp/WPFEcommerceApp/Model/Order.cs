using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class Order {
        public Order(List<BProduct> orderList,
            string iD = "000",
            string status = "Processing",
            string iDCustomer = null,
            string iDrating = null,
            double shipTotal = 0) {
            ID=iD;
            Status=status;
            ShopProduct = orderList;
            IDCustomer=iDCustomer;
            IDrating=iDrating;
            ShipTotal=shipTotal;
            SubTotal = 0;
            OrderTotal = 0;
            Discount = 0;

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

            for(int i = 0; i<ShopProduct.Count; i++) {
                SubTotal += ShopProduct[i].Subtotal;
                Discount += ShopProduct[i].Subtotal * ShopProduct[i].Discount / 100;
            }
            OrderTotal = SubTotal + ShipTotal - Discount;
        }
        public Order(Order o) {
            ID = o.ID;
            Status = o.Status;
            IDCustomer = o.IDCustomer;
            IDrating = o.IDrating;
            ShipTotal = o.ShipTotal;
            SubTotal = o.SubTotal;
            Discount = o.Discount;
            OrderTotal = o.OrderTotal;
            ShopProduct = new List<BProduct>(o.ShopProduct);
            ProductPreview = new List<Product>(o.ProductPreview);
        }

        public string ID { get; set; }
        public string Status { get; set; }
        public string IDCustomer { get; set; }
        public string IDrating { get; set; }
        public List<BProduct> ShopProduct { get; set; }
        public List<Product> ProductPreview { get; set; }
        public double ShipTotal { get; set; }
        public double SubTotal { get; set; }
        public double Discount { get; set; }
        public double OrderTotal { get; set; }

    }
}
