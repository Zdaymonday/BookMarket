using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMarket.DataLayer.Migrations
{
    public partial class AddWorkIdInReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Works_WorkId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "WorkId",
                table: "Reviews",
                newName: "WorkID");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_WorkId",
                table: "Reviews",
                newName: "IX_Reviews_WorkID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Works_WorkID",
                table: "Reviews",
                column: "WorkID",
                principalTable: "Works",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Works_WorkID",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "WorkID",
                table: "Reviews",
                newName: "WorkId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_WorkID",
                table: "Reviews",
                newName: "IX_Reviews_WorkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Works_WorkId",
                table: "Reviews",
                column: "WorkId",
                principalTable: "Works",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
