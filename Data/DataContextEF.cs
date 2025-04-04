using fruits_store_backend_asp_net.Models;
using Microsoft.EntityFrameworkCore;

namespace fruits_store_backend_asp_net.Data
{
    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _config;

        public DataContextEF(IConfiguration config)
        {
            _config = config;
        }

        public virtual DbSet<Fruit> Fruits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    _config.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.EnableRetryOnFailure()
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("FruitsStoreBackendSchema");

            modelBuilder
                .Entity<Fruit>()
                .ToTable("Fruit", "FruitsStoreBackendSchema")
                .HasKey(f => f.FruitId);
        }
    }
}
