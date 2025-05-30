using Microsoft.EntityFrameworkCore;
using CompanyManagementSystem.Models;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace CompanyManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<LineItem> LineItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure DateTime properties to use UTC
            modelBuilder.Entity<PurchaseOrder>()
                .Property(po => po.OrderDate)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<PurchaseOrder>()
                .Property(po => po.NotificationTime)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<LineItem>()
                .Property(li => li.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(po => po.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Company>()
                .HasOne(c => c.User)
                .WithMany(u => u.Companies)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.User)
                .WithMany(u => u.PurchaseOrders)
                .HasForeignKey(po => po.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.Company)
                .WithMany(c => c.PurchaseOrders)
                .HasForeignKey(po => po.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LineItem>()
                .HasOne(li => li.PurchaseOrder)
                .WithMany(po => po.LineItems)
                .HasForeignKey(li => li.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=(localdb)\\mssqllocaldb;Database=CompanyManagementSystem;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }
    }
}