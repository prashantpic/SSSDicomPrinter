using Microsoft.EntityFrameworkCore.Migrations;
using TheSSS.DICOMViewer.Infrastructure.Persistence.Entities;

#nullable disable

namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnonymizationProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ProfileData = table.Column<string>(type: "TEXT", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymizationProfiles", x => x.Id);
                });

            // Additional CreateTable statements for all entities
            // Foreign key constraints and indexes omitted for brevity
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AnonymizationProfiles");
            // Additional DropTable statements in reverse order
        }
    }
}