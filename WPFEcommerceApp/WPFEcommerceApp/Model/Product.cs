using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public partial class Product {
        public Product(
            string productImage = "..\\..\\..\\..\\Assests\\Images\\2.jpg",
            string name = "Nike Air Zoom Pegasus 39 Men’s Road Running Shoes",
            string size = "12",
            string color = "Dark smoke",
            string description = "Nothing",
            double price = 21.85,
            int amount = 10,
            string iD = null) {
            ProductImage=productImage;
            Name=name;
            Size=size;
            Color=color;
            Description=description;
            Price=(long)price;
            Amount=amount;
            Subtotal=amount * price;
            ID=iD;
        }

        public string ProductImage { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public double Subtotal { get; set; }
        public string ID { get; set; }


    }
}
