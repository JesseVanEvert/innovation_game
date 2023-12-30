using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game.DAL.Migrations
{
    public partial class DeletedGameIDFromGameUserScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameUsersScores_GameUsers_GameID_UserID",
                table: "GameUsersScores");

            migrationBuilder.DropIndex(
                name: "IX_GameUsersScores_GameID_UserID",
                table: "GameUsersScores");

            migrationBuilder.RenameColumn(
                name: "GameID",
                table: "GameUsersScores",
                newName: "GameUserUserID");

            migrationBuilder.AlterColumn<Guid>(
                name: "GameUserUserID",
                table: "GameUsersScores",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AddColumn<Guid>(
                name: "GameUserGameID",
                table: "GameUsersScores",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "GameUserUserID",
                table: "GameUsersScores",
                newName: "GameID");

            migrationBuilder.AlterColumn<Guid>(
                name: "GameID",
                table: "GameUsersScores",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.CreateIndex(
                name: "IX_GameUsersScores_GameID_UserID",
                table: "GameUsersScores",
                columns: new[] { "GameID", "UserID" });

            migrationBuilder.AddForeignKey(
                name: "FK_GameUsersScores_GameUsers_GameID_UserID",
                table: "GameUsersScores",
                columns: new[] { "GameID", "UserID" },
                principalTable: "GameUsers",
                principalColumns: new[] { "UserID", "GameID" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
