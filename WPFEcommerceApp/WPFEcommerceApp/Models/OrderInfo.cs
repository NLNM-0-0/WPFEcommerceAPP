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
    
    public partial class OrderInfo
    {
        public int IdOrder { get; set; }
        public int IdProduct { get; set; }
        public int Count { get; set; }
        public long TotalPrice { get; set; }
    
        public virtual MOrder MOrder { get; set; }
        public virtual Product Product { get; set; }
    }
}
