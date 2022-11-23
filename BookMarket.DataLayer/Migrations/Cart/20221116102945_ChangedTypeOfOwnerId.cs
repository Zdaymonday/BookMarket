using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMarket.DataLayer.Migrations.Cart
{
    public partial class ChangedTypeOfOwnerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_OwnerId",
                table: "Carts",
                column: "OwnerId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_OwnerId",
                table: "Carts");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Carts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
