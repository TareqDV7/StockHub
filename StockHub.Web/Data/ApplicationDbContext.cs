using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockHub.Web.Core.Models;

namespace StockHub.Web.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Warehouse> Warehouses { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<Employee> Employees { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<Beneficiary> Beneficiaries { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Explicitly define Beneficiary - ApplicationUser relationship
            builder.Entity<Beneficiary>()
                   .HasOne(b => b.User)
                   .WithOne(u => u.Beneficiary)
                   .HasForeignKey<Beneficiary>(b => b.UserId);
            // Configure CreatedBy and LastUpdatedBy (Optional, if you need the relationships)
            builder.Entity<Beneficiary>()
                 .HasOne(b => b.CreatedBy)
                 .WithMany()
                 .HasForeignKey(b => b.CreatedById)
                 .OnDelete(DeleteBehavior.Restrict); // Or DeleteBehavior.Restrict


            builder.Entity<Beneficiary>()
                 .HasOne(b => b.LastUpdatedBy)
                 .WithMany()
                 .HasForeignKey(b => b.LastUpdatedById)
                 .OnDelete(DeleteBehavior.Restrict); // Or DeleteBehavior.Restrict

            builder.Entity<Warehouse>()
           .HasOne(w => w.Manager)
           .WithMany() // Or WithOne if you only want one warehouse per user
           .HasForeignKey(w => w.ManagerId); // Explicit foreign key property
            builder.Entity<Warehouse>()
                .HasOne(w => w.CreatedBy)
                .WithMany()
                .HasForeignKey(w => w.CreatedById);
            builder.Entity<Warehouse>()
              .HasOne(w => w.LastUpdatedBy)
               .WithMany()
              .HasForeignKey(w => w.LastUpdatedById);
        }
    }
}
