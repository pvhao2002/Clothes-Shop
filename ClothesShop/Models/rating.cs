namespace ClothesShop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("rating")]
    public partial class rating
    {
        [Key]
        public int rate_id { get; set; }

        public int? user_id { get; set; }

        public int? product_id { get; set; }

        public int? rate { get; set; }

        public virtual product product { get; set; }

        public virtual user user { get; set; }
    }
}
