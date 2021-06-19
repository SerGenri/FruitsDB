namespace FruitsDB.DB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelDb : DbContext
    {
        public ModelDb()
            : base("name=ModelDb")
        {
        }

        public virtual DbSet<FruitsCatalog> FruitsCatalog { get; set; }
        public virtual DbSet<PriceCatalog> PriceCatalog { get; set; }
        public virtual DbSet<ProvidersCatalog> ProvidersCatalog { get; set; }
        public virtual DbSet<Stock> Stock { get; set; }
        public virtual DbSet<StockFruits> StockFruits { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FruitsCatalog>()
                .HasMany(e => e.PriceCatalog)
                .WithRequired(e => e.FruitsCatalog)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FruitsCatalog>()
                .HasMany(e => e.StockFruits)
                .WithRequired(e => e.FruitsCatalog)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProvidersCatalog>()
                .HasMany(e => e.PriceCatalog)
                .WithRequired(e => e.ProvidersCatalog)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProvidersCatalog>()
                .HasMany(e => e.Stock)
                .WithRequired(e => e.ProvidersCatalog)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Stock>()
                .HasMany(e => e.StockFruits)
                .WithRequired(e => e.Stock)
                .WillCascadeOnDelete(false);
        }
    }
}
