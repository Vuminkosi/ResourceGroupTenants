using Dna;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using ResourceGroupTenants.Core.Models;
using ResourceGroupTenants.Core.Models.Masters;
using ResourceGroupTenants.Relational.Data.Identity;
using ResourceGroupTenants.Relational.EntityModeling.Models;
using ResourceGroupTenants.Relational.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.Data
{
    public class TenantDBContext:IdentityDbContext<ApplicationUser>
    {
        public string TenantId { get; set; }
        private readonly ITenantService _tenantService;
        public TenantDBContext(DbContextOptions<TenantDBContext> options) : base(options)
        {
        }
        public TenantDBContext(DbContextOptions<TenantDBContext> options, ITenantService tenantService) : base(options)
        {
            _tenantService = tenantService;
            TenantId = _tenantService.GetTenant()?.ResourceCode;
        }
        public DbSet<ProductModel> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProductModel>().HasQueryFilter(a => a.ResourceCode == TenantId);
            modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var tenantConnectionString = _tenantService.GetConnectionString();
            if (!string.IsNullOrEmpty(tenantConnectionString))
            {
                optionsBuilder.UseSqlServer(_tenantService.GetConnectionString());
            }
            // optionsBuilder.UseSqlServer("Server=.\\SERVER; Database=SaaSTenant;User ID=Admin;Password=Admin@123;MultipleActiveResultSets=true;");
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseModel>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        entry.Entity.ResourceCode = TenantId;
                        break;
                }
            }
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
    }

    public class TenantDBContextFactory : IDesignTimeDbContextFactory<TenantDBContext>
    {
        public TenantDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDBContext>();
            optionsBuilder.UseSqlServer("Server=.\\SERVER; Database=SaaSTenant;User ID=Admin;Password=Admin@123;MultipleActiveResultSets=true;");

            return new TenantDBContext(optionsBuilder.Options);
        }

    }
}
