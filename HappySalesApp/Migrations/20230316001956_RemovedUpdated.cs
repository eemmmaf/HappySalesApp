using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HappySalesApp.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Products",
                type: "datetime",
                nullable: true);
        }
    }
}
