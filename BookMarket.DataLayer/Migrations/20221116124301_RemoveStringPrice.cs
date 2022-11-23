using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMarket.DataLayer.Migrations
{
    public partial class RemoveStringPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "PageInStore");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Price",
                table: "PageInStore",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
