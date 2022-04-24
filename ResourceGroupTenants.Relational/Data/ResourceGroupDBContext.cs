using Dna;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using ResourceGroupTenants.Core.Models.Resources;
using ResourceGroupTenants.Relational.EntityModeling.Resource;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.Data
{
    public class ResourceGroupDBContext:DbContext
    {
        public ResourceGroupDBContext(DbContextOptions<ResourceGroupDBContext> contextOptions):base(contextOptions)
        { 
        } 
        public DbSet<ResourceGroupModel> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ResourceGroupEntityTypeConfiguration());
        }
    }
    public class ResourceGroupDBContextFactory : IDesignTimeDbContextFactory<ResourceGroupDBContext>
    {
        public ResourceGroupDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ResourceGroupDBContext>();
            optionsBuilder.UseSqlServer("Server=.\\SERVER; Database=ResourceSaasTenant;User ID=sa;Password=Roots@97;MultipleActiveResultSets=true;");

            return new ResourceGroupDBContext(optionsBuilder.Options);
        }

    }
}
