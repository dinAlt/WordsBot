using Microsoft.EntityFrameworkCore.Migrations;

namespace WordsBot.Migrations
{
    public partial class AddGiveUpsGameCounter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GiveUpsCount",
                table: "GameSessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiveUpsCount",
                table: "GameSessions");
        }
    }
}
