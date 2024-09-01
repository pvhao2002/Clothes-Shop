namespace ClothesShop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("about")]
    public partial class about
    {
        [Key]
        public int about_id { get; set; }

        public string content { get; set; }
    }
}
