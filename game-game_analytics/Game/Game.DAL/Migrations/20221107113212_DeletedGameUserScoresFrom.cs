using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game.DAL.Migrations
{
    public partial class DeletedGameUserScoresFrom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameUsersScores_GameUsers_GameUserUserID_GameUserGameID",
                table: "GameUsersScores");

            migrationBuilder.DropIndex(
                name: "IX_GameUsersScores_GameUserUserID_GameUserGameID",
                table: "GameUsersScores");

            migrationBuilder.DropColumn(
                name: "GameUserGameID",
                table: "GameUsersScores");

            migrationBuilder.DropColumn(
                name: "GameUserUserID",
                table: "GameUsersScores");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "GameUsersScores");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GameUserGameID",
                table: "GameUsersScores",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GameUserUserID",
                table: "GameUsersScores",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "GameUsersScores",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"))
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.CreateIndex(
                name: "IX_GameUsersScores_GameUserUserID_GameUserGameID",
                table: "GameUsersScores",
                columns: new[] { "GameUserUserID", "GameUserGameID" });

            migrationBuilder.AddForeignKey(
                name: "FK_GameUsersScores_GameUsers_GameUserUserID_GameUserGameID",
                table: "GameUsersScores",
                columns: new[] { "GameUserUserID", "GameUserGameID" },
                principalTable: "GameUsers",
                principalColumns: new[] { "UserID", "GameID" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
