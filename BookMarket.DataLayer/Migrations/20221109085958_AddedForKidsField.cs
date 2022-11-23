using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMarket.DataLayer.Migrations
{
    public partial class AddedForKidsField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isForKids",
                table: "Works",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isForKids",
                table: "Works");
        }
    }
}
