//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WPFEcommerceApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.Carts = new HashSet<Cart>();
            this.ImageProducts = new HashSet<ImageProduct>();
            this.OrderInfoes = new HashSet<OrderInfo>();
            this.MUsers = new HashSet<MUser>();
            this.Promoes = new HashSet<Promo>();
        }
    
        public string Id { get; set; }
        public string Name { get; set; }
        public string IdCategory { get; set; }
        public string IdBrand { get; set; }
        public string IdShop { get; set; }
        public double Price { get; set; }
        public int Sale { get; set; }
        public int InStock { get; set; }
        public int Sold { get; set; }
        public bool IsOneSize { get; set; }
        public bool IsHadSizeS { get; set; }
        public bool IsHadSizeM { get; set; }
        public bool IsHadSizeL { get; set; }
        public bool IsHadSizeXL { get; set; }
        public bool IsHadSizeXXL { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> DateOfSale { get; set; }
        public Nullable<int> BanLevel { get; set; }
    
        public virtual Brand Brand { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImageProduct> ImageProducts { get; set; }
        public virtual MUser MUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderInfo> OrderInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MUser> MUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Promo> Promoes { get; set; }
    }
}
