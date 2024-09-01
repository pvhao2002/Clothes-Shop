namespace ClothesShop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("slider")]
    public partial class slider
    {
        [Key]
        public int slider_id { get; set; }

        public string slider_img { get; set; }

        public int? position { get; set; }
    }
}
