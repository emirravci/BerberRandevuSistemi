using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BerberRandevu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HizmetUcretEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Ucret",
                table: "Hizmetler",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ucret",
                table: "Hizmetler");
        }
    }
}
