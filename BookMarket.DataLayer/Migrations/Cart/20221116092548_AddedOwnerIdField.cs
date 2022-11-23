using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMarket.DataLayer.Migrations.Cart
{
    public partial class AddedOwnerIdField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Carts");
        }
    }
}
