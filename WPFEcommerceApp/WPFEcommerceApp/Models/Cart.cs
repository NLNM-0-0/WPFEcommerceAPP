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
    
    public partial class Cart
    {
        public int IdUser { get; set; }
        public int IdProduct { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<bool> IsChooseSizeS { get; set; }
        public Nullable<bool> IsChooseSizeM { get; set; }
        public Nullable<bool> IsChooseSizeL { get; set; }
        public Nullable<bool> IsChooseSizeXL { get; set; }
        public Nullable<bool> IsChooseSizeXXL { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual MUser MUser { get; set; }
    }
}