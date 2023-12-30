using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardDeck.DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    CardID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FrontSideText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackSideText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.CardID);
                });

            migrationBuilder.CreateTable(
                name: "Decks",
                columns: table => new
                {
                    DeckID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decks", x => x.DeckID);
                });

            migrationBuilder.CreateTable(
                name: "CardDecks",
                columns: table => new
                {
                    DeckID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardDecks", x => new { x.CardID, x.DeckID });
                    table.ForeignKey(
                        name: "FK_CardDecks_Cards_CardID",
                        column: x => x.CardID,
                        principalTable: "Cards",
                        principalColumn: "CardID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardDecks_Decks_DeckID",
                        column: x => x.DeckID,
                        principalTable: "Decks",
                        principalColumn: "DeckID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamDecks",
                columns: table => new
                {
                    DeckID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamDecks", x => new { x.DeckID, x.TeamID });
                    table.ForeignKey(
                        name: "FK_TeamDecks_Decks_DeckID",
                        column: x => x.DeckID,
                        principalTable: "Decks",
                        principalColumn: "DeckID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardDecks_DeckID",
                table: "CardDecks",
                column: "DeckID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardDecks");

            migrationBuilder.DropTable(
                name: "TeamDecks");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Decks");
        }
    }
}
