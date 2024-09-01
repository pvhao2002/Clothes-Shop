namespace ClothesShop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("policy")]
    public partial class policy
    {
        [Key]
        public int policy_id { get; set; }

        public string policy_content { get; set; }
    }
}
