using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ResourceGroupTenants.Core.Models.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.EntityModeling.Resource
{
    public class ResourceGroupEntityTypeConfiguration : IEntityTypeConfiguration<ResourceGroupModel>
    {
        public void Configure(EntityTypeBuilder<ResourceGroupModel> builder)
        {
            builder.HasKey(x => x.Id); 
            builder.Property<string?>(x => x.ResourceCode).HasColumnType("nvarchar(80)");
            builder.Property<double>(x => x.UpdateDate).HasColumnType("float");
            builder.Property<bool>(x => x.IsMarkedForDelete).HasColumnType("bit");

            builder.Property<string?>(x => x.CompanyName).HasColumnType("nvarchar(80)");
            builder.Property<string?>(x => x.CompanyRegNo).HasColumnType("nvarchar(80)");
            builder.Property<string?>(x => x.CompanyPhone).HasColumnType("nvarchar(80)");
            builder.Property<string?>(x => x.CompanyAddress).HasColumnType("nvarchar(80)");
            builder.Property<string?>(x => x.Admin).HasColumnType("nvarchar(120)");
            builder.Property<string?>(x => x.Password).HasColumnType("nvarchar(300)");
        }
    }
}
