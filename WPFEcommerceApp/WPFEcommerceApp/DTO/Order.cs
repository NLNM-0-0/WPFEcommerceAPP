﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DataAccessLayer;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    [DebuggerDisplay("{"+nameof(GetDebuggerDisplay)+"(),nq}")]
    public class Order {
        public Order(
            string iD,
            string iDCustomer,
            string iDShop,
            string status,
            double shipTotal,
            List<Product> productList,
            DateTime dateBegin,
            string shopName,
            string shopImage,
            Nullable<int> shippingSpeedMethod = null) {

            ID=iD;
            IDCustomer=iDCustomer;
            IDShop=iDShop;
            Status=status;
            ShipTotal=shipTotal;
            ProductList=productList;
            DateBegin=dateBegin;
            ShopName=shopName;
            ShopImage=shopImage;
            ShippingSpeedMethod=shippingSpeedMethod;

            SubTotal=0;
            Discount=0;
            OrderTotal=0;

            for(int i = 0; i< ProductList.Count; i++) {
                SubTotal += ProductList[i].Subtotal;
                Discount += ProductList[i].Subtotal * ProductList[i].Discount / 100;
            }

            OrderTotal = SubTotal + ShipTotal - Discount;
        }

        public Order(Order o) {
            ID = o.ID;
            Status = o.Status;
            IDCustomer = o.IDCustomer;
            IDShop = o.IDShop;
            ShopName = o.ShopName;
            ShopImage = o.ShopImage;
            ShipTotal = o.ShipTotal;
            ProductList = o.ProductList;
            DateBegin = o.DateBegin;
            DateEnd = o.DateEnd;
            SubTotal = o.SubTotal;
            Discount = o.Discount;
            OrderTotal = o.OrderTotal;
            ShippingSpeedMethod = o.ShippingSpeedMethod;
        }

        public string ID { get; set; }
        public string IDCustomer { get; set; }
        public string IDShop { get; set; }
        public string ShopName { get; set; }
        public string ShopImage { get; set; }
        public string Status { get; set; }
        public double ShipTotal { get; set; }
        public List<Product> ProductList { get; set; }
        public Nullable<DateTime> DateBegin { get; set; }
        public Nullable<DateTime> DateEnd { get; set; }
        public double SubTotal { get; set; }
        public double Discount { get; set; }
        public double OrderTotal { get; set; }
        public Nullable<int> ShippingSpeedMethod { get; set; }

        private string GetDebuggerDisplay() {
            return ToString();
        }
    }
}
