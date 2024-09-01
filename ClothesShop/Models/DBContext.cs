using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ClothesShop.Models
{
    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DBContext")
        {
        }
        public static DBContext Create()
        {
            return new DBContext();
        }
        public virtual DbSet<about> abouts { get; set; }
        public virtual DbSet<cart_items> cart_items { get; set; }
        public virtual DbSet<cart> carts { get; set; }
        public virtual DbSet<category> categories { get; set; }
        public virtual DbSet<comment> comments { get; set; }
        public virtual DbSet<order_items> order_items { get; set; }
        public virtual DbSet<order> orders { get; set; }
        public virtual DbSet<policy> policies { get; set; }
        public virtual DbSet<product> products { get; set; }
        public virtual DbSet<rating> ratings { get; set; }
        public virtual DbSet<slider> sliders { get; set; }
        public virtual DbSet<user> users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<cart_items>()
                .Property(e => e.total_price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<cart>()
                .Property(e => e.total_price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<category>()
                .Property(e => e.status)
                .IsUnicode(false);

            modelBuilder.Entity<order_items>()
                .Property(e => e.total_price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<order>()
                .Property(e => e.total_price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<order>()
                .Property(e => e.status)
                .IsUnicode(false);

            modelBuilder.Entity<product>()
                .Property(e => e.price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<product>()
                .Property(e => e.status)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.role)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.status)
                .IsUnicode(false);
        }
    }
}
