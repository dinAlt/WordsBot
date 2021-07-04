using Microsoft.EntityFrameworkCore.Migrations;

namespace WordsBot.Migrations
{
    public partial class AddGameSessionCuccessCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SuccessCount",
                table: "GameSessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuccessCount",
                table: "GameSessions");
        }
    }
}
