using Microsoft.EntityFrameworkCore.Migrations;

namespace Quizzy.Migrations
{
    public partial class AddedLQ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastQuestionAnswers",
                table: "UserInfo",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastQuestionAnswers",
                table: "UserInfo");
        }
    }
}
