using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMarket.DataLayer.Migrations.Cart
{
    public partial class RemoveCartItemAndAddOrderPositionInstead : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "OrderPosition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    ItemQuantity = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPosition_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderPosition_OrderId",
                table: "OrderPosition",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderPosition");

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
    }
}
