using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HappySalesApp.Migrations
{
    /// <inheritdoc />
    public partial class NotMapped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCount",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductCount",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
