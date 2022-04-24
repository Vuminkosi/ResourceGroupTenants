using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ResourceGroupTenants.Core.Models.Masters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.EntityModeling.Models
{
    public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<ProductModel>
    {
        public void Configure(EntityTypeBuilder<ProductModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property<string?>(x => x.ResourceCode).HasColumnType("nvarchar(80)");
            builder.Property<double>(x => x.UpdateDate).HasColumnType("float");
            builder.Property<bool>(x => x.IsMarkedForDelete).HasColumnType("bit");

            builder.Property<string?>(x => x.Name).HasColumnType("nvarchar(80)");
            builder.Property<string?>(x => x.Description).HasColumnType("nvarchar(80)");
            builder.Property<string?>(x => x.Code).HasColumnType("nvarchar(80)");
            builder.Property<string?>(x => x.BarCode).HasColumnType("nvarchar(80)");
            builder.Property<string?>(x => x.UOM).HasColumnType("nvarchar(80)");
            builder.Property<string?>(x => x.Weight).HasColumnType("nvarchar(80)");
            builder.Property<double>(x => x.UnitPrice).HasColumnType("float");
            builder.Property<double>(x => x.SellingPrice).HasColumnType("float");
        }
    }
}
