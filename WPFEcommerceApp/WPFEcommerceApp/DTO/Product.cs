using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public partial class Product {

        public Product(Models.Product p, string size, int amount) {
            if(p.ImageProducts.Count > 0)
                ProductImage = p.ImageProducts.First().Source;
            Name=p.Name;
            Size=size;
            Color = p.Color;
            Description=p.Description;
            Price= p.Price;
            Amount=amount;
            Subtotal = Price*amount;
            ID=p.Id;
            Banned = p.BanLevel>0 || p.InStock < 1;
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
        public bool Banned { get; set; }
    }
}
