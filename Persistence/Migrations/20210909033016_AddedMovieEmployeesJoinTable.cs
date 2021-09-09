using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddedMovieEmployeesJoinTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieEmployee_Movies_MovieId",
                table: "MovieEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieEmployee_People_PersonId",
                table: "MovieEmployee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieEmployee",
                table: "MovieEmployee");

            migrationBuilder.RenameTable(
                name: "MovieEmployee",
                newName: "MovieEmployees");

            migrationBuilder.RenameIndex(
                name: "IX_MovieEmployee_MovieId",
                table: "MovieEmployees",
                newName: "IX_MovieEmployees_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieEmployees",
                table: "MovieEmployees",
                columns: new[] { "PersonId", "MovieId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MovieEmployees_Movies_MovieId",
                table: "MovieEmployees",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieEmployees_People_PersonId",
                table: "MovieEmployees",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieEmployees_Movies_MovieId",
                table: "MovieEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieEmployees_People_PersonId",
                table: "MovieEmployees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieEmployees",
                table: "MovieEmployees");

            migrationBuilder.RenameTable(
                name: "MovieEmployees",
                newName: "MovieEmployee");

            migrationBuilder.RenameIndex(
                name: "IX_MovieEmployees_MovieId",
                table: "MovieEmployee",
                newName: "IX_MovieEmployee_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieEmployee",
                table: "MovieEmployee",
                columns: new[] { "PersonId", "MovieId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MovieEmployee_Movies_MovieId",
                table: "MovieEmployee",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieEmployee_People_PersonId",
                table: "MovieEmployee",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
