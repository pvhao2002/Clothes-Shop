namespace ClothesShop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public product()
        {
            cart_items = new HashSet<cart_items>();
            comments = new HashSet<comment>();
            order_items = new HashSet<order_items>();
            ratings = new HashSet<rating>();
        }

        [Key]
        public int product_id { get; set; }

        [StringLength(255)]
        public string product_name { get; set; }

        public decimal? price { get; set; }

        public int? stock { get; set; }

        public string description { get; set; }

        [StringLength(2000)]
        public string product_image { get; set; }

        public int? category_id { get; set; }

        [StringLength(50)]
        public string status { get; set; }

        public int? sold { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cart_items> cart_items { get; set; }

        public virtual category category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<comment> comments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<order_items> order_items { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<rating> ratings { get; set; }
    }
}
