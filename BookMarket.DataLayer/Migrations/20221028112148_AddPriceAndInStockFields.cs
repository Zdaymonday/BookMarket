using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMarket.DataLayer.Migrations
{
    public partial class AddPriceAndInStockFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InStock",
                table: "PageInStore",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Price",
                table: "PageInStore",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InStock",
                table: "PageInStore");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "PageInStore");
        }
    }
}
