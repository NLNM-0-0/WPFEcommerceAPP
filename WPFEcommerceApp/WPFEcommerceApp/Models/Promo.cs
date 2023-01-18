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
    
    public partial class Promo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Promo()
        {
            this.MOrders = new HashSet<MOrder>();
            this.Products = new HashSet<Product>();
        }
    
        public string Id { get; set; }
        public string IdShop { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> DateBegin { get; set; }
        public Nullable<System.DateTime> DateEnd { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<int> AmountUsed { get; set; }
        public Nullable<double> MaxSale { get; set; }
        public Nullable<double> MinCost { get; set; }
        public Nullable<int> CustomerType { get; set; }
        public Nullable<double> Sale { get; set; }
        public Nullable<int> Status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOrder> MOrders { get; set; }
        public virtual MUser MUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
