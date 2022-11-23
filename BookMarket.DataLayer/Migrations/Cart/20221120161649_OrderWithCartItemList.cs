using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMarket.DataLayer.Migrations.Cart
{
    public partial class OrderWithCartItemList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "CartItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_OrderId",
                table: "CartItem",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Orders_OrderId",
                table: "CartItem",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Orders_OrderId",
                table: "CartItem");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_OrderId",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "CartItem");

            migrationBuilder.AddColumn<int>(
                name: "PageId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
