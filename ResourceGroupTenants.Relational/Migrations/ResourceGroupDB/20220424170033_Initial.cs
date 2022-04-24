using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceGroupTenants.Relational.Migrations.ResourceGroupDB
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(80)", nullable: true),
                    CompanyRegNo = table.Column<string>(type: "nvarchar(80)", nullable: true),
                    CompanyPhone = table.Column<string>(type: "nvarchar(80)", nullable: true),
                    CompanyAddress = table.Column<string>(type: "nvarchar(80)", nullable: true),
                    Admin = table.Column<string>(type: "nvarchar(120)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(300)", nullable: true),
                    ResourceCode = table.Column<string>(type: "nvarchar(80)", nullable: true),
                    UpdateDate = table.Column<double>(type: "float", nullable: false),
                    IsMarkedForDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
