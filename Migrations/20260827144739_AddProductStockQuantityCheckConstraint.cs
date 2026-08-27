using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopZone.Api.Migrations
{
    public partial class AddProductStockQuantityCheckConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_StockQuantity_NonNegative",
                table: "Products",
                sql: "\"StockQuantity\" >= 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_StockQuantity_NonNegative",
                table: "Products");
        }
    }
}
