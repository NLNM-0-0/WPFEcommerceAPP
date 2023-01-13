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
            string iD = null,
            double discount = 0) {
            ProductImage=productImage;
            Name=name;
            Size=size;
            Color=color;
            Description=description;
            Price=(long)price;
            Amount=amount;
            Subtotal=amount * price;
            ID=iD;
            Discount=discount;
        }

        public Product(Models.Product p, string size, int amount) {
            if(p.ImageProducts.Count > 0)
                ProductImage = p.ImageProducts.First().Source;
            Name=p.Name;
            Size=size;
            Color = p.Color;
            Description=p.Description;
            Price=(long)p.Price;
            Amount=amount;
            Subtotal = Price*amount;
            ID=p.Id;
            Discount=p.Sale * Subtotal / 100;
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
        public double Discount { get; set; }

    }
}
