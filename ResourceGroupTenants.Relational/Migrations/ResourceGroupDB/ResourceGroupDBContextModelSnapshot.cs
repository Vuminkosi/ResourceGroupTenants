// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ResourceGroupTenants.Relational.Data;

#nullable disable

namespace ResourceGroupTenants.Relational.Migrations.ResourceGroupDB
{
    [DbContext(typeof(ResourceGroupDBContext))]
    partial class ResourceGroupDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ResourceGroupTenants.Core.Models.Resources.ResourceGroupModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Admin")
                        .HasColumnType("nvarchar(120)");

                    b.Property<string>("CompanyAddress")
                        .HasColumnType("nvarchar(80)");

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(80)");

                    b.Property<string>("CompanyPhone")
                        .HasColumnType("nvarchar(80)");

                    b.Property<string>("CompanyRegNo")
                        .HasColumnType("nvarchar(80)");

                    b.Property<bool>("IsMarkedForDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("ResourceCode")
                        .HasColumnType("nvarchar(80)");

                    b.Property<double>("UpdateDate")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Tenants");
                });
#pragma warning restore 612, 618
        }
    }
}
