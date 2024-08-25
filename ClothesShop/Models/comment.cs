namespace ClothesShop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("comment")]
    public partial class comment
    {
        [Key]
        public int comment_id { get; set; }

        public int? user_id { get; set; }

        public int? product_id { get; set; }

        public string content { get; set; }

        public DateTime? comment_date { get; set; }

        public virtual product product { get; set; }

        public virtual user user { get; set; }
    }
}
